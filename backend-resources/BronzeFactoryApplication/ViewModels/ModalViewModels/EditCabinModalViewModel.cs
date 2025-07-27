using BronzeFactoryApplication.ApplicationServices.MessangerService;
using BronzeFactoryApplication.ApplicationServices.NavigationService;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using DocumentFormat.OpenXml.Presentation;
using GlassesOrdersModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class EditCabinModalViewModel : ModalViewModel, IRecipient<EditLivePartMessage>
    {
        private CabinOrderRow _undoStore = CabinOrderRow.Empty();

        private readonly CabinViewModelFactory cabinVmFactory;
        private readonly OpenLiveEditPartModalService openEditPartModal;
        private readonly CloseModalService closeModalService;
        private readonly PartSetsApplicatorService partsApplicator;
        private readonly IMessenger messenger;

        [ObservableProperty]
        private CabinViewModel cabin = new();

        private string referencePA0 = string.Empty;
        public string ReferencePA0
        {
            get => referencePA0;
            set
            {
                if (SetProperty(ref referencePA0, value))
                {
                    IsEdited = true;
                }
            }
        }

        private string notes = string.Empty;
        public string Notes
        {
            get => notes;
            set
            {
                if (SetProperty(ref notes, value))
                {
                    IsEdited = true;
                }
            }
        }

        private int quantity;
        public int Quantity
        {
            get => quantity;
            set
            {
                if (SetProperty(ref quantity, value))
                {
                    IsEdited = true;
                }
            }
        }

        [ObservableProperty]
        private bool isEdited;

        public CabinCalculationsTableViewModel CalculationsTable { get; init; }

        public void SetCabinRow(CabinOrderRow row)
        {
            if (row.OrderedCabin is null) throw new InvalidOperationException($"Ordered Cabin is Null ... Cannot Edit");

            Title = row.OrderedCabin.Model?.ToString().TryTranslateKey() ?? "Undefined Cabin Model";

            // Store the Row being edited
            _undoStore = row;

            // Produce the Cabin ViewModel to alter the Cabin if needed by passing in a clone , and Set the Rows details
            Cabin = cabinVmFactory.Create(row.OrderedCabin.GetDeepClone());
            // Cannot Connect with Primary Model when Edited Under Glass DataGrids
            Cabin.CanConnectWithPrimary = false;
            this.Cabin.Parts.CanConnectWithPrimary = false;
            ReferencePA0 = row.ReferencePA0;
            Notes = row.Notes;
            Quantity = row.Quantity;

            //Set but do not calculate , the cabin under Edit already has glasses that not need to be changed until an edition happens on the CabinViewModel
            CalculationsTable.SetCabin(Cabin, false);

            //Needs to be in the end otherwise it will always turn true at construction
            IsEdited = false;
            Cabin.CabinChanged += OnEdits;
            Cabin.Parts.PartChanged += OnPartsChanged;
        }

        /// <summary>
        /// Replaces parts with the Parts Applicator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPartsChanged(object? sender, PartChangedEventsArgs e)
        {
            //Delegate the Set Part Changes to the Applicator
            if (e.NewPart is not null
                && Cabin is not null
                && partsApplicator.ShouldApplySetChange(e.Identifier, e.NewPart.Code))
            {
                var partsList = Cabin.Parts?.GetPartsObject();
                if (partsList != null)
                {
                    var spotsApplied = partsApplicator.TryApplySetChange(partsList, e.Spot, e.NewPart.Code, e.Identifier);
                    foreach (var spot in spotsApplied)
                    {
                        Cabin.Parts?.InformSpotPartChanged(spot);
                    }
                }
            }
        }

        private void OnEdits(object? sender, Cabin e)
        {
            IsEdited = true;
        }

        #region CONSTRUCTOR
        public EditCabinModalViewModel(Func<CabinCalculationsTableViewModel> tableFactory,
                                       CabinViewModelFactory cabinVmFactory,
                                       OpenLiveEditPartModalService openEditPartModal,
                                       CloseModalService closeModalService,
                                       PartSetsApplicatorService partsApplicator,
                                       IMessenger messenger)
        {
            CalculationsTable = tableFactory.Invoke();
            this.cabinVmFactory = cabinVmFactory;
            this.openEditPartModal = openEditPartModal;
            this.closeModalService = closeModalService;
            this.partsApplicator = partsApplicator;
            this.closeModalService.ModalClosing += OnModalClosing;
            this.messenger = messenger;
            this.messenger.RegisterAll(this);
        } 
        #endregion

        private void OnModalClosing(object? sender, ModalClosingEventArgs e)
        {
            // Only when is Edited ask if User wants to Close and If the Closing modal is this
            if (IsEdited && e.ClosingModal is EditCabinModalViewModel)
            {
                if (MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
                {
                    e.ShouldCancelClose = true;
                }
            }
        }

        /// <summary>
        /// When on of the Parts gets Edited
        /// </summary>
        /// <param name="message"></param>
        public void Receive(EditLivePartMessage message)
        {
            if (message.Sender == Cabin.Parts)
            {
                //Open the Part Edit Modal
                openEditPartModal.OpenModal(message.PartToEdit, message.SpotToEdit, message.Sender);
                //Set that the Cabin Has Been Edited (This should marks it as edited even if Part is not edited , but to implement this needs to add Undo functionality in EditPart ViewModels...)
                IsEdited = true;
            }
        }

        [RelayCommand]
        private void SaveEditsAndClose()
        {
            if (IsEdited)
            {
                if (!CalculationsTable.IsValid && QuestionUndoAndClose() == MessageBoxResult.Cancel)
                {
                    //If calcs are not Valid and User Does not Want to Close Window
                    //Otherwise it will go to the end and just close modal
                    return;
                }
                else
                {
                    //If calcs are correct proceed
                    var newCabinRow = ProduceNewRow();
                    IsEdited = false; //revert is edited to be able to close without asking
                    messenger.Send(new CabinRowEditMessage(_undoStore, newCabinRow));
                }
            }
            MessageService.Information.SaveSuccess();
            closeModalService.CloseModal(this);
        }

        /// <summary>
        /// Undos Any Edits done to the Cabin
        /// </summary>
        /// <exception cref="InvalidOperationException">When the Cabin is Null (Should never Happen)</exception>
        [RelayCommand]
        private void UndoEdits()
        {
            SetCabinRow(_undoStore);
        }

        /// <summary>
        /// Produces the new Edited Row
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">When CabinObject is null</exception>
        private CabinOrderRow ProduceNewRow()
        {
            //Create new GlassRows if the Glasses are Different from Before
            var oldGlasses = _undoStore.GlassesRows.Select(r => r.OrderedGlass);
            var currentGlasses = Cabin.Glasses;
            List<GlassOrderRow> newGlassRows = new();

            //If there is something left from any of the below checks it means there are changes 
            //otherwise the glasses have not changed at all which means we just use the old rows
            if (oldGlasses.Except(currentGlasses).Any() || currentGlasses.Except(oldGlasses).Any())
            {
                foreach (var glass in Cabin.Glasses)
                {
                    var row = GlassOrderRow.CreateNew(referencePA0, "", quantity, glass, _undoStore.CabinKey, _undoStore.RowId,false, 0, 0);
                    row.OrderId = _undoStore.OrderId;
                    newGlassRows.Add(row);
                }
            }
            else
            {
                //Just use the old rows by not changing any notes but change the CabinKey
                newGlassRows = _undoStore.GlassesRows.Select(r => r.GetDeepClone()).ToList();
                // The Parent will be Set at the CabinRow Anyways
            }

            // Create a new Cabin Key for the new Row
            Guid newCabinKey = Guid.NewGuid();

            // Pass the key to all the new Glasses
            // Pass the Quantity to all new Glasses
            foreach (var glassRow in newGlassRows)
            {
                glassRow.CabinRowKey = newCabinKey;
                glassRow.Quantity = quantity;
            }

            // Get the Cabin from the ViewModel
            Cabin newCabin = Cabin?.CabinObject ?? throw new InvalidOperationException($"{nameof(CabinViewModel.CabinObject)} was null while trying to save Edits");

            var editedRow = CabinOrderRow.CreateNew(referencePA0, notes, quantity, newCabin, newGlassRows, _undoStore.SynthesisKey, newCabinKey, _undoStore.RowId);
            editedRow.Created = _undoStore.Created;
            editedRow.LastModified = DateTime.Now;
            return editedRow;
        }

        /// <summary>
        /// Ask the User Weather to Close the Modal but Undo Edits , When Validation Fails
        /// </summary>
        /// <returns></returns>
        private MessageBoxResult QuestionUndoAndClose()
        {
            StringBuilder builder = new();
            int index = 0;
            foreach (string error in CalculationsTable.ValidationErrors)
            {
                index++;
                builder.Append(index)
                       .Append('.')
                       .Append(' ')
                       .Append(error)
                       .Append(Environment.NewLine)
                       .Append(Environment.NewLine);
            }
            builder.Append("lngCorrectErrorsOrReturn".TryTranslateKey());
            return MessageService.Question(builder.ToString(), "lngInformation".TryTranslateKey(), "lngOk".TryTranslateKey(), "lngCancel".TryTranslateKey());
        }

        private bool _disposed;
        public override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                this.messenger.UnregisterAll(this);
                Cabin.CabinChanged -= OnEdits;
                Cabin.Parts.PartChanged -= OnPartsChanged;
                this.closeModalService.ModalClosing -= OnModalClosing;
                CalculationsTable.SetCabin(null);
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            base.Dispose(disposing);
        }

    }
}

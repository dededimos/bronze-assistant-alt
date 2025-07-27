using BronzeFactoryApplication.ApplicationServices.MessangerService;
using BronzeFactoryApplication.ApplicationServices.NavigationService;
using BronzeFactoryApplication.ApplicationServices.NumberingServices;
using BronzeFactoryApplication.ViewModels.ComponentsUCViewModels.DrawsViewModels;
using BronzeFactoryApplication.ViewModels.ModalViewModels;
using BronzeFactoryApplication.Views.Dialogs;
using CommunityToolkit.Diagnostics;
using DataAccessLib;
using FluentValidation.Results;
using GlassesOrdersModels.Models;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using Microsoft.Extensions.DependencyInjection;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.WSSmartModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.DirectoryServices;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BronzeFactoryApplication.ViewModels.ComponentsUCViewModels
{
    public partial class GlassesOrderViewModel : BaseViewModel,
        IRecipient<AddCabinRowsMessage>,
        IRecipient<GlassRowEditMessage>,
        IRecipient<CabinRowEditMessage>,
        IRecipient<GlassesOrderDetailsEditMessage>
    {
        private readonly IMessenger messenger;
        private readonly IGlassOrderRepository glassesRepo;
        private readonly OpenEditCabinModalService editCabinModalService;
        private readonly OpenEditGlassRowModalService editGlassRowModalService;
        private readonly OpenEditGlassesOrderDetailsModalService openEditOrderDetailsModalService;
        private readonly OpenPrintCabinBomModalService openPrintBomService;
        private readonly IDialogService dialogService;
        private readonly GlassNumberingService glassNumberingService;
        [ObservableProperty]
        private string orderId = "????";

        /// <summary>
        /// Weather this viewmodel represents a new Order
        /// </summary>
        [ObservableProperty]
        private bool isNewOrder = true;
        [ObservableProperty]
        private bool hasUnsavedOrderEdits;

        [ObservableProperty]
        private string notes = string.Empty;
        [ObservableProperty]
        private DateTime created;
        [ObservableProperty]
        private DateTime lastModified;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(GlassRows))]
        [NotifyPropertyChangedFor(nameof(GlassesCount))]
        [NotifyPropertyChangedFor(nameof(CabinsCount))]
        [NotifyPropertyChangedFor(nameof(PA0PAMCount))]
        [NotifyPropertyChangedFor(nameof(Status))]
        [NotifyPropertyChangedFor(nameof(SelectedCabinRow))]
        private ObservableCollection<CabinOrderRow> cabinRows = [];


        private CabinOrderRow? selectedCabinRow;
        public CabinOrderRow? SelectedCabinRow
        {
            get => selectedCabinRow;
            set
            {
                if (selectedCabinRow != value)
                {
                    selectedCabinRow = value;
                    OnPropertyChanged(nameof(SelectedCabinRow));
                    OnPropertyChanged(nameof(SelectedCabinGlasses));
                    OnPropertyChanged(nameof(SelectedGlass));
                    SetSynthesisDraw(selectedCabinRow);
                }
            }
        }
        private void SetSynthesisDraw(CabinOrderRow? selectedRow)
        {
            if (selectedRow == null)
            {
                SelectedSynthesisDraw.SetSynthesis(null);
                return;
            }

            var cabins = CabinRows.Where(c => c.SynthesisKey == selectedRow.SynthesisKey).Select(r => r.OrderedCabin).OrderBy(c => c.SynthesisModel);
            if (cabins.Any(c => c.SynthesisModel == CabinSynthesisModel.Primary))
            {
                try
                {
                    var primary = cabins.FirstOrDefault(c => c.SynthesisModel == CabinSynthesisModel.Primary);
                    var secondary = cabins.FirstOrDefault(c => c.SynthesisModel == CabinSynthesisModel.Secondary);
                    var tertiary = cabins.FirstOrDefault(c => c.SynthesisModel == CabinSynthesisModel.Tertiary);
                    var synthesis = CabinFactory.CreateSynthesis(primary, secondary, tertiary);
                    SelectedSynthesisDraw.SetSynthesis(synthesis);
                    switch (selectedRow.OrderedCabin.SynthesisModel)
                    {
                        case CabinSynthesisModel.Primary:
                            if (selectedRow.OrderedCabin is SingleGlassCabinContainer)
                            {
                                SelectedSynthesisDraw.SelectedDrawIndex = 3;
                            }
                            else
                            {
                                SelectedSynthesisDraw.SelectedDrawIndex = 0;
                            }
                            break;
                        case CabinSynthesisModel.Secondary:
                            SelectedSynthesisDraw.SelectedDrawIndex = 1;
                            break;
                        case CabinSynthesisModel.Tertiary:
                            SelectedSynthesisDraw.SelectedDrawIndex = 2;
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    SelectedSynthesisDraw.SetSynthesis(null);
                }
            }

        }

        public SynthesisDrawViewModel SelectedSynthesisDraw { get; }
        public GlassDrawViewModel SelectedGlassDraw { get; } = new();

        public ObservableCollection<GlassOrderRow>? SelectedCabinGlasses { get => selectedCabinRow?.GlassesRows != null ? new(selectedCabinRow.GlassesRows) : null; }
        public ObservableCollection<GlassOrderRow> GlassRows { get => new(CabinRows.SelectMany(r => r.GlassesRows)); }

        private GlassOrderRow? selectedGlass;
        public GlassOrderRow? SelectedGlass
        {
            get => selectedGlass;
            set
            {
                if (selectedGlass != value)
                {
                    selectedGlass = value;
                    OnPropertyChanged(nameof(SelectedGlass));
                    //Set the Index of the Draws ViewModel to that of the Glasses
                    if (selectedGlass != null) SelectedSynthesisDraw?.SetIndexToGlasses();
                }
            }
        }

        public OrderStatus Status { get => GlassesOrder.GetCombinedStatus(CabinRows.Select(r => r.Status)); }
        public int GlassesCount { get => GlassRows.Sum(r => r.Quantity); }
        public int CabinsCount { get => CabinRows.Sum(r => r.Quantity); }
        public int PA0PAMCount { get => CabinRows.Select(r => r.ReferencePA0).Distinct().Count(); }


        #region CONSTRUCTOR
        public GlassesOrderViewModel(IMessenger messenger,
                                     IGlassOrderRepository glassesRepo,
                                     Func<GlassNumberingService> numberingServiceProvider,
                                     Func<SynthesisDrawViewModel> synthesisDrawFactory,
                                     OpenEditCabinModalService editCabinModalService,
                                     OpenEditGlassRowModalService editGlassRowModalService,
                                     OpenEditGlassesOrderDetailsModalService openEditOrderDetailsModalService,
                                     OpenPrintCabinBomModalService openPrintBomService,
                                     IDialogService dialogService)
        {
            this.messenger = messenger;
            this.glassesRepo = glassesRepo;
            this.SelectedSynthesisDraw = synthesisDrawFactory.Invoke();
            messenger.RegisterAll(this);
            this.glassNumberingService = numberingServiceProvider.Invoke();
            this.glassNumberingService.SetGlassesOrder(this);

            this.editCabinModalService = editCabinModalService;

            this.editGlassRowModalService = editGlassRowModalService;
            this.openEditOrderDetailsModalService = openEditOrderDetailsModalService;
            this.openPrintBomService = openPrintBomService;
            this.dialogService = dialogService;
            this.cabinRows.CollectionChanged += Rows_CollectionChanged;
        }
        #endregion

        /// <summary>
        /// Sets an Order for Display to the ViewModel
        /// </summary>
        /// <param name="orderToSet">The Order to Set</param>
        public void SetExistingOrder(GlassesOrder orderToSet)
        {
            if (HasUnsavedOrderEdits)
            {
                if (MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            this.OrderId = orderToSet.OrderId;
            this.Created = orderToSet.Created;
            this.LastModified = orderToSet.LastModified;
            this.Notes = orderToSet.Notes;

            this.CabinRows.CollectionChanged -= Rows_CollectionChanged;
            this.CabinRows = new(orderToSet.CabinRows);
            this.CabinRows.CollectionChanged += Rows_CollectionChanged;

            this.IsNewOrder = false;
        }
        public void SetNewOrder()
        {
            if (HasUnsavedOrderEdits)
            {
                if (MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            this.OrderId = "????";
            this.Created = DateTime.Now;
            this.LastModified = DateTime.Now;
            this.Notes = "";

            this.CabinRows.CollectionChanged -= Rows_CollectionChanged;
            this.CabinRows = new();
            this.CabinRows.CollectionChanged += Rows_CollectionChanged;

            this.IsNewOrder = true;
        }

        #region Receive Message Functions

        public void Receive(AddCabinRowsMessage message)
        {
            // The Add Range Method Does not Raise the Collection Changed Event so Property Change on the Rest must be Raised Manually
            // Otherwise we can add all the items one by one but it sucks
            CabinRows.AddRange(message.Rows);

            //Apply Numbering to the New Glasses
            glassNumberingService.ApplyMissingLetteringNumbering();

            LastModified = DateTime.Now;
            HasUnsavedOrderEdits = true;
            OnPropertyChanged(nameof(GlassRows));
            OnPropertyChanged(nameof(CabinRows));
            OnPropertyChanged(nameof(GlassesCount));
            OnPropertyChanged(nameof(CabinsCount));
            OnPropertyChanged(nameof(PA0PAMCount));
        }
        public void Receive(GlassRowEditMessage message)
        {
            HasUnsavedOrderEdits = true;
            // Reset the Glass Selection
            SelectedGlass = null;

            // Remove the Old Row
            var rowContainingGlass = CabinRows.FirstOrDefault(r => r.CabinKey == message.OldRow.CabinRowKey) ?? throw new InvalidOperationException("Edited Row Not Found");
            rowContainingGlass.GlassesRows.Remove(message.OldRow);

            // Add the New Row and Update the UI
            rowContainingGlass.GlassesRows.Add(message.NewRow);
            //Apply Numbering to the New Row
            glassNumberingService.ApplyMissingLetteringNumbering();

            OnPropertyChanged(nameof(SelectedCabinGlasses));
            OnPropertyChanged(nameof(GlassesCount));
            OnPropertyChanged(nameof(GlassRows));
            OnPropertyChanged(nameof(Status));
            LastModified = DateTime.Now;

            // Show Success Prompt
            MessageService.Info("lngSaveSuccessful".TryTranslateKey(), "lngInformation".TryTranslateKey());
        }
        public void Receive(CabinRowEditMessage message)
        {
            HasUnsavedOrderEdits = true;

            // Reset the Cabin and Glass Selections
            SelectedCabinRow = null;
            SelectedGlass = null;

            // Replace The Old Row
            var indexOfOldRow = CabinRows.IndexOf(message.OldRow);
            CabinRows[indexOfOldRow] = message.NewRow;
            SelectedCabinRow = CabinRows[indexOfOldRow];

            //Apply Numbering for the new Row
            glassNumberingService.ApplyMissingLetteringNumbering();

            OnPropertyChanged(nameof(GlassRows));
            OnPropertyChanged(nameof(GlassesCount));
            OnPropertyChanged(nameof(CabinsCount));
            OnPropertyChanged(nameof(PA0PAMCount));
            OnPropertyChanged(nameof(Status));
        }
        public void Receive(GlassesOrderDetailsEditMessage message)
        {
            HasUnsavedOrderEdits = true;
            OrderId = message.OrderId;
            Notes = message.Notes;
        }

        #endregion

        public GlassesOrder ToGlassesOrder()
        {
            GlassesOrder order = new()
            {
                OrderId = this.OrderId,
                Notes = this.Notes,
                Created = this.Created,
                LastModified = this.LastModified,
            };
            order.AddNewRows(CabinRows);
            return order;
        }
        /// <summary>
        /// Inform Sums Changed whenever the Rows Collection Changes
        /// This is not enough though , Any Glass Deletion or Cabin Edition must be also documented (otherwise we have to bind to each and every collection change)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rows_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(GlassRows));
            OnPropertyChanged(nameof(CabinRows));
            OnPropertyChanged(nameof(GlassesCount));
            OnPropertyChanged(nameof(CabinsCount));
            OnPropertyChanged(nameof(PA0PAMCount));
        }

        [RelayCommand]
        private async Task TrySaveOrder()
        {
            if (!HasUnsavedOrderEdits)
            {
                MessageService.Warnings.AlreadySaved();
                return;
            }
            if (!GlassesOrder.OrderIdRegex.IsMatch(this.OrderId) || this.OrderId == "????")
            {
                MessageService.Warnings.InvalidOrderId();
                return;
            }
            try
            {
                if (IsNewOrder)
                {
                    await glassesRepo.InsertNewOrderAsync(this.ToGlassesOrder());
                    IsNewOrder = false;
                }
                else
                {
                    await glassesRepo.UpdateOrderAsync(this.ToGlassesOrder());
                }

                HasUnsavedOrderEdits = false;
                MessageService.Info("lngSaveSuccessful".TryTranslateKey(), "lngInformation".TryTranslateKey());
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
        }

        [RelayCommand]
        private void DeleteRow(CabinOrderRow rowToRemove)
        {
            var shouldRemoveResult = MessageService.Question($"{"lngShouldDeleteRow".TryTranslateKey()}{Environment.NewLine}{"lngCode".TryTranslateKey()}:{rowToRemove.OrderedCabin.Code}", "lngRemove".TryTranslateKey(), "lngOk".TryTranslateKey(), "lngCancel".TryTranslateKey());
            if (shouldRemoveResult == MessageBoxResult.OK)
            {
                HasUnsavedOrderEdits = true;
                // Remove the Selection and Then remove the Selected Row | otherwise it searches for the removed item
                SelectedCabinRow = null;
                bool isRemoved = CabinRows.Remove(rowToRemove);
                if (!isRemoved)
                {
                    MessageService.Error("lngErrorAtRowDeletion".TryTranslateKey(), "lngDeleteFailure".TryTranslateKey());
                }
                else
                {
                    LastModified = DateTime.Now;
                }
                //Does not need prop Change for Count Props the Collection Changed Takes Care of It
            }
        }

        [RelayCommand]
        private void DeleteGlassRow(GlassOrderRow rowToRemove)
        {
            var shouldRemoveResult = MessageService.Question($"{"lngShouldDeleteRow".TryTranslateKey()}{Environment.NewLine}{"lngGlass".TryTranslateKey()} : {rowToRemove.OrderedGlass.GlassType.ToString().TryTranslateKey()}", "lngRemove".TryTranslateKey(), "lngOk".TryTranslateKey(), "lngCancel".TryTranslateKey());
            if (shouldRemoveResult == MessageBoxResult.OK)
            {
                HasUnsavedOrderEdits = true;
                // Remove the Selection and Then remove the Selected Row | otherwise it searches for the removed item
                SelectedGlass = null;

                // Find the Cabin that contains this row and remove it
                var cabinRowWithGlass = rowToRemove?.ParentCabinRow ?? throw new InvalidOperationException($"{nameof(rowToRemove.ParentCabinRow)} was null");
                bool isRemoved = cabinRowWithGlass.GlassesRows.Remove(rowToRemove);
                if (!isRemoved)
                {
                    MessageService.Error("lngErrorAtRowDeletion".TryTranslateKey(), "lngDeleteFailure".TryTranslateKey());
                }
                else
                {
                    //remove it also from the Cabin
                    Log.Information("Removed Glass : {glassType} - from Cabin :{cabinCode}", rowToRemove.OrderedGlass.GlassType.ToString().TryTranslateKey(), rowToRemove.ParentCabinRow?.OrderedCabin.Code ?? "UndefinedCabin");
                    LastModified = DateTime.Now;
                    //Needs this there is no other event listening to glass rows deletion
                    OnPropertyChanged(nameof(GlassRows));
                    OnPropertyChanged(nameof(GlassesCount));
                    OnPropertyChanged(nameof(PA0PAMCount));
                    OnPropertyChanged(nameof(Status));
                    OnPropertyChanged(nameof(SelectedCabinGlasses));
                    //OnPropertyChanged(nameof(CabinRows));

                }
            }
        }

        [RelayCommand]
        private void OpenGlassEdit(GlassOrderRow rowToEdit)
        {
            editGlassRowModalService.OpenModal(rowToEdit);
        }

        [RelayCommand]
        private void OpenCabinEdit(CabinOrderRow rowToEdit)
        {
            //Case where Cabin is Only Cabin Container
            if (rowToEdit.OrderedCabin is SingleGlassCabinContainer)
            {
                MessageService.Warning("lngCannotEditGlassContainer".TryTranslateKey(), "lngFailure".TryTranslateKey());
                return;
            }
            if (rowToEdit.HasGlassFromStock)
            {
                MessageService.Warning("lngCannotEditCabinsWithSwappedGlasses".TryTranslateKey(), "lngFailure".TryTranslateKey());
                return;
            }

            editCabinModalService.OpenModal(rowToEdit);
        }
        [RelayCommand]
        private void OpenOrderDetailsEdit()
        {
            openEditOrderDetailsModalService.OpenModal(this.ToGlassesOrder(), IsNewOrder);
        }
        [RelayCommand]
        private void OpenSinglePrintBom(CabinOrderRow rowToPrint)
        {
            List<CabinOrderRow> rows = new() { rowToPrint };

            openPrintBomService.OpenModal(rows);
        }
        [RelayCommand]
        private void OpenAllPrintBom()
        {
            openPrintBomService.OpenModal(this.CabinRows);
        }
        [RelayCommand]
        private void ReApplyLetteringNumbering()
        {
            if (MessageService.Questions.ReapplyLetteringNumbering() == MessageBoxResult.OK)
            {
                SelectedGlass = null;
                SelectedCabinRow = null;
                glassNumberingService.ReApplyOrderGlassLetteringNumbering();
                HasUnsavedOrderEdits = true;
            }
        }
        [RelayCommand]
        private void OpenDrawsDialog()
        {
            //Find if it is already open and bring it into view else create it
            var existingWindow = Application.Current.Windows.OfType<DialogWindow>().FirstOrDefault(w => w.DataContext is SynthesisDrawModalViewModel);
            if (existingWindow is null)
            {
                SynthesisDrawModalViewModel modal = new();
                modal.SetDraws(SelectedSynthesisDraw);
                dialogService.OpenDialogAsWindow(modal);
            }
            else
            {
                existingWindow.Activate();
            }
        }

        //IF MADE TRANSIENT WE HAVE TO DISPOSE ON CLOSING WINDOW . CURRENTLY ITS SINGLETON DOES NOT NEED DISPOSING
        private bool _disposed;
        public override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                messenger.UnregisterAll(this);
                this.glassNumberingService.SetGlassesOrder(null);
                this.CabinRows.CollectionChanged -= Rows_CollectionChanged;
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            //base.Dispose(disposing);
        }

    }
}

using BronzeFactoryApplication.ApplicationServices.MessangerService;
using BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels.Parts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    /// <summary>
    /// Edits the List and Quantities of a List of Parts
    /// </summary>
    public partial class EditSubPartsModalViewModel : ModalViewModel
    {
        private IEnumerable<CabinPart> _undoStoreAdditionalParts = Enumerable.Empty<CabinPart>();
        private Dictionary<CabinIdentifier, List<CabinPart>> _undoStoreAdditionalPartsPerStructure = new();
        private EditPartViewModel? editPartVm;

        private readonly ICabinMemoryRepository repo;
        private readonly CloseModalService closeModalService;

        #region A. Normal SubParts Properties
        [ObservableProperty]
        private ObservableCollection<EditSubPartQuantityViewModel> subParts = new();
        [ObservableProperty]
        private CabinPart? newSubPartToAdd;
        [ObservableProperty]
        private double quantityOfNewSubPart;
        #endregion


        #region B. Sub Parts per Structure Properties
        /* User Selects a Draw => SelectableModels & SynthesisModels Repopulate , 
         * Then Selects Model => SelectableSynthesisModels Repopulate 
         * If Draw Changes then Model becomes Null if not Valid
         * If Model Changes then Synthesis Model becomes Null if not Valid
         */

        /// <summary>
        /// The Dictionary of All the Identifiers and Parts
        /// This needs to create a new dictionary to update , every time a single change is made to anything in the Dictionary
        /// </summary>
        [ObservableProperty]
        private Dictionary<CabinIdentifier, List<CabinPart>> subPartsPerStructure = new();

        /// <summary>
        /// The Draw of the New CabinIdentifier to be Added
        /// </summary>
        private CabinDrawNumber? selectedDraw;
        public CabinDrawNumber? SelectedDraw
        {
            get => selectedDraw;
            set
            {
                if (value != selectedDraw)
                {
                    selectedDraw = value;
                    OnPropertyChanged(nameof(SelectedDraw));

                    //If Model is not Valid Nullify it or Put the First in the List -- These First so Values Change before anyone else is informed about them
                    if (SelectableModels.Any(m => m == selectedModel) is false) SelectedModel = SelectableModels.FirstOrDefault();
                    //If SynthesisModel is not Valid Nullify it
                    if (SelectableSynthesisModels.Any(sm=> sm == SelectedSynthesisModel) is false) { SelectedSynthesisModel = SelectableSynthesisModels.FirstOrDefault(); }

                    //Update Selectables
                    OnPropertyChanged(nameof(CanSelectModel));
                    OnPropertyChanged(nameof(CanSelectSynthesisModel));
                    OnPropertyChanged(nameof(SelectableModels));
                    OnPropertyChanged(nameof(SelectableSynthesisModels));

                }
            }
        }

        /// <summary>
        /// The Model of the New CabinIdentifier to be Added
        /// </summary>
        private CabinModelEnum? selectedModel;
        public CabinModelEnum? SelectedModel 
        {
            get => selectedModel;
            set 
            {
                if (value != selectedModel)
                {
                    selectedModel = value;

                    OnPropertyChanged(nameof(SelectedModel));
                    //If Synthesis Model is not Valid Nullify it (This first so selection gets null before anyoneElse gets informed of the change)
                    if(SelectableSynthesisModels.Any(sm=> sm == SelectedSynthesisModel) is false) { SelectedSynthesisModel = SelectableSynthesisModels.FirstOrDefault(); }
                    //Update Synthesis Model Selectables
                    OnPropertyChanged(nameof(CanSelectSynthesisModel));
                    OnPropertyChanged(nameof(SelectableSynthesisModels));
                    
                }
            }
        }

        /// <summary>
        /// Weather a Model can be Selected
        /// </summary>
        public bool CanSelectModel { get => selectedDraw != null; }
        /// <summary>
        /// The Models that can Be Selected Currently (According to the Valid Identifiers)
        /// </summary>
        public IEnumerable<CabinModelEnum> SelectableModels { get => selectedDraw is not null ? repo.GetMatchingModels((CabinDrawNumber)selectedDraw) : Enumerable.Empty<CabinModelEnum>(); }
                
        /// <summary>
        /// The SynthesisModel of the New CabinIdentifier to be Added
        /// </summary>
        [ObservableProperty]
        private CabinSynthesisModel? selectedSynthesisModel;

        /// <summary>
        /// Can a SynthesisModel be Selected
        /// </summary>
        public bool CanSelectSynthesisModel { get => selectedDraw != null && selectedModel != null; }
        /// <summary>
        /// The SynthesisModels that can Be Selected Currently (According to the Valid Identifiers)
        /// </summary>
        public IEnumerable<CabinSynthesisModel> SelectableSynthesisModels { get => (selectedDraw is not null && selectedModel is not null) ? repo.GetMatchingSynthesisModels((CabinDrawNumber)selectedDraw, (CabinModelEnum)selectedModel) : Enumerable.Empty<CabinSynthesisModel>(); }

        /// <summary>
        /// The Currently Selected SubParts List of one of the Structures , that is UnderEdit
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<CabinPart> subPartsListToEdit = new();
        [ObservableProperty]
        private CabinIdentifier subPartsIdentifierToEdit = new();
        /// <summary>
        /// Weather any Structure List is Currently being Edited
        /// </summary>
        [ObservableProperty]
        private bool isAnyListUnderEdit = false;
        /// <summary>
        /// The Part that will be Added in the Structure List that gets Manipulated
        /// </summary>
        [ObservableProperty]
        private CabinPart? newStructurePartToAdd;
        /// <summary>
        /// The Quantity of the Part that will be added to the Structure List that is being manipulated
        /// </summary>
        [ObservableProperty]
        private double quantityOfNewStructurePart; 
        #endregion



        /// <summary>
        /// All the Parts Excluding the owner Part which Contains these Parts
        /// </summary>
        public IEnumerable<CabinPart> SelectableParts { get => repo.GetAllParts().Where(p => p.Code != editPartVm?.Code); }

        #region CONSTRUCTOR
        public EditSubPartsModalViewModel(ICabinMemoryRepository repo, CloseModalService closeModalService)
        {
            Title = "lngSubParts".TryTranslateKey();
            this.repo = repo;
            this.closeModalService = closeModalService;
            closeModalService.ModalClosing += CloseModalService_ModalClosing;
        } 
        #endregion

        private void CloseModalService_ModalClosing(object? sender, ModalClosingEventArgs e)
        {
            if (e.ClosingModal is EditSubPartsModalViewModel)
            {
                ApplyChanges();
            }
        }

        /// <summary>
        /// Sets the List of Parts to Edit
        /// </summary>
        /// <param name="parts">The List of SubParts to Edit</param>
        /// <param name="ownerCode">The Code of the Part that Owns the SubParts</param>
        public void SetEditPartViewModel(EditPartViewModel editPartVm)
        {
            this.editPartVm = editPartVm;
            Title = $"{Title} - {editPartVm.GetTranslation(((App)App.Current).SelectedLanguage)} : {editPartVm.Code}";
            _undoStoreAdditionalParts = editPartVm.AdditionalParts;
            _undoStoreAdditionalPartsPerStructure = editPartVm.AdditionalPartsPerStructure;
            SubParts = new(_undoStoreAdditionalParts.Select(p => new EditSubPartQuantityViewModel(p)));
            SubPartsPerStructure = _undoStoreAdditionalPartsPerStructure.ToDictionary(kvp=> kvp.Key,kvp=> new List<CabinPart>(kvp.Value.Select(p=>p.GetDeepClone())));
        }

        #region 1.SUB PART COMMANDS
        [RelayCommand]
        private void RemoveSubPart(EditSubPartQuantityViewModel subPart)
        {
            SubParts.Remove(subPart);
        }
        [RelayCommand]
        private void AddSubPart()
        {
            if (NewSubPartToAdd is not null && QuantityOfNewSubPart != 0)
            {
                //Pass first the Selected Quantity then add it
                var partToAdd = NewSubPartToAdd.GetDeepClone();
                partToAdd.Quantity = QuantityOfNewSubPart;
                SubParts.Add(new(partToAdd));
                NewSubPartToAdd = null;
            }
            else
            {
                MessageService.Warning("Please Add a Valid SubPart with Quantity > 0", "Invalid Part");
            }
        }
        #endregion

        #region 2.SUB PART PER STRUCTURE COMMANDS

        [RelayCommand]
        private void AddStructure()
        {
            if (SelectedModel is null || SelectedDraw is null || SelectedSynthesisModel is null)
            {
                MessageService.Warning("Please select All three (Model-Draw-Synthesis Model)", "Failed to Add Structure...");
                return;
            }
            
            CabinIdentifier identifier = new((CabinModelEnum)SelectedModel, (CabinDrawNumber)SelectedDraw, (CabinSynthesisModel)SelectedSynthesisModel);
            
            if (SubPartsPerStructure.ContainsKey(identifier))
            {
                MessageService.Warning($"There is Already a Structure with the Selected Options :{Environment.NewLine}{identifier}","Failed to Add Structure...");
                return;
            }

            //Copy the Current Dictionary to a new , Add the Value and Set it to the ViewModel so it can be Updated
            Dictionary<CabinIdentifier, List<CabinPart>> newDict = new(SubPartsPerStructure)
            {
                { identifier, new() }
            };
            SubPartsPerStructure = newDict;
        }
        [RelayCommand]
        private void RemoveStructure(CabinIdentifier identifier)
        {
            if (SubPartsPerStructure.ContainsKey(identifier))
            {
                Dictionary<CabinIdentifier, List<CabinPart>> newDict = new(SubPartsPerStructure);
                newDict.Remove(identifier);
                SubPartsPerStructure = newDict;
            }
            else
            {
                MessageService.Error($"The Selected Idintifier was not Found... The List could not get Removed{Environment.NewLine}{identifier}", "Failed to Remove...");
            }
        }
        /// <summary>
        /// Stops Editing List and Scraps changes to the Structure
        /// </summary>
        [RelayCommand]
        private void StopStructureListEdit()
        {
            SubPartsListToEdit = new();
            IsAnyListUnderEdit = false;
        }
        /// <summary>
        /// Selects a Structure List for Edit
        /// </summary>
        /// <param name="identifier"></param>
        [RelayCommand]
        private void EditStructureList(CabinIdentifier identifier)
        {
            if (SubPartsPerStructure.ContainsKey(identifier))
            {
                SubPartsIdentifierToEdit = identifier;
                //Assign the List of Parts to a new List so the UI Can Update (Use Clones for the Parts)
                SubPartsListToEdit = new(SubPartsPerStructure[identifier].Select(p=>p.GetDeepClone()));
                //Open The List
                IsAnyListUnderEdit = true; 
            }
            else
            {
                MessageService.Error($"The Selected Sturcture was not Found... {identifier}","Failure to Edit...");
            }
        }
        /// <summary>
        /// Adds a Part to the Structure List that is Currently being under edit
        /// </summary>
        [RelayCommand]
        private void AddPartToStructureList()
        {
            if (NewStructurePartToAdd is null)
            {
                MessageService.Warning("The Currently Selected Part is Empty,Please Choose a Valid Part", "Failure to Add");
                return;
            }
            if (QuantityOfNewStructurePart < 0.5d)
            {
                MessageService.Warning("The Quantity of the Selected Part cannot be Less than 0.5", "Failure to Add");
                return;
            }
            if (SubPartsListToEdit.Any(p=> p.Code == NewStructurePartToAdd.Code))
            {
                MessageService.Warning($"The Part you Are Trying to Add is Already in the List{Environment.NewLine}If you wish to Modify the Quantity , Remove it and add a new one", "Failure to Add");
                return;
            }
            var partToAdd = NewStructurePartToAdd.GetDeepClone();
            NewStructurePartToAdd = null;
            partToAdd.Quantity = QuantityOfNewStructurePart;
            SubPartsListToEdit.Add(partToAdd);
        }
        /// <summary>
        /// Removes a Part from the Structure List that is Currently being under edit
        /// </summary>
        [RelayCommand]
        private void RemovePartFromStructureList(CabinPart part)
        {
            if (!SubPartsListToEdit.Contains(part))
            {
                MessageService.Error($"Selected part {part?.Code} was not Found , Removal Failed...", "Removal Failed...");
                return;
            }
            SubPartsListToEdit.Remove(part);
        }
        /// <summary>
        /// Applies the Changes to the StructueList that is Being Edited and Passes it to the Dictionary of the Structured Lists
        /// </summary>
        [RelayCommand]
        private void ApplyStructureListEdit()
        {
            if (!SubPartsPerStructure.ContainsKey(SubPartsIdentifierToEdit))
            {
                MessageService.Error("The Structure Trying to Be Saved was not Found in the DictionaryList....", "This Failure Should never Happen...");
                return;
            }

            //Create a Copy of all the Dictionary (Sucks... But otherwise does not propagate changes unless we make n ObservableDictionary but cba...)
            var newDict = SubPartsPerStructure.ToDictionary(kvp => kvp.Key, kvp => new List<CabinPart>(kvp.Value.Select(p => p.GetDeepClone())));
            //Replace the List that there was there before with the new Edited List by Cloning all parts of it
            newDict[SubPartsIdentifierToEdit] = new(SubPartsListToEdit.Select(p=> p.GetDeepClone()));
            //Update the Dictionary
            SubPartsPerStructure = newDict;
            //Close the Edit Structure
            StopStructureListEdit();
        }
        #endregion


        [RelayCommand]
        private void Undo()
        {
            SubParts = new(_undoStoreAdditionalParts.Select(p => new EditSubPartQuantityViewModel(p)));
            SubPartsPerStructure = _undoStoreAdditionalPartsPerStructure.ToDictionary(kvp => kvp.Key, kvp => new List<CabinPart>(kvp.Value.Select(p => p.GetDeepClone())));
        }
        /// <summary>
        /// Apply Changes to the ViewModel that is being edited
        /// </summary>
        private void ApplyChanges()
        {
            if (editPartVm is not null)
            {
                editPartVm.AdditionalParts = new(SubParts.Select(p => p.GetPartObject()));
                editPartVm.AdditionalPartsPerStructure = SubPartsPerStructure.ToDictionary(kvp => kvp.Key, kvp => new List<CabinPart>(kvp.Value.Select(p => p.GetDeepClone())));
            }
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
                closeModalService.ModalClosing -= CloseModalService_ModalClosing;
            }

            //object has been disposed
            _disposed = true;
            base.Dispose(disposing);

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            //base.Dispose(disposing);
        }
    }

    public partial class EditSubPartQuantityViewModel : BaseViewModel
    {
        private readonly CabinPart subPart;
        public string Code { get => subPart.Code; }
        public string Description { get => subPart.Description; }
        public string PhotoPath { get => subPart.PhotoPath; }
        public double Quantity { get => subPart.Quantity; set => SetProperty(subPart.Quantity, value, subPart, (m, v) => m.Quantity = v); }
        public EditSubPartQuantityViewModel(CabinPart subPart)
        {
            this.subPart = subPart.GetDeepClone();
        }

        public CabinPart GetPartObject()
        {
            return subPart;
        }
    }


}

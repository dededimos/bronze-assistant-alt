using Azure;
using BronzeFactoryApplication.ApplicationServices.DataService;
using BronzeFactoryApplication.ApplicationServices.ExcelXlsService;
using BronzeFactoryApplication.Properties;
using BronzeFactoryApplication.ViewModels.CabinsViewModels.ConstraintsViewModels;
using BronzeFactoryApplication.ViewModels.CabinsViewModels.ModelsViewModels.SettingsViewModels;
using BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels.DefaultPartsLists;
using BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels.Parts;
using CommonHelpers;
using DataAccessLib;
using DataAccessLib.MongoDBAccess;
using DataAccessLib.NoSQLModels;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using HandyControl.Tools.Extension;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BronzeFactoryApplication.ViewModels;

public partial class ManagmentViewModel : BaseViewModel, IOperationOnNavigatingAway
{
    //Locks to Change Observable Collection on the UI Thread
    private readonly object _constraintsLock = new();
    private readonly object _partsLock = new();
    private readonly object _partsListsLock = new();
    private readonly object _settingsLock = new();

    private readonly OpenEditSubPartsModal openEditSubPartsModal;
    private readonly OpenEditPartSetsModalService openEditPartSetsModal;

    //The Repos that Provide db Crud per MongoCollection
    private readonly ICabinConstraintsRepository constraintsDb;
    private readonly ICabinPartsRepository partsDb;
    private readonly ICabinPartsListsRepository partsListsDb;
    private readonly ICabinSettingsRepository settingsDb;
    private readonly ICabinMemoryRepository memoryRepo;
    private readonly IMongoSessionHandler sessionHandler;
    private readonly PartSetsApplicatorService partSetsApplicatorService;

    /// <summary>
    /// Has a Singleton Lifetime and does not get Disposed during the applications Lifetime
    /// </summary>
    public override bool IsDisposable => false;

    /// <summary>
    /// The Selected Tab Index (so it can be Stored and Not change each time the View Changes)
    /// </summary>
    [ObservableProperty]
    private int selectedTabIndex;

    /// <summary>
    /// Weather the ViewModel Has Changed any of the Default Values of Constraints/Parts/PartsLists e.t.c.
    /// </summary>
    [ObservableProperty]
    private bool hasChangedFromDefaults = false;

    #region 1. CONSTRAINTS

    private CabinConstraintsEntity? selectedConstraints;
    private ConstraintsViewModel? selectedConstraintsViewModel;

    /// <summary>
    /// The Constraints entities as they Appear in The Database
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredConstraints))]
    private ObservableCollection<CabinConstraintsEntity> constraints = new();

    public ObservableCollection<CabinConstraintsEntity> FilteredConstraints { get => GetConstraintsFilteredCollection(); }
    private ObservableCollection<CabinConstraintsEntity> GetConstraintsFilteredCollection()
    {
        //Filter with Model
        IEnumerable<CabinConstraintsEntity> list = ConstraintsFilterModel is not null
            ? Constraints.Where(c => c.Model == ConstraintsFilterModel)
            : Constraints;

        //Filter with Draw
        list = ConstraintsFilterDraw is not null
            ? list.Where(c => c.DrawNumber == ConstraintsFilterDraw)
            : list;

        //If there are any FOund return them otherwise return everything
        return list.Any() ? new(list) : Constraints;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredConstraints))]
    private CabinModelEnum? constraintsFilterModel;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredConstraints))]
    private CabinDrawNumber? constraintsFilterDraw;

    /// <summary>
    /// The Selected Constraints Entity
    /// </summary>
    public CabinConstraintsEntity? SelectedConstraints
    {
        get => selectedConstraints;
        set
        {
            if (selectedConstraints != value)
            {
                if (HasUnsavedConstraintsEdits && ShouldNotScrapChanges())
                {
                    return;
                }
                selectedConstraints = value;
                OnPropertyChanged(nameof(SelectedConstraints));
                OnPropertyChanged(nameof(IsConstraintSelected));
                SelectedConstraintsViewModel = selectedConstraints?.Constraints is not null
                                               ? GetConstraintsVM(selectedConstraints.Constraints.GetDeepClone())
                                               : null;

            }
        }
    }
    /// <summary>
    /// The ViewModel of the Selected Constraints Entity
    /// </summary>
    public ConstraintsViewModel? SelectedConstraintsViewModel
    {
        get => selectedConstraintsViewModel;
        private set
        {
            if (selectedConstraintsViewModel is not null)
            {
                //Unhook previous selected ViewModel
                selectedConstraintsViewModel.PropertyChanged -= SelectedConstraintsViewModel_PropertyChanged;
            }

            selectedConstraintsViewModel = value;

            if (selectedConstraintsViewModel is not null)
            {
                //Hook to the New ViewModel
                selectedConstraintsViewModel.PropertyChanged += SelectedConstraintsViewModel_PropertyChanged;
            }

            OnPropertyChanged(nameof(SelectedConstraintsViewModel));
        }
    }
    /// <summary>
    /// Weather the Current Selected Constraints ViewModel has Unsaved Edits
    /// </summary>
    [ObservableProperty]
    private bool hasUnsavedConstraintsEdits;

    /// <summary>
    /// Weather a constraint has been selected or not
    /// </summary>
    public bool IsConstraintSelected { get => selectedConstraints != null; }

    #endregion

    #region 2. PARTS
    private CabinPartEntity? selectedPart;
    private EditPartViewModel? selectedPartViewModel;

    /// <summary>
    /// All the Parts Entities as Retrieved from the Database
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredParts))]
    private ObservableCollection<CabinPartEntity> parts = new();


    /// <summary>
    /// The Filtered List of the Parts Entities , According to the Provided Filters
    /// </summary>
    public ObservableCollection<CabinPartEntity> FilteredParts { get => GetPartsFilteredCollection(); }
    /// <summary>
    /// Retrieved the Filtered Collection according to the Provided Parts Filters from the User
    /// </summary>
    /// <returns></returns>
    private ObservableCollection<CabinPartEntity> GetPartsFilteredCollection()
    {
        //Filter with Type
        IEnumerable<CabinPartEntity> list = PartsFilterType is not null
            ? Parts.Where(p => p.Part.Part == PartsFilterType)
            : Parts;

        //Filter with Code
        list = PartsFilterCode is not null
            ? list.Where(p => p.Part.Code.ToUpperInvariant().Contains(PartsFilterCode.ToUpperInvariant()))
            : list;

        //If there are any FOund return them otherwise return everything
        return list.Any() ? new(list) : Parts;
    }

    /// <summary>
    /// The Parts Type Filter
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredParts))]
    private CabinPartType? partsFilterType;
    /// <summary>
    /// The Parts Code Filter
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredParts))]
    private string? partsFilterCode;

    /// <summary>
    /// The Selected Part Entity
    /// </summary>
    public CabinPartEntity? SelectedPart
    {
        get => selectedPart;
        set
        {
            if (selectedPart != value)
            {
                if (HasUnsavedPartEdits && ShouldNotScrapChanges())
                {
                    return;
                }
                selectedPart = value;
                OnPropertyChanged(nameof(SelectedPart));
                SelectedPartViewModel = GetEditPartVm();
                OnPropertyChanged(nameof(IsPartSelected));
            }
        }
    }

    /// <summary>
    /// The Edit ViewModel of the Selected Part Entity
    /// </summary>
    public EditPartViewModel? SelectedPartViewModel
    {
        get => selectedPartViewModel;
        private set
        {
            if (selectedPartViewModel is not null)
            {
                //Unhook previous selected ViewModel
                selectedPartViewModel.PropertyChanged -= SelectedPartViewModel_PropertyChanged;
            }

            selectedPartViewModel = value;

            if (selectedPartViewModel is not null)
            {
                //Hook to the New ViewModel
                selectedPartViewModel.PropertyChanged += SelectedPartViewModel_PropertyChanged;
            }

            OnPropertyChanged(nameof(SelectedPartViewModel));
            OnPropertyChanged(nameof(IsPartSelected));
        }
    }

    /// <summary>
    /// Weather the Current Selected Part ViewModel has Unsaved Edits
    /// </summary>
    [ObservableProperty]
    private bool hasUnsavedPartEdits;

    /// <summary>
    /// Weather a Part has been selected or not
    /// </summary>
    public bool IsPartSelected { get => selectedPart != null || (selectedPartViewModel != null && selectedPartViewModel.IsEdit is false); }

    #endregion

    #region 3.PARTSLISTS
    private DefaultPartsListEntity? selectedDefaultList;
    private EditDefaultPartsViewModel? selectedDefaultListViewModel;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredDefaultLists))]
    private ObservableCollection<DefaultPartsListEntity> defaultLists = new();

    public ObservableCollection<DefaultPartsListEntity> FilteredDefaultLists { get => GetFilteredDefaultList(); }

    private ObservableCollection<DefaultPartsListEntity> GetFilteredDefaultList()
    {
        //Filter with Model
        IEnumerable<DefaultPartsListEntity> list = DefaultListsFilterModel is not null
            ? DefaultLists.Where(c => c.Model == DefaultListsFilterModel)
            : DefaultLists;

        //Filter with Draw
        list = DefaultListsFilterDraw is not null
            ? list.Where(c => c.DrawNumber == DefaultListsFilterDraw)
            : list;

        //If there are any FOund return them otherwise return everything
        return list.Any() ? new(list) : DefaultLists;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredDefaultLists))]
    private CabinModelEnum? defaultListsFilterModel;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredDefaultLists))]
    private CabinDrawNumber? defaultListsFilterDraw;

    /// <summary>
    /// The Selected PartsList Entity
    /// </summary>
    public DefaultPartsListEntity? SelectedDefaultList
    {
        get => selectedDefaultList;
        set
        {
            if (selectedDefaultList != value)
            {
                if (HasUnsavedDefaultListEdits && ShouldNotScrapChanges())
                {
                    return;
                }
                selectedDefaultList = value;
                OnPropertyChanged(nameof(SelectedDefaultList));
                OnPropertyChanged(nameof(IsDefaultListSelected));
                SelectedDefaultListViewModel = selectedDefaultList?.DefaultPartsList is not null
                                               ? new EditDefaultPartsViewModel(selectedDefaultList.DefaultPartsList, new(selectedDefaultList.Model, selectedDefaultList.DrawNumber, selectedDefaultList.SynthesisModel), memoryRepo)
                                               : null;

            }
        }
    }
    /// <summary>
    /// The ViewModel of the Selected PartsListEntity
    /// </summary>
    public EditDefaultPartsViewModel? SelectedDefaultListViewModel
    {
        get => selectedDefaultListViewModel;
        private set
        {
            if (selectedDefaultListViewModel is not null)
            {
                //Unhook previous selected ViewModel
                selectedDefaultListViewModel.PropertyChanged -= SelectedDefaultListViewModel_PropertyChanged;
            }

            selectedDefaultListViewModel = value;

            if (selectedDefaultListViewModel is not null)
            {
                //Hook to the New ViewModel
                selectedDefaultListViewModel.PropertyChanged += SelectedDefaultListViewModel_PropertyChanged;
            }

            OnPropertyChanged(nameof(SelectedDefaultListViewModel));
        }
    }

    /// <summary>
    /// Weather the Current Selected DefaultList ViewModel has Unsaved Edits
    /// </summary>
    [ObservableProperty]
    private bool hasUnsavedDefaultListEdits;
    /// <summary>
    /// Weather a Default List has been selected or not
    /// </summary>
    public bool IsDefaultListSelected { get => selectedDefaultList != null; }

    #endregion

    #region 4.CABIN SETTINGS
    private CabinSettingsEntity? selectedSettings;
    private EditCabinSettingsViewModel? selectedSettingsViewModel;

    /// <summary>
    /// All the Settings Entities as Retrieved from the Database
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredSettings))]
    private ObservableCollection<CabinSettingsEntity> settings = new();

    /// <summary>
    /// The Filtered List of the Settings Entities , According to the Provided Filters
    /// </summary>
    public ObservableCollection<CabinSettingsEntity> FilteredSettings { get => GetSettingsFilteredCollection(); }
    /// <summary>
    /// Retrieved the Filtered Collection according to the Provided Settings Filters from the User
    /// </summary>
    /// <returns></returns>
    private ObservableCollection<CabinSettingsEntity> GetSettingsFilteredCollection()
    {
        //Filter with Model
        IEnumerable<CabinSettingsEntity> list = SettingsFilterModel is not null
            ? Settings.Where(c => c.Model == SettingsFilterModel)
            : Settings;

        //Filter with Draw
        list = SettingsFilterDraw is not null
            ? list.Where(c => c.DrawNumber == SettingsFilterDraw)
            : list;

        //If there are any FOund return them otherwise return everything
        return list.Any() ? new(list) : Settings;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredSettings))]
    private CabinModelEnum? settingsFilterModel;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredSettings))]
    private CabinDrawNumber? settingsFilterDraw;

    /// <summary>
    /// The Selected Settings Entity
    /// </summary>
    public CabinSettingsEntity? SelectedSettings
    {
        get => selectedSettings;
        set
        {
            if (selectedSettings != value)
            {
                if (HasUnsavedSettingsEdits && ShouldNotScrapChanges())
                {
                    return;
                }
                selectedSettings = value;
                OnPropertyChanged(nameof(SelectedSettings));
                OnPropertyChanged(nameof(IsSettingsSelected));
                SelectedSettingsViewModel = selectedSettings?.Settings is not null
                                               ? new EditCabinSettingsViewModel(selectedSettings)
                                               : null;

            }
        }
    }

    /// <summary>
    /// The ViewModel of the Selected Settings Entity
    /// </summary>
    public EditCabinSettingsViewModel? SelectedSettingsViewModel
    {
        get => selectedSettingsViewModel;
        private set
        {
            if (selectedSettingsViewModel is not null)
            {
                //Unhook previous selected ViewModel
                selectedSettingsViewModel.PropertyChanged -= SelectedSettingsViewModel_PropertyChanged;
            }

            selectedSettingsViewModel = value;

            if (selectedSettingsViewModel is not null)
            {
                //Hook to the New ViewModel
                selectedSettingsViewModel.PropertyChanged += SelectedSettingsViewModel_PropertyChanged;
            }

            OnPropertyChanged(nameof(SelectedSettingsViewModel));
        }
    }

    /// <summary>
    /// Weather the Current Selected Settings ViewModel has Unsaved Edits
    /// </summary>
    [ObservableProperty]
    private bool hasUnsavedSettingsEdits;

    /// <summary>
    /// Weather a constraint has been selected or not
    /// </summary>
    public bool IsSettingsSelected { get => selectedSettings != null; }

    #endregion

    #region 5.CONSTRUCTOR
    public ManagmentViewModel(
    OpenEditSubPartsModal openEditSubPartsModal,
    OpenEditPartSetsModalService openEditPartSetsModal,
    PartSetsApplicatorService partSetsApplicatorService,
    ICabinConstraintsRepository constraintsDb,
    ICabinPartsRepository partsDb,
    ICabinMemoryRepository memoryRepo,
    ICabinPartsListsRepository partsListsDb,
    ICabinSettingsRepository settingsDb,
    IMongoSessionHandler sessionHandler)
    {
        this.sessionHandler = sessionHandler;
        this.openEditSubPartsModal = openEditSubPartsModal;
        this.openEditPartSetsModal = openEditPartSetsModal;
        this.partSetsApplicatorService = partSetsApplicatorService;
        this.constraintsDb = constraintsDb;
        this.partsDb = partsDb;
        this.memoryRepo = memoryRepo;
        this.partsListsDb = partsListsDb;
        this.settingsDb = settingsDb;
        BindingOperations.EnableCollectionSynchronization(Constraints, _constraintsLock);
        BindingOperations.EnableCollectionSynchronization(Parts, _partsLock);
        BindingOperations.EnableCollectionSynchronization(Parts, _partsListsLock);
        BindingOperations.EnableCollectionSynchronization(Settings, _settingsLock);
        PropertyChanged += OnSelectedIndexChanged;
    }

    private async void OnSelectedIndexChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(SelectedTabIndex))
        {
            await OnNavigatingAwayOperation();
            
            //Reload Db Items if there were already loaded before , so that they get the new Values
            switch (SelectedTabIndex)
            {
                case 0:
                    if (Parts.Any()) await LoadParts();
                    break;
                case 1:
                    if (Settings.Any()) await LoadSettings();
                    break;
                case 2:
                    if (Constraints.Any()) await LoadConstraints();
                    break;
                case 3:
                    if (DefaultLists.Any()) await LoadDefaultPartsLists();
                    break;
                default:
                    break;
            }
        }
    }
    #endregion

    /// <summary>
    /// Whenever a Property Changes in Constraints , Mark that there are unsaved Edits.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SelectedConstraintsViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        HasUnsavedConstraintsEdits = true;
    }
    /// <summary>
    /// Whenever a Property changed in a Part , Mark that there are unsaved Edits.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SelectedPartViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        HasUnsavedPartEdits = true;
    }
    /// <summary>
    /// Whenever a Property changed in a DefaultPartsList , Mark that there are unsaved Edits.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SelectedDefaultListViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        HasUnsavedDefaultListEdits = true;
    }
    private void SelectedSettingsViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        HasUnsavedSettingsEdits = true;
    }

    /// <summary>
    /// Asks Weather it should Scrap Changes and Proceeds Accordingly
    /// </summary>
    /// <returns></returns>
    private bool ShouldNotScrapChanges()
    {
        var result = MessageService.Question("lngUnsavedChangesExist".TryTranslateKey(), "lngUnsavedChanges".TryTranslateKey(), "lngContinue".TryTranslateKey(), "lngDoNotContinue".TryTranslateKey());
        if (result is MessageBoxResult.Cancel)
        {
            return true;
        }
        else
        {
            HasUnsavedConstraintsEdits = false;
            HasUnsavedPartEdits = false;
            HasUnsavedDefaultListEdits = false;
            HasUnsavedSettingsEdits = false;
            return false;
        }
    }

    /// <summary>
    /// Returns a Constraints ViewModel for the requested Constraints Object
    /// </summary>
    /// <param name="constraints">The Constraints for Which a ViewModel is Needed</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException">When Not Implemented ViewModel for the Selected Constraints</exception>
    public static ConstraintsViewModel GetConstraintsVM(CabinConstraints constraints)
    {
        ConstraintsViewModel constraintsVm = constraints switch
        {
            Constraints9S => new Constraints9SViewModel(),
            Constraints94 => new Constraints94ViewModel(),
            Constraints9A => new Constraints9AViewModel(),
            Constraints9B => new Constraints9BViewModel(),
            Constraints9F => new Constraints9FViewModel(),
            Constraints9C => new Constraints9CViewModel(),
            ConstraintsDB => new ConstraintsDBViewModel(),
            ConstraintsE => new ConstraintsEViewModel(),
            ConstraintsW => new ConstraintsWViewModel(),
            ConstraintsWFlipper => new ConstraintsWFlipperViewModel(),
            ConstraintsHB => new ConstraintsHBViewModel(),
            ConstraintsVA => new ConstraintsVAViewModel(),
            ConstraintsVS => new ConstraintsVSViewModel(),
            ConstraintsV4 => new ConstraintsV4ViewModel(),
            ConstraintsVF => new ConstraintsVFViewModel(),
            ConstraintsNB => new ConstraintsNBViewModel(),
            ConstraintsNP => new ConstraintsNPViewModel(),
            ConstraintsWS => new ConstraintsWSViewModel(),
            _ => throw new NotImplementedException("Requested Constraints Class Does not have an Implemented ViewModel"),
        };
        constraintsVm.SetNewConstraints(constraints);
        return constraintsVm;
    }

    /// <summary>
    /// Returns an Edit Part ViewModel for the Requested PartObject Selected
    /// </summary>
    /// <returns></returns>
    private EditPartViewModel? GetEditPartVm()
    {
        return (selectedPart?.Part) switch
        {
            CabinAngle => new EditAngleViewModel(selectedPart),
            CabinHandle => new EditHandleViewModel(selectedPart),
            CabinHinge => new EditHingeViewModel(selectedPart),
            Profile => new EditProfileViewModel(selectedPart, memoryRepo.GetStrips(CabinStripType.PolycarbonicMagnet,new()).Select(s => s.Code)),
            GlassStrip => new EditStripViewModel(selectedPart),
            CabinSupport => new EditSupportViewModel(selectedPart),
            SupportBar => new EditSupportBarViewModel(selectedPart),
            null => null,
            _ => new EditPartViewModel(selectedPart ?? new CabinPartEntity(new CabinPart() { Code = "ERROR", Description = "Error" },new(), new())),
        };
    }

    /// <summary>
    /// Validates a Default List for Saving (Maybe this should be transfered for Validation Inside the Repository Saving Method)
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    private bool IsDefaultListValidForSaving(DefaultPartsList list)
    {
        //1.Check that there is no NON optional Spot with empty Values
        bool areOptionalsNotFilled = list.SpotDefaults.Values.Any(d =>
        !d.IsOptional && (string.IsNullOrWhiteSpace(d.DefaultCode) || d.ValidCodes.Any() is false));
        if (areOptionalsNotFilled) MessageService.Warning("Cannot Save when Spot is Not Optional and DefaultCode is Empty or Selectable Codes Are Empty", "lngFailure".TryTranslateKey());

        //2.Check that there are no Empty entries in the ValidCodes of each Default 
        bool areValidCodesValidParts = list.SpotDefaults.Values.All(d =>
        d.ValidCodes.All(c => !string.IsNullOrEmpty(c)));
        if (!areValidCodesValidParts) MessageService.Warning("There are Codes that Are Empty in the Selectable Codes of the Default Lists Trying to be Saved", "lngFailure".TryTranslateKey());


        return !areOptionalsNotFilled && areValidCodesValidParts;
    }

    #region A. Save To Db Commands

    /// <summary>
    /// Updates a Part Edit on the Database
    /// </summary>
    /// <returns></returns>
    private async Task SavePartEdit()
    {
        if (selectedPartViewModel is null || selectedPart is null)
        {
            return;
        }

        if (DefaultLists.Any() is false) await LoadDefaultPartsLists();

        IsBusy = true;
        var mainPartEntityUnderEdit = selectedPartViewModel.GetPartEntity();
        mainPartEntityUnderEdit.Uses = new(selectedPart.Uses); //Pass also the Uses to the Part , otherwise they will be wiped in the updating

        //Declare a Store for the Entities that will need to change after the Transaction has Commited (in the Transactions only clones are passed to the Database)
        IEnumerable<CabinPartEntity> partEntitiesContainingEditedPart = Enumerable.Empty<CabinPartEntity>();

        //Start a Transaction Session because if the Code Changes we need to change also the Default Lists
        using (IClientSessionHandle session = await sessionHandler.StartSessionAsync())
        {
            try
            {
                session.StartTransaction();
                //Update the Part that has Changed
                var operation = await partsDb.UpdatePartAsync(mainPartEntityUnderEdit, session);
                if (operation.HasFailed) throw new Exception(operation.FailureMessage);

                //If the Part code has changed then proceed with changing it also in all Default Lists that is present
                if (selectedPart.Part.Code != mainPartEntityUnderEdit.Part.Code)
                {
                    var defaultListsWherePartIsPresent = DefaultLists.Where(l => mainPartEntityUnderEdit.Uses.Any(use => use.Identifier == l.GetIdentifier()));
                    foreach (var list in defaultListsWherePartIsPresent)
                    {
                        var listToChange = list.DefaultPartsList.GetDeepClone();
                        listToChange.ReplaceCodeWithNew(selectedPart.Part.Code, mainPartEntityUnderEdit.Part);
                        var operationUpdate = await partsListsDb.UpdatePartsListAsync(listToChange, list.GetIdentifier(), session);
                        if (operationUpdate.HasFailed) throw new Exception(operation.FailureMessage, operation.Exception);
                    }
                }

                //If this part is an Additional Part in any of the other parts . Those parts must be equally updated
                //To reflect the change in this
                //Find those Parts then
                partEntitiesContainingEditedPart = Parts.Where(p => p.HasAdditionalPart(selectedPart.Part.Code));

                //If Any of the Parts which contain as Additional The Part is the Part Itself then Throw
                if (partEntitiesContainingEditedPart.Any(entit => entit.Part.Code == selectedPart.Part.Code)) throw new Exception($"A part cannot contain as Additional Part ITSELF : {selectedPart.Part.Code}");

                //Update their AdditionalParts List
                foreach (var ent in partEntitiesContainingEditedPart)
                {
                    //Create a new Part Entity (to not change the local one in case of Rollback)
                    CabinPartEntity clone = ent.GetDeepClone();
                    //Replace the AdditionalParts
                    clone.ReplaceAdditionalPart(selectedPart.Part.Code, mainPartEntityUnderEdit.Part);
                    //Update the part 
                    var replacePartOperation = await partsDb.UpdatePartAsync(clone, session);
                    if (replacePartOperation.HasFailed) throw new Exception(replacePartOperation.FailureMessage);
                }

                await session.CommitTransactionAsync();
                HasUnsavedPartEdits = false;
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                MessageService.LogAndDisplayException(ex);
                return;
            }
            finally
            {
                if (session.IsInTransaction) await session.AbortTransactionAsync();
                IsBusy = false;
            }

            //FIRST
            //Update all The Lists that Changed if the code has changed
            if (selectedPart.Part.Code != mainPartEntityUnderEdit.Part.Code)
            {
                lock (_partsListsLock)
                {
                    foreach (var partListEntity in DefaultLists)
                    {
                        partListEntity.DefaultPartsList.ReplaceCodeWithNew(selectedPart.Part.Code, mainPartEntityUnderEdit.Part);
                        partListEntity.LastModified = DateTime.Now;
                    }
                    //Inform the Item Source changed otherwise changes are not reflected in the Datagrid
                    OnPropertyChanged(nameof(FilteredDefaultLists));
                }
            }

            //AFTER
            //Change the Local Part to the New Modified One
            lock (_partsLock)
            {
                //Do not change the underlying entity just pass the news in without changing ref , 
                //otherwise Datagrid does not get a relay of the changes from an Observable collection ?!

                //FIRST - Change all the Parts that had their Additionals Modified
                foreach (var entityPart in partEntitiesContainingEditedPart)
                {
                    entityPart.ReplaceAdditionalPart(selectedPart.Part.Code, mainPartEntityUnderEdit.Part);
                }

                //AFTER - Change the part that was edited
                selectedPart.Part = mainPartEntityUnderEdit.Part;
                selectedPart.AdditionalPartsPerStructure = mainPartEntityUnderEdit.AdditionalPartsPerStructure;
                selectedPart.LanguageDescriptions = mainPartEntityUnderEdit.LanguageDescriptions;
                selectedPart.Notes = mainPartEntityUnderEdit.Notes;
                selectedPart.LastModified = DateTime.Now;

                OnPropertyChanged(nameof(FilteredParts));
            }

            //Mark the ViewModel as Having Changed Defaults
            HasChangedFromDefaults = true;

            MessageService.Info("lngSaveSuccessful".TryTranslateKey(), "lngInformation".TryTranslateKey());
        }
    }
    /// <summary>
    /// Saves a New Part to the Database
    /// </summary>
    /// <returns></returns>
    private async Task SaveNewPart()
    {
        try
        {
            IsBusy = true;
            if (selectedPartViewModel is not null)
            {
                var entity = selectedPartViewModel.GetPartEntity();
                var operation = await partsDb.InsertNewPartAsync(entity.Part,entity.AdditionalPartsPerStructure, entity.LanguageDescriptions, entity.Notes, new());

                if (operation.IsSuccessful)
                {
                    HasUnsavedPartEdits = false;

                    //Assign the Id to the New Entity , so a reference exists also to the current loaded Results
                    entity.Id = operation.Result;

                    lock (_partsLock)
                    {
                        Parts.Add(entity);
                    }

                    //Mark the ViewModel as Having Changed Defaults
                    HasChangedFromDefaults = true;
                    HasUnsavedPartEdits = false;
                    //Change the ViewModel to Edit Mode as the Part has Been Saved
                    selectedPartViewModel.ChangeToEditMode();

                    MessageService.Info("lngSaveSuccessful".TryTranslateKey(), "lngInformation".TryTranslateKey());
                }
                else
                {
                    MessageService.Warning(operation.FailureMessage, "lngSaveFailure".TryTranslateKey());
                }
            }
        }
        catch (Exception ex)
        {
            MessageService.Warning(ex.Message, "Unknown Save Error");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SavePart()
    {
        if (!HasUnsavedPartEdits || selectedPartViewModel is null)
        {
            return;
        }

        if (selectedPartViewModel.IsEdit) await SavePartEdit();
        else await SaveNewPart();

    }

    /// <summary>
    /// Updates a Constraints Edit on the Database
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task SaveConstraintEdit()
    {
        if (!HasUnsavedConstraintsEdits)
        {
            return;
        }
        try
        {
            IsBusy = true;
            if (selectedConstraintsViewModel?.GetConstraintsObject() is not null && selectedConstraints is not null)
            {
                var operation = await constraintsDb.UpdateConstraintAsync(selectedConstraintsViewModel.GetConstraintsObject()!, selectedConstraints.Model, selectedConstraints.DrawNumber, selectedConstraints.SynthesisModel);

                if (operation.IsSuccessful)
                {
                    HasUnsavedConstraintsEdits = false;

                    //Change the Local List of Constraints to appear correctly 
                    selectedConstraints.Constraints = selectedConstraintsViewModel.GetConstraintsObject()!;
                    selectedConstraints.LastModified = DateTime.Now;

                    //Mark the ViewModel as Having Changed Defaults
                    HasChangedFromDefaults = true;

                    MessageService.Info("lngSaveSuccessful".TryTranslateKey(), "lngInformation".TryTranslateKey());
                }
                else
                {
                    MessageService.Warning(operation.FailureMessage, "lngSaveFailure".TryTranslateKey());
                }
            }
        }
        catch (Exception ex)
        {
            MessageService.Warning(ex.Message, "Unknown Save Error");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Updates a DefaultPartsList on the Database and the Uses of a Part at the same time in a single Transaction
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task SaveDefaultListEdit()
    {

        #region 1.Check Unsaved Data and Nullity
        //Check that there are actual NonSaved Data 
        if (!HasUnsavedDefaultListEdits)
        {
            return;
        }
        if (selectedDefaultList is null)
        {
            MessageService.Info("lngNoSelectionHasBeenMadeForSaving".TryTranslateKey(), "lngNothingSelected".TryTranslateKey());
            return;
        }
        #endregion

        #region 2.Grab the Default List for Modification and Validate
        //The New Default List as modified in the ViewModel
        var newDefault = selectedDefaultListViewModel!.GetDefaultList();
        if (!IsDefaultListValidForSaving(newDefault)) return;
        #endregion

        IsBusy = true;

        #region 3.Find Parts Newly used or ones that must be Dep[recated
        //Get all the codes that are being used by the new DefaultList.
        var newDefaultAllCodes = newDefault.GetUsedCodes();
        var oldDefaultAllCodes = selectedDefaultList.DefaultPartsList.GetUsedCodes();

        //The Codes present in the old list but not in the new
        var codesDeletedInNewDefault = oldDefaultAllCodes.Where(c => !newDefaultAllCodes.Contains(c));

        //The Codes present in the new List but not in the Old
        var newCodes = newDefaultAllCodes.Where(c => !oldDefaultAllCodes.Contains(c));

        //Create a new CabinIdentifier Matching the Model/Draw/Synthesis of the Default List being Saved
        CabinIdentifier modifiedDefaultsIdentifier = new(
            selectedDefaultList.Model,
            selectedDefaultList.DrawNumber,
            selectedDefaultList.SynthesisModel);


        if (Parts.Count < 1) await LoadParts(); //Laod Parts if not loaded

        //Find all Parts where the new Uses should be updated
        var allPartsToInsertUse = Parts.Where(e => newCodes.Contains(e.Part.Code))
                                       .Where(e => e.Uses.Any(i => i.Identifier == modifiedDefaultsIdentifier) is false);
        //Find all the Parts where the old Uses should be deleted 
        var allPartsToRemoveUse = Parts.Where(e => codesDeletedInNewDefault.Contains(e.Part.Code))
                                      .Where(e => e.Uses.Any(i => i.Identifier == modifiedDefaultsIdentifier));
        #endregion

        //Start Transaction session here then pass it in all the CRUD Methods
        using (IClientSessionHandle session = await sessionHandler.StartSessionAsync())
        {
            try
            {
                session.StartTransaction();

                #region 4. Update the Default Parts List
                //Update the Edited List to the Database
                var operation = await partsListsDb.UpdatePartsListAsync(newDefault, modifiedDefaultsIdentifier);
                if (operation.HasFailed) throw new Exception(operation.FailureMessage, operation.Exception);
                #endregion


                //Log the Part Uses that are being modified (new and deleted)
                Log.Information("Parts with new use {count}", allPartsToInsertUse.Count());
                Log.Information("Parts with removed use {count}", allPartsToRemoveUse.Count());

                #region 5.Insert the New Uses
                //Find all parts for which to Insert Use and Update them                
                foreach (var entity in allPartsToInsertUse)
                {
                    //Create a new Entity with the Added Use
                    CabinPartEntity newEntity = new(entity.Part,entity.AdditionalPartsPerStructure, entity.LanguageDescriptions)
                    {
                        //Otherwise it will Never Find the Part to Update it 
                        Id = entity.Id,
                        //Add the New Use to the Entity that will be updated
                        Uses = new(entity.Uses.Append(new(modifiedDefaultsIdentifier))),
                        Notes = entity.Notes
                    };
                    //Update the Entity
                    var op = await partsDb.UpdatePartAsync(newEntity, session);
                    if (op.HasFailed) throw new Exception(op.FailureMessage, op.Exception);
                    Log.Information("Updated New Uses for Part : {code}", entity.Part.Code);
                }
                #endregion

                #region 6.Remove any Old Uses
                //Find all parts for which to Remove Use and Update them
                foreach (var entity in allPartsToRemoveUse)
                {
                    //Create a new Entity with the Removed Use
                    CabinPartEntity newEntity = new(entity.Part,entity.AdditionalPartsPerStructure, entity.LanguageDescriptions)
                    {
                        Id = entity.Id,
                        //Pass all the uses of the enttity being updated apart from the identitfier that has been now removed
                        Uses = new(entity.Uses.Where(u => u.Identifier != modifiedDefaultsIdentifier)),
                        Notes = entity.Notes
                    };
                    var op = await partsDb.UpdatePartAsync(newEntity, session);
                    if (op.HasFailed) throw new Exception(op.FailureMessage, op.Exception);
                    Log.Information("Updated Part removing Use : {code}", entity.Part.Code);
                }
                #endregion

                await session.CommitTransactionAsync();
                HasUnsavedDefaultListEdits = false;
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();

                MessageService.Error(ex.Message, "lngSaveFailure".TryTranslateKey());
                Log.Error(ex, "Exception Message : {message}", ex.Message);
                IsBusy = false;
                return;
            }
            finally
            {
                if (session.IsInTransaction) await session.AbortTransactionAsync();
                IsBusy = false;
            }
        }

        #region 7. Update the Local Data instead of retrieving everything again
        //Runs only when transaction has commited

        lock (_partsListsLock)
        {
            //Replace the old Default list with the Updated One
            selectedDefaultList.DefaultPartsList = newDefault;
            selectedDefaultList.LastModified = DateTime.Now;

            //Inform the Item Source changed otherwise changes are not reflected in the Datagrid
            OnPropertyChanged(nameof(FilteredDefaultLists));
        }

        lock (_partsLock)
        {
            //Add the identifier also to the local Lists when the Transaction Succeeds
            foreach (var entity in allPartsToInsertUse)
            {
                entity.Uses.Add(new(modifiedDefaultsIdentifier));
                OnPropertyChanged(nameof(FilteredParts));
            }
            // Remove the Identifier from the Local Parts where it has been removed
            foreach (var entity in allPartsToRemoveUse)
            {
                entity.Uses.RemoveWhere(u => u.Identifier == modifiedDefaultsIdentifier);
                OnPropertyChanged(nameof(FilteredParts));
            }
        }
        #endregion

        //Mark the ViewModel as Having Changed Defaults
        HasChangedFromDefaults = true;

        MessageService.Info("lngSaveSuccessful".TryTranslateKey(), "lngInformation".TryTranslateKey());
        IsBusy = false;
    }

    /// <summary>
    /// Updates a Settings Edit on the Database
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task SaveSettingsEdit()
    {
        if (!HasUnsavedSettingsEdits)
        {
            return;
        }
        try
        {
            IsBusy = true;
            var newSettings = selectedSettingsViewModel?.GetSettingsObject();
            var notes = selectedSettingsViewModel?.Notes;
            if (newSettings is not null && selectedSettings is not null && notes is not null)
            {
                var operation = await settingsDb.UpdateSettingAsync(newSettings, selectedSettings.Model, selectedSettings.DrawNumber, selectedSettings.SynthesisModel, notes);

                if (operation.IsSuccessful)
                {
                    HasUnsavedSettingsEdits = false;

                    //Change the Local List of Constraints to appear correctly 
                    selectedSettings.Settings = newSettings;
                    selectedSettings.Notes = notes;
                    selectedSettings.LastModified = DateTime.Now;

                    //Mark the ViewModel as Having Changed Defaults
                    HasChangedFromDefaults = true;

                    MessageService.Info("lngSaveSuccessful".TryTranslateKey(), "lngInformation".TryTranslateKey());
                }
                else
                {
                    MessageService.Warning(operation.FailureMessage, "lngSaveFailure".TryTranslateKey());
                }
            }
        }
        catch (Exception ex)
        {
            MessageService.Warning(ex.Message, "Unknown Save Error");
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion

    #region B.Load from Db Commands

    /// <summary>
    /// Loads the Parts from the Database
    /// </summary>
    /// <returns></returns>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task LoadParts()
    {
        try
        {
            IsBusy = true;
            var operation = await partsDb.GetAllPartsAsync();
            if (operation.IsSuccessful)
            {
                lock (_partsLock)
                {
                    Parts = new();
                    foreach (var part in operation.Result ?? throw new Exception("Returned Results Where Null"))
                    {
                        Parts.Add(part);
                    }
                    OnPropertyChanged(nameof(FilteredParts));//To Land in the same filtered items as before Loading
                }
            }
            else
            {
                MessageService.Warning(operation.FailureMessage, "Failed to Retrieve");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "{message}", ex.Message);
            MessageService.Warning(ex.Message, "ERROR While Retrieving");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Loads all Constraints From the Databse
    /// </summary>
    /// <returns></returns>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task LoadConstraints()
    {
        try
        {
            IsBusy = true;
            var operation = await constraintsDb.GetAllConstraintsAsync();
            if (operation.IsSuccessful)
            {
                lock (_constraintsLock)
                {
                    Constraints = new();
                    foreach (var constraints in operation.Result ?? throw new Exception("Returned Results Where Null"))
                    {
                        Constraints.Add(constraints);
                    }
                    OnPropertyChanged(nameof(FilteredConstraints));//To Sync at the same filters when the Data Is Reloaded
                }
            }
            else
            {
                MessageService.Warning(operation.FailureMessage, "Failed to Retrieve");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "{message}", ex.Message);
            MessageService.Warning(ex.Message, "ERROR While Retrieving");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Loads the Parts from the Database
    /// </summary>
    /// <returns></returns>
    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task LoadDefaultPartsLists()
    {
        try
        {
            IsBusy = true;
            var operation = await partsListsDb.GetAllPartsListsAsync();
            if (operation.IsSuccessful)
            {
                lock (_partsListsLock)
                {
                    DefaultLists = new();
                    foreach (var defaultList in operation.Result ?? throw new Exception("Returned Results Where Null"))
                    {
                        DefaultLists.Add(defaultList);
                    }
                    OnPropertyChanged(nameof(FilteredDefaultLists));//To Land in the same filtered items as before Loading
                }
            }
            else
            {
                MessageService.Warning(operation.FailureMessage, "Failed to Retrieve");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "{message}", ex.Message);
            MessageService.Warning(ex.Message, "ERROR While Retrieving");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task LoadSettings()
    {
        try
        {
            IsBusy = true;
            var operation = await settingsDb.GetAllSettingsAsync();
            if (operation.IsSuccessful)
            {
                lock (_settingsLock)
                {
                    Settings = new();
                    foreach (var setting in operation.Result ?? throw new Exception("Returned Results Where Null"))
                    {
                        Settings.Add(setting);
                    }
                    OnPropertyChanged(nameof(FilteredSettings));//To Land in the same filtered items as before Loading
                }
            }
            else
            {
                MessageService.Warning(operation.FailureMessage, "Failed to Retrieve");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "{message}", ex.Message);
            MessageService.Warning(ex.Message, "ERROR While Retrieving");
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion

    #region C.Clear Filters Commands

    [RelayCommand]
    private void ClearPartsFilters()
    {
        PartsFilterType = null;
        PartsFilterCode = null;
    }

    [RelayCommand]
    private void ClearConstraintsFilters()
    {
        ConstraintsFilterModel = null;
        ConstraintsFilterDraw = null;
    }

    [RelayCommand]
    private void ClearDefaultListsFilters()
    {
        DefaultListsFilterModel = null;
        DefaultListsFilterDraw = null;
    }

    [RelayCommand]
    private void ClearSettingsFilters()
    {
        SettingsFilterModel = null;
        SettingsFilterDraw = null;
    }

    #endregion

    #region D.Create New Commands

    /// <summary>
    /// Created the ViewModel for creating a New Part
    /// </summary>
    /// <param name="partType">The Parts Type</param>
    [RelayCommand]
    private void CreateNewPart(CabinPartType partType)
    {
        if (HasUnsavedPartEdits && ShouldNotScrapChanges())
        {
            return;
        }
        SelectedPart = null;
        switch (partType)
        {
            case CabinPartType.Handle:
                SelectedPartViewModel = new EditHandleViewModel();
                break;
            case CabinPartType.Hinge:
                SelectedPartViewModel = new EditHingeViewModel();
                break;
            case CabinPartType.ProfileHinge:
                SelectedPartViewModel = new EditProfileViewModel(true);
                break;
            case CabinPartType.Profile:
                SelectedPartViewModel = new EditProfileViewModel();
                break;
            case CabinPartType.MagnetProfile:
                SelectedPartViewModel = new EditProfileViewModel() { PartType = partType };
                break;
            case CabinPartType.FloorStopperW:
                SelectedPartViewModel = new EditSupportViewModel(true);
                break;
            case CabinPartType.BarSupport:
                SelectedPartViewModel = new EditSupportBarViewModel();
                break;
            case CabinPartType.SmallSupport:
                SelectedPartViewModel = new EditSupportViewModel();
                break;
            case CabinPartType.Strip:
                SelectedPartViewModel = new EditStripViewModel();
                break;
            case CabinPartType.AnglePart:
                SelectedPartViewModel = new EditAngleViewModel();
                break;
            //Any Other Part
            case CabinPartType.GenericPart:
            default:
                SelectedPartViewModel = new EditPartViewModel(partType);
                return;
        }
    }
    [RelayCommand]
    private void CreateNewPartFromCopy(CabinPartEntity entity)
    {
        if (HasUnsavedPartEdits && ShouldNotScrapChanges())
        {
            return;
        }
        SelectedPart = null;
        SelectedPartViewModel = (entity.Part) switch
        {
            CabinAngle => new EditAngleViewModel(entity, false),
            CabinHandle => new EditHandleViewModel(entity, false),
            CabinHinge => new EditHingeViewModel(entity, false),
            Profile => new EditProfileViewModel(entity, memoryRepo.GetStrips(CabinStripType.PolycarbonicMagnet,new()).Select(s => s.Code), false),
            GlassStrip => new EditStripViewModel(entity, false),
            CabinSupport => new EditSupportViewModel(entity, false),
            SupportBar => new EditSupportBarViewModel(entity, false),
            null => null,
            _ => new EditPartViewModel(entity ?? new CabinPartEntity(new CabinPart() { Code = "ERROR", Description = "Error" },new(), new()), false),
        };
    }

    #endregion

    #region E.Delete from Db Commands

    [RelayCommand]
    private async Task DeletePart()
    {
        if (selectedPartViewModel is null || !selectedPartViewModel.IsEdit)
        {
            MessageService.Warning("Cannot Delete a newly created or an Empty Part", "lngFailure".TryTranslateKey());
            return;
        }

        if (MessageService.Question($"This Command Will Delete the Selected Part : {selectedPartViewModel.Code}{Environment.NewLine}Do you want to proceed ?{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}THIS PROCESS IS IRRIVERSIBLE","Part Deletion",$"Delete : {selectedPartViewModel.Code}","Cancel") == MessageBoxResult.Cancel)
        {
            return;
        }

        try
        {
            var operation = await partsDb.DeletePartAsync(selectedPartViewModel.Code);
            if (operation.IsSuccessful)
            {
                Log.Information("Deleted Part : {code}", selectedPartViewModel.Code);

                //Any Unsaved Data was Scrapped with the Deletion
                HasUnsavedPartEdits = false;
                //Delete the Entity from the Cached Parts
                var entityToDelete = Parts.FirstOrDefault(e => e.Part.Code == selectedPartViewModel.Code) ?? throw new InvalidOperationException($"The Part Marked for Deletion has not been Found in the Local Cache of Parts.... ?!{Environment.NewLine}The Part has been deleted though.Reload Parts to Reflect Changes.");
                Parts.Remove(entityToDelete);

                //Set the ViewModel to Null to Reflect that part was Deleted
                SelectedPart = null;
                selectedPartViewModel = null;
                OnPropertyChanged(nameof(FilteredParts));
                MessageService.Info("lngDeleteSuccessful".TryTranslateKey(), "lngInformation".TryTranslateKey());
            }
            else
            {
                MessageService.Warning(operation.FailureMessage, "lngDeleteFailure".TryTranslateKey());
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception Message : {message}", ex.Message);
            MessageService.Error(ex.Message, "Unknown Deletion Error");
        }

    }

    #endregion

    #region F. Save to Excel List Commands
    [RelayCommand]
    private async Task SavePartsToExcelAsync()
    {
        await SaveListToExcelAsync(FilteredParts);
    }
    [RelayCommand]
    private async Task SaveDefaultListsToExcelAsync()
    {
        await SaveListToExcelAsync(FilteredDefaultLists);
    }
    [RelayCommand]
    private async Task SaveConstraintsToExcelAsync()
    {
        await SaveListToExcelAsync(FilteredConstraints);
    }
    [RelayCommand]
    private async Task SaveCabinSettingsToExcelAsync()
    {
        await SaveListToExcelAsync(FilteredSettings);
    }

    /// <summary>
    /// Saves an Enumerable to an Excel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    private async Task SaveListToExcelAsync<T>(IEnumerable<T> list)
    {
        BusyPrompt = SAVING;
        IsBusy = true;
        try
        {
            if (list.Any())
            {
                var fileName = await Task.Run(() =>
                {
                    var fileName = ExcelService.ReportXls.SaveAsXlsReport(list);
                    return fileName;
                });
                if (MessageService.Questions.ExcelSavedAskOpenFile(fileName) == MessageBoxResult.OK)
                {
                    //Open the file if users reply is positive
                    Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = true });
                }
            }
            else MessageService.Info("No Rows To Save", "Empty Rows");
        }
        catch (Exception ex)
        {
            MessageService.LogAndDisplayException(ex);
        }
        finally
        {
            IsBusy = false;
            BusyPrompt = LOADING;
        }
    }

    #endregion

    /// <summary>
    /// Reinitilize memory Repository on Navigating Away From Managment ViewModel
    /// </summary>
    /// <returns></returns>
    public async Task OnNavigatingAwayOperation()
    {
        if (HasChangedFromDefaults)
        {
            try
            {
                IsBusy = true;
                //Re initilize Repo
                await memoryRepo.InitilizeRepo(((App)App.Current).SelectedLanguage);
                //Re initilize Applicator
                partSetsApplicatorService.InitilizeService();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "{message}", ex.Message);
                MessageService.Errors.FailedToInitilizeMemoryRepo();
            }
            finally
            {
                HasChangedFromDefaults = false;
                IsBusy = false;
            }
        }

    }

    [RelayCommand]
    private async Task CheckAndAddAllPartsUses()
    {
        //Load parts if not already loaded
        if (Parts.Count is 0)
        {
            await LoadParts();
        }
        IsBusy = true;
        try
        {
            foreach (var defaultList in DefaultLists)
            {
                //Get all the Codes Used for each Default List
                var uses = defaultList.DefaultPartsList.GetUsedCodes();

                foreach (var use in uses)
                {
                    var partToUpdate = Parts.FirstOrDefault(p => p.Part.Code == use) ?? throw new InvalidOperationException($"Part with Code {use} Not Found...");
                    if (partToUpdate.Uses.Any(idc => idc.Identifier == defaultList.GetIdentifier()) is false)
                    {
                        partToUpdate.Uses.Add(new(defaultList.GetIdentifier()));
                        SelectedPart = partToUpdate;
                        await SavePartEdit();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageService.LogAndDisplayException(ex);
        }
        finally
        {
            IsBusy = false;
        }

    }

    [RelayCommand]
    private void OpenEditSubPart(EditPartViewModel? partBeingEdited)
    {
        if (partBeingEdited is null)
        {
            MessageService.Warning("Cannot Edit a Null Part", "lngFailure".TryTranslateKey());
        }
        else
        {
            openEditSubPartsModal.OpenModal(partBeingEdited);
        }
    }

    [RelayCommand]
    private void OpenEditPartSet(EditDefaultPartsViewModel? defaultPartsListEdited)
    {
        if (defaultPartsListEdited is null)
        {
            MessageService.Warning("Cannot Edit a Null Default Parts List", "lngFailure".TryTranslateKey());
        }
        else
        {
            openEditPartSetsModal.OpenModal(defaultPartsListEdited);
        }
    }

}
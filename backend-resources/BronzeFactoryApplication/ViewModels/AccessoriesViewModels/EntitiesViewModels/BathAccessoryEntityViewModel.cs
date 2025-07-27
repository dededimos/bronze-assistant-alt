using AccessoriesRepoMongoDB.Entities;
using AccessoriesRepoMongoDB.Repositories;
using BathAccessoriesModelsLibrary;
using BronzeFactoryApplication.ViewModels.AccessoriesViewModels.EntitiesViewModels;
using BronzeFactoryApplication.ViewModels.AccessoriesViewModels.EntitiesViewModels.HelperTraitValueViewModels;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using CommunityToolkit.Diagnostics;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Microsoft.Win32;
using MongoDB.Bson;
using MongoDbCommonLibrary.CommonEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BathAccessoriesModelsLibrary.AccessoryTrait;

namespace BronzeFactoryApplication.ViewModels.AccessoriesViewModels;

public partial class BathAccessoryEntityViewModel : BaseViewModel, IEditorViewModel<BathAccessoryEntity>
{
    private readonly OpenEditLocalizedStringModalService openEditLocalizedStringModalService;
    private readonly OpenImageViewerModalService openImageViewerService;
    private readonly CloseModalService closeModalService;
    private readonly IAccessoryEntitiesRepository repo;
    private readonly ITraitGroupEntitiesRepository traitGroupsRepo;
    [ObservableProperty]
    private DescriptiveEntityViewModel baseDescriptiveEntity;

    [ObservableProperty]
    private bool isOnline;

    public string Code { get => GetCode(); }

    /// <summary>
    /// Returns the Generated Code
    /// </summary>
    /// <returns></returns>
    private string GetCode()
    {
        var finishCode = FinishTraits.FirstOrDefault(f => f.Id.ToString() == SelectedFinishId)?.Code ?? "??";
        return BathAccessoryEntity.GenerateSpecificCode(MainCode, finishCode, ExtraCode, UsesOnlyMainCode);
    }

    [ObservableProperty]
    private int sortNo = 99999;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Code))]
    private string mainCode = string.Empty;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Code))]
    private string extraCode = string.Empty;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Code))]
    private bool usesOnlyMainCode;

    [ObservableProperty]
    private string mainPhotoUrl = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> extraPhotosURL = new();

    [ObservableProperty]
    private string pdfURL = string.Empty;
    
    [ObservableProperty]
    private string dimensionsPhotoUrl = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Code))]
    private string selectedFinishId = string.Empty;

    /// <summary>
    /// The Available Default Finishes that Can be Selected as A default Finish
    /// </summary>
    public IEnumerable<TraitEntity> AvailableDefaultFinishes { get => AvailableFinishes?.Select(av => av.Finish) ?? Enumerable.Empty<TraitEntity>(); }

    /// <summary>
    /// The Selected Finish to Add in the Available Finishes
    /// </summary>
    [ObservableProperty]
    private TraitEntity? selectedAccessoryFinishVariation;
    /// <summary>
    /// The Selected FinishGroup to Add in the Available Finishes
    /// </summary>
    [ObservableProperty]
    private TraitGroupEntity? selectedFinishGroupToAdd;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AvailableDefaultFinishes))]
    private ObservableCollection<AccessoryFinishInfoViewModel> availableFinishes = new();
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAnyAvailableFinishSelected))]
    private AccessoryFinishInfoViewModel? selectedAvailableFinish;
    public bool IsAnyAvailableFinishSelected { get => SelectedAvailableFinish != null; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedMaterial))]
    private string selectedMaterialId = string.Empty;
    public TraitEntity? SelectedMaterial { get => MaterialTraits.FirstOrDefault(m => m.Id.ToString() == SelectedMaterialId); }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedSize))]
    private string selectedSizeId = string.Empty;
    public TraitEntity? SelectedSize { get => SizeTraits.FirstOrDefault(s => s.Id.ToString() == SelectedSizeId); }

    [ObservableProperty]
    private ObservableCollection<BathAccessoryEntity> sizeVariations = new();
    [ObservableProperty]
    private BathAccessoryEntity? selectedSizeVariation;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelectedShape))]
    private string selectedShapeId = string.Empty;
    public TraitEntity? SelectedShape { get => ShapeTraits.FirstOrDefault(s => s.Id.ToString() == SelectedShapeId); }

    [ObservableProperty]
    private ObservableCollection<PrimaryTypeTraitEntity> primaryTypes = new();
    [ObservableProperty]
    private PrimaryTypeTraitEntity? selectedPrimaryType;

    [ObservableProperty]
    private ObservableCollection<TraitEntity> secondaryTypes = new();
    [ObservableProperty]
    private TraitEntity? selectedSecondaryType;
    /// <summary>
    /// The Secondary Types that are Currently Available , Taken from the Primary Types Selected for this Accessory
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<TraitEntity> currentlyAvailableSecondaryTypes = new();


    [ObservableProperty]
    private ObservableCollection<TraitEntity> categories = new();
    [ObservableProperty]
    private TraitEntity? selectedCategory;

    [ObservableProperty]
    private ObservableCollection<TraitEntity> series = new();
    [ObservableProperty]
    private TraitEntity? selectedSeries;

    [ObservableProperty]
    private ObservableCollection<TraitEntity> mountingTypes = new();
    [ObservableProperty]
    private TraitEntity? selectedMountingType;

    [ObservableProperty]
    private ObservableCollection<BathAccessoryEntity> mountingVariations = new();
    [ObservableProperty]
    private BathAccessoryEntity? selectedMountingVariation;

    [ObservableProperty]
    private ObservableCollection<TraitEntityDoubleViewModel> selectedDimensions = new();
    [ObservableProperty]
    private TraitEntity? selectedDimension;
    [ObservableProperty]
    private double selectedDimensionValue;

    [ObservableProperty]
    private ObservableCollection<PriceInfoViewModel> pricesInfo = new();
    
    /// <summary>
    /// The Selected PriceInfoFinish to Add (Cancels the Group)
    /// </summary>
    private TraitEntity? selectedPriceInfoFinish;
    public TraitEntity? SelectedPriceInfoFinish 
    {
        get => selectedPriceInfoFinish;
        set
        {
            if (selectedPriceInfoFinish != value)
            {
                selectedPriceInfoFinish = value;
                OnPropertyChanged(nameof(SelectedPriceInfoFinish));
                if (selectedPriceInfoFinish != null)
                {
                    SelectedPriceInfoFinishGroup = null;
                }
            }
        }
    }

    /// <summary>
    /// The Selected PriceInfoFinishGroup to Add (Cancels the Finish)
    /// </summary>
    private TraitGroupEntity? selectedPriceInfoFinishGroup;
    public TraitGroupEntity? SelectedPriceInfoFinishGroup
    {
        get => selectedPriceInfoFinishGroup;
        set
        {
            if (selectedPriceInfoFinishGroup != value)
            {
                selectedPriceInfoFinishGroup = value;
                OnPropertyChanged(nameof(SelectedPriceInfoFinishGroup));
                if (selectedPriceInfoFinishGroup != null)
                {
                    SelectedPriceInfoFinish = null;
                }
            }
        }
    }

    [ObservableProperty]
    private TraitEntity? selectedPrice;
    [ObservableProperty]
    private decimal selectedPriceValue;
    /// <summary>
    /// The Finishes Available in the Price Info Menu
    /// </summary>
    public IEnumerable<TraitEntity> PriceInfoAvailableFinishes { get => AvailableFinishes.Select(af => af.Finish); }
    /// <summary>
    /// The Finish Groups Available in the Price info Menu
    /// </summary>
    public IEnumerable<TraitGroupEntity> PriceInfoAvailableGroups { get => FinishTraitGroups.Where(fg => AvailableFinishes.SelectMany(af => af.Finish.AssignedGroups).Distinct().Any(gId => gId == fg.IdAsString)); }

    public IEnumerable<TraitEntity> FinishTraits { get => repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.FinishTrait).OrderBy(t=> t.Trait.GetLocalizedValue(((App)Application.Current).SelectedLanguage)); }
    public IEnumerable<TraitEntity> SizeTraits { get => repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.SizeTrait).OrderBy(t => t.Trait.GetLocalizedValue(((App)Application.Current).SelectedLanguage)); }
    public IEnumerable<TraitEntity> MaterialTraits { get => repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.MaterialTrait).OrderBy(t => t.Trait.GetLocalizedValue(((App)Application.Current).SelectedLanguage)); }
    public IEnumerable<TraitEntity> SeriesTraits { get => repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.SeriesTrait).OrderBy(t => t.Trait.GetLocalizedValue(((App)Application.Current).SelectedLanguage)); }
    public IEnumerable<TraitEntity> MountingTraits { get => repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.MountingTypeTrait).OrderBy(t => t.Trait.GetLocalizedValue(((App)Application.Current).SelectedLanguage)); }
    public IEnumerable<TraitEntity> CategoryTraits { get => repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.CategoryTrait).OrderBy(t => t.Trait.GetLocalizedValue(((App)Application.Current).SelectedLanguage)); }
    public IEnumerable<TraitEntity> ShapeTraits { get => repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.ShapeTrait).OrderBy(t => t.Trait.GetLocalizedValue(((App)Application.Current).SelectedLanguage)); }
    public IEnumerable<PrimaryTypeTraitEntity> PrimaryTypesTraits { get => repo.Traits.Cache.OfType<PrimaryTypeTraitEntity>().OrderBy(t => t.Trait.GetLocalizedValue(((App)Application.Current).SelectedLanguage)); }
    public IEnumerable<TraitEntity> SecondaryTypesTraits { get => repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.SecondaryTypeTrait).OrderBy(t => t.Trait.GetLocalizedValue(((App)Application.Current).SelectedLanguage)); }
    public IEnumerable<TraitEntity> DimensionsTraits { get => repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.DimensionTrait).OrderBy(t => t.Trait.GetLocalizedValue(((App)Application.Current).SelectedLanguage)); }
    public IEnumerable<TraitEntity> PriceTraits { get => repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.PriceTrait).OrderBy(t => t.Trait.GetLocalizedValue(((App)Application.Current).SelectedLanguage)); }
    public IEnumerable<TraitGroupEntity> FinishTraitGroups { get => traitGroupsRepo.Cache.Where(tg => tg.PermittedTraitTypes.Contains(TypeOfTrait.FinishTrait)).OrderBy(g=>g.SortNo); }
    public IEnumerable<BathAccessoryEntity> AllAccessories { get => repo.Cache; }

    public BathAccessoryEntityViewModel(Func<DescriptiveEntityViewModel> descriptiveEntityVmFactory,
        OpenEditLocalizedStringModalService openEditLocalizedStringModalService,
        OpenImageViewerModalService openImageViewerService,
        CloseModalService closeModalService,
        IAccessoryEntitiesRepository repo,
        ITraitGroupEntitiesRepository traitGroupsRepo)
    {
        baseDescriptiveEntity = descriptiveEntityVmFactory.Invoke();
        this.openEditLocalizedStringModalService = openEditLocalizedStringModalService;
        this.openImageViewerService = openImageViewerService;
        this.closeModalService = closeModalService;
        this.repo = repo;
        this.traitGroupsRepo = traitGroupsRepo;
        closeModalService.ModalClosing += CloseModalService_ModalClosing;
    }

    /// <summary>
    /// Informs that a Localized String has Changed this way changes are propagated to any class that Uses Property Change Handler of this Vm to track Changes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CloseModalService_ModalClosing(object? sender, ModalClosingEventArgs e)
    {
        // Have to check that the Localized String Vms are those of this Reference
        if (e.ClosingModal is LocalizedStringEditModalViewModel modal && (
            modal.LocalizedStringVm == BaseDescriptiveEntity.Name ||
            modal.LocalizedStringVm == BaseDescriptiveEntity.Description ||
            modal.LocalizedStringVm == BaseDescriptiveEntity.ExtendedDescription)
            && modal.HasMadeChanges)
        {
            OnPropertyChanged(nameof(BaseDescriptiveEntity));
        }
    }
    /// <summary>
    /// Informs for Property Changes of items of AvailableFinishes Collection this way changes are propagated to any class that Uses Property Change Handler of this Vm to track Changes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FinishInfoVm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Triggers the EditContext for subscribers of this ViewModels Changes when any of the Available Finishes Changes
        OnPropertyChanged(nameof(AvailableFinishes));
        OnPropertyChanged(nameof(PriceInfoAvailableFinishes));
        OnPropertyChanged(nameof(PriceInfoAvailableGroups));
    }
    public void SetModel(BathAccessoryEntity model)
    {
        BaseDescriptiveEntity.SetModel(model);
        IsOnline = model.IsOnline;
        SortNo = model.SortNo;
        MainCode = model.MainCode;
        ExtraCode = model.ExtraCode;
        UsesOnlyMainCode = model.UsesOnlyMainCode;
        MainPhotoUrl = model.MainPhotoURL;
        ExtraPhotosURL.Clear();
        foreach (var url in model.ExtraPhotosURL)
        {
            ExtraPhotosURL.Add(url);
        }
        PdfURL = model.PdfURL;
        DimensionsPhotoUrl = model.DimensionsPhotoUrl;
        SelectedFinishId = model.Finish;
        SelectedMaterialId = model.Material;
        SelectedSizeId = model.Size;
        SizeVariations = new(repo.Cache.Where(a => model.SizeVariations.Contains(a.Id.ToString())));
        SelectedShapeId = model.Shape;
        PrimaryTypes = new(PrimaryTypesTraits.Where(pt => model.PrimaryTypes.Any(id => id == pt.Id.ToString())));
        SecondaryTypes = new(SecondaryTypesTraits.Where(st => model.SecondaryTypes.Any(id => id == st.Id.ToString())));
        Categories = new(CategoryTraits.Where(ct => model.Categories.Any(id => id == ct.Id.ToString())));
        Series = new(SeriesTraits.Where(st => model.Series.Any(id => id == st.Id.ToString())));
        MountingTypes = new(MountingTraits.Where(mt => model.MountingTypes.Any(id => id == mt.Id.ToString())));
        MountingVariations = new(repo.Cache.Where(a => model.MountingVariations.Contains(a.Id.ToString())));
        SelectedDimensions.Clear();
        foreach (var dimensionKvp in model.Dimensions)
        {
            var dimensionTrait = DimensionsTraits.FirstOrDefault(d => d.Id.ToString() == dimensionKvp.Key) ?? throw new Exception($"Dimension with Id {dimensionKvp.Key} and Value {dimensionKvp.Value} of AccessoryEntity {model.Code} was not Found in the Dimensions List for and Unexpected Reason...");
            var dimensionValue = dimensionKvp.Value;
            SelectedDimensions.Add(new TraitEntityDoubleViewModel(dimensionTrait, dimensionValue));
        }
        
        // Unsubscribe previous PropertyChange Subscriptions
        UnsubscribeAvailableFinishesChanges();
        AvailableFinishes.Clear();
        //Available Finishes
        foreach (var af in model.AvailableFinishes)
        {
            var finishTrait = FinishTraits.FirstOrDefault(ft => ft.IdAsString == af.FinishId);
            if (finishTrait is not null)
            {
                AccessoryFinishInfoViewModel finishInfoVm = new(finishTrait, af);
                AvailableFinishes.Add(finishInfoVm);
                finishInfoVm.PropertyChanged += FinishInfoVm_PropertyChanged;
            }
            else
            {
                MessageService.Warning($"Trait with Id {af.FinishId} was not Found in the Repository Traits", "Trait Not Found while Setting model ...");
            }
        }

        // Set the Allowed Secondary Types to Appear in the Selections Based on the PrimaryTypes currently Selected
        var allowedSecTypes = PrimaryTypes.SelectMany(pt => pt.AllowedSecondaryTypes);
        CurrentlyAvailableSecondaryTypes = new(SecondaryTypesTraits.Where(st => allowedSecTypes.Any(id => id == st.Id)));

        //Set all the Prices Info
        PricesInfo.Clear();
        foreach (var pi in model.PricesInfo)
        {
            var priceTrait = PriceTraits.FirstOrDefault(p => p.IdAsString == pi.PriceTraitId) ?? throw new Exception($"Price with Id {pi.PriceTraitId} of AccessoryEntity {model.Code} was not Found in the Prices List for and Unexpected Reason...");
            var referingFinishTrait = FinishTraits.FirstOrDefault(ft => ft.IdAsString == pi.RefersToFinishId);
            var referingFinishGroup = FinishTraitGroups.FirstOrDefault(fg => fg.IdAsString == pi.RefersToFinishGroupId);
            var priceInfoVm = new PriceInfoViewModel(priceTrait, pi.PriceValue, referingFinishTrait, referingFinishGroup);
            PricesInfo.Add(priceInfoVm);
        }

        //Initilize to always have a value if possible
        SelectedFinishGroupToAdd = FinishTraitGroups.FirstOrDefault();
        SelectedAvailableFinish = AvailableFinishes.FirstOrDefault();
    }
    public BathAccessoryEntity GetModel()
    {
        BathAccessoryEntity model = new();
        return CopyPropertiesToModel(model);
    }
    public BathAccessoryEntity CopyPropertiesToModel(BathAccessoryEntity model)
    {
        BaseDescriptiveEntity.CopyPropertiesToModel(model);
        model.IsOnline = IsOnline;
        model.SortNo = SortNo;
        model.MainCode = MainCode;
        model.ExtraCode = ExtraCode;
        model.UsesOnlyMainCode = UsesOnlyMainCode;
        model.MainPhotoURL = MainPhotoUrl;
        model.ExtraPhotosURL = new(ExtraPhotosURL);
        model.PdfURL = PdfURL;
        model.DimensionsPhotoUrl = DimensionsPhotoUrl;
        model.Finish = SelectedFinishId;
        model.AvailableFinishes = new(AvailableFinishes.Select(af => af.GetFinishInfoObject()));
        model.Material = SelectedMaterialId;
        model.Size = SelectedSizeId;
        model.SizeVariations = new(SizeVariations.Select(a => a.Id.ToString()));
        model.Shape = SelectedShapeId;
        model.PrimaryTypes = PrimaryTypes.Select(pt => pt.Id.ToString()).ToList();
        model.SecondaryTypes = SecondaryTypes.Select(st => st.Id.ToString()).ToList();
        model.Categories = Categories.Select(c => c.Id.ToString()).ToList();
        model.Series = Series.Select(s => s.Id.ToString()).ToList();
        model.MountingTypes = MountingTypes.Select(mt => mt.Id.ToString()).ToList();
        model.MountingVariations = new(MountingVariations.Select(a => a.Id.ToString()));
        model.Dimensions = SelectedDimensions.ToDictionary(d => d.Trait.Id.ToString(), d => d.Value);
        model.PricesInfo = PricesInfo.Select(pi => pi.GetPriceInfoObject()).ToList();

        return model;
    }

    /// <summary>
    /// Changes the Main Photo Url with a Local File
    /// </summary>
    [RelayCommand]
    private void ChangeMainPhotoUrl()
    {
        var filePath = GeneralHelpers.SelectImageFile();
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }
        else
        {
            MainPhotoUrl = filePath;
        }
    }
    [RelayCommand]
    private void ChangeDimensionsPhotoUrl()
    {
        var filePath = GeneralHelpers.SelectImageFile();
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }
        else
        {
            DimensionsPhotoUrl = filePath;
        }
    }
    [RelayCommand]
    private void ChangePhotoUrlAvailbleFinish()
    {
        Guard.IsNotNull(SelectedAvailableFinish);
        var filePath = GeneralHelpers.SelectImageFile();
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }
        else
        {
            SelectedAvailableFinish.PhotoUrl = filePath;
        }
    }
    [RelayCommand]
    private void ChangeDimensionsPhotoUrlAvailableFinish()
    {
        Guard.IsNotNull(SelectedAvailableFinish);
        var filePath = GeneralHelpers.SelectImageFile();
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }
        else
        {
            SelectedAvailableFinish.DimensionsPhotoUrl = filePath;
        }
    }
    /// <summary>
    /// Changes the Pdf Url with a Local File
    /// </summary>
    [RelayCommand]
    private void ChangePdfUrl()
    {
        var filePath = GeneralHelpers.SelectPdfFile();
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }
        else
        {
            var msg = string.IsNullOrWhiteSpace(PdfURL) ? $"New Pdf File Added Successfuly{Environment.NewLine}{Environment.NewLine}{filePath}" : $"Pdf File Changed {Environment.NewLine}{Environment.NewLine}From: {PdfURL}{Environment.NewLine}{Environment.NewLine}To: {filePath}";
            PdfURL = filePath;
            MessageService.Info(msg, "Pdf Selection");
        }
    }
    [RelayCommand]
    private void ChangePdfUrlAvailableFinish()
    {
        Guard.IsNotNull(SelectedAvailableFinish);
        var filePath = GeneralHelpers.SelectPdfFile();
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }
        else
        {
            SelectedAvailableFinish.PdfUrl = filePath;
        }
    }
    [RelayCommand]
    private void RemoveMainPhotoUrl()
    {
        if (string.IsNullOrEmpty(MainPhotoUrl)) return;
        if (MessageService.Question($"Do you want to Remove Photo : {Environment.NewLine}{Environment.NewLine}{MainPhotoUrl}", "Remove Pdf", "Remove", "Cancel")
            == MessageBoxResult.OK)
        {
            MainPhotoUrl = string.Empty;
        }
    }
    [RelayCommand]
    private void RemovePdfUrl()
    {
        if (string.IsNullOrEmpty(PdfURL))
        {
            return;
        }
        if (MessageService.Question($"Do you want to Remove Pdf : {Environment.NewLine}{Environment.NewLine}{PdfURL}", "Remove Pdf", "Remove", "Cancel")
            == MessageBoxResult.OK)
        {
            PdfURL = string.Empty;
        }
    }
    [RelayCommand]
    private void RemoveMainPhotoUrlAvailableFinish()
    {
        Guard.IsNotNull(SelectedAvailableFinish);
        if (string.IsNullOrEmpty(SelectedAvailableFinish.PhotoUrl)) return;
        if (MessageService.Question($"Do you want to Remove Photo : {Environment.NewLine}{Environment.NewLine}{SelectedAvailableFinish.PhotoUrl}", "Remove Pdf", "Remove", "Cancel")
            == MessageBoxResult.OK)
        {
            SelectedAvailableFinish.PhotoUrl = string.Empty;
        }
    }
    [RelayCommand]
    private void RemovePdfUrlAvailableFinish()
    {
        Guard.IsNotNull(SelectedAvailableFinish);
        if (string.IsNullOrEmpty(SelectedAvailableFinish.PdfUrl))
        {
            return;
        }
        if (MessageService.Question($"Do you want to Remove Pdf : {Environment.NewLine}{Environment.NewLine}{SelectedAvailableFinish.PdfUrl}", "Remove Pdf", "Remove", "Cancel")
            == MessageBoxResult.OK)
        {
            SelectedAvailableFinish.PdfUrl = string.Empty;
        }
    }
    /// <summary>
    /// Adds a FilePath to the ExtraPhotos Urls
    /// </summary>
    [RelayCommand]
    private void AddExtraPhotoUrl()
    {
        var filePath = GeneralHelpers.SelectImageFile();
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }
        else
        {
            ExtraPhotosURL.Add(filePath);
            OnPropertyChanged(nameof(ExtraPhotosURL));
        }
    }
    [RelayCommand]
    private void AddExtraPhotoUrlAvailableFinish()
    {
        Guard.IsNotNull(SelectedAvailableFinish);
        var filePath = GeneralHelpers.SelectImageFile();
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }
        else
        {
            SelectedAvailableFinish.ExtraPhotosUrl.Add(filePath);
            OnPropertyChanged(nameof(AvailableFinishes));
            OnPropertyChanged(nameof(PriceInfoAvailableFinishes));
            OnPropertyChanged(nameof(PriceInfoAvailableGroups));
        }
    }
    /// <summary>
    /// Removes a Url from the ExtraPhotoURLs
    /// </summary>
    /// <param name="url">The Url To Remove</param>
    [RelayCommand]
    private void RemoveExtraPhotoUrl(string url)
    {
        if (MessageService.Question($"This will Remove the Selected Photo with URL:{Environment.NewLine}{Environment.NewLine}{url}{Environment.NewLine}{Environment.NewLine}Would you like to Proceed?", "Remove Photo", "Remove", "Cancel")
            == MessageBoxResult.OK)
        {
            ExtraPhotosURL.Remove(url);
            OnPropertyChanged(nameof(ExtraPhotosURL));
        }
    }
    [RelayCommand]
    private void RemoveDimensionsPhotoUrl()
    {
        if (string.IsNullOrEmpty(DimensionsPhotoUrl)) return;
        if (MessageService.Question($"This will Remove the Selected Photo with URL:{Environment.NewLine}{Environment.NewLine}{DimensionsPhotoUrl}{Environment.NewLine}{Environment.NewLine}Would you like to Proceed?", "Remove Photo", "Remove", "Cancel")
            == MessageBoxResult.OK)
        {
            DimensionsPhotoUrl = string.Empty;
        }
    }
    [RelayCommand]
    private void RemoveExtraPhotoUrlAvailableFinish(string url)
    {
        Guard.IsNotNull(SelectedAvailableFinish);
        if (MessageService.Question($"This will Remove the Selected Photo with URL:{Environment.NewLine}{Environment.NewLine}{url}{Environment.NewLine}{Environment.NewLine}Would you like to Proceed?", "Remove Photo", "Remove", "Cancel")
            == MessageBoxResult.OK)
        {
            SelectedAvailableFinish.ExtraPhotosUrl.Remove(url);
            OnPropertyChanged(nameof(AvailableFinishes));
            OnPropertyChanged(nameof(PriceInfoAvailableFinishes));
            OnPropertyChanged(nameof(PriceInfoAvailableGroups));
        }
    }
    [RelayCommand]
    private void RemoveDimensionsPhotoUrlAvailableFinish()
    {
        Guard.IsNotNull(SelectedAvailableFinish);
        if (string.IsNullOrEmpty(SelectedAvailableFinish.DimensionsPhotoUrl)) return;
        if (MessageService.Question($"This will Remove the Selected Photo with URL:{Environment.NewLine}{Environment.NewLine}{SelectedAvailableFinish.DimensionsPhotoUrl}{Environment.NewLine}{Environment.NewLine}Would you like to Proceed?", "Remove Photo", "Remove", "Cancel")
            == MessageBoxResult.OK)
        {
            SelectedAvailableFinish.DimensionsPhotoUrl = string.Empty;
        }
    }
    [RelayCommand]
    private void RemoveAvailableFinish(AccessoryFinishInfoViewModel finishInfoToRemove)
    {
        var pricesWhichContainThisFinish = PricesInfo.Where(pi => pi.ReferingFinishTrait?.Id == finishInfoToRemove.Finish.Id).ToList();
        if (pricesWhichContainThisFinish.Any())
        {
            if(MessageService.Question("This finish is Being Used in Prices of this Accessory, if you remove it the Prices will also get Deleted, Continue?", "Price will also be Removed","Continue","Cancel") == MessageBoxResult.Cancel) return;
        }

        if (AvailableFinishes.Remove(finishInfoToRemove))
        {
            finishInfoToRemove.PropertyChanged -= FinishInfoVm_PropertyChanged;
            OnPropertyChanged(nameof(AvailableFinishes));
            OnPropertyChanged(nameof(AvailableDefaultFinishes));
            OnPropertyChanged(nameof(PriceInfoAvailableFinishes));
            OnPropertyChanged(nameof(PriceInfoAvailableGroups));
            foreach (var price in pricesWhichContainThisFinish)
            {
                PricesInfo.Remove(price);
                OnPropertyChanged(nameof(PricesInfo));
            }
        };
    }
    [RelayCommand]
    private void RemoveSizeVariation(BathAccessoryEntity entityToRemove)
    {
        if (SizeVariations.Remove(entityToRemove))
        {
            OnPropertyChanged(nameof(SizeVariations));
        };
    }
    [RelayCommand]
    private void RemovePrimaryType(PrimaryTypeTraitEntity primaryTypeToRemove)
    {
        bool removed = PrimaryTypes.Remove(primaryTypeToRemove);
        if (removed)
        {
            // Find the Secondary Types Selected already for this Primary Type and Remove Them
            foreach (var secondType in primaryTypeToRemove.AllowedSecondaryTypes)
            {
                if (SecondaryTypes.Any(st => st.Id == secondType))
                {
                    var secondTypeToRemove = SecondaryTypes.First(st => st.Id == secondType);
                    SecondaryTypes.Remove(secondTypeToRemove);
                }
                // Remove the Secondary Types taht are currently Available for Selection , based on the Primary Types Selected Already
                if (CurrentlyAvailableSecondaryTypes.Any(st => st.Id == secondType))
                {
                    var secondTypeToRemove = CurrentlyAvailableSecondaryTypes.First(st => st.Id == secondType);
                    CurrentlyAvailableSecondaryTypes.Remove(secondTypeToRemove);
                }
            }
            OnPropertyChanged(nameof(PrimaryTypes));
            OnPropertyChanged(nameof(SecondaryTypes));
        }
    }
    [RelayCommand]
    private void RemoveSecondaryType(TraitEntity secondaryTypeToRemove)
    {
        if (SecondaryTypes.Remove(secondaryTypeToRemove)) OnPropertyChanged(nameof(SecondaryTypes));
    }
    [RelayCommand]
    private void RemoveCategory(TraitEntity categoryToRemove)
    {
        if (Categories.Remove(categoryToRemove)) OnPropertyChanged(nameof(Categories));
    }
    [RelayCommand]
    private void RemoveSeries(TraitEntity seriesToRemove)
    {
        if (Series.Remove(seriesToRemove)) OnPropertyChanged(nameof(Series));
    }
    [RelayCommand]
    private void RemoveMountingType(TraitEntity mountingTypeToRemove)
    {
        if (MountingTypes.Remove(mountingTypeToRemove)) OnPropertyChanged(nameof(MountingTypes));
    }
    [RelayCommand]
    private void RemoveMountingVariation(BathAccessoryEntity entityToRemove)
    {
        if (MountingVariations.Remove(entityToRemove)) OnPropertyChanged(nameof(MountingVariations));
    }
    [RelayCommand]
    private void RemoveDimension(TraitEntityDoubleViewModel dimensionToRemove)
    {
        if (SelectedDimensions.Remove(dimensionToRemove)) OnPropertyChanged(nameof(SelectedDimensions));
    }
    [RelayCommand]
    private void RemovePriceInfo(PriceInfoViewModel priceToRemove)
    {
        if (MessageService.Question($"Remove this Price {priceToRemove.PriceTrait.Trait.GetDefaultValue()}=>{priceToRemove.PriceInfoRefersToName.DefaultValue} ?", "Remove Price", "Remove", "Cancel") == MessageBoxResult.Cancel)
        {
            return;
        }
        if (PricesInfo.Remove(priceToRemove))
        {
            OnPropertyChanged(nameof(PricesInfo));
        }
    }
    [RelayCommand]
    private void AddAvailableFinish(TraitEntity entityToAdd)
    {
        if (entityToAdd is null)
        {
            MessageService.Warning("Please Select a Finish First...", "Warning");
            return;
        }
        if (AvailableFinishes.Any(af => af.Finish.Id == entityToAdd.Id))
        {
            MessageService.Warning($"Selected Finish '{entityToAdd.Trait.DefaultValue}' is already Present in the List ", "Warning");
            return;
        }
        
        //Add the Finish Vm , Subscribe to Prop Changes to Inform edit Contexts that items insde the list have changed
        var finishInfoVm = new AccessoryFinishInfoViewModel(entityToAdd);
        AvailableFinishes.Add(finishInfoVm);
        finishInfoVm.PropertyChanged += FinishInfoVm_PropertyChanged;
        OnPropertyChanged(nameof(AvailableFinishes));
        OnPropertyChanged(nameof(AvailableDefaultFinishes));
        OnPropertyChanged(nameof(PriceInfoAvailableFinishes));
        OnPropertyChanged(nameof(PriceInfoAvailableGroups));
    }
    [RelayCommand]
    private void AddAvailableFinishByGroup()
    {
        if (SelectedFinishGroupToAdd is null)
        {
            MessageService.Warning("Please Select a Finish Group First...", "Warning");
            return;
        }
        //Check which finishes of the Selected Group Are not Already Added
        var possibleFinishesToAdd = FinishTraits.Where(f => f.AssignedGroups.Any(gId => gId == SelectedFinishGroupToAdd.IdAsString));
        foreach (var f in possibleFinishesToAdd)
        {
            // If the finish is not present Add it
            if (AvailableFinishes.Any(af=> af.Finish.Id == f.Id) is false)
            {
                //Add the Finish Vm , Subscribe to Prop Changes to Inform edit Contexts that items insde the list have changed
                var finishInfoVm = new AccessoryFinishInfoViewModel(f);
                AvailableFinishes.Add(finishInfoVm);
                finishInfoVm.PropertyChanged += FinishInfoVm_PropertyChanged;
                OnPropertyChanged(nameof(AvailableFinishes));
                OnPropertyChanged(nameof(AvailableDefaultFinishes));
                OnPropertyChanged(nameof(PriceInfoAvailableFinishes));
                OnPropertyChanged(nameof(PriceInfoAvailableGroups));
            }
        }
    }
    [RelayCommand]
    private void AddSizeVariation(BathAccessoryEntity entityToAdd)
    {
        if (entityToAdd is null)
        {
            MessageService.Warning("Please Select an Accessory First...", "Warning");
            return;
        }
        if (SizeVariations.Any(a => a.Id == entityToAdd.Id))
        {
            MessageService.Warning($"Selected '{entityToAdd.Code}' Variation is already Present in the List ", "Warning");
            return;
        }
        SizeVariations.Add(entityToAdd);
        OnPropertyChanged(nameof(SizeVariations));
    }
    [RelayCommand]
    private void AddPrimaryType(PrimaryTypeTraitEntity primaryTypeToAdd)
    {
        if (primaryTypeToAdd is null)
        {
            MessageService.Warning("Please Select a Type First...", "Warning");
            return;
        }
        if (PrimaryTypes.Any(pt => pt.Id == primaryTypeToAdd.Id))
        {
            MessageService.Warning($"Selected '{primaryTypeToAdd.Trait.DefaultValue}' is already Present in the List ", "Warning");
            return;
        }
        PrimaryTypes.Add(primaryTypeToAdd);
        OnPropertyChanged(nameof(PrimaryTypes));

        // Add also the Allowed Secondary Types that are Available For Selection , with this new Primary Type
        foreach (var allowedSecondType in primaryTypeToAdd.AllowedSecondaryTypes)
        {
            // Check that is not Already Present
            if (CurrentlyAvailableSecondaryTypes.Any(st => st.Id == allowedSecondType) is false)
            {
                var secondaryTypeToAdd = SecondaryTypesTraits.FirstOrDefault(st => st.Id == allowedSecondType) ?? throw new Exception($"Secondary Type with Id {allowedSecondType} was not Found in The SecondaryTypes List of Traits for Unexpected Reason ...");
                CurrentlyAvailableSecondaryTypes.Add(secondaryTypeToAdd);
            }
        }
    }
    [RelayCommand]
    private void AddSecondaryType(TraitEntity secondaryTypeToAdd)
    {
        if (secondaryTypeToAdd is null)
        {
            MessageService.Warning("Please Select a Type First...", "Warning");
            return;
        }
        if (SecondaryTypes.Any(st => st.Id == secondaryTypeToAdd.Id))
        {
            MessageService.Warning($"Selected '{secondaryTypeToAdd.Trait.DefaultValue}' Type is already Present in the List ", "Warning");
            return;
        }
        SecondaryTypes.Add(secondaryTypeToAdd);
        OnPropertyChanged(nameof(SecondaryTypes));
    }
    [RelayCommand]
    private void AddCategory(TraitEntity categoryToAdd)
    {
        if (categoryToAdd is null)
        {
            MessageService.Warning("Please Select a Category First...", "Warning");
            return;
        }
        if (Categories.Any(a => a.Id == categoryToAdd.Id))
        {
            MessageService.Warning($"Selected '{categoryToAdd.Trait.DefaultValue}' Category is already Present in the List ", "Warning");
            return;
        }
        Categories.Add(categoryToAdd);
        OnPropertyChanged(nameof(Categories));
    }
    [RelayCommand]
    private void AddSeries(TraitEntity seriesToAdd)
    {
        if (seriesToAdd is null)
        {
            MessageService.Warning("Please Select a Series First...", "Warning");
            return;
        }
        if (Series.Any(a => a.Id == seriesToAdd.Id))
        {
            MessageService.Warning($"Selected '{seriesToAdd.Trait.DefaultValue}' Series is already Present in the List ", "Warning");
            return;
        }
        Series.Add(seriesToAdd);
        OnPropertyChanged(nameof(Series));
    }
    [RelayCommand]
    private void AddMountingType(TraitEntity mountingTypeToAdd)
    {
        if (mountingTypeToAdd is null)
        {
            MessageService.Warning("Please Select a Mounting Type First...", "Warning");
            return;
        }
        if (MountingTypes.Any(a => a.Id == mountingTypeToAdd.Id))
        {
            MessageService.Warning($"Selected '{mountingTypeToAdd.Trait.DefaultValue}' Mounting Type is already Present in the List ", "Warning");
            return;
        }
        MountingTypes.Add(mountingTypeToAdd);
        OnPropertyChanged(nameof(MountingTypes));
    }
    [RelayCommand]
    private void AddMountingVariation(BathAccessoryEntity entityToAdd)
    {
        if (entityToAdd is null)
        {
            MessageService.Warning("Please Select an Accessory First...", "Warning");
            return;
        }
        if (MountingVariations.Any(a => a.Id == entityToAdd.Id))
        {
            MessageService.Warning($"Selected '{entityToAdd.Code}' Variation is already Present in the List ", "Warning");
            return;
        }
        MountingVariations.Add(entityToAdd);
        OnPropertyChanged(nameof(MountingVariations));
    }
    [RelayCommand]
    private void AddDimension()
    {
        if (SelectedDimension is null)
        {
            MessageService.Warning("Please Select a Dimension First...", "Warning");
            return;
        }
        if (SelectedDimensions.Any(d => d.Trait.Id == SelectedDimension.Id))
        {
            MessageService.Warning($"Selected '{SelectedDimension.Trait.DefaultValue}' Dimension is already Present in the List ", "Warning");
            return;
        }

        SelectedDimensions.Add(new TraitEntityDoubleViewModel(SelectedDimension, SelectedDimensionValue));
        OnPropertyChanged(nameof(SelectedDimensions));
    }
    [RelayCommand]
    private void AddPriceInfo()
    {
        if (SelectedPrice is null)
        {
            MessageService.Warning("Please Select a Price First...", "Warning");
            return;
        }
        if (SelectedPriceInfoFinish == null && SelectedPriceInfoFinishGroup == null)
        {
            MessageService.Warning("Please first Select a Finish Group or a Finish with which the Price will Match", "Finish or Finish Group not Selected");
            return;
        }
        var priceInfoVm = new PriceInfoViewModel(SelectedPrice, SelectedPriceValue, SelectedPriceInfoFinish, SelectedPriceInfoFinishGroup);

        if (PricesInfo.Any(pi=> 
        pi.PriceTrait.IdAsString == priceInfoVm.PriceTrait.IdAsString &&
        (
        (pi.ReferingFinishTrait != null && pi.ReferingFinishTrait?.IdAsString == priceInfoVm.ReferingFinishTrait?.IdAsString) 
        ||
        (pi.ReferingFinishGroup != null && pi.ReferingFinishGroup?.IdAsString == priceInfoVm.ReferingFinishGroup?.IdAsString)
        )
        ))
        {
            MessageService.Warning("The Selected Price=> Finish/Group Combination has already been inserted please check again", "Combination Already Present");
            return;
        }

        PricesInfo.Add(priceInfoVm);
        OnPropertyChanged(nameof(PricesInfo));
    }
    [RelayCommand]
    private void EditLocalizedString(string localizedStringPropertyName)
    {
        LocalizedStringViewModel vm = localizedStringPropertyName switch
        {
            nameof(DescriptiveEntityViewModel.Name) => BaseDescriptiveEntity.Name,
            nameof(DescriptiveEntityViewModel.Description) => BaseDescriptiveEntity.Description,
            nameof(DescriptiveEntityViewModel.ExtendedDescription) => BaseDescriptiveEntity.ExtendedDescription,
            _ => throw new Exception("Unrecognized Localized String Entity to Edit"),
        };
        openEditLocalizedStringModalService.OpenModal(vm, $"{"lngEdit".TryTranslateKey()} - {string.Concat("lng", localizedStringPropertyName).TryTranslateKey()}");
    }
    [RelayCommand]
    private void OpenImageViewer(string imageUrl)
    {
        openImageViewerService.OpenModal(imageUrl);
    }

    /// <summary>
    /// Unsubscribes from Property Cahnged Events of all the AvailableFinish objects (FinishInfoVms)
    /// </summary>
    private void UnsubscribeAvailableFinishesChanges()
    {
        foreach (var af in AvailableFinishes)
        {
            af.PropertyChanged -= FinishInfoVm_PropertyChanged;
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
            closeModalService.ModalClosing -= CloseModalService_ModalClosing;
            UnsubscribeAvailableFinishesChanges();
        }

        //object has been disposed
        _disposed = true;

        //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
        //The subclasses only implement the virtual method and a field '_disposed'
        //Call the base Dispose(bool)
        base.Dispose(disposing);
    }
}

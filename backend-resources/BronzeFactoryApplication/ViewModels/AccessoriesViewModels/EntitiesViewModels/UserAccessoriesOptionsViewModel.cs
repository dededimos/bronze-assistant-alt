using AccessoriesRepoMongoDB.Entities;
using AccessoriesRepoMongoDB.Repositories;
using BathAccessoriesModelsLibrary;
using BathAccessoriesModelsLibrary.AccessoriesUserOptions;
using BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using MongoDbCommonLibrary.CommonEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.AccessoriesViewModels.EntitiesViewModels
{
    public partial class UserAccessoriesOptionsViewModel : BaseViewModel, IEditorViewModel<UserAccessoriesOptionsEntity>
    {
        private readonly ITraitGroupEntitiesRepository groupsRepo;
        private readonly MongoPriceRuleEntityRepo customPriceRulesRepo;
        [ObservableProperty]
        private DescriptiveEntityViewModel baseDescriptiveEntity;
        [ObservableProperty]
        private string name = string.Empty;
        [ObservableProperty]
        private string selectedAppearingDimensionsGroupId = string.Empty;
        [ObservableProperty]
        private string selectedPriceGroupId = string.Empty;
        [ObservableProperty]
        private bool isEnabled;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalDiscountPercent))]
        private decimal mainDiscountPercent;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalDiscountPercent))]
        private decimal secondaryDiscountPercent;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalDiscountPercent))]
        private decimal tertiaryDiscountPercent;

        public decimal TotalDiscountPercent { get => (1 - (1 - MainDiscountPercent) * (1 - SecondaryDiscountPercent) * (1 - TertiaryDiscountPercent)) * 100; }

        [ObservableProperty]
        private decimal quantityDiscPrimaryPercent;
        [ObservableProperty]
        private int quantityDiscQuantityPrimary;
        [ObservableProperty]
        private decimal quantityDiscSecondaryPercent;
        [ObservableProperty]
        private int quantityDiscQuantitySecondary;
        [ObservableProperty]
        private decimal quantityDiscTertiaryPercent;
        [ObservableProperty]
        private int quantityDiscQuantityTertiary;

        [ObservableProperty]
        private ObservableCollection<CustomPriceRuleEntity> customPriceRules = new();
        [ObservableProperty]
        private string? selectedCustomPriceRuleId ;


        /// <summary>
        /// Show only Price Trait Groups that can only be Assigned to PRICES
        /// </summary>
        public IEnumerable<TraitGroupEntity> PriceGroups { get => groupsRepo.Cache.Where(g => g.PermittedTraitTypes.Count == 1 && g.PermittedTraitTypes.Contains(TypeOfTrait.PriceTrait)); }
        public IEnumerable<TraitGroupEntity> AppearingDimensionsGroups { get => groupsRepo.Cache.Where(g => g.PermittedTraitTypes.Contains(TypeOfTrait.DimensionTrait)); }
        public IEnumerable<CustomPriceRuleEntity> AllCustomPriceRules { get => customPriceRulesRepo.Cache; }

        public UserAccessoriesOptionsViewModel(Func<DescriptiveEntityViewModel> baseDescriptiveEntityVmFactory,
                                               ITraitGroupEntitiesRepository groupsRepo,
                                               MongoPriceRuleEntityRepo customPriceRulesRepo)
        {
            baseDescriptiveEntity = baseDescriptiveEntityVmFactory.Invoke();
            this.groupsRepo = groupsRepo;
            this.customPriceRulesRepo = customPriceRulesRepo;
            this.customPriceRulesRepo.OnCacheRefresh += CustomPriceRulesRepo_OnCacheRefresh;
        }

        private void CustomPriceRulesRepo_OnCacheRefresh(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(AllCustomPriceRules));
        }

        [RelayCommand]
        private void AddCustomPriceRule()
        {
            if (string.IsNullOrEmpty(SelectedCustomPriceRuleId))
            {
                MessageService.Warning("Please Select a Price Rule to Add First", "Price Rulle not Selected");
                return;
            }
            var priceRule = AllCustomPriceRules.FirstOrDefault(pr => pr.IdAsString == SelectedCustomPriceRuleId);
            if (priceRule is not null)
            {
                CustomPriceRules.Add(priceRule);
                OnPropertyChanged(nameof(CustomPriceRules));
            }
            else
            {
                MessageService.Error("The Selected Price Rule was not Found in the List of Rules , for an Unexpected Reason", "Rule Not Found...");
            }
        }
        [RelayCommand]
        private void RemoveCustomPriceRule(CustomPriceRuleEntity priceRule)
        {
            if (MessageService.Question($"Do you Want to Remove this Rule ?{Environment.NewLine}{Environment.NewLine}{priceRule.Name}",
                "Rule Removal",
                "Ok","Cancel") == MessageBoxResult.Cancel)
            {
                return;
            }
            if (CustomPriceRules.Remove(priceRule)) 
            { 
                OnPropertyChanged(nameof(CustomPriceRules));
            };
        }
        
        public UserAccessoriesOptionsEntity CopyPropertiesToModel(UserAccessoriesOptionsEntity model)
        {
            BaseDescriptiveEntity.CopyPropertiesToModel(model);
            model.IsEnabled = IsEnabled;
            model.AppearingDimensionsGroup = SelectedAppearingDimensionsGroupId;
            model.PricesGroup = SelectedPriceGroupId;
            model.Discounts.MainDiscount = MainDiscountPercent / 100;
            model.Discounts.SecondaryDiscount = SecondaryDiscountPercent / 100;
            model.Discounts.TertiaryDiscount = TertiaryDiscountPercent / 100;
            model.Discounts.QuantityDiscPrimary = QuantityDiscPrimaryPercent / 100;
            model.Discounts.QuantityDiscQuantityPrimary = QuantityDiscQuantityPrimary;
            model.Discounts.QuantityDiscSecondary = QuantityDiscSecondaryPercent / 100;
            model.Discounts.QuantityDiscQuantitySecondary = QuantityDiscQuantitySecondary;
            model.Discounts.QuantityDiscTertiary = QuantityDiscTertiaryPercent / 100;
            model.Discounts.QuantityDiscQuantityTertiary = QuantityDiscQuantityTertiary;
            model.CustomPriceRules = new(this.CustomPriceRules.Select(pr=> pr.IdAsString));
            return model;
        }

        public UserAccessoriesOptionsEntity GetModel()
        {
            UserAccessoriesOptionsEntity model = new();
            return CopyPropertiesToModel(model);
        }

        public void SetModel(UserAccessoriesOptionsEntity model)
        {
            BaseDescriptiveEntity.SetModel(model);
            IsEnabled = model.IsEnabled;
            SelectedAppearingDimensionsGroupId = model.AppearingDimensionsGroup;
            SelectedPriceGroupId = model.PricesGroup;
            MainDiscountPercent = model.Discounts.MainDiscount * 100;
            SecondaryDiscountPercent = model.Discounts.SecondaryDiscount * 100;
            TertiaryDiscountPercent = model.Discounts.TertiaryDiscount * 100;

            QuantityDiscPrimaryPercent = model.Discounts.QuantityDiscPrimary * 100;
            QuantityDiscQuantityPrimary = model.Discounts.QuantityDiscQuantityPrimary;

            QuantityDiscSecondaryPercent = model.Discounts.QuantityDiscSecondary * 100;
            QuantityDiscQuantitySecondary = model.Discounts.QuantityDiscQuantitySecondary;

            QuantityDiscTertiaryPercent = model.Discounts.QuantityDiscTertiary * 100;
            QuantityDiscQuantityTertiary = model.Discounts.QuantityDiscQuantityTertiary;
            CustomPriceRules = new(AllCustomPriceRules.Where(pr=> model.CustomPriceRules.Any(id=> id == pr.IdAsString)));
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
                this.customPriceRulesRepo.OnCacheRefresh -= CustomPriceRulesRepo_OnCacheRefresh;
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

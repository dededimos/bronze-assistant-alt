using AKSoftware.Localization.MultiLanguages;
using BathAccessoriesModelsLibrary;
using BathAccessoriesModelsLibrary.AccessoriesUserOptions;
using BathAccessoriesModelsLibrary.Services;
using Blazored.LocalStorage;
using BronzeArtWebApplication.Shared.Enums;
using BronzeArtWebApplication.Shared.Models;
using BronzeArtWebApplication.Shared.Services;
using BronzeArtWebApplication.Shared.Services.SaveToStorageServices;
using BronzeRulesPricelistLibrary;
using BronzeRulesPricelistLibrary.Models;
using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.AccessoriesPriceables;
using CommonHelpers;
using CommonInterfacesBronze;
using CommunityToolkit.Mvvm.ComponentModel;
using MementosLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BronzeArtWebApplication.Shared.ViewModels.AccessoriesPageViewModels
{
#nullable enable
    public partial class BasketViewModel : BaseViewModelCT, ITransformable<BasketSave>, IRestorable<BasketSave>
    {
        private readonly IAccessoriesMemoryRepository repo;
        private readonly BronzeUser user;
        private readonly ILanguageContainerService lc;
        private readonly AccessoriesUrlHelper urlHelper;
        private readonly RulesDirector rulesDirector = new();
        private UserAccessoriesOptions currentlySelectedOptions = UserAccessoriesOptions.Undefined();
        public UserAccessoriesOptions CurrentlySelectedOptions { get => currentlySelectedOptions; }

        public ObservableCollection<BasketItemViewModel> Products { get; set; } = new();

        public int ItemsCount { get => Products.Count; }
        public decimal TotalNet { get => Products.Sum(p => p.Priceable.GetTotalQuantityNetPrice()); }
        public decimal TotalWithVat { get => Products.Sum(p => p.Priceable.GetTotalQuantityNetPriceWithVat()); }
        public decimal TotalRetailNet { get => Products.Sum(p => p.RetailPriceable.GetTotalQuantityNetPrice()); }
        public decimal TotalRetailWithVat { get => Products.Sum(p => p.RetailPriceable.GetTotalQuantityNetPriceWithVat()); }
        public bool PricesEnabled { get => user.IsPricingVisible && repo.AccessoriesOptions.PricesGroup != null; }
        public bool IsVatEnabled { get => PricesEnabled && user.VatFactor != 1 && user.SelectedAppMode == BronzeAppMode.Retail; }

        private BasketItemSave? savedCopyPreEdit;
        [ObservableProperty]
        private BasketItemViewModel? itemUnderEdit;


        [ObservableProperty]
        private string currentBasketName = string.Empty;
        [ObservableProperty]
        private string currentBasketNotes = string.Empty;


        public BasketViewModel(IAccessoriesMemoryRepository repo, BronzeUser user, ILanguageContainerService lc, AccessoriesUrlHelper urlHelper)
        {
            this.repo = repo;
            this.user = user;
            this.lc = lc;
            this.urlHelper = urlHelper;
            Products.CollectionChanged += Products_CollectionChanged;
            user.PropertyChanged += User_PropertyChanged;
            repo.OnRepositoryCreated += Repo_OnRepositoryCreated;
        }

        private void Repo_OnRepositoryCreated(object? sender, EventArgs e)
        {
            //put the Default Options on Repository Creation
            ChangeSelectedOptions(repo.AccessoriesOptions);
        }

        private void User_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BronzeUser.SelectedAppMode))
            {
                OnPropertyChanged(nameof(PricesEnabled));
            }
        }

        private void Products_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ItemsCount));
            OnPropertyChanged(nameof(TotalNet));
            OnPropertyChanged(nameof(TotalWithVat));
            OnPropertyChanged(nameof(TotalRetailNet));
            OnPropertyChanged(nameof(TotalRetailWithVat));
        }

        /// <summary>
        /// Adds a product to the Basket
        /// </summary>
        /// <param name="priceable"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void AddAccessoryProduct(BathroomAccessory acc, AccessoryFinish selectedFinish, int quantity)
        {
            var priceable = AccessoryPriceable.CreatePriceable(acc, selectedFinish, quantity, user.VatFactor, urlHelper);

            // Pass the Ref the Users Discounts
            var product = new BasketItemViewModel(
                priceable,
                currentlySelectedOptions ?? repo.AccessoriesOptions,
                rulesDirector
                );
            Products.Add(product);
        }
        /// <summary>
        /// Removes a product from the Basket
        /// </summary>
        /// <param name="product"></param>
        public void RemoveProduct(BasketItemViewModel product)
        {
            Products.Remove(product);
        }

        /// <summary>
        /// Start Editing an Item
        /// </summary>
        /// <param name="item"></param>
        public void StartEditItem(BasketItemViewModel item)
        {
            //Unsubscribe From Previous if it was not null
            if (ItemUnderEdit is not null)
            {
                ItemUnderEdit.PropertyChanged -= ItemUnderEdit_PropertyChanged;
            }
            // Set the New
            ItemUnderEdit = item;
            savedCopyPreEdit = item.GetTransformation();
            // Subscribe to the New
            ItemUnderEdit.PropertyChanged += ItemUnderEdit_PropertyChanged;
        }
        /// <summary>
        /// Finished Editing Item
        /// </summary>
        public void FinishEditItem(bool revertEdits)
        {
            if (ItemUnderEdit is not null)
            {
                ItemUnderEdit.PropertyChanged -= ItemUnderEdit_PropertyChanged;

                if (revertEdits && savedCopyPreEdit != null)
                {
                    //replace the currently edited item with its save
                    ItemUnderEdit.Restore(savedCopyPreEdit);
                }
            }
            ItemUnderEdit = null;
        }

        /// <summary>
        /// Inform that Item Under Edit Has Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemUnderEdit_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ItemUnderEdit));
        }

        public BasketSave GetTransformation()
        {
            BasketSave save = new()
            {
                BasketName = CurrentBasketName,
                BasketNotes = CurrentBasketNotes,
                SavedProducts = Products.Select(p => p.GetTransformation()).ToList(),
                TotalAmount = TotalNet,
                TotalAmounRetail = TotalRetailNet,
                IsUserAuthenticated = user.IsAuthenticated,
                SelectedAccessoriesOptions = currentlySelectedOptions?.Id ?? repo.AccessoriesOptions.Id
            };
            return save;
        }

        /// <summary>
        /// Restores the Basket to a new State based on the provided save Item
        /// </summary>
        /// <param name="restorator">The Save object</param>
        public void Restore(BasketSave restorator)
        {

            this.CurrentBasketNotes = restorator.BasketNotes;
            this.CurrentBasketName = restorator.BasketName;
            this.Products.Clear();
            
            //Change the Options to the Selected ONes from the Restorator Object . If its null use the Default Options for the current User
            var newOptionsToRestore = repo.AllAccessoriesOptions.FirstOrDefault(o => o.Id == restorator.SelectedAccessoriesOptions);
            ChangeSelectedOptions(newOptionsToRestore ?? repo.AccessoriesOptions);

            foreach (var itemRestorator in restorator.SavedProducts)
            {
                var accessory = repo.GetAccessoryById(itemRestorator.PriceableId);
                var finish = accessory?.AvailableFinishes.FirstOrDefault(af => af.Finish.Id == itemRestorator.SelectedFinishId);
                // if not Found with the Given Ids
                if (accessory == null || finish == null)
                {
                    accessory = BathroomAccessory.Empty(lc.Keys["ItemModifiedNotAvailable"]);
                    finish = AccessoryFinish.Empty();
                    itemRestorator.Quantity = 0;
                }

                var priceable = AccessoryPriceable.CreatePriceable(accessory, finish, decimal.ToInt32(itemRestorator.Quantity), user.VatFactor, urlHelper);
                BasketItemViewModel product =
                    new(
                        priceable,
                        currentlySelectedOptions ?? repo.AccessoriesOptions,
                        rulesDirector);

                // the basketItem restores only its basic props the backing fields cannot be restored by the basketItem itself
                product.Restore(itemRestorator);
                this.Products.Add(product);
            }
        }

        /// <summary>
        /// Clears the Retail Discount from each Item
        /// </summary>
        public void ClearRetailDiscounts()
        {
            foreach (var item in Products)
            {
                item.RevertRetailDiscountsToDefault();
            }
            OnPropertyChanged(nameof(Products));
        }
        /// <summary>
        /// Applies the Selected RetailDiscount to All items
        /// </summary>
        /// <param name="discountPercent"></param>
        public void ApplyRetailTotalDiscountToAll(decimal discountPercent)
        {
            foreach (var item in Products)
            {
                item.RetailDiscountPercent = discountPercent;
            }
            OnPropertyChanged(nameof(Products));
        }

        /// <summary>
        /// Disables Rules in all Items and sets Starting Price to Default  , if prompted to strip discounts => it also sets all discounts to zero
        /// </summary>
        /// <param name="stripDiscounts">Weather to clear discounts also apart from rules</param>
        public void DisableRulesApplyCataloguePriceToAll(bool stripDiscounts)
        {
            foreach (var item in Products)
            {
                item.AreExceptionRulesDisabled = true;
                item.RevertStartingPriceToDefault();
                if (stripDiscounts)
                {
                    item.PrimaryDiscountPercent = 0;
                    item.SecondaryDiscountPercent = 0;
                    item.TertiaryDiscountPercent = 0;
                }
            }
            OnPropertyChanged(nameof(Products));
        }

        /// <summary>
        /// Applies the User Options Discounts to All Items of the Wholesale Table
        /// </summary>
        public void RevertAllItemsToDefaultsWholeSale()
        {
            foreach (var item in Products)
            {
                item.RevertCodeToDefault();
                item.RevertDescriptionToDefault();
                item.RevertDiscountsToDefaults();
                item.RevertStartingPriceToDefault();
                item.AdditionalDiscountPercentRules = 0;
                item.AreExceptionRulesDisabled = false;
            }
            OnPropertyChanged(nameof(Products));
        }
        /// <summary>
        /// Applies Extra Discount to All Wholesale table Items
        /// </summary>
        /// <param name="discountPercent"></param>
        public void ApplyWholeSaleDiscountToAll(decimal discountPercent)
        {
            foreach (var item in Products)
            {
                if (item.AreExceptionRulesDisabled)
                {
                    //Put the Extra discount into Tertiary
                    item.TertiaryDiscountPercent = (1 - (1 - item.TertiaryDiscountPercent * 0.01m) * (1 - discountPercent * 0.01m)) * 100m;
                }
                else
                {
                    item.AdditionalDiscountPercentRules = (1 - (1 - item.AdditionalDiscountPercentRules * 0.01m) * (1 - discountPercent * 0.01m)) * 100m;
                }
            }
            OnPropertyChanged(nameof(Products));
        }

        public async Task<IEnumerable<UserAccessoriesOptions>> SearchUserAccessoriesOptions(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return repo.AllAccessoriesOptions;
            }
            else
            {
                return await Task.Run(() =>
                {
                    return repo.AllAccessoriesOptions.Where(o => o.DescriptionInfo.Name.Contains(searchTerm));
                });
            }
        }

        /// <summary>
        /// Generates Priceables for All The Accessories Combinations (All Available Finishes for All Accessories Codes)
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="cancelationToken"></param>
        /// <returns></returns>
        public async Task<List<AccessoryPriceable>> GeneratePriceablesForAllAccessories(IProgress<TaskProgressReport>? progress = null ,CancellationToken? cancelationToken = null)
        {
            double stepsCompleted = 0;
            cancelationToken?.ThrowIfCancellationRequested();
            progress?.Report(new(repo.Accessories.Count, stepsCompleted, $"{lc.Keys["CreatingPriceables..."]}"));

            // list to save priceables
            List<AccessoryPriceable> allPriceables = new();

            // Same rules and options for all priceables
            PricingRulesOptionsAccessories options = new()
            {
                MainDiscountDecimal = CurrentlySelectedOptions.Discounts.MainDiscountDecimal,
                SecondaryDiscountDecimal = CurrentlySelectedOptions.Discounts.SecondaryDiscountDecimal,
                TertiaryDiscountDecimal = CurrentlySelectedOptions.Discounts.TertiaryDiscountDecimal,
                //AdditionalFinalDiscountDecimal = 0,
                UsesCustomPrice = false,
                //CustomStartingPrice = --,
                WithQuantityDiscountsEnabled = false,
                PriceExceptions = CurrentlySelectedOptions.CustomPriceRules,
                AccessoryPriceGroupId = CurrentlySelectedOptions.PricesGroup?.Id ?? "",
            };
            
            //Generate the set of rules
            rulesDirector.GenerateNewRules(options);

            //Price each accessory in each finish
            foreach (var a in repo.Accessories)
            {
                foreach (var finish in a.AvailableFinishes)
                {
                    var p = AccessoryPriceable.CreatePriceable(a, finish, 1, user.VatFactor, urlHelper);
                    rulesDirector.ApplyRules(p);
                    allPriceables.Add(p);
                }
                if (progress is not null)
                {
                    stepsCompleted++;
                    cancelationToken?.ThrowIfCancellationRequested();
                    progress?.Report(new(repo.Accessories.Count, stepsCompleted, $"{lc.Keys["CreatingPriceables..."]}"));
                    await Task.Delay(1);
                }
            }

            //return all the Priceables
            return allPriceables;
        }

        /// <summary>
        /// Changes the Selected Accessories User Options to new Ones
        /// </summary>
        /// <param name="newOptions">The new Options</param>
        public void ChangeSelectedOptions(UserAccessoriesOptions newOptions)
        {
            if (currentlySelectedOptions != newOptions)
            {
                currentlySelectedOptions = newOptions;
                foreach (var item in Products)
                {
                    item.ChangeDefaultOptions(currentlySelectedOptions);
                }
            }
            OnPropertyChanged(nameof(Products));
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
                repo.OnRepositoryCreated -= Repo_OnRepositoryCreated;
                Products.CollectionChanged -= Products_CollectionChanged;
                if (ItemUnderEdit is not null)
                {
                    ItemUnderEdit.PropertyChanged -= ItemUnderEdit_PropertyChanged;
                    user.PropertyChanged -= User_PropertyChanged;
                    Products.CollectionChanged -= Products_CollectionChanged;
                }
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

using BathAccessoriesModelsLibrary;
using BathAccessoriesModelsLibrary.AccessoriesUserOptions;
using BronzeArtWebApplication.Shared.Enums;
using BronzeArtWebApplication.Shared.Models;
using BronzeArtWebApplication.Shared.Services.SaveToStorageServices;
using BronzeRulesPricelistLibrary;
using BronzeRulesPricelistLibrary.Models;
using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.AccessoriesPriceables;
using CommonInterfacesBronze;
using CommunityToolkit.Mvvm.ComponentModel;
using MementosLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BronzeArtWebApplication.Shared.ViewModels.AccessoriesPageViewModels
{
#nullable enable
    public partial class BasketItemViewModel : BaseViewModelCT, ITransformable<BasketItemSave>, IRestorable<BasketItemSave>
    {
        private UserAccessoriesOptions defaultAccessoryOptions;
        private decimal defaultAccessoryStartingPrice;
        private readonly string accessoryId = string.Empty;
        private readonly AccessoryFinish accessorySelectedFinish = AccessoryFinish.Empty();
        private readonly RulesDirector rulesDirector;
        private readonly IPriceable priceable;
        private readonly IPriceable retailPriceable;
        public IPriceable Priceable { get => priceable; }
        public IPriceable RetailPriceable { get => retailPriceable; }

        public AccessoryFinish SelectedAccessoryFinish { get => accessorySelectedFinish; }
        public decimal DefaultAccessoryStartingPrice { get => defaultAccessoryStartingPrice; }

        [ObservableProperty]
        private string overiddenCode = string.Empty;
        [ObservableProperty]
        private string notes = string.Empty;
        [ObservableProperty]
        private string overiddenDescription = string.Empty;
        public decimal Quantity
        {
            get => priceable.Quantity;
            set
            {
                if (SetProperty(priceable.Quantity, value, priceable, (model, val) => model.Quantity = val)) GeneratePricing();
            }
        }

        private decimal customizedPrice;
        public decimal CustomizedPrice
        {
            get => customizedPrice;
            set
            {
                if (customizedPrice != value)
                {
                    customizedPrice = value;
                    OnPropertyChanged(nameof(CustomizedPrice));
                    OnPropertyChanged(nameof(IsCataloguePrice));
                    GeneratePricing();
                }
            }
        }


        private decimal retailCustomizedPrice;
        public decimal RetailCustomizedPrice 
        {
            get => retailCustomizedPrice;
            set
            {
                if (value != retailCustomizedPrice)
                {
                    retailCustomizedPrice = value;
                    OnPropertyChanged(nameof(RetailCustomizedPrice));
                    OnPropertyChanged(nameof(IsRetailPriceCatalogue));
                    GenerateRetailPricing();
                }
            }
        }

        private decimal retailDiscountPercent;
        public decimal RetailDiscountPercent 
        {
            get => retailDiscountPercent;
            set
            {
                if (retailDiscountPercent != value)
                {
                    retailDiscountPercent = value;
                    OnPropertyChanged(nameof(RetailDiscountPercent));
                    GenerateRetailPricing();
                }
            }
        }


        private decimal primaryDiscountPercent;
        public decimal PrimaryDiscountPercent
        {
            get => primaryDiscountPercent;
            set
            {
                if (value != primaryDiscountPercent)
                {
                    primaryDiscountPercent = value;
                    //Do not trigger the Setter of the Total Discount 
                    totalManualDiscountPercent = GetTotalBasicDiscount();
                    OnPropertyChanged(nameof(PrimaryDiscountPercent));
                    OnPropertyChanged(nameof(TotalManualDiscountPercent));
                    OnPropertyChanged(nameof(AreDefaultDiscounts));
                    //Generate pricing on each subsequent set (sets only once from itself)
                    GeneratePricing();
                }
            }
        }
        private decimal secondaryDiscountPercent;
        public decimal SecondaryDiscountPercent
        {
            get => secondaryDiscountPercent;
            set
            {
                if (value != secondaryDiscountPercent)
                {
                    secondaryDiscountPercent = value;
                    //Do not trigger the Setter of the Total Discount 
                    totalManualDiscountPercent = GetTotalBasicDiscount();
                    OnPropertyChanged(nameof(SecondaryDiscountPercent));
                    OnPropertyChanged(nameof(TotalManualDiscountPercent));
                    OnPropertyChanged(nameof(AreDefaultDiscounts));
                    //Generate pricing on each subsequent set (sets only once from itself)
                    GeneratePricing();
                }
            }
        }
        private decimal tertiaryDiscountPercent;
        public decimal TertiaryDiscountPercent
        {
            get => tertiaryDiscountPercent;
            set
            {
                if (value != tertiaryDiscountPercent)
                {
                    tertiaryDiscountPercent = value;
                    //Do not trigger the Setter of the Total Discount 
                    totalManualDiscountPercent = GetTotalBasicDiscount();
                    OnPropertyChanged(nameof(TertiaryDiscountPercent));
                    OnPropertyChanged(nameof(TotalManualDiscountPercent));
                    OnPropertyChanged(nameof(AreDefaultDiscounts));
                    //Generate pricing on each subsequent set (sets only once from itself)
                    GeneratePricing();
                }
            }
        }

        private decimal totalManualDiscountPercent;
        public decimal TotalManualDiscountPercent
        {
            get => AreExceptionRulesDisabled ? totalManualDiscountPercent : Priceable.GetTotalDiscountPercent();
            set
            {
                if (value != totalManualDiscountPercent)
                {
                    totalManualDiscountPercent = value;
                    //Set only the backing fields so to not trigger the setters , inform for change and trigger the getters . This way there is no infinite loop
                    primaryDiscountPercent = totalManualDiscountPercent;
                    secondaryDiscountPercent = 0;
                    tertiaryDiscountPercent = 0;
                    OnPropertyChanged(nameof(TotalManualDiscountPercent));
                    OnPropertyChanged(nameof(PrimaryDiscountPercent));
                    OnPropertyChanged(nameof(SecondaryDiscountPercent));
                    OnPropertyChanged(nameof(TertiaryDiscountPercent));
                    OnPropertyChanged(nameof(AreDefaultDiscounts));
                    OnPropertyChanged(nameof(RulesOnlyDiscountPercent));
                    //Generate pricing on each subsequent set (sets only once from itself)
                    GeneratePricing();
                }
            }
        }

        /// <summary>
        /// Discount Applied On top of everything else in the End
        /// </summary>
        private decimal additionalDiscountPercentRules;
        public decimal AdditionalDiscountPercentRules 
        {
            get => additionalDiscountPercentRules;
            set
            {
                if (value!= additionalDiscountPercentRules)
                {
                    additionalDiscountPercentRules = value;
                    OnPropertyChanged(nameof(additionalDiscountPercentRules));
                    GeneratePricing();
                }
            }
        }

        /// <summary>
        /// The Discount of the Rules Only , if any
        /// </summary>
        public decimal RulesOnlyDiscountPercent { get => TotalManualDiscountPercent == 100 ? 0 :(1 - Priceable.GetTotalDiscountFactor() / (1 - TotalManualDiscountPercent * 0.01m)) * 100; }
        
        private bool areExceptionRulesDisabled;
        public bool AreExceptionRulesDisabled
        {
            get => areExceptionRulesDisabled;
            set
            {
                if (value != areExceptionRulesDisabled)
                {
                    areExceptionRulesDisabled = value;
                    OnPropertyChanged(nameof(AreExceptionRulesDisabled));
                    GeneratePricing();
                }
            }
        }

        public bool AreDefaultDiscounts { get => TotalManualDiscountPercent == defaultAccessoryOptions.Discounts.GetTotalBasicDiscountPercent(); }
        public bool IsCataloguePrice { get => CustomizedPrice == defaultAccessoryStartingPrice; }
        public bool IsRetailPriceCatalogue { get => RetailCustomizedPrice == defaultAccessoryStartingPrice; }

        public BasketItemViewModel(IPriceable priceable, 
            UserAccessoriesOptions defaultOptions,
            RulesDirector rulesDirector)
        {
            //Set the Default Values where needed
            this.defaultAccessoryOptions = defaultOptions;
            this.priceable = priceable;
            this.retailPriceable = priceable.GetNewCopy();
            this.rulesDirector = rulesDirector;
            if (priceable is AccessoryPriceable accessory)
            {
                defaultAccessoryStartingPrice = accessory.GetCataloguePrice(defaultOptions.PricesGroup?.Id ?? "").PriceValue;
                accessorySelectedFinish = accessory.SelectedFinish;
                accessoryId = accessory.Product.Id;
            }

            // Set Default Code
            RevertCodeToDefault();
            //Set Default Description
            RevertDescriptionToDefault();
            //Initilize the Discount Boxes with the Default Users Discounts
            RevertDiscountsToDefaults();
            //Initilize Starting Price with the Default Starting Price
            RevertStartingPriceToDefault();
            //Initilize Retail Price
            RevertRetailStartingPriceToDefault();
        }

        /// <summary>
        /// Passes the Users Discounts into the Discounts of the ViewModel
        /// </summary>
        public void RevertDiscountsToDefaults()
        {
            PrimaryDiscountPercent = defaultAccessoryOptions.Discounts.MainDiscountPercent;
            SecondaryDiscountPercent = defaultAccessoryOptions.Discounts.SecondaryDiscountPercent;
            TertiaryDiscountPercent = defaultAccessoryOptions.Discounts.TertiaryDiscountPercent;
        }
        /// <summary>
        /// Reverts the Starting Price to Default
        /// </summary>
        public void RevertStartingPriceToDefault()
        {
            CustomizedPrice = defaultAccessoryStartingPrice;
        }
        /// <summary>
        /// Reverts Retail Starting Price to Default
        /// </summary>
        public void RevertRetailStartingPriceToDefault()
        {
            RetailCustomizedPrice = defaultAccessoryStartingPrice;
        }
        public void RevertRetailDiscountsToDefault()
        {
            RetailDiscountPercent = 0;
        }
        public void RevertDescriptionToDefault()
        {
            OveriddenDescription = GetDefaultDescription();
        }
        private string GetDefaultDescription()
        {
            return string.Join(' ', priceable.DescriptionKeys);
        }
        public void RevertCodeToDefault()
        {
            OveriddenCode = priceable.Code;
        }

        /// <summary>
        /// Calculates the Total Discount
        /// </summary>
        /// <returns></returns>
        private decimal GetTotalBasicDiscount()
        {
            return (1 - (1 - PrimaryDiscountPercent * 0.01m) * (1 - SecondaryDiscountPercent * 0.01m) * (1 - TertiaryDiscountPercent * 0.01m)) * 100;
        }

        /// <summary>
        /// Generates Pricing for this Item
        /// </summary>
        /// <param name="rules"></param>
        public void GeneratePricing()
        {
            PricingRulesOptionsAccessories options = new()
            {
                MainDiscountDecimal = PrimaryDiscountPercent * 0.01m,
                SecondaryDiscountDecimal = SecondaryDiscountPercent * 0.01m,
                TertiaryDiscountDecimal = TertiaryDiscountPercent * 0.01m,
                AdditionalFinalDiscountDecimal = AreExceptionRulesDisabled ? 0 : (AdditionalDiscountPercentRules * 0.01m),
                UsesCustomPrice = !IsCataloguePrice,
                CustomStartingPrice = CustomizedPrice,
                WithQuantityDiscountsEnabled = false,
                PriceExceptions = AreExceptionRulesDisabled ? new() : defaultAccessoryOptions.CustomPriceRules,
                AccessoryPriceGroupId = defaultAccessoryOptions.PricesGroup?.Id ?? "",
            };
            rulesDirector.GenerateNewRules(options);
            rulesDirector.ApplyRules(this.Priceable, true);
        }

        public void GenerateRetailPricing()
        {
            PricingRulesOptionsAccessories options = new()
            {
                MainDiscountDecimal = RetailDiscountPercent * 0.01m,
                SecondaryDiscountDecimal = 0,
                TertiaryDiscountDecimal = 0,
                AdditionalFinalDiscountDecimal = 0,
                UsesCustomPrice = !IsRetailPriceCatalogue,
                CustomStartingPrice = RetailCustomizedPrice,
                WithQuantityDiscountsEnabled = false,
                PriceExceptions = new(),
                AccessoryPriceGroupId = defaultAccessoryOptions.PricesGroup?.Id ?? "",
            };
            rulesDirector.GenerateNewRules(options);
            rulesDirector.ApplyRules(this.RetailPriceable, true);
        }

        /// <summary>
        /// Returns a <see cref="BasketItemSave"/> transformation
        /// </summary>
        /// <returns></returns>
        public BasketItemSave GetTransformation()
        {
            BasketItemSave save = new()
            {
                PriceableId = accessoryId,
                SelectedFinishId = accessorySelectedFinish.Finish.Id,
                IsCodeOverriden = OveriddenCode != Priceable.Code,
                OveriddenCode = this.OveriddenCode,
                ItemNotes = this.Notes,
                IsDescriptionOveridden = OveriddenDescription != GetDefaultDescription(),
                OveriddenDescription = this.OveriddenDescription,
                Quantity = this.Quantity,
                CustomizedPrice = this.CustomizedPrice,
                PrimaryDiscountPercent = this.PrimaryDiscountPercent,
                SecondaryDiscountPercent = this.SecondaryDiscountPercent,
                TertiaryDiscountPercent = this.TertiaryDiscountPercent,
                //When non Authed default discounts are zero
                AppliesDefaultDiscounting =
                defaultAccessoryOptions.Discounts.MainDiscountPercent == PrimaryDiscountPercent &&
                defaultAccessoryOptions.Discounts.SecondaryDiscountPercent == SecondaryDiscountPercent &&
                defaultAccessoryOptions.Discounts.TertiaryDiscountPercent == TertiaryDiscountPercent,
                //When non Authed default price is zero
                AppliesDefaultStartingPrice = defaultAccessoryStartingPrice == CustomizedPrice,
                AreExceptionRulesDisabled = AreExceptionRulesDisabled,
                RetailAppliesDefaultStartingPrice = IsRetailPriceCatalogue,
                RetailCustomizedPrice = this.RetailCustomizedPrice,
                RetailDiscountPercent = this.RetailDiscountPercent,
                AdditionalRulesDiscountPercent = this.AdditionalDiscountPercentRules,
                
            };
            return save;
        }
        public void ChangeDefaultOptions(UserAccessoriesOptions newOptions)
        {
            //Before Changing to the New Options check if the item has overriden staff
            //if it has the overriden staff should not be reverted.

            //1.Check Rules/Discounts/StartingPrice before reverting
            bool shouldChangeDiscountsToNewDefaults = false;
            bool shouldChangeCataloguePriceToNewDefault = false;
            bool shouldRevertRetailToCatalogue = false;
            if (!AreExceptionRulesDisabled)
            {
                shouldChangeDiscountsToNewDefaults = true;
                shouldChangeCataloguePriceToNewDefault = true;
            }
            else
            {
                if (AreDefaultDiscounts)
                {
                    shouldChangeDiscountsToNewDefaults = true;
                }
                if (IsCataloguePrice)
                {
                    shouldChangeCataloguePriceToNewDefault = true;
                }
            }

            if (IsRetailPriceCatalogue)
            {
                shouldRevertRetailToCatalogue = true;
            }
            
            //Revert the Options
            defaultAccessoryOptions = newOptions;
            if (priceable is AccessoryPriceable accessory) 
            { 
                defaultAccessoryStartingPrice = accessory.GetCataloguePrice(defaultAccessoryOptions.PricesGroup?.Id ?? "").PriceValue;
            }
            //Revert anything else that needs reverting
            if (shouldChangeDiscountsToNewDefaults)
            {
                RevertDiscountsToDefaults();
            }
            if (shouldChangeCataloguePriceToNewDefault)
            {
                RevertStartingPriceToDefault();
            }
            if (shouldRevertRetailToCatalogue)
            {
                RevertRetailStartingPriceToDefault();
            }
            GeneratePricing();
        }
        public void Restore(BasketItemSave restorator)
        {
            if (restorator.IsCodeOverriden && !string.IsNullOrWhiteSpace(restorator.OveriddenCode))
            {
                this.OveriddenCode = restorator.OveriddenCode;
            }
            else
            {
                RevertCodeToDefault();
            }
            if (restorator.IsDescriptionOveridden && !string.IsNullOrWhiteSpace(restorator.OveriddenDescription))
            {
                this.OveriddenDescription = restorator.OveriddenDescription;
            }
            else
            {
                RevertDescriptionToDefault();
            }
            
            this.Quantity = restorator.Quantity;
            this.AdditionalDiscountPercentRules = restorator.AdditionalRulesDiscountPercent;
            this.Notes = restorator.ItemNotes;

            if (restorator.AppliesDefaultStartingPrice) RevertStartingPriceToDefault();
            else this.CustomizedPrice = restorator.CustomizedPrice;

            if (restorator.AppliesDefaultDiscounting) RevertDiscountsToDefaults();
            else
            {
                this.PrimaryDiscountPercent = restorator.PrimaryDiscountPercent;
                this.SecondaryDiscountPercent = restorator.SecondaryDiscountPercent;
                this.TertiaryDiscountPercent = restorator.TertiaryDiscountPercent;
            }
            AreExceptionRulesDisabled = restorator.AreExceptionRulesDisabled;

            this.RetailDiscountPercent = restorator.RetailDiscountPercent;
            if (restorator.RetailAppliesDefaultStartingPrice) RevertRetailStartingPriceToDefault();
            else this.RetailCustomizedPrice = restorator.RetailCustomizedPrice;
        }

    }


}

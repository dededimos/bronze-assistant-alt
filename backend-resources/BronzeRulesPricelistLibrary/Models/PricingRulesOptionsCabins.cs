using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.Models
{
    public class PricingRulesOptionsCabins
    {
        /// <summary>
        /// Gives a Starting Price to a Cabin derived Priceable
        /// </summary>
        public bool WithBasePriceRule { get; set; } = true;
        /// <summary>
        /// Applys an increased price when the Dimension is Special
        /// </summary>
        public bool WithSpecialDimensionRule { get; set; } = true;
        /// <summary>
        /// Applys a flat Increase to Base Price when the Dimension is Special and Meters Exceed Catalogue Meters
        /// </summary>
        public bool WithExceededSQMRule { get; set; } = false;
        /// <summary>
        /// Applies a flat increase to Base Price when the Hinges are more than 2 (Exception is some cases of DBCabin)
        /// </summary>
        public bool WithExtraHingesPriceRule { get; set; } = true;
        /// <summary>
        /// Applies a premium to Base Price for the Various Available Finishes 
        /// </summary>
        public bool WithFinishPriceRule { get; set; } = true;
        /// <summary>
        /// Prices the StepCut CabinExtra
        /// </summary>
        public bool WithStepCutRule { get; set; } = true;
        /// <summary>
        /// Prices the Bronze Clean CabinExtra
        /// </summary>
        public bool WithBronzeCleanPriceRule { get; set; } = true;
        /// <summary>
        /// Prices the SafeKids CabinExtra
        /// </summary>
        public bool WithSafeKidsPriceRule { get; set; } = true;
        /// <summary>
        /// Prices the Various Cabin Parts
        /// </summary>
        public bool WithPartsPricingRule { get; set; } = true;
        /// <summary>
        /// The Combined Discount of the User for the Cabins Catalogue
        /// </summary>
        public decimal UserCombinedDiscountCabins { get; set; } = 0m;
        /// <summary>
        /// Applies the User Discount to the Base Price of an Item
        /// </summary>
        public bool WithUserDiscountRule { get; set; } = true;
        /// <summary>
        /// Wheather an Added Premium is calculated on Irregular Glass Thicknesses 6/8 , 8/10 , 10
        /// </summary>
        public bool WithIrregularGlassThicknessRule { get; set; } = true;
        public bool IsDiscountToCabinApplicable { get; set; } = true;
        public bool IsDiscountToBronzeCleanApplicable { get; set; } = true;
        public bool IsDiscountToSafeKidsApplicable { get; set; } = true;
        public bool IsDiscountToStepCutApplicable { get; set; } = true;
        public bool IsDiscountToPartsApplicable { get; set; } = true;

        public bool WithIncreasePriceRule { get; set; } = false;
        public decimal PriceIncreaseFactor { get; set; } = 1m;

        //**********************13-06-2022**********************
        #region EXTRA DISCOUNT RULES - DEPRECATED WITH NEW PRICELIST
        /// <summary>
        /// Applies an Extra Discount on top of the User Discount (This Discount is Applies AFTER the UserDiscount 
        /// , it is not on the BasicPrice but on the Discounted Price )
        /// </summary>
        public bool WithFreeExtraDiscountRule { get; set; } = false;
        /// <summary>
        /// Applies an Extra Discount on top of the User Discount (This Discount is Applies AFTER the UserDiscount 
        /// , it is not on the BasicPrice but on the Discounted Price )
        /// </summary>
        public bool WithNiagaraExtraDiscountRule { get; set; } = false;
        /// <summary>
        /// Applies an Extra Discount on top of the User Discount (This Discount is Applies AFTER the UserDiscount 
        /// , it is not on the BasicPrice but on the Discounted Price )
        /// </summary>
        public bool WithHotel8000ExtraDiscountRule { get; set; } = false;
        /// <summary>
        /// Applies an Extra Discount on top of the User Discount (This Discount is Applies AFTER the UserDiscount 
        /// , it is not on the BasicPrice but on the Discounted Price )
        /// </summary>
        public bool WithInox304ExtraDiscountRule { get; set; } = false;
        /// <summary>
        /// Applies an Extra Discount on top of the User Discount (This Discount is Applies AFTER the UserDiscount 
        /// , it is not on the BasicPrice but on the Discounted Price )
        /// </summary>
        public bool WithB6000ExtraDiscountRule { get; set; } = false;

        /// <summary>
        /// The Extra Discount Applied for Niagara Items
        /// </summary>
        public decimal ExtraDiscountNiagara { get; set; } = 10m;
        /// <summary>
        /// The Extra Discount Applied for Hotel8000 Items
        /// </summary>
        public decimal ExtraDiscountHotel8000 { get; set; } = 10m;
        /// <summary>
        /// The Extra Discount Applied for Free Items
        /// </summary>
        public decimal ExtraDiscountFree { get; set; } = 10m;
        /// <summary>
        /// The Extra Discount Applied for Inox304 items
        /// </summary>
        public decimal ExtraDiscountInox304 { get; set; } = 10m;
        /// <summary>
        /// The Extra Discount Applied for B6000 items
        /// </summary>
        public decimal ExtraDiscountB6000 { get; set; } = 10m;

        #endregion
        //**********************13-06-2022**********************

        /// <summary>
        /// The Flat Price Increase per Excessive SQM from the Catalogue Dimensions
        /// </summary>
        public decimal FlatIncreasePerExceededSQM { get; set; } = 112m; //Based on the Cost of 8mm Glass

        /// <summary>
        /// The Flat Increase on Base Price per Extra Hinge (Other than the 2Hinges used as a default)
        /// </summary>
        public decimal FlatIncreasePerExtraHinge { get; set; } = 120m; //Based on the Cost of a Cut plus Cost of Hinge

        /// <summary>
        /// For this Thickness ExtraDiscount will not be Applied for B6000
        /// </summary>
        public CabinThicknessEnum ThicknessNotToApplyExtraDiscountB6000 { get; set; } = CabinThicknessEnum.Thick6mm;

        /// <summary>
        /// The models where the Structure will be detected as Customized if the Default Handle is Changed
        /// </summary>
        public List<Type> CabinTypesToApplyCustomDimOnHandleChange { get; set; } = new() { typeof(B6000), typeof(Cabin9C) };

        /// <summary>
        /// If not set B6000 Handle will be Charged
        /// </summary>
        public string DefaultB6000HandleCode { get; set; } = string.Empty;

        /// <summary>
        /// Default Options
        /// </summary>
        public PricingRulesOptionsCabins()
        {
            
        }
    }
}

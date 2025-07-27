using BronzeRulesPricelistLibrary.ConcreteRules;
using BronzeRulesPricelistLibrary.ConcreteRules.CommonRules;
using BronzeRulesPricelistLibrary.ConcreteRules.RulesAccessories;
using BronzeRulesPricelistLibrary.ConcreteRules.RulesCabins;
using BronzeRulesPricelistLibrary.ConcreteRules.RulesMirrors;
using BronzeRulesPricelistLibrary.Models;
using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.AccessoriesPriceables;
using BronzeRulesPricelistLibrary.Models.Priceables.CabinsPriceables;
using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary
{
    public static class RulesFactory
    {
        public static List<IPricingRule> GetCabinsRuleList(PricingRulesOptionsCabins options)
        {
            List<IPricingRule> rules = new();

            //Basic Rules
            rules.AddIf(options.WithBasePriceRule,               new CabinBasePriceRule());
            rules.AddIf(options.WithSpecialDimensionRule,        new CabinSpecialDimensionRule(options.CabinTypesToApplyCustomDimOnHandleChange,options.DefaultB6000HandleCode));
            rules.AddIf(options.WithIrregularGlassThicknessRule, new IrregularGlassThicknessRule());
            rules.AddIf(options.WithStepCutRule,                 new StepCutRule());
            rules.AddIf(options.WithBronzeCleanPriceRule,        new BronzeCleanPriceRule());
            rules.AddIf(options.WithSafeKidsPriceRule,           new SafeKidsPriceRule());
            rules.AddIf(options.WithFinishPriceRule,             new CabinFinishRule());
            rules.AddIf(options.WithExceededSQMRule ,            new CabinExceededSQMRule(options.FlatIncreasePerExceededSQM));
            rules.AddIf(options.WithExtraHingesPriceRule,        new ExrtaHingePriceRule(options.FlatIncreasePerExtraHinge));
            rules.AddIf(options.WithPartsPricingRule,            new CabinPartPricingRule());
            
            //PriceIncrease Rule
            rules.AddIf(options.WithIncreasePriceRule,    new IncreasePriceRule(options.PriceIncreaseFactor));

            //Client Discount Rule
            rules.AddIf(options.WithUserDiscountRule && options.IsDiscountToCabinApplicable,         new DiscountPriceRule(options.UserCombinedDiscountCabins, typeof(PriceableCabin)));
            rules.AddIf(options.WithUserDiscountRule && options.IsDiscountToCabinApplicable,         new DiscountPriceRule(options.UserCombinedDiscountCabins, typeof(PriceableCabin9C)));
            rules.AddIf(options.WithUserDiscountRule && options.IsDiscountToStepCutApplicable,       new DiscountPriceRule(options.UserCombinedDiscountCabins, typeof(PriceableStepCut)));
            rules.AddIf(options.WithUserDiscountRule && options.IsDiscountToBronzeCleanApplicable,   new DiscountPriceRule(options.UserCombinedDiscountCabins, typeof(PriceableBronzeClean)));
            rules.AddIf(options.WithUserDiscountRule && options.IsDiscountToSafeKidsApplicable,      new DiscountPriceRule(options.UserCombinedDiscountCabins, typeof(PriceableSafekids)));
            rules.AddIf(options.WithUserDiscountRule && options.IsDiscountToPartsApplicable,         new DiscountPriceRule(options.UserCombinedDiscountCabins, typeof(PriceableCabinPart)));
            rules.AddIf(options.WithUserDiscountRule && options.IsDiscountToPartsApplicable,         new DiscountPriceRule(options.UserCombinedDiscountCabins, typeof(PriceableWFrame)));            

            //Extra Discount Rules (DEPRECATED) *************** WE COULD USE DISCOUNT PRICE RULE AND MODIFY THE CONSTRUCTOR A BIT TO ACCEPT DIFFERENT NAME-TYPES-CONSTRAINTS
            //INSTEAD OF USING TWO DISCOUNT TYPE RULES
            rules.AddIf(options.WithFreeExtraDiscountRule,      new ExtraDiscountRule(options.ExtraDiscountFree, typeof(Free)));
            rules.AddIf(options.WithNiagaraExtraDiscountRule,   new ExtraDiscountRule(options.ExtraDiscountNiagara, typeof(Niagara6000)));
            rules.AddIf(options.WithHotel8000ExtraDiscountRule, new ExtraDiscountRule(options.ExtraDiscountHotel8000, typeof(Hotel8000)));
            rules.AddIf(options.WithInox304ExtraDiscountRule,   new ExtraDiscountRule(options.ExtraDiscountInox304, typeof(Inox304)));
            rules.AddIf(options.WithB6000ExtraDiscountRule,     new ExtraDiscountRule(options.ExtraDiscountB6000, typeof(B6000),options.ThicknessNotToApplyExtraDiscountB6000));
            //*************************************************


            return rules;
        }
        public static List<IPricingRule> GetMirrorsRuleList(PricingRulesOptionsMirrors options)
        {
            List<IPricingRule> rules = new();
            //Basic Rules
            rules.AddIf(options.WithCustomMirrorPriceRule,      new CustomMirrorPriceRule());
            rules.AddIf(options.WithCatalogueMirrorPriceRule,   new CatalogueMirrorPriceRule());
            rules.AddIf(options.WithSandblastPriceRule,         new SandblastPriceRule());
            rules.AddIf(options.WithLightPriceRule,             new LightingPriceRule());
            rules.AddIf(options.WithSupportPriceRule,           new MirrorSupportPriceRule());
            rules.AddIf(options.WithMirrorExtraPriceRule,       new MirrorExtraPriceRule());
            rules.AddIf(options.WithCapsulePriceRule,           new CapsuleMirrorPriceRule(options.CapsuleAddedPremiumPerSQM,options.CapsuleFlatMinimumAddedPremium));
            rules.AddIf(options.WithCircularPriceRule,          new CircularMirrorPriceRule(options.CircularAddedPremiumPerSQM,options.CircuarFlatMinimumAddedPremium));
            rules.AddIf(options.WithEllipsePriceRule,           new EllipseMirrorPriceRule(options.EllipseAddedPremiumPerSQM,options.EllipseFlatMinimumAddedPremium));
            //Price Increase Rule
            rules.AddIf(options.WithIncreasePriceRule,          new IncreasePriceRule(options.PriceIncreaseFactor));
            
            //Client Discount Rule
            rules.AddIf(options.WithUserDiscountPriceRule,      new DiscountPriceRule(options.UserCombinedDiscountMirrors, typeof(IPriceable)));

            return rules;
        }
        public static List<IPricingRule> GetAccessoriesRuleList(PricingRulesOptionsAccessories options)
        {
            List<IPricingRule> rules = new();

            rules.Add(new AccessoryBasePriceRule(options));

            rules.AddIf(options.MainDiscountDecimal > 0,      new DiscountPriceRule(options.MainDiscountDecimal, typeof(AccessoryPriceable),discountFormatLessThan1:true));
            rules.AddIf(options.SecondaryDiscountDecimal > 0, new DiscountPriceRule(options.SecondaryDiscountDecimal, typeof(AccessoryPriceable),"SecondaryDiscountRule", true));
            rules.AddIf(options.TertiaryDiscountDecimal > 0,  new DiscountPriceRule(options.TertiaryDiscountDecimal, typeof(AccessoryPriceable),"TertiaryDiscountRule", true));
            
            //Add any Irregular Rules
            foreach (var pe in options.PriceExceptions)
            {
                rules.Add(new AccessoryPriceRuleException(pe));
            }

            //Add Additional Final Discount Rule if any
            rules.AddIf(options.AdditionalFinalDiscountDecimal > 0, new DiscountPriceRule(options.AdditionalFinalDiscountDecimal, typeof(AccessoryPriceable),$"{nameof(ExtraDiscountRule)}", true));

            return rules;
        }

    }
}

using BronzeRulesPricelistLibrary.ConcreteRules.CommonRules;
using BronzeRulesPricelistLibrary.ConcreteRules.RulesAccessories;
using BronzeRulesPricelistLibrary.ConcreteRules.RulesCabins;
using BronzeRulesPricelistLibrary.ConcreteRules.RulesMirrors;
using FluentValidation.Validators;
using System.Collections.Generic;

namespace BronzeArtWebApplication.Shared.Helpers
{
    public static class CommonStaticInfo
    {
        public static readonly Dictionary<string, string> PricingRulesDescKeys = new()
        {
            { nameof(BronzeCleanPriceRule),        "BronzeCleanRule"},
            { nameof(CabinBasePriceRule),          "CabinBasePriceRule" },
            { nameof(CabinFinishRule),             "CabinFinishPriceRule" },
            { nameof(CabinSpecialDimensionRule),   "CabinSpecialDimensionRule" },
            { nameof(IrregularGlassThicknessRule), "IrregularGlassThicknessRule" },
            { nameof(ExtraDiscountRule),           "CabinExtraDiscountRule" },
            { nameof(SafeKidsPriceRule),           "SafeKidsPriceRule" },
            { nameof(StepCutRule),                 "StepCutPriceRule" },
            { nameof(CabinExceededSQMRule),        "CabinExceededSQMRule"},
            { nameof(ExrtaHingePriceRule),         "ExtraHingePriceRule" },
            { nameof(CabinPartPricingRule),        "CabinPartPricingRule"},
            { nameof(CustomMirrorPriceRule) ,      "CustomMirrorPriceRule" },
            { nameof(CatalogueMirrorPriceRule),    "CatalogueMirrorPriceRule"},
            { nameof(DiscountPriceRule),           "ClientDiscountPriceRule" },
            { nameof(IncreasePriceRule),           "IncreasePriceRule" },
            { nameof(SandblastPriceRule),          "SandblastPriceRule" },
            { nameof(LightingPriceRule),           "LightingPriceRule" },
            { nameof(MirrorSupportPriceRule),      "MirrorSupportPriceRule" },
            { nameof(MirrorExtraPriceRule),        "MirrorExtraPriceRule" },
            { nameof(CapsuleMirrorPriceRule),      "CapsuleMirrorPriceRule" },
            { nameof(CircularMirrorPriceRule),     "CircularMirrorPriceRule" },
            { nameof(EllipseMirrorPriceRule),      "EllipseMirrorPriceRule" },
            { nameof(AccessoryBasePriceRule),      "AccessoryBasePriceRule" },
        };
    }
}

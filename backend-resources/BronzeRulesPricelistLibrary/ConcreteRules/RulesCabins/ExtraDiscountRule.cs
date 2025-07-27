using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.CabinsPriceables;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.ConcreteRules.RulesCabins
{
    public class ExtraDiscountRule : IPricingRule
    {
        public string RuleName { get => nameof(ExtraDiscountRule); }
        public string RuleApplicationDescription { get; set; } = "";

        private readonly decimal extraDiscountToApply;
        private readonly Type typeOfProductToApply;
        private readonly CabinThicknessEnum? notAppliedThickness1 = null; //Rule will not be Applied for this Thickness
        private readonly CabinThicknessEnum? notAppliedThickness2 = null; //Rule will not be Applied for this Thickness
        public string ErrorMessage { get; set; } = "";
        public bool HasError { get; set; }
        public RuleModification ModificationType { get => RuleModification.SetsAdditionalDiscountPercent; }

        public ExtraDiscountRule(decimal extraDiscountToApply, Type typeOfProductToApplyRule)
        {
            this.extraDiscountToApply = extraDiscountToApply;
            typeOfProductToApply = typeOfProductToApplyRule;
        }

        public ExtraDiscountRule(decimal extraDiscountToApply, Type typeOfProductToApplyRule, CabinThicknessEnum? notAppliedForThickness1 = null, CabinThicknessEnum? notAppliedForThickness2 = null)
        {
            this.extraDiscountToApply = extraDiscountToApply;
            typeOfProductToApply = typeOfProductToApplyRule;
            notAppliedThickness1 = notAppliedForThickness1;
            notAppliedThickness2 = notAppliedForThickness2;
        }

        /// <summary>
        /// Add an Error Encountered during the Rule's application
        /// </summary>
        /// <param name="message">error message</param>
        public void AddError(string message)
        {
            ErrorMessage = message;
            HasError = true;
        }

        public void ApplyRule(IPriceable product)
        {
            product.Discounts.Add(extraDiscountToApply / 100m);
            RuleApplicationDescription = $"-{extraDiscountToApply / 100m:0.00%}";
        }

        public bool IsApplicable(IPriceable product)
        {
            //WrappedObject of IPriceable has the same type or Derives from the Type Passed in the Constructor of the Rule
            if (product.GetWrappedObjectType().IsSubclassOf(typeOfProductToApply) || product.GetWrappedObjectType() == typeOfProductToApply)
            {
                //Do Not Apply if item is a cabin with a thickness one of the non Applied Thicknesses
                if (product is PriceableCabin c
                            && (c.CabinProperties.Thicknesses == notAppliedThickness1 || c.CabinProperties.Thicknesses == notAppliedThickness2))
                {
                    return false;
                }
                return true;
            }
            //Return false if Types do not Match
            return false;
        }
    }
}

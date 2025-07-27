using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.MirrorsPriceables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.ConcreteRules.CommonRules
{
    /// <summary>
    /// Represents a Rule that Applys a Discount to Certain Types of IPriceables
    /// </summary>
    public class DiscountPriceRule : IPricingRule
    {
        private readonly string ruleNameOverride = string.Empty;
        public string RuleName { get => string.IsNullOrEmpty(ruleNameOverride) ? nameof(DiscountPriceRule) : ruleNameOverride; }
        public string RuleApplicationDescription { get; set; } = "";

        private readonly decimal discountToApply; //The Discount to be Applied
        private readonly Type discountedType; //The Type of Object that gets discounted
        private readonly bool discFormatLessThan1;

        public string ErrorMessage { get; set; } = "";
        public bool HasError { get; set; }
        public RuleModification ModificationType { get => RuleModification.SetsAdditionalDiscountPercent; }

        /// <summary>
        /// Creates a rule Object that applies a discount to tthe specified type
        /// </summary>
        /// <param name="discountToApply">The Discount to be Applied (Format 100.00%)</param>
        /// <param name="productType">The Type of items receiving the Discount(Valid IPriceable Items)</param>
        /// <param name="discountFormatLessThan1">Weather the Discount is in less than 1 format ex. 0.35 instead of 35% </param>
        /// <exception cref="NotSupportedException">Only IPriceables are accepted as types</exception>
        public DiscountPriceRule(decimal discountToApply, Type productType, string ruleNameOverride = "", bool discountFormatLessThan1 = false)
        {
            //Checks if the product type implements IPriceable or throws
            if (typeof(IPriceable).IsAssignableFrom(productType) == false)
            {
                throw new NotSupportedException("Discount Rule can Only be Created for Objects that implement IPriceable");
            }
            else
            {
                this.discountToApply = discountToApply;
                this.discountedType = productType;
                this.ruleNameOverride = ruleNameOverride;
            }

            this.discFormatLessThan1 = discountFormatLessThan1;
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
            if (discFormatLessThan1)
            {
                product.Discounts.Add(discountToApply);
                RuleApplicationDescription = $"-{discountToApply:0.00%}";
            }
            else
            {
                product.Discounts.Add(discountToApply / 100m);
                RuleApplicationDescription = $"-{discountToApply / 100m:0.00%}";
            }
        }

        public bool IsApplicable(IPriceable product)
        {
            if (product.GetType() == discountedType ||
                product.GetType().IsSubclassOf(discountedType) ||
                discountedType.IsAssignableFrom(product.GetType()))//If the DiscountedType is an Interface that is Implemented by the type of the product
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

using BronzeRulesPricelistLibrary.Models.Priceables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.ConcreteRules.CommonRules
{
    public class IncreasePriceRule : IPricingRule
    {
        public string RuleName { get => nameof(IncreasePriceRule); }
        public string RuleApplicationDescription { get; set; } = "";

        private readonly decimal increaseFactor; //The Discount to be Applied

        public string ErrorMessage { get; set; } = "";
        public bool HasError { get; set; }
        public RuleModification ModificationType { get => RuleModification.SetsAdditionalPricePercent; }

        /// <summary>
        /// Creates a rule Object that applies an Increase 
        /// </summary>
        /// <param name="increaseFactor">The Increase to be Applied (Format 1.00)</param>
        public IncreasePriceRule(decimal increaseFactor)
        {
            this.increaseFactor = increaseFactor;
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
            product.StartingPrice *= increaseFactor;
            RuleApplicationDescription = $"+{increaseFactor - 1:0.00%}";
        }

        public bool IsApplicable(IPriceable product)
        {
            //Applied only when an increase Factor exists 
            if (increaseFactor > 1)
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

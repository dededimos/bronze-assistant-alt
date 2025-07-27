using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.MirrorsPriceables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.ConcreteRules.RulesMirrors
{
    public class SandblastPriceRule : IPricingRule
    {
        public string RuleName { get => nameof(SandblastPriceRule); }
        public string RuleApplicationDescription { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
        public bool HasError { get; set; }
        public RuleModification ModificationType { get => RuleModification.SetsStartingPrice; }

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
            product.StartingPrice = 0m;
            RuleApplicationDescription = $"+{product.StartingPrice:0.00€}";
        }

        public bool IsApplicable(IPriceable product)
        {
            if (product is PriceableSandblast)
            {
                return true;
            }
            return false;
        }
    }
}

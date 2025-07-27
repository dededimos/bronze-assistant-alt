using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.CabinsPriceables;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.ConcreteRules.RulesCabins
{
    public class StepCutRule : IPricingRule
    {
        public string RuleName { get => nameof(StepCutRule); }
        public string ErrorMessage { get; set; } = "";
        public bool HasError { get; set; }
        public string RuleApplicationDescription { get; set; } = "";
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
            product.StartingPrice += 100;
            RuleApplicationDescription = $"+{100:0.00€}";
        }

        public bool IsApplicable(IPriceable product)
        {
            if (product is PriceableStepCut)
            {
                return true;
            }
            return false;
        }
    }
}

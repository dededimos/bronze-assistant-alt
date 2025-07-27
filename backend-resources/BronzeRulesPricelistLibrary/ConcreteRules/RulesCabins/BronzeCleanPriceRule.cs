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
    public class BronzeCleanPriceRule : IPricingRule
    {
        public string RuleName { get => nameof(BronzeCleanPriceRule); }
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
            PriceableBronzeClean extra = (PriceableBronzeClean)product;
            extra.StartingPrice = CabinsPricelist.BronzeCleanPricelist[extra.ModelOfCabin];
            RuleApplicationDescription = $"+{extra.StartingPrice.ToString("0.00\u20AC")}";
        }

        public bool IsApplicable(IPriceable product)
        {
            if (product is PriceableBronzeClean)
            {
                return true;
            }
            return false;
        }
    }
}

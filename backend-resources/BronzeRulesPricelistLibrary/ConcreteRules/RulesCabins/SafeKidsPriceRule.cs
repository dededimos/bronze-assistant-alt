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
    public class SafeKidsPriceRule : IPricingRule
    {
        public string RuleName { get => nameof(SafeKidsPriceRule); }
        public string RuleApplicationDescription { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
        public bool HasError { get; set; }
        public RuleModification ModificationType { get => RuleModification.SetsStartingPrice; }

        public SafeKidsPriceRule()
        {

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
            try
            {
                if (product is PriceableSafekids extra)
                {
                    product.StartingPrice = CabinsPricelist.SafeKidsPricelist[extra.ModelOfCabin];
                    RuleApplicationDescription = $"({extra.ModelOfCabin.ToString().Replace("Model","")}) +{product.StartingPrice}\u20AC";
                }
                else
                {
                    throw new NullReferenceException("Application Error");
                }
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                product.StartingPrice = 0;
                RuleApplicationDescription = $"Error Executing Rule";
            }
        }

        public bool IsApplicable(IPriceable product)
        {
            if (product is PriceableSafekids)
            {
                return true;
            }
            return false;
        }
    }
}

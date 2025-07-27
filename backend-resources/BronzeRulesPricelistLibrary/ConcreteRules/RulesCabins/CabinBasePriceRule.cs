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
    public class CabinBasePriceRule : IPricingRule
    {
        public string RuleName { get => nameof(CabinBasePriceRule); }
        public string RuleApplicationDescription { get; set; } = "";

        /// <summary>
        /// The Encountered Error While Applying the Rule
        /// </summary>
        public string ErrorMessage { get; set; } = "";
        /// <summary>
        /// True If the Rule Encountered an Error during its application
        /// </summary>
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

        /// <summary>
        /// Apply the Rule
        /// </summary>
        /// <param name="cabin"></param>
        public void ApplyRule(IPriceable product)
        {
            try
            {
                if (product is PriceableCabin cabinProduct)
                {
                    product.StartingPrice = CabinsPricelist.GetBasePrice(cabinProduct.CabinProperties);
                }
                else if (product is PriceableCabin9C cabin9CProduct)
                {
                    decimal secondPiecePrice = CabinsPricelist.GetBasePrice(cabin9CProduct.CabinPropertiesSecond);
                    decimal firstPiecePrice = CabinsPricelist.GetBasePrice(cabin9CProduct.CabinPropertiesFirst);
                    product.StartingPrice = Math.Max(firstPiecePrice, secondPiecePrice);
                }
                else
                {
                    throw new ArgumentException("Invalid CabinBasePrice Rule Application attempted to Run on Non Valid IPriceable");
                }

                RuleApplicationDescription = $"+{product.StartingPrice:0.00€}";
            }
            catch (ArgumentException ex)
            {
                AddError(ex.Message);
                product.StartingPrice = 0;
                RuleApplicationDescription = $"Error Executing Rule";
            }
        }

        /// <summary>
        /// Check if the Rule should be applied
        /// </summary>
        /// <param name="cabin"></param>
        /// <returns></returns>
        public bool IsApplicable(IPriceable product)
        {
            if (product is PriceableCabin or PriceableCabin9C)
            {
                return true;
            }
            return false;
        }
    }
}

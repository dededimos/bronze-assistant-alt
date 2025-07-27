using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.CabinsPriceables;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels.HandlesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.ConcreteRules.RulesCabins
{
    public class CabinPartPricingRule : IPricingRule
    {
        public string ErrorMessage { get; set; } = string.Empty;
        public bool HasError { get; set; }
        public string RuleName { get => nameof(CabinPartPricingRule); }
        public string RuleApplicationDescription { get; set; } = string.Empty;
        public RuleModification ModificationType { get => RuleModification.SetsStartingPrice; }

        public void AddError(string message)
        {
            ErrorMessage = message;
            HasError = true;
        }

        public void ApplyRule(IPriceable product)
        {
            try
            {
                if (product is PriceableCabinPart)
                {
                    product.StartingPrice = CabinsPricelist.GetBasePartPrice(product.Code);
                }
                else if (product is PriceableWFrame frame)
                {
                    frame.StartingPrice = CabinsPricelist.GetBaseWFramePrice(frame.FrameFinish);
                }
                RuleApplicationDescription = $"{product.StartingPrice:0.00€}";

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
            if (product is PriceableCabinPart or PriceableWFrame)
            {
                return true;
            }
            return false;
        }
    }
}

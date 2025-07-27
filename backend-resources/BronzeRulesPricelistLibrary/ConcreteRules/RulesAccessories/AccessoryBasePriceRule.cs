using BronzeRulesPricelistLibrary.Models;
using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.AccessoriesPriceables;
using BronzeRulesPricelistLibrary.Models.Priceables.CabinsPriceables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.ConcreteRules.RulesAccessories
{
    public class AccessoryBasePriceRule : IPricingRule
    {
        public const string CustomStartingPriceRuleName = "CustomStartingPriceRule";
        private readonly PricingRulesOptionsAccessories options;
        private string ruleNameOverride;

        public string ErrorMessage { get; set; } = string.Empty;
        public bool HasError { get; set; }
        public string RuleName { get => string.IsNullOrWhiteSpace(ruleNameOverride) ? nameof(AccessoryBasePriceRule) : ruleNameOverride; }
        public string RuleApplicationDescription { get; set; } = string.Empty;
        public RuleModification ModificationType { get => RuleModification.SetsStartingPrice; }

        /// <summary>
        /// A Rule Setting the Starting Price of an AccessoryPriceable Only
        /// </summary>
        /// <param name="options">The Options</param>
        /// <param name="ruleNameOverride">Weather to override the Name of the Rule (Maybe its used multiple times ?)</param>
        public AccessoryBasePriceRule(PricingRulesOptionsAccessories options , string ruleNameOverride = "")
        {
            this.options = options;
            this.ruleNameOverride = ruleNameOverride;
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
                if (product is AccessoryPriceable accessoryProduct)
                {
                    if (options.UsesCataloguePrice || options.CustomStartingPrice == accessoryProduct.GetCataloguePrice(options.AccessoryPriceGroupId).PriceValue)
                    {
                        product.StartingPrice = accessoryProduct.GetCataloguePrice(options.AccessoryPriceGroupId).PriceValue;
                    }
                    //else put the customized price and change also the rule naming if its not already overidden
                    else
                    {
                        product.StartingPrice = options.CustomStartingPrice;
                        if (string.IsNullOrEmpty(ruleNameOverride))
                        {
                            ruleNameOverride = CustomStartingPriceRuleName;
                        }
                    }
                    
                }
                
                RuleApplicationDescription = $"+{product.StartingPrice:0.00€}";

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
            if (product is AccessoryPriceable)
            {
                return true;
            }
            return false;
        }
    }
}

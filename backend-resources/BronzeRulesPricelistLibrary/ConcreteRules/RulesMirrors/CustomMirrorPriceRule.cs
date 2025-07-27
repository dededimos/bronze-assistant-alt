using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.MirrorsPriceables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.ConcreteRules.RulesMirrors
{
    public class CustomMirrorPriceRule : IPricingRule
    {
        public string RuleName { get => nameof(CustomMirrorPriceRule); }
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

        public void ApplyRule(IPriceable product)
        {
            try
            {
                PriceableMirror mirrorProduct = product as PriceableMirror ?? throw new NotSupportedException("IPriceable is not a Mirror");

                product.StartingPrice = MirrorsPricelist.GetMirrorPriceWithoutExtras(mirrorProduct.MirrorProperties);
                RuleApplicationDescription = $"+{product.StartingPrice:0.00€}";
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                product.StartingPrice = 0;
                RuleApplicationDescription = $"Error Executing {RuleName}";
            }
        }

        public bool IsApplicable(IPriceable product)
        {
            if (product is PriceableMirror mirror && mirror.MirrorProperties.IsFromCatalogue() is false)
            {
                return true;
            }
            return false;
        }
    }
}

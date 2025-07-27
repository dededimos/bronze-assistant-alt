using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.MirrorsPriceables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.ConcreteRules.RulesMirrors
{
    public class CatalogueMirrorPriceRule : IPricingRule
    {
        public string RuleName { get => nameof(CatalogueMirrorPriceRule); }
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
                PriceableMirror mirrorProduct = product as PriceableMirror ?? throw new ArgumentException("IPriceable is not a Mirror");

                product.StartingPrice = MirrorsPricelist.GetMirrorPriceFromCatalogue(product.Code);
                RuleApplicationDescription = $"+{product.StartingPrice.ToString("0.00\u20AC")}";
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
            if (product is PriceableMirror mirror && mirror.MirrorProperties.IsFromCatalogue())
            {
                return true;
            }
            return false;
        }
    }
}

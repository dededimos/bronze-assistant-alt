using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.MirrorsPriceables;
using MirrorsModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.ConcreteRules.RulesMirrors
{
    public class MirrorSupportPriceRule : IPricingRule
    {
        public string RuleName { get => nameof(MirrorSupportPriceRule); }
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
            PriceableMirrorSupport supportProduct = product as PriceableMirrorSupport ?? throw new NotSupportedException("Only Mirror Supports Alowed for Support Price Rule Application");
            product.StartingPrice = MirrorsPricelist.GetMirrorSupportPrice(supportProduct.ParentMirror);

            RuleApplicationDescription = $"+{product.StartingPrice:0.00€}";
        }

        public bool IsApplicable(IPriceable product)
        {
            if (product is PriceableMirrorSupport)
            {
                return true;
            }
            return false;
        }
    }
}

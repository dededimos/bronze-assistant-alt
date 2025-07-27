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
    public class MirrorExtraPriceRule : IPricingRule
    {
        public string RuleName { get => nameof(MirrorExtraPriceRule); }
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
            PriceableMirrorExtra extraProduct = product as PriceableMirrorExtra ?? throw new NotSupportedException("Only MirrorExtras Alowed for MirrorExtra Price Rule Application");
            Mirror? equivalentCatalogueMirror = extraProduct.ParentMirror.GetEquivalentCatalogueMirror();
            bool isMirrorSpecialDimension = equivalentCatalogueMirror == null;
            bool isDefaultExtra = equivalentCatalogueMirror?.HasExtra(extraProduct.ExtraProperties.Option) ?? false;

            product.StartingPrice = MirrorsPricelist.GetMirrorExtraPrice(extraProduct.ExtraProperties.Option, isMirrorSpecialDimension, isDefaultExtra);
            RuleApplicationDescription = $"+{product.StartingPrice:0.00€}";
        }

        public bool IsApplicable(IPriceable product)
        {
            if (product is PriceableMirrorExtra)
            {
                return true;
            }
            return false;
        }
    }
}

using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.MirrorsPriceables;
using MirrorsModelsLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.ConcreteRules.RulesMirrors
{
    public class CapsuleMirrorPriceRule : IPricingRule
    {
        /// <summary>
        /// The Premium Added to the Basic Price of a Capsule Mirror
        /// </summary>
        private readonly decimal addedPremiumPerSQM = 0;
        /// <summary>
        /// The Minimum Premium Added to a Capsule Mirror if the SQM premium is lower than this
        /// </summary>
        private readonly decimal minimumAddedPremium = 0;

        public string RuleName { get => nameof(CapsuleMirrorPriceRule); }
        public string RuleApplicationDescription { get; set; } = "";

        /// <summary>
        /// The Encountered Error While Applying the Rule
        /// </summary>
        public string ErrorMessage { get; set; } = "";
        /// <summary>
        /// True If the Rule Encountered an Error during its application
        /// </summary>
        public bool HasError { get; set; }
        public RuleModification ModificationType { get => RuleModification.SetsAdditionalPriceValue; }

        /// <summary>
        /// Add an Error Encountered during the Rule's application
        /// </summary>
        /// <param name="message">error message</param>
        public void AddError(string message)
        {
            ErrorMessage = message;
            HasError = true;
        }

        public CapsuleMirrorPriceRule(decimal addedPremiumPerSQM,decimal minimumAddedPremium)
        {
            this.addedPremiumPerSQM = addedPremiumPerSQM;
            this.minimumAddedPremium = minimumAddedPremium;
        }

        public void ApplyRule(IPriceable product)
        {
            try
            {
                PriceableMirror mirrorProduct = product as PriceableMirror ?? throw new NotSupportedException("IPriceable is not a Mirror Cannot Apply Rule");

                if (mirrorProduct.MirrorProperties.Shape is MirrorShape.Capsule)
                {
                    //Calculate Mirror SQM
                    double mirrorSqm = (mirrorProduct.MirrorProperties.Lengthmm / 1000d) * (mirrorProduct.MirrorProperties.Heightmm / 1000d);
                    decimal finalAddedPremium = Math.Max(addedPremiumPerSQM * Convert.ToDecimal(mirrorSqm), minimumAddedPremium);
                    //Calculate AddedPremium or if low take Minimum Premium
                    mirrorProduct.StartingPrice += finalAddedPremium;
                    RuleApplicationDescription = $"({mirrorSqm:0.00m\u00B2} x {addedPremiumPerSQM:0€}) +{finalAddedPremium:0.00€}";
                }
                else
                {
                    throw new NotSupportedException("MirrorShape is Not a Capsule - Cannot Apply Rule");
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
            if (product is PriceableMirror m && 
                m.MirrorProperties.Shape is MirrorShape.Capsule &&
                m.MirrorProperties.IsFromCatalogue() is false)
            {
                return true;
            }
            return false;
        }
    }
}

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
    public class CircularMirrorPriceRule : IPricingRule
    {
        /// <summary>
        /// The Premium Added to the Basic Price of a Capsule Mirror
        /// </summary>
        private readonly decimal addedPremiumPerSQM = 0;
        /// <summary>
        /// The Minimum Premium Added to a Capsule Mirror if the SQM premium is lower than this
        /// </summary>
        private readonly decimal minimumAddedPremium = 0;

        public string RuleName { get => nameof(CircularMirrorPriceRule); }
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

        public CircularMirrorPriceRule(decimal addedPremiumPerSQM, decimal minimumAddedPremium)
        {
            this.addedPremiumPerSQM = addedPremiumPerSQM;
            this.minimumAddedPremium = minimumAddedPremium;
        }

        public void ApplyRule(IPriceable product)
        {
            try
            {
                PriceableMirror mirrorProduct = product as PriceableMirror ?? throw new NotSupportedException("IPriceable is not a Mirror Cannot Apply Rule");

                if (mirrorProduct.MirrorProperties.Shape is MirrorShape.Circular)
                {
                    //Calculate Mirror SQM
                    double mirrorSqm = Math.Pow(mirrorProduct.MirrorProperties.Diametermm / 1000d,2);
                    decimal finalAddedPremium = Math.Max(addedPremiumPerSQM * Convert.ToDecimal(mirrorSqm), minimumAddedPremium);
                    //Calculate AddedPremium or if low take Minimum Premium
                    mirrorProduct.StartingPrice += finalAddedPremium;
                    RuleApplicationDescription = $"({mirrorSqm:0.00m\u00B2} x {addedPremiumPerSQM.ToString("0\u20AC")}) +{finalAddedPremium.ToString("0.00\u20AC")}";
                }
                else
                {
                    throw new NotSupportedException("MirrorShape is Not Circular - Cannot Apply Rule");
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
            //Applys only to Circulars not from Catalogue
            if (product is PriceableMirror m && 
                m.MirrorProperties.Shape is MirrorShape.Circular &&
                m.MirrorProperties.IsFromCatalogue() is false)
            {
                return true;
            }
            return false;
        }
    }
}

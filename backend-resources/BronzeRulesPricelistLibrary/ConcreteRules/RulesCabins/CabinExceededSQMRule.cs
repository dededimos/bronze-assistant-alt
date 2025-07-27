using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.CabinsPriceables;
using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.ConcreteRules.RulesCabins
{
    /// <summary>
    /// Rule Aplying a Flat Increase per SQM of the Cabin when Dimensions are above a certain Level
    /// </summary>
    public class CabinExceededSQMRule : IPricingRule
    {
        public string RuleName { get => nameof(CabinExceededSQMRule); }
        public string RuleApplicationDescription { get; set; } = "";

        public string ErrorMessage { get; set; } = "";
        public bool HasError { get; set; }
        public RuleModification ModificationType { get => RuleModification.SetsAdditionalPriceValue; }

        /// <summary>
        /// The Flat Increase in Price when SQM Exceed the Allowable Catalogue SQM
        /// </summary>
        private readonly decimal flatIncreasePerExceededSQM;
        private int cataloguePricedHeight;
        private int catalogueMaxPricedLength;

        /// <summary>
        /// Rule Applying a flat Increase per SQM when The SQM exceed the Maximum SQM of the Catalogue
        /// </summary>
        /// <param name="flatIncreasePerExceededSQM">The Flat Increase in Euros/SquareMeter </param>
        public CabinExceededSQMRule(decimal flatIncreasePerExceededSQM)
        {
            this.flatIncreasePerExceededSQM = flatIncreasePerExceededSQM;
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
            //When Height or MAXLength is Exceeded , We have to Find also the Matching Length of the Current Custom Dimension.
            //If length is smaller ,but height is bigger than catalogue ,we must calculate SQM with the Current Matching Length not the MaxLength
            PriceableCabin cabin = product as PriceableCabin ?? throw new NullReferenceException();
            CabinModelEnum model = cabin.CabinProperties.Model ?? throw new NullReferenceException($"Cabin Model is Null");

            //Find the Nearest Length 
            int nearestLength = CalculationsHelpers.MatchToNearest(cabin.CabinProperties.NominalLength, CabinsPricelist.GetBaseLengths(model));

            //Find the Allowable NonCharged SQM for this Cabin
            decimal catalogueNonChargedSQM = cataloguePricedHeight * nearestLength / 1000000m; //Must Convert to meters

            //Find the Current SQM 
            decimal currentSQM = cabin.CabinProperties.Height * cabin.CabinProperties.NominalLength / 1000000m;

            //If Current SQM bigger than Allowable -- Apply the Flat Increase for the Difference 
            decimal flatIncrease = currentSQM > catalogueNonChargedSQM
                                       ? flatIncreasePerExceededSQM * (currentSQM - catalogueNonChargedSQM)
                                       : 0;
            cabin.StartingPrice += flatIncrease;
            RuleApplicationDescription = $"+{flatIncrease:0.00€}";
        }

        public bool IsApplicable(IPriceable product)
        {
            //Applies Only when we have Exceeding SQM
            if (product is PriceableCabin c)
            {
                return IsSQMExceeded(c);
            }
            return false;
        }

        /// <summary>
        /// Checks Wheather the SQM are exceeding the Non-Chragable SQM from the Catalogue
        /// </summary>
        /// <returns>true if the Do</returns>
        private bool IsSQMExceeded(PriceableCabin priceableCabin)
        {
            //Check if exceeds max Catalogue Length or Height
            //If no sqm are Exceeded do not Apply.
            CabinModelEnum model = priceableCabin.CabinProperties.Model ?? throw new NullReferenceException($"Cabin Model is Null");
            catalogueMaxPricedLength = CabinsPricelist.GetBaseLengths(model).Max();
            cataloguePricedHeight = CabinsPricelist.GetBaseHeights(model).Max();
            if (priceableCabin.CabinProperties.Height > cataloguePricedHeight || priceableCabin.CabinProperties.NominalLength > catalogueMaxPricedLength)
            {
                return true;
            }
            return false;
        }

    }
}

using BronzeRulesPricelistLibrary.Models;
using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.CabinsPriceables;
using CommonHelpers;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.OptionsInterfaces.PartsInterfaces;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.ConcreteRules.RulesCabins
{
    public class CabinSpecialDimensionRule : IPricingRuleComplex
    {
        public string RuleName { get => nameof(CabinSpecialDimensionRule); }
        public string ErrorMessage { get; set; } = "";
        public bool HasError { get; set; }
        public string RuleApplicationDescription { get; set; } = "";
        public List<CalculationDescriptor> CalculationsDescriptors { get; } = new();
        public RuleModification ModificationType { get => RuleModification.SetsAdditionalPriceValue; }

        private string defaultHandleCode = string.Empty;

        private readonly IEnumerable<Type> modelsWhereHandleAppliesCustomDimension = new List<Type>();

        public CabinSpecialDimensionRule(IEnumerable<Type> modelsWhereCustomHandleAppliesCustomDim , string defaultHandleCode)
        {
            modelsWhereHandleAppliesCustomDimension = modelsWhereCustomHandleAppliesCustomDim;
            this.defaultHandleCode = defaultHandleCode;
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
            CalculationsDescriptors.Clear();//Refresh its time it is applied
            if (product is PriceableCabin9C cabin9C)
            {
                //Find which is the biggest model to draw Price from this 
                var cabin9CBiggest = cabin9C.CabinPropertiesFirst.LengthMin >= cabin9C.CabinPropertiesSecond.LengthMin
                    ? cabin9C.CabinPropertiesFirst : cabin9C.CabinPropertiesSecond;
                decimal incFactor = CabinsPricelist.CustomizeFactors[cabin9CBiggest.Model ?? throw new InvalidOperationException("Cabin Model Cannot be Null")];
                decimal priceInc = cabin9C.StartingPrice * incFactor;
                decimal curPrice = product.StartingPrice + priceInc;
                
                RuleApplicationDescription = $"({incFactor:0.00%}) {priceInc:+0.00€;-#.00€}";
                CalculationsDescriptors.Add(new("OriginalPrice", $"{cabin9CBiggest.NominalLength/1000m:0.00m} x {cabin9CBiggest.Height / 1000m:0.00m} => {product.StartingPrice:0.00€}"));
                CalculationsDescriptors.Add(new("IncreaseFactor", $"{incFactor:0.00%}"));
                CalculationsDescriptors.Add(new("AddedPrice", $"{product.StartingPrice:0.00€} * {incFactor:0.00%} = {priceInc:0.00€}"));
                CalculationsDescriptors.Add(new("CurrentPrice", $"{product.StartingPrice:0.00€} + {priceInc:0.00€} = {curPrice:0.00€}"));

                //Change Starting Price to Reflect on Catalogue Price
                product.StartingPrice = curPrice;
            }
            else
            {
                //Do % increase only for B6000 for the rest do 0% but apply the rule as 0%
                var cabin = product as PriceableCabin ?? throw new InvalidOperationException("Provided Pricable is not a Cabin");
                decimal increaseFactor = CabinsPricelist.CustomizeFactors[cabin.CabinProperties.Model ?? throw new InvalidOperationException("Cabin Model Cannot be Null")];

                decimal originalLengthm = CalculationsHelpers
                    .MatchToNearest(cabin.CabinProperties.NominalLength, CabinsPricelist.GetBaseLengths(cabin.CabinProperties.Model ?? throw new InvalidOperationException("Cabin Model Cannot be Null")))
                    / 1000m;
                //Height must not get matched -- this way cabins with multiple heights at the Catalogue Price will not get puzzled in between
                decimal originalHeightm = CalculationsHelpers
                    .MatchToNearest(cabin.CabinProperties.Height, CabinsPricelist.GetBaseHeights(cabin.CabinProperties.Model ?? throw new InvalidOperationException("Cabin Model Cannot be Null")))
                    / 1000m;

                decimal originalSQM = originalHeightm * originalLengthm;
                decimal currentHeightm = cabin.CabinProperties.Height / 1000m;
                decimal currentLengthm = cabin.CabinProperties.NominalLength / 1000m;
                decimal currentSQM = (currentHeightm) * (currentLengthm);

                decimal originalPricePerSQM = product.StartingPrice / originalSQM;
                decimal currentPricePerSQM = originalPricePerSQM * (1 + increaseFactor);
                decimal currentPrice = currentPricePerSQM * currentSQM;

                decimal minimumCurrentPrice = CabinsPricelist.GetMinPrice(cabin.CabinProperties);

                //If the Current Price is less than the Base*IncreaseFactor then set it to the Minimum! (~IncreaseFactor% more than the base)
                decimal finalcurrentPrice = currentPrice > minimumCurrentPrice ? currentPrice : minimumCurrentPrice;

                decimal addedPriceFromBase = finalcurrentPrice - product.StartingPrice;



                RuleApplicationDescription = $"{addedPriceFromBase:+0.00€;-#.00€}";
                //Descriptors for the Price Calculation of the Special Dimension
                CalculationsDescriptors.Add(new("OriginalPrice", $"{originalLengthm:0.00m} x {originalHeightm:0.00m} => {product.StartingPrice:0.00€}"));
                CalculationsDescriptors.Add(new("IncreaseFactor", $"{increaseFactor:0.00%}"));
                CalculationsDescriptors.Add(new("OriginalSQM", $"{originalLengthm:0.00m} x {originalHeightm:0.00m} = {originalSQM:0.00m²}"));
                CalculationsDescriptors.Add(new("CurrentSQM", $"{currentLengthm:0.00m} x {currentHeightm:0.00m} = {currentSQM:0.00m²}"));
                CalculationsDescriptors.Add(new("OriginalPricePerSQM", $"{product.StartingPrice:0€}/{originalSQM:0.00m²} = {originalPricePerSQM:0.00€/m²}"));
                CalculationsDescriptors.Add(new("CurrentPricePerSQM", $"{originalPricePerSQM:0.00€/m²} * {increaseFactor:0.00%} = {currentPricePerSQM:0.00€/m²}"));
                CalculationsDescriptors.Add(new("CurrentPrice", $"{currentPricePerSQM:0.00€/m²} * {currentSQM:0.00m²} = {currentPrice:0.00€}"));
                CalculationsDescriptors.Add(new("MinimumPrice", $"{minimumCurrentPrice:0.00€}"));
                CalculationsDescriptors.Add(new("AddedPrice", $"{finalcurrentPrice:0.00€} - {product.StartingPrice:0.00€} = {addedPriceFromBase:0.00€}"));

                // Change the Priceables Starting price to the New Final Price
                // So the reflected Catalogue Price is Correct when needed to be retrieved
                product.StartingPrice = finalcurrentPrice;
            }
        }

        public bool IsApplicable(IPriceable product)
        {
            if (product is PriceableCabin c && IsCustomDimension(c.CabinProperties))
            {
                return true;
            }
            //For Case 9C Check Also the Second Size for Custom Dimensioning (9C Is Sides are Combined to one item)
            else if (product is PriceableCabin9C cabin9C && (IsCustomDimension(cabin9C.CabinPropertiesFirst) || IsCustomDimension(cabin9C.CabinPropertiesSecond)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the Given Cabin Dimension is Custom or if it exists in the Catalogue
        /// </summary>
        /// <param name="cabin"></param>
        /// <returns></returns>
        public bool IsCustomDimension(Cabin cabin)
        {
            bool isCustomLength = false;
            bool isCustomHeight = false;
            bool isCustomHandle = false; //When a different handle from the default is Used
            bool isCustomThickness = cabin.Thicknesses is CabinThicknessEnum.Thick6mm8mm 
                or CabinThicknessEnum.Thick8mm10mm 
                or CabinThicknessEnum.Thick10mm && cabin.Model != CabinModelEnum.ModelWS;
            
            //early escape
            if (isCustomThickness) return true;

            if (cabin?.Model != null)
            {
                isCustomLength = (cabin.NominalLength - CalculationsHelpers.MatchToNearest(cabin.NominalLength, CabinsPricelist.GetBaseLengths((CabinModelEnum)cabin.Model))) 
                                 != 0;
                isCustomHeight = (cabin.Height - CalculationsHelpers.MatchToNearest(cabin.Height, CabinsPricelist.GetBaseHeights((CabinModelEnum)cabin.Model))) 
                                 != 0;
                isCustomHandle = (cabin.Parts is IHandle handle && handle.Handle is not null) 
                              && (defaultHandleCode != handle.Handle.Code)
                              && (modelsWhereHandleAppliesCustomDimension.Any(t=> cabin.GetType().IsSubclassOf(t) || t == cabin.GetType()));
            }
            bool isCustom = isCustomLength || isCustomHeight || isCustomHandle;
            return isCustom;
        }
    }
}

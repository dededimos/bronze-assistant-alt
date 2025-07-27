using BronzeRulesPricelistLibrary.Models;
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
    public class IrregularGlassThicknessRule : IPricingRuleComplex
    {
        public string RuleName { get => nameof(IrregularGlassThicknessRule); }
        public string RuleApplicationDescription { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public bool HasError { get; set; }
        public List<CalculationDescriptor> CalculationsDescriptors { get; } = new();
        public RuleModification ModificationType { get => RuleModification.SetsAdditionalPriceValue; }

        public IrregularGlassThicknessRule()
        {

        }

        public void AddError(string message)
        {
            ErrorMessage = message;
            HasError = true;
        }

        public void ApplyRule(IPriceable product)
        {
            try
            {
                if (product is PriceableCabin cabinProduct)
                {
                    decimal totalPremiumAdded = 0;
                    decimal premiumPerSQM = CabinsPricelist.ThicknessPremiumPerSQM[cabinProduct.CabinProperties.Thicknesses ?? throw new ArgumentException("Invalid Thickness")];
                    double sqmOfGlassChanged = 0;

                    if (cabinProduct.CabinProperties.Thicknesses is CabinThicknessEnum.Thick6mm8mm or CabinThicknessEnum.Thick8mm10mm)
                    {
                        IEnumerable<Glass> fixedGlasses = cabinProduct.CabinProperties.Glasses.Where(g => g.GlassType == GlassTypeEnum.FixedGlass);
                        //Get the Total sqm of all the Fixed Glasses and Calculate the added Premium per sqm of Fixed Glass (The Doors remain unchanged)
                        sqmOfGlassChanged = fixedGlasses.Sum(g => g.Height / 1000d * g.Length / 1000d);
                        totalPremiumAdded = Convert.ToDecimal(sqmOfGlassChanged) * premiumPerSQM ;

                        foreach (var glass in fixedGlasses)
                        {
                            double sqm = glass.Length/1000d * glass.Height/1000d;
                            CalculationsDescriptors.Add(new("FixedGlass", $"{glass.Length/1000d:0.000} x {glass.Height / 1000d:0.000}m"));
                        }
                    }
                    else if (cabinProduct.CabinProperties.Thicknesses is CabinThicknessEnum.Thick10mm)
                    {
                        //All the Cabin
                        sqmOfGlassChanged = cabinProduct.CabinProperties.Height / 1000d * cabinProduct.CabinProperties.NominalLength / 1000d;
                        totalPremiumAdded = Convert.ToDecimal(sqmOfGlassChanged) * premiumPerSQM;
                        CalculationsDescriptors.Add(new("CabinSQM", $"{cabinProduct.CabinProperties.NominalLength / 1000d:0.00m}x{cabinProduct.CabinProperties.Height / 1000d:0.000m}"));
                    }
                    else
                    {
                        throw new ArgumentException("Cabin Thickness is not a Part of Thicknesses where a Premium is Applied");
                    }

                    CalculationsDescriptors.Add(new("TotalSQM", $"{sqmOfGlassChanged:0.000m²}"));
                    CalculationsDescriptors.Add(new("PremiumPerSQM", $"{premiumPerSQM:0.00€/m²}"));
                    CalculationsDescriptors.Add(new("AddedPrice", $"{totalPremiumAdded:+0.00€;-#.00€}"));
                    RuleApplicationDescription = $" {totalPremiumAdded:+0.00€;-#.00€}";
                    product.StartingPrice += totalPremiumAdded;
                }
                else if (product is PriceableCabin9C cabin9CProduct && cabin9CProduct.CabinPropertiesFirst.Thicknesses == CabinThicknessEnum.Thick6mm8mm)
                {
                    decimal totalPremiumAdded = 0;
                    decimal premiumPerSQM = CabinsPricelist.ThicknessPremiumPerSQM[cabin9CProduct.CabinPropertiesFirst.Thicknesses ?? throw new ArgumentException("Invalid Thickness")];
                    double sqmOfGlassChanged = 0;

                    Glass fixedGlass1 = cabin9CProduct.CabinPropertiesFirst.Glasses.FirstOrDefault(g=>g.GlassType == GlassTypeEnum.FixedGlass) ?? throw new ArgumentException("Cabin Does not Have Glasses");
                    Glass fixedGlass2 = cabin9CProduct.CabinPropertiesSecond.Glasses.FirstOrDefault(g => g.GlassType == GlassTypeEnum.FixedGlass) ?? throw new ArgumentException("Cabin Does not Have Glasses");
                    double sqm1 = fixedGlass1.Height / 1000d + fixedGlass1.Length / 1000d;
                    double sqm2 = fixedGlass2.Height / 1000d + fixedGlass2.Length / 1000d;
                    //Get the Total sqm of all the Fixed Glasses and Calculate the added Premium per sqm of Fixed Glass (The Doors remain unchanged)
                    sqmOfGlassChanged = sqm1 + sqm2;
                    totalPremiumAdded = Convert.ToDecimal(sqmOfGlassChanged) * premiumPerSQM;

                    CalculationsDescriptors.Add(new("FixedGlass", $"{fixedGlass1.Length / 1000d:0.00m}x{fixedGlass1.Height / 1000d:0.00m} = {sqm1:0.00m²}"));
                    CalculationsDescriptors.Add(new("FixedGlass", $"{fixedGlass2.Length / 1000d:0.00m}x{fixedGlass2.Height / 1000d:0.00m} = {sqm2:0.00m²}"));
                    CalculationsDescriptors.Add(new("TotalSQM", $"{sqmOfGlassChanged:0.00m²}"));
                    CalculationsDescriptors.Add(new("PremiumPerSQM", $"{premiumPerSQM:+0.00€;-#.00€}"));
                    CalculationsDescriptors.Add(new("AddedPrice", $"{totalPremiumAdded:+0.00€;-#.00€}"));
                    RuleApplicationDescription = $" {totalPremiumAdded:+0.00€;-#.00€}";
                    product.StartingPrice += totalPremiumAdded;
                }
                else
                {
                    throw new ArgumentException("Invalid CabinBasePrice Rule Application attempted to Run on Non Valid IPriceable");
                }
            }
            catch (ArgumentException ex)
            {
                AddError(ex.Message);
                product.StartingPrice = 0;
                RuleApplicationDescription = $"Error Executing Rule";
            }
        }

        public bool IsApplicable(IPriceable product)
        {
            if (product is PriceableCabin a)
            {
                //Exception WS
                if (a.CabinProperties.Model == CabinModelEnum.ModelWS && a.CabinProperties.Thicknesses is CabinThicknessEnum.Thick8mm10mm) return false;
                else return a.CabinProperties.Thicknesses
                            is CabinThicknessEnum.Thick6mm8mm
                            or CabinThicknessEnum.Thick8mm10mm
                            or CabinThicknessEnum.Thick10mm;
            }
            else if (product is PriceableCabin9C b)
            { 
                return b.CabinPropertiesFirst.Thicknesses is CabinThicknessEnum.Thick6mm8mm; 
            }
            return false;
        }
    }
}

using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.CabinsPriceables;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.ConcreteRules.RulesCabins
{
    public class CabinFinishRule : IPricingRule
    {
        public string RuleName { get => nameof(CabinFinishRule); }
        public string ErrorMessage { get; set; } = "";
        public bool HasError { get; set; }
        public string RuleApplicationDescription { get; set; } = "";
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

        public void ApplyRule(IPriceable product)
        {
            try
            {
                CabinModelEnum model;
                CabinFinishEnum finish;
                if (product is PriceableCabin c)
                {
                    model = c.CabinProperties.Model ?? throw new NullReferenceException("Model Not Defined");
                    finish = c.CabinProperties.MetalFinish ?? throw new NullReferenceException("Finish Not Defined");
                }
                else if (product is PriceableCabin9C cc)
                {
                    model = cc.CabinPropertiesFirst.Model ?? throw new NullReferenceException("Model Not Defined");
                    finish = cc.CabinPropertiesFirst.MetalFinish ?? throw new NullReferenceException("Finish Not Defined");
                }
                else
                {
                    throw new ArgumentException("Unrecognized IPriceable in CabinFinishRule Application");
                }
                decimal finishPrice = CabinsPricelist.FinishesPricelist[model][finish] ?? throw new NullReferenceException("This metal Finish is not available for the Selected Model");
                product.StartingPrice += finishPrice;
                RuleApplicationDescription = $"+{finishPrice:0.00€}";
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                //product.StartingPrice = 0; (Leave starting price as it was)
                RuleApplicationDescription = $"Error Executing Rule";
            }
        }

        public bool IsApplicable(IPriceable product)
        {
            if (product is PriceableCabin c && c.CabinProperties.Model != null && c.CabinProperties.MetalFinish != null)
            {
                return true;
            }
            if (product is PriceableCabin9C cc && cc.CabinPropertiesFirst.Model != null && cc.CabinPropertiesFirst.MetalFinish != null)
            {
                return true;
            }
            return false;
        }
    }
}

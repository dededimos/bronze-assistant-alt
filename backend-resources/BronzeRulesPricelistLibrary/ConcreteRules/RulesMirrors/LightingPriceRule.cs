using BronzeRulesPricelistLibrary.Models.Priceables;
using BronzeRulesPricelistLibrary.Models.Priceables.MirrorsPriceables;
using MirrorsModelsLibrary.Enums;
using MirrorsModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.ConcreteRules.RulesMirrors
{
    public class LightingPriceRule : IPricingRule
    {
        public string RuleName { get => nameof(LightingPriceRule); }
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
            PriceableLighting lightingProduct = product as PriceableLighting ?? throw new NotSupportedException("Rule not Applicabe to Non Lighting Objects");
            Mirror? equivaentCatMirror = lightingProduct.ParentMirror.GetEquivalentCatalogueMirror();
            if (equivaentCatMirror is null) //Mirror is not from Catalogue
            {
                //Get Light price for Special Dimension Mirror
                product.StartingPrice = MirrorsPricelist.GetLightPrice(
                    lightingProduct.LightingProperties.Light ?? MirrorLight.WithoutLight,
                    true,
                    false);
            }
            else
            {
                //Get Light Price for Equivaent Catalogue Mirror
                bool isDefaultCatalogueLight = equivaentCatMirror.Lighting.Light == lightingProduct.LightingProperties.Light;
                product.StartingPrice = MirrorsPricelist.GetLightPrice(lightingProduct.LightingProperties.Light ?? MirrorLight.WithoutLight,
                    false,
                    isDefaultCatalogueLight);
            }
            RuleApplicationDescription = $"+{product.StartingPrice:0.00€}";
        }

        public bool IsApplicable(IPriceable product)
        {
            if (product is PriceableLighting)
            {
                return true;
            }
            return false;
        }
    }
}

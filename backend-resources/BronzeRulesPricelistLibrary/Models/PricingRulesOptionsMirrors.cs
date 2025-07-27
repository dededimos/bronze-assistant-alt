using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.Models
{
    public class PricingRulesOptionsMirrors
    {
        /// <summary>
        /// Sets a Starting Price to a Mirror derived Priceable
        /// </summary>
        public bool WithCustomMirrorPriceRule { get; set; } = true;

        /// <summary>
        /// Wheather to Use also the Standard Ready Mirrors From the Catalogue in Pricing
        /// </summary>
        public bool WithCatalogueMirrorPriceRule { get; set; } = true;

        /// <summary>
        /// Sets a Starting Price to a SandblastPriceable
        /// </summary>
        public bool WithSandblastPriceRule { get; set; } = true;

        /// <summary>
        /// Sets a Starting Price to a LightingPriceable
        /// </summary>
        public bool WithLightPriceRule { get; set; } = true;

        /// <summary>
        /// Sets a Starting Price to a MirrorSupportPriceable
        /// </summary>
        public bool WithSupportPriceRule { get; set; } = true;

        /// <summary>
        /// Sets a Starting Price to a MirrorExtra Priceable
        /// </summary>
        public bool WithMirrorExtraPriceRule { get; set; } = true;

        /// <summary>
        /// Wheather to Include a Capsule Price Rule , Else Capsule Price is Taken from Rectangular Mirrors
        /// </summary>
        public bool WithCapsulePriceRule { get; set; } = true;
        /// <summary>
        /// The Capsule Mirror Added Premium per Square Meter of Mirror
        /// </summary>
        public decimal CapsuleAddedPremiumPerSQM { get; set; } = 60m;
        /// <summary>
        /// The minimum Applied Premium for a Capsule Mirror
        /// </summary>
        public decimal CapsuleFlatMinimumAddedPremium = 30m;

        /// <summary>
        /// Wheather to Include a Capsule Price Rule , Else Capsule Price is Taken from Rectangular Mirrors
        /// </summary>
        public bool WithEllipsePriceRule { get; set; } = true;
        /// <summary>
        /// The Capsule Mirror Added Premium per Square Meter of Mirror
        /// </summary>
        public decimal EllipseAddedPremiumPerSQM { get; set; } = 60m;
        /// <summary>
        /// The minimum Applied Premium for a Capsule Mirror
        /// </summary>
        public decimal EllipseFlatMinimumAddedPremium = 30m;

        /// <summary>
        /// Wheather to Include a Circular Price Rule , Else Circular Price is Taken from Rectangular Mirrors
        /// </summary>
        public bool WithCircularPriceRule { get; set; } = true;
        /// <summary>
        /// The Circular Mirror Added Premium per Square Meter of Mirror
        /// </summary>
        public decimal CircularAddedPremiumPerSQM { get; set; } = 40m;
        /// <summary>
        /// The minimum Applied Premium for a Circular Mirror
        /// </summary>
        public decimal CircuarFlatMinimumAddedPremium = 0m;

        public bool WithUserDiscountPriceRule { get; set; } = true;
        public decimal UserCombinedDiscountMirrors { get; set; } = 0m;

        /// <summary>
        /// Wheather to Include a Price Increase Rule to ALL Mirror derived Priceables
        /// </summary>
        public bool WithIncreasePriceRule { get; set; } = true;
        /// <summary>
        /// The Increase Factor
        /// </summary>
        public decimal PriceIncreaseFactor { get; set; } = 1.00m;

        public PricingRulesOptionsMirrors()
        {

        }
    }
}

using BathAccessoriesModelsLibrary.AccessoriesUserOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.Models
{
    public class PricingRulesOptionsAccessories
    {
        public bool WithQuantityDiscountsEnabled { get; set; } = false;
        /// <summary>
        /// Weather to use the Catalogue Price for the Starting Price of the item
        /// </summary>
        public bool UsesCataloguePrice { get => !UsesCustomPrice; }
        /// <summary>
        /// Weather it uses a Custom Price and not the Catalogue
        /// </summary>
        public bool UsesCustomPrice { get; set; }
        /// <summary>
        /// A custom Starting Price 
        /// </summary>
        public decimal CustomStartingPrice { get; set; }

        public decimal MainDiscountDecimal { get; set; }
        public decimal SecondaryDiscountDecimal { get; set; }
        public decimal TertiaryDiscountDecimal { get; set; }
        /// <summary>
        /// An Additional Discount made after any Rules have applied
        /// </summary>
        public decimal AdditionalFinalDiscountDecimal { get; set; }

        /// <summary>
        /// The Id of the Accessories Price Group , if not provided Starting Price will be zero
        /// </summary>
        public string AccessoryPriceGroupId { get; set; } = string.Empty;

        private List<CustomPriceRule> priceExceptions = new();
        public List<CustomPriceRule> PriceExceptions 
        {
            get => priceExceptions;
            set
            {
                priceExceptions = value;
                priceExceptions.Sort((x,y)=> x.SortNo.CompareTo(y.SortNo));
            }
        }

        public PricingRulesOptionsAccessories()
        {
            
        }
    }
}

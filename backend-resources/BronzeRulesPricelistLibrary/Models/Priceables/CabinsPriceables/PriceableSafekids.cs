using BronzeRulesPricelistLibrary.ConcreteRules;
using ShowerEnclosuresModelsLibrary.Enums;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.Models.Priceables.CabinsPriceables
{
    public class PriceableSafekids : Priceable<CabinExtra>
    {
        public CabinExtra SafeKidsProperties { get => Product; }
        
        /// <summary>
        /// The Model to Which SafeKids is Applied to
        /// </summary>
        public CabinModelEnum ModelOfCabin { get; }

        public PriceableSafekids(CabinExtra safeKids , CabinModelEnum modelToWhichApplies) : base (safeKids)
        {
            if (safeKids.ExtraType is not CabinExtraType.SafeKids)
            {
                throw new ArgumentException("Invalid CabinExtraType - PriceableSafekids accepts only SafeKids Type");
            }
            this.ModelOfCabin = modelToWhichApplies;
        }

        public override IPriceable GetNewCopy()
        {
            return new PriceableSafekids(Product,ModelOfCabin) { Quantity = this.Quantity, VatFactor = this.VatFactor };
        }
    }
}

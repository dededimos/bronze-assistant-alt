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
    public class PriceableBronzeClean : Priceable<CabinExtra>
    {
        public CabinExtra BronzeCleanProperties { get => Product; }

        /// <summary>
        /// The Model to Which this Bronze Clean is Applied to
        /// </summary>
        public CabinModelEnum ModelOfCabin { get; }

        public override string Code { get => Product.Code; }

        /// <summary>
        /// Creates a Bronze Clean Priceable Product
        /// </summary>
        /// <param name="bronzeClean">The Bronze Clean Extra</param>
        /// <param name="modelToWhichApplies">The Model which will receivi the Bronze Clean Extra</param>
        /// <exception cref="ArgumentException"></exception>
        public PriceableBronzeClean(CabinExtra bronzeClean, CabinModelEnum modelToWhichApplies) : base(bronzeClean)
        {
            if (bronzeClean.ExtraType is not CabinExtraType.BronzeClean and not CabinExtraType.BronzeCleanNano)
            {
                throw new ArgumentException("Invalid CabinExtraType - PriceableBronzeClean accepts only BronzeClean Types");
            }
            this.ModelOfCabin = modelToWhichApplies;
        }

        public override IPriceable GetNewCopy()
        {
            return new PriceableBronzeClean(Product, ModelOfCabin) { Quantity = this.Quantity, VatFactor = this.VatFactor };
        }
    }
}

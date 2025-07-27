using BronzeRulesPricelistLibrary.ConcreteRules;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.Models.Priceables.CabinsPriceables
{
    public class PriceableCabin9C : Priceable<Cabin9C>
    {
        private readonly Cabin9C secondSide;
        public Cabin9C CabinPropertiesFirst { get => Product; }
        public Cabin9C CabinPropertiesSecond { get => secondSide; }

        private readonly string code;
        public override string Code { get => code; }

        /// <summary>
        /// Builds a Product of Cabin9C - Code needs to be Provided
        /// </summary>
        /// <param name="firstSide">The First Side of 9C Cabin</param>
        /// <param name="secondSide">The Second Side</param>
        /// <param name="code">The Code of the Cabin</param>
        public PriceableCabin9C(Cabin9C firstSide, Cabin9C secondSide, string code) : base(firstSide)
        {
            this.secondSide = secondSide;
            this.code = code;
        }

        public override IPriceable GetNewCopy()
        {
            return new PriceableCabin9C(Product, secondSide , code) { Quantity = this.Quantity, VatFactor = this.VatFactor };
        }
    }
}

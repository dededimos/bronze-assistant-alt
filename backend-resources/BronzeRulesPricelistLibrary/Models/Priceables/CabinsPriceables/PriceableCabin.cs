using BronzeRulesPricelistLibrary.ConcreteRules;
using CommonInterfacesBronze;
using ShowerEnclosuresModelsLibrary.Models;
using ShowerEnclosuresModelsLibrary.Models.SeriesModels.B6000Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.Models.Priceables.CabinsPriceables
{
    public class PriceableCabin : Priceable<Cabin>
    {
        public Cabin CabinProperties { get => Product; }
        public PriceableCabin(Cabin cabin) : base(cabin)
        {
            if (cabin is Cabin9C)
            {
                throw new ArgumentException("Invalid Cabin Type , Cabin9C Priceable is Seperate from PriceableCabin");
            }
        }

        public override IPriceable GetNewCopy()
        {
            return new PriceableCabin(Product) { Quantity = this.Quantity, VatFactor = this.VatFactor };
        }
    }
}

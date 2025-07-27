using BronzeRulesPricelistLibrary.ConcreteRules;
using CommonInterfacesBronze;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.Models.Priceables.CabinsPriceables
{
    public class PriceableStepCut : Priceable<StepCut>
    {
        public StepCut StepProperties { get => Product; }

        public PriceableStepCut(StepCut step) : base(step)
        {
            
        }

        public override IPriceable GetNewCopy()
        {
            return new PriceableStepCut(Product) { Quantity = this.Quantity, VatFactor = this.VatFactor };
        }
    }
}

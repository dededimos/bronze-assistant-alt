using BronzeRulesPricelistLibrary.ConcreteRules;
using MirrorsModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.Models.Priceables.MirrorsPriceables
{
    public class PriceableMirror : Priceable<Mirror>
    {
        public Mirror MirrorProperties { get => Product; }
        
        public PriceableMirror(Mirror mirror) : base(mirror)
        {
            
        }

        public override IPriceable GetNewCopy()
        {
            return new PriceableMirror(Product) { Quantity = this.Quantity, VatFactor = this.VatFactor };
        }
    }
}

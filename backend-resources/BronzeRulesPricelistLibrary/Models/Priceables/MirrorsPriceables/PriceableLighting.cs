using BronzeRulesPricelistLibrary.ConcreteRules;
using MirrorsModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.Models.Priceables.MirrorsPriceables
{
    public class PriceableLighting : Priceable<LightingModel>
    {
        private readonly Mirror parentMirror;
        public LightingModel LightingProperties { get => Product; }
        public Mirror ParentMirror { get => parentMirror; }

        public PriceableLighting(Mirror mirror) : base(mirror.Lighting)
        {
            if (mirror is null)
            {
                throw new ArgumentNullException(nameof(mirror), "Mirror Cannot be Null");
            }
            if (mirror.HasLight() is false)
            {
                throw new ArgumentException("Mirror Has No Light to Add");
            }
            this.parentMirror = mirror;
        }

        public override IPriceable GetNewCopy()
        {
            return new PriceableLighting(parentMirror) { Quantity = this.Quantity, VatFactor = this.VatFactor };
        }
    }
}

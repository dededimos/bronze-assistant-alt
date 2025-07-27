using BronzeRulesPricelistLibrary.ConcreteRules;
using MirrorsModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.Models.Priceables.MirrorsPriceables
{
    public class PriceableMirrorSupport : Priceable<SupportModel>
    {
        private readonly Mirror parentMirror;
        public SupportModel LightingProperties { get => Product; }
        public Mirror ParentMirror { get => parentMirror; }

        public PriceableMirrorSupport(Mirror mirror) : base(mirror.Support)
        {
            if (mirror is null)
            {
                throw new ArgumentNullException(nameof(mirror),"Mirror Cannot be Null");
            }
            if (mirror.HasSupport() is false)
            {
                throw new ArgumentException("Mirror Has No Support to Add");
            }
            this.parentMirror = mirror;
        }

        public override IPriceable GetNewCopy()
        {
            return new PriceableMirrorSupport(parentMirror) { Quantity = this.Quantity, VatFactor = this.VatFactor };
        }
    }
}

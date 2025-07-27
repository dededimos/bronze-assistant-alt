using BronzeRulesPricelistLibrary.ConcreteRules;
using MirrorsModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.Models.Priceables.MirrorsPriceables
{
    public class PriceableMirrorExtra : Priceable<MirrorExtra>
    {
        private readonly Mirror parentMirror;
        public MirrorExtra ExtraProperties { get => Product; }
        public Mirror ParentMirror { get => parentMirror; }
        
        public PriceableMirrorExtra(Mirror mirror , MirrorExtra extra):base(extra)
        {
            if (mirror is null)
            {
                throw new ArgumentNullException(nameof(mirror), "Mirror Cannot be Null");
            }
            if (extra is null)
            {
                throw new ArgumentNullException(nameof(extra), "MirrorExtra Cannot be Null");
            }
            if (mirror.HasExtra(extra.Option) is false)
            {
                throw new ArgumentException($"Cannot Create {nameof(PriceableMirrorExtra)}-{extra.Option} -- the Parent Mirror does not Have it");
            }
            this.parentMirror = mirror;
        }

        public override IPriceable GetNewCopy()
        {
            return new PriceableMirrorExtra(ParentMirror, Product) { Quantity = this.Quantity, VatFactor = this.VatFactor };
        }
    }
}

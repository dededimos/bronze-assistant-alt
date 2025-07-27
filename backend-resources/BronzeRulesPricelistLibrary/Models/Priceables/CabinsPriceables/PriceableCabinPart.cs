using BronzeRulesPricelistLibrary.ConcreteRules;
using ShowerEnclosuresModelsLibrary.Models.CabinAllParts.PartsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.Models.Priceables.CabinsPriceables
{
    internal class PriceableCabinPart : Priceable<CabinPart>
    {
        private string thumbnailPhotoPath = string.Empty;

        public CabinPart PartProperties { get => Product; }
        
        /// <summary>
        /// Returns the Parts Photo Path if the Set Photo Path is Empty
        /// </summary>
        public override string ThumbnailPhotoPath
        {
            get => string.IsNullOrWhiteSpace(thumbnailPhotoPath) ? Product.PhotoPath : thumbnailPhotoPath;
            set
            {
                if (value != thumbnailPhotoPath)
                {
                    thumbnailPhotoPath = value;
                }
            }
        }

        public PriceableCabinPart(CabinPart part) : base(part)
        {
            
        }

        public override IPriceable GetNewCopy()
        {
            return new PriceableCabinPart(Product) { Quantity = this.Quantity, VatFactor = this.VatFactor };
        }
    }
}

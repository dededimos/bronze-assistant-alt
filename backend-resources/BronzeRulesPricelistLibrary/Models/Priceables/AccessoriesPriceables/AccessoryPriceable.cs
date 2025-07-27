using BathAccessoriesModelsLibrary;
using BathAccessoriesModelsLibrary.Services;
using BronzeRulesPricelistLibrary.ConcreteRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeRulesPricelistLibrary.Models.Priceables.AccessoriesPriceables
{
    public class AccessoryPriceable : Priceable<BathroomAccessory>
    {
        public AccessoryFinish SelectedFinish { get; set; } = AccessoryFinish.Empty();
        public override string Code { get => Product.GetSpecificCode(SelectedFinish.Finish.Code); }
        
        private AccessoryPriceable(BathroomAccessory accessory,AccessoryFinish selectedFinish) : base(accessory)
        {
            SelectedFinish = selectedFinish;
        }


        /// <summary>
        /// Returns the Default Starting Price of the Accessory Priceable for the Provided PriceGroup
        /// </summary>
        /// <param name="accessoryPriceGroupId">The price groups Id</param>
        /// <returns></returns>
        public AccessoryPrice GetCataloguePrice(string accessoryPriceGroupId)
        {
            var price = Product.GetPriceFirstOrDefaultByIds(SelectedFinish.Finish.Id, accessoryPriceGroupId);
            return price;
        }

        public static AccessoryPriceable CreatePriceable(
            BathroomAccessory accessory,AccessoryFinish selectedFinish,int quantity, decimal vatFactor, AccessoriesUrlHelper urlHelper)
        {

            AccessoryPriceable priceable = new(accessory, selectedFinish)
            {
                DescriptionKeys = new() { accessory.GetName(),accessory.Series.Trait,selectedFinish.Finish.Trait},
                ThumbnailPhotoPath = urlHelper.GetThumbnail(accessory.GetPhotoUrlFromFinish(selectedFinish.Finish.Code)),
                Quantity = quantity,
                VatFactor = vatFactor,
            };
            return priceable;
        }

        public override IPriceable GetNewCopy()
        {
            return new AccessoryPriceable(Product, SelectedFinish) { DescriptionKeys = new(this.DescriptionKeys), ThumbnailPhotoPath = this.ThumbnailPhotoPath, Quantity = this.Quantity, VatFactor = this.VatFactor };
        }
    }
}

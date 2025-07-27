using CommonInterfacesBronze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BathAccessoriesModelsLibrary
{
    public class BathroomAccessoryFlattened
    {
        public ObjectDescriptionInfo DescriptionInfo { get; set; } = ObjectDescriptionInfo.Empty();
        public string Code { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public string DimensionsPhotoUrl { get; set; } = string.Empty;
        public List<string> ExtraPhotosUrl { get; set; } = new();
        public string Finish { get; set; } = string.Empty;
        public List<string> AvailableFinishes { get; set; } = new();
        public string Material { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Shape { get; set; } = string.Empty;
        public string PrimaryType { get; set; } = string.Empty;
        public string SecondaryType { get; set; } = string.Empty;
        public List<string> Categories { get; set; } = new();
        public string Series { get; set; } = string.Empty;
        public List<string> MountingTypes { get; set; } = new();
        public List<(string, double)> Dimensions { get; set; } = new();
        public decimal Price { get; set; }

        /// <summary>
        /// Converts a Bathroom Accessory into a Flattened Bath Accessory (single Finish , single Price , single Code)
        /// The Flattened Accessories Price is the first Catalogue Price Found for the selected Finish
        /// </summary>
        /// <param name="accessory">The Accessory to convert</param>
        /// <param name="selectedFinishCode">The Finish code of the Flattened Object</param>
        /// <returns></returns>
        public static BathroomAccessoryFlattened ToFlattenedAccessory(BathroomAccessory accessory , string selectedFinishCode)
        {
            BathroomAccessoryFlattened acc = new()
            {
                DescriptionInfo = accessory.DescriptionInfo.GetDeepClone(),
                Code = accessory.GetSpecificCode(selectedFinishCode),
                PhotoUrl = accessory.GetPhotoUrlFromFinish(selectedFinishCode),
                DimensionsPhotoUrl = accessory.DimensionsPhotoUrl,
                ExtraPhotosUrl = new(accessory.ExtraPhotosUrl),
                Finish = accessory.GetAvailableFinish(selectedFinishCode).Finish.Trait,
                AvailableFinishes = accessory.AvailableFinishes.Select(f => f.Finish.Trait).ToList(),
                Material = accessory.Material.Trait,
                Size = accessory.Size.Trait,
                Shape = accessory.Shape.Trait,
                PrimaryType = accessory.PrimaryType.Trait,
                SecondaryType = accessory.SecondaryType.Trait,
                Categories = accessory.Categories.Select(c=> c.Trait).ToList(),
                Series = accessory.Series.Trait,
                MountingTypes = accessory.MountingTypes.Select(mt=> mt.Trait).ToList(),
                Dimensions = accessory.Dimensions.Select(d=> (d.Key.Trait,d.Value)).ToList(),
                Price = accessory.GetPriceFirstOrDefault(selectedFinishCode).PriceValue
            };
            return acc;
        }
    }
}

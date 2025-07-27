using CommonHelpers;
using CommonInterfacesBronze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BathAccessoriesModelsLibrary
{
    public class BathroomAccessoryDTO
    {
        public string Id { get; set; } = string.Empty;
        public ObjectDescriptionInfo DescriptionInfo { get; set; } = ObjectDescriptionInfo.Empty();
        public int SortNo { get; set; }
        public string MainCode { get; set; } = string.Empty;
        public string ExtraCode { get; set; } = string.Empty;
        public bool UsesOnlyMainCode { get; set; }
        public string MainPhotoUrl { get; set; } = string.Empty;
        public string DimensionsPhotoUrl { get; set; } = string.Empty;
        public List<string> ExtraPhotosUrl { get; set; } = [];
        public string PdfUrl { get; set; } = string.Empty;
        public string Finish { get; set; } = string.Empty;
        public List<AccessoryFinishInfoDTO> AvailableFinishes { get; set; } = [];
        public string Material { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public List<string> SizeVariations { get; set; } = [];
        public string Shape { get; set; } = string.Empty;
        public string PrimaryType { get; set; } = string.Empty;
        public List<string> OtherPrimaryTypes { get; set; } = [];
        public string SecondaryType { get; set; } = string.Empty;
        public List<string> OtherSecondaryTypes { get; set; } = [];
        public List<string> Categories { get; set; } = [];
        public string Series { get; set; } = string.Empty;
        public List<string> OtherSeries { get; set; } = [];
        public List<string> MountingTypes { get; set; } = [];
        public List<string> MountingVariations { get; set; } = [];
        public Dictionary<string, double> Dimensions { get; set; } = [];

        /// <summary>
        /// Convert to a Bathroom Accessory without its Pricing
        /// </summary>
        /// <param name="traits">The Traits Dictionary by Id</param>
        /// <returns></returns>
        public BathroomAccessory ToBathroomAccessory(Dictionary<string,AccessoryTrait> traits , List<AccessoryPrice> pricesInfo)
        {
            BathroomAccessory accessory = new()
            {
                Id = this.Id,
                MainCode = this.MainCode,
                ExtraCode = this.ExtraCode,
                UsesOnlyMainCode = this.UsesOnlyMainCode,
                SortNo = this.SortNo,
                DescriptionInfo = new(this.DescriptionInfo.Name,this.DescriptionInfo.Description,this.DescriptionInfo.ExtendedDescription),
                MainPhotoUrl = this.MainPhotoUrl,
                DimensionsPhotoUrl = this.DimensionsPhotoUrl,
                ExtraPhotosUrl = new(this.ExtraPhotosUrl),
                PdfUrl = this.PdfUrl,
                BasicFinish = traits.TryGetValue(this.Finish,out AccessoryTrait? finish) ? finish : AccessoryTrait.Empty(TypeOfTrait.FinishTrait),
                AvailableFinishes = this.AvailableFinishes.Select(af=> af.ToAccessoryFinish(traits)).ToList(),
                Material = traits.TryGetValue(this.Material, out AccessoryTrait? material) ? material : AccessoryTrait.Empty(TypeOfTrait.MaterialTrait),
                Size = traits.TryGetValue(this.Size, out AccessoryTrait? size) ? size : AccessoryTrait.Empty(TypeOfTrait.SizeTrait),
                SizeVariations = new(this.SizeVariations),
                Shape = traits.TryGetValue(this.Shape, out AccessoryTrait? shape) ? shape : AccessoryTrait.Empty(TypeOfTrait.ShapeTrait),
                PrimaryType = traits.TryGetValue(this.PrimaryType, out AccessoryTrait? primaryType) ? primaryType : AccessoryTrait.Empty(TypeOfTrait.PrimaryTypeTrait),
                SecondaryType = traits.TryGetValue(this.SecondaryType, out AccessoryTrait? secondaryType) ? secondaryType : AccessoryTrait.Empty(TypeOfTrait.SecondaryTypeTrait),
                Series = traits.TryGetValue(this.Series, out AccessoryTrait? series) ? series : AccessoryTrait.Empty(TypeOfTrait.SeriesTrait),
                Categories = this.Categories.Select(c=> traits.TryGetValue(c,out AccessoryTrait? category) ? category : AccessoryTrait.Empty(TypeOfTrait.CategoryTrait)).Where(t=> t.Id != AccessoryTrait.EmptyAccessoryTraitId).ToList(),
                MountingTypes = this.MountingTypes.Select(mt => traits.TryGetValue(mt, out AccessoryTrait? mountingType) ? mountingType : AccessoryTrait.Empty(TypeOfTrait.MountingTypeTrait)).Where(t=> t.Id != AccessoryTrait.EmptyAccessoryTraitId).ToList(),
                MountingVariations = new(this.MountingVariations),
                Dimensions = this.Dimensions.ToDictionary(dim => 
                traits.TryGetValue(dim.Key,out AccessoryTrait? dimension) ? dimension : AccessoryTrait.Empty(TypeOfTrait.DimensionTrait),
                dim=> dim.Value),
                PricesInfo = pricesInfo
            };

            // Remove Dimensions - Available Finishes that are Empty
            foreach (var dim in accessory.Dimensions.Keys)
            {
                if (dim.IsEmpty)
                {
                    accessory.Dimensions.Remove(dim);
                }
            }
            foreach (var af in accessory.AvailableFinishes)
            {
                if (af.Finish.IsEmpty)
                {
                    accessory.AvailableFinishes.Remove(af);
                }
            }

            //TODO TODO !! NEED TO CHECK ALL 
            //If Finish is Empty then Replace it with the First Available Finish , If that is empty also set the Accessory to Empty
            if (accessory.BasicFinish.IsEmpty)
            {
                accessory.BasicFinish = accessory.AvailableFinishes.FirstOrDefault()?.Finish ?? AccessoryTrait.Empty(TypeOfTrait.FinishTrait);
            }

            //Fill all 'Other' Properties
            foreach (var ptId in this.OtherPrimaryTypes)
            {
                AccessoryTrait? pt = traits.TryGetValue(ptId, out AccessoryTrait? foundType) ? foundType : null;
                if (pt != null) accessory.OtherPrimaryTypes.Add(pt);
            }
            foreach (var stId in this.OtherSecondaryTypes)
            {
                AccessoryTrait? st = traits.TryGetValue(stId, out AccessoryTrait? foundType) ? foundType : null;
                if (st != null) accessory.OtherSecondaryTypes.Add(st);
            }
            foreach (var serId in this.OtherSeries)
            {
                AccessoryTrait? ser = traits.TryGetValue(serId, out AccessoryTrait? foundType) ? foundType : null;
                if (ser != null) accessory.OtherSeries.Add(ser);
            }

            return accessory;
        }
    }
}

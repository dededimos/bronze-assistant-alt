using CommonHelpers;
using CommonInterfacesBronze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BathAccessoriesModelsLibrary
{
    public class BathroomAccessory : ICodeable
    {
        //Cannot be Deepcloned .. All Object Here are References to the Main Repository and cannot change
        //Only the Repository can be Cloned
        public string Id { get; set; } = string.Empty;
        public ObjectDescriptionInfo DescriptionInfo { get; set; } = ObjectDescriptionInfo.Empty();
        public int SortNo { get; set; }
        public string Code { get => GetGeneralCode(); }
        public string MainCode { get; set; } = string.Empty;
        public string ExtraCode { get; set; } = string.Empty;
        public bool UsesOnlyMainCode { get; set; }
        public string MainPhotoUrl { get; set; } = string.Empty;
        public string DimensionsPhotoUrl { get; set; } = string.Empty;
        public List<string> ExtraPhotosUrl { get; set; } = new();
        public string PdfUrl { get; set; } = string.Empty;
        public AccessoryTrait BasicFinish { get; set; } = AccessoryTrait.Undefined();
        public List<AccessoryFinish> AvailableFinishes { get; set; } = new();
        public AccessoryTrait Material { get; set; } = AccessoryTrait.Undefined();
        public AccessoryTrait Size { get; set; } = AccessoryTrait.Undefined();
        /// <summary>
        /// The Accessories Ids of other Sizes
        /// </summary>
        public List<string> SizeVariations { get; set; } = new();
        public AccessoryTrait Shape { get; set; } = AccessoryTrait.Undefined();
        public AccessoryTrait PrimaryType { get; set; } = AccessoryTrait.Undefined();
        public List<AccessoryTrait> OtherPrimaryTypes { get; set; } = new();
        public AccessoryTrait SecondaryType { get; set; } = AccessoryTrait.Undefined();
        public List<AccessoryTrait> OtherSecondaryTypes { get; set; } = new();
        public List<AccessoryTrait> Categories { get; set; } = new();
        public AccessoryTrait Series { get; set; } = AccessoryTrait.Undefined();
        public List<AccessoryTrait> OtherSeries { get; set; } = new();
        public List<AccessoryTrait> MountingTypes { get; set; } = new();
        /// <summary>
        /// The Accessories Ids of other Mounting Types
        /// </summary>
        public List<string> MountingVariations { get; set; } = new();
        public Dictionary<AccessoryTrait, double> Dimensions { get; set; } = new();
        public List<AccessoryPrice> PricesInfo { get; set; } = new();
        public string Tag { get; set; } = string.Empty;

        public static BathroomAccessory Empty() => new();
        public static BathroomAccessory Empty(string descriptionInfoName) => new() { DescriptionInfo = new(descriptionInfoName, descriptionInfoName, descriptionInfoName) };
        public string GetPhotoUrlFromFinish(string finishCode)
        {
            if (string.IsNullOrWhiteSpace(finishCode))
            {
                var defaultFinish = AvailableFinishes.FirstOrDefault(af => af.Finish.Code == BasicFinish.Code);
                return string.IsNullOrEmpty(defaultFinish?.PhotoUrl) ? MainPhotoUrl : defaultFinish.PhotoUrl;
            }
            var finish = AvailableFinishes.FirstOrDefault(f => f.Finish.Code == finishCode);
            if (finish is null || string.IsNullOrWhiteSpace(finish.PhotoUrl))
            {
                var defaultFinish = AvailableFinishes.FirstOrDefault(af => af.Finish.Code == BasicFinish.Code);
                return string.IsNullOrEmpty(defaultFinish?.PhotoUrl) ? MainPhotoUrl : defaultFinish.PhotoUrl;
            }
            else return finish.PhotoUrl;
        }
        /// <summary>
        /// Returns the Photo of the Requested Finish or a Default Value if the photo does not exist
        /// </summary>
        /// <param name="finishCode">The Code of the finish</param>
        /// <param name="defaultValue">The Default value if the photo does not exist</param>
        /// <returns></returns>
        public string GetPhotoUrlFromFinishOrDefault(string finishCode, string defaultValue = "")
        {
            if (string.IsNullOrWhiteSpace(finishCode))
            {
                return defaultValue;
            }
            var finish = AvailableFinishes.FirstOrDefault(f => f.Finish.Code == finishCode);
            if (finish is null || string.IsNullOrWhiteSpace(finish.PhotoUrl))
            {
                return defaultValue;
            }
            else return finish.PhotoUrl;
        }

        /// <summary>
        /// Returns true if there is a Photo for the provided finish Code 
        /// </summary>
        /// <param name="finishCode"></param>
        /// <returns></returns>
        public bool HasFinishPhoto(string finishCode)
        {
            if (string.IsNullOrEmpty(finishCode) || BasicFinish.Code == finishCode) return true;
            return !string.IsNullOrWhiteSpace(AvailableFinishes.FirstOrDefault(af => af.Finish.Code == finishCode)?.PhotoUrl);
        }
        
        /// <summary>
        /// Gets the PhotoUrl of the Accessory when it has the Basic Finish
        /// </summary>
        /// <returns></returns>
        public string GetAccessoryBasicFinishUrl()
        {
            var basicFinish = AvailableFinishes.FirstOrDefault(af => af.Finish.Code == BasicFinish.Code);
            if (basicFinish != null && !string.IsNullOrWhiteSpace(basicFinish.PhotoUrl))
            {
                return basicFinish.PhotoUrl;
            }
            else
            {
                return MainPhotoUrl;
            }
        }

        /// <summary>
        /// Get the First Found Starting Price of this Accessory for the Provided Finish Code
        /// If the PriceInfo contains More than one price for the designated finish it returns the first found one
        /// </summary>
        /// <param name="finishCode"></param>
        /// <returns></returns>
        public AccessoryPrice GetPriceFirstOrDefault(string finishCode)
        {
            if (string.IsNullOrWhiteSpace(finishCode)) return AccessoryPrice.Undefined();

            // Check if there is a price designated specifically for this finish (the AccessoryPrice object has a Designated FinishTrait and not FinishTraitGroup)
            var specificPrice = PricesInfo.FirstOrDefault(pi => pi.FinishTrait?.Code == finishCode);
            if (specificPrice != null) return specificPrice;

            // Otherwise
            // Find which is the FinishGroup of the Provided FinishCode
            // and then iterate the Prices to see which Price one has this FinishGroup 
            var finish = AvailableFinishes.FirstOrDefault(af => af.Finish.Code == finishCode);
            if (finish == null) return AccessoryPrice.Undefined();
            
            // Check which Price Group Corresponds to the Group of this Finish
            var priceGroup = PricesInfo.FirstOrDefault(pi => finish.Finish.Groups.Any(g => g.Code == pi.FinishTraitGroup?.Code));
            return priceGroup ?? AccessoryPrice.Undefined();
        }

        /// <summary>
        /// Returns the First Found Price of the Accessory for the provided Finish Id and PriceGroupId (Greek-Italian Catalogue e.t.c.)
        /// </summary>
        /// <param name="finishId">The Id of the Finish for which to Find the Price</param>
        /// <param name="priceTraitGroupId">The PriceTraitGroup Id</param>
        /// <returns></returns>
        public AccessoryPrice GetPriceFirstOrDefaultByIds(string finishId, string priceTraitGroupId)
        {
            if (string.IsNullOrWhiteSpace(finishId) || string.IsNullOrWhiteSpace(priceTraitGroupId))
            {
                return AccessoryPrice.Undefined();
            }

            //Get the Prices that Have only the provided PriceTraitGroupId
            var possiblePrices = PricesInfo.Where(pi => pi.PriceTrait.Groups.Any(g => g.Id == priceTraitGroupId));
            //Now check if any of those Prices have a this specific Finish Mentioned and return that price
            var priceWithSpecificFinish = possiblePrices.FirstOrDefault(pi => pi.FinishTrait?.Id == finishId);
            if(priceWithSpecificFinish != null) return priceWithSpecificFinish;

            //If there was no Price with this specific finish
            //Find the Finish Object of the provided finishCode
            var finish = AvailableFinishes.FirstOrDefault(af=> af.Finish.Id == finishId);
            if(finish == null) return AccessoryPrice.Undefined();

            //Return the first price that its finishGroup matches one of the Finish Groups of the found Finish
            var priceWithFinishGroup = possiblePrices.FirstOrDefault(pi => finish.Finish.Groups.Any(g => g.Id == pi.FinishTraitGroup?.Id));
            return priceWithFinishGroup ?? AccessoryPrice.Undefined();
        }

        /// <summary>
        /// Returns all the <see cref="AccessoryPrice"/> objects for the provided finishId and the mentioned PriceGroupIds (Gr , It Catalogue e.t.c.)
        /// </summary>
        /// <param name="finishId">The Id of the Finish for which to Find the Prices</param>
        /// <param name="priceGroupIds">The priceGroupIds for which to return the Price Objects</param>
        /// <returns></returns>
        public List<AccessoryPrice> GetAllPricesOfFinish(string finishId , IEnumerable<string> priceGroupIds)
        {
            //The Method should ONE single Price for each PriceGroupId for this finish 
            //There might be AccessoryPrice objects that refer to this specific Finish or to a FinishTraitGroup that this finish is a part of 
            //The Method should return the Specific Finish one if they exists and if not Only then the FinishGroup ones
            //This applies for each PRICETraitGroupId seperatly (meaning 1 price for each PriceTraitGroup => GR Catalogue , IT Catalogue e.t.c.)

            //1.Seperate the PriceObjects into groups based on the PriceTraitGroup that they have.
            //2.Each Price has a single PriceGroup so take the first one
            List<AccessoryPrice> pricesToReturn = new();
            foreach (var priceGroupId in priceGroupIds)
            {
                var priceForThisGroup = GetPriceFirstOrDefaultByIds(finishId, priceGroupId);
                
                if(priceForThisGroup != null && !priceForThisGroup.IsUndefined )
                    pricesToReturn.Add(priceForThisGroup);
            }
            return pricesToReturn;
        }

        public IEnumerable<(AccessoryTrait dim,double dimValue)> GetBasicDimensions(string basicDimensionsGroupId)
        {
            return Dimensions.Where(d => d.Key.Groups.Any(g => g.Id == basicDimensionsGroupId)).Select(d=> (d.Key,d.Value));
        }
        /// <summary>
        /// Returns all the Photos Urls of the Accessory
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllPhotoUrls()
        {
            List<string> urls = new();
            urls.AddIf(!string.IsNullOrEmpty(MainPhotoUrl),MainPhotoUrl);
            urls.AddRange(ExtraPhotosUrl.Where(url => !string.IsNullOrEmpty(url)));
            foreach (var finish in AvailableFinishes)
            {
                urls.AddIf(!string.IsNullOrEmpty(finish.PhotoUrl), finish.PhotoUrl);
                urls.AddRange(finish.ExtraPhotosUrl.Where(url => !string.IsNullOrEmpty(url)));
            }
            return urls;
        }

        /// <summary>
        /// Returns the General Code of the Accessory without any finish information
        /// </summary>
        /// <returns></returns>
        public string GetGeneralCode()
        {
            if (string.IsNullOrWhiteSpace(MainCode))
            {
                return "??";
            }
            if (UsesOnlyMainCode)
            {
                return MainCode;
            }
            return string.Join('-', MainCode, ExtraCode).TrimEnd('-');
        }
        /// <summary>
        /// Returns the Code of the Accessory based on the Provided Finish
        /// </summary>
        /// <param name="finishCode">The Finish Code of the Accessory</param>
        /// <returns>The Accessory Specific Finish Code</returns>
        public string GetSpecificCode(string finishCode)
        {
            if (string.IsNullOrWhiteSpace(MainCode)) return "??";
            if (UsesOnlyMainCode) return MainCode;

            if (string.IsNullOrEmpty(finishCode)) finishCode = BasicFinish.Code;
            return string.Join('-', MainCode, finishCode, ExtraCode).TrimEnd('-');
        }

        /// <summary>
        /// Returns the Name of the Accessory if there is one otherwise the Secondary Type Trait;
        /// </summary>
        /// <returns></returns>
        public string GetName() 
        {
            return string.IsNullOrWhiteSpace(DescriptionInfo.Name) ? SecondaryType.Trait : DescriptionInfo.Name;
        }

        /// <summary>
        /// Returns the Full Description of an Accessory Containing its Main Series and Finish
        /// </summary>
        /// <param name="finishDesc"></param>
        /// <returns></returns>
        public string GetFullDescription(string finishDesc)
        {
            return $"{GetName()} {Series.Trait} {finishDesc}";
        }

        public AccessoryFinish GetAvailableFinish(string finishCode)
        {
            if (string.IsNullOrWhiteSpace(finishCode)) finishCode = BasicFinish.Code;
            return AvailableFinishes.FirstOrDefault(af => af.Finish.Code == finishCode) ?? AccessoryFinish.Empty();
        }
        public AccessoryFinish? GetAvailableFinishByIdOrNull(string finishId)
        {
            if (string.IsNullOrWhiteSpace(finishId)) return null;
            return AvailableFinishes.FirstOrDefault(af => af.Finish.Id == finishId);
        }

        /// <summary>
        /// Returns all the Photos Associated with this Accessory
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAvailablePhotosLinks()
        {
            List<string> photos = new();
            var photosInFinishes = AvailableFinishes.Where(af => !string.IsNullOrEmpty(af.PhotoUrl)).Select(af=> af.PhotoUrl);
            var extraPhotos = AvailableFinishes.Where(af => af.ExtraPhotosUrl.Any()).SelectMany(af => af.ExtraPhotosUrl);
            photos.AddRange(extraPhotos);
            photos.AddRange(photosInFinishes);
            photos.Add(MainPhotoUrl);
            photos.AddRange(ExtraPhotosUrl);
            return photos;
        }
    }

}

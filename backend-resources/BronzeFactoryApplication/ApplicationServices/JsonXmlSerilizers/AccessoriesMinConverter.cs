using AccessoriesConversions;
using AccessoriesRepoMongoDB.Entities;
using AccessoriesRepoMongoDB.Repositories;
using BathAccessoriesModelsLibrary;
using BathAccessoriesModelsLibrary.Services;
using MongoDB.Bson;

namespace BronzeFactoryApplication.ApplicationServices.JsonXmlSerilizers
{
    public class AccessoriesMinConverter : IAccessoryWebItemConverter<BathAccessoryEntity>
    {
        private readonly IAccessoryEntitiesRepository repo;
        private readonly ITraitGroupEntitiesRepository groupsRepo;
        private readonly AccessoriesUrlHelper urlHelper;
        private WebItemConversionOptions webItemConversionOptions = WebItemConversionOptions.Defaults();

        private IEnumerable<BathAccessoryEntity> AccessoriesEntities { get => repo.Cache; }
        private IEnumerable<TraitEntity> TraitEntities { get => repo.Traits.Cache; }
        private IEnumerable<TraitClassEntity> TraitClassesEntities { get => repo.Traits.TraitClasses.Cache; }
        private IEnumerable<TraitGroupEntity> TraitGroupEntities { get => groupsRepo.Cache; }

        public AccessoriesMinConverter(IAccessoryEntitiesRepository repo , ITraitGroupEntitiesRepository groupsRepo, AccessoriesUrlHelper urlHelper)
        {
            this.repo = repo;
            this.groupsRepo = groupsRepo;
            this.urlHelper = urlHelper;
        }

        /// <summary>
        /// Converts the List of Accessories Entities into a List of AccessoryMin
        /// </summary>
        /// <returns>The list of Accessory Min</returns>
        public AccessoriesJsonStash GetAccessoriesMinStash(AccessoriesMinStashConversionOptions options)
        {
            AccessoryJson.SetSerilizableProperties(options.AccessorySerilizableProps);
            TraitJson.SetSerilizableProperties(options.TraitSeriliazableProps);
            TraitClassJson.SetSerilizableProperties(options.TraitClassSerilizableProps);
            AccessoriesJsonStash stash = new()
            {
                Accessories = AccessoriesEntities.Select(a => ConvertEntityToAccessoryJson(a, options.LanguageIdentifier,options.PriceGroupId)).ToList(),
                Traits = TraitEntities.Select(t=> ConvertEntityToTraitMin(t, options.LanguageIdentifier)).ToList(),
                TraitClasses = TraitClassesEntities.Select(tc=> ConvertEntityToTraitClassMin(tc, options.LanguageIdentifier)).ToList()
            };

            return stash;
        }
        
        /// <summary>
        /// Converts an AccessoryEntity into an Accessory Min
        /// </summary>
        /// <param name="entity">The Entity</param>
        /// <param name="lngIdentifier">The Language Identifier</param>
        /// <returns>The Accessory Min Object</returns>
        private AccessoryJson ConvertEntityToAccessoryJson(BathAccessoryEntity entity , string lngIdentifier , string priceGroupId)
        {
            //Gather All Photos from the Entity and Append the Containers Path
            var finishesUrls = entity.AvailableFinishes.Where(af => !string.IsNullOrWhiteSpace(af.PhotoUrl)).Select(af => urlHelper.AppendContainerPathToUrl(af.PhotoUrl));
            var extraPhotosFinishesUrls = entity.AvailableFinishes.SelectMany(af => af.ExtraPhotosUrl.Where(url => !string.IsNullOrWhiteSpace(url)).Select(url => urlHelper.AppendContainerPathToUrl(url)));
            var finishesPdfUrls = entity.AvailableFinishes.Where(af => !string.IsNullOrWhiteSpace(af.PdfUrl)).Select(af => urlHelper.AppendContainerPathToUrl(af.PdfUrl));
            var finishTraits = TraitEntities.Where(t => t.TraitType == TypeOfTrait.FinishTrait);
            // Find Which Price Traits Have the priceGroupId
            IEnumerable<TraitEntity> priceTraitsIdsWithPriceGroup = TraitEntities.Where(te => te.TraitType == TypeOfTrait.PriceTrait && te.AssignedGroups.Any(gId => gId == priceGroupId));
            AccessoryJson accessoryMin = new()
            {
                Name = entity.Name.GetLocalizedValue(lngIdentifier, true),
                Description = entity.Description.GetLocalizedValue(lngIdentifier, true),
                ExtendedDescription = entity.ExtendedDescription.GetLocalizedValue(lngIdentifier, true),
                SortNo = entity.SortNo,
                Code = entity.Code,
                MainPhotoUrl = urlHelper.AppendContainerPathToUrl(entity.MainPhotoURL),
                ExtraPhotosUrl = entity.ExtraPhotosURL.Where(url => !string.IsNullOrWhiteSpace(url))
                                                      .Select(url => urlHelper.AppendContainerPathToUrl(url))
                                                      .Concat(finishesUrls)
                                                      .Concat(extraPhotosFinishesUrls)
                                                      .ToList(),
                PdfUrl = urlHelper.AppendContainerPathToUrl(entity.PdfURL),
                ExtraPdfsUrl = finishesPdfUrls.ToList(),
                DefaultFinish = GetTraitValueOrEmpty(entity.Finish, lngIdentifier),
                AvailableFinishes = entity.AvailableFinishes.Select(af=> GetTraitValueOrEmpty(af.FinishId,lngIdentifier)).ToList(),

                Material = GetTraitValueOrEmpty(entity.Material,lngIdentifier),
                Size = GetTraitValueOrEmpty(entity.Size,lngIdentifier),
                SizeVariationsCodes = AccessoriesEntities.Where(a => entity.SizeVariations.Any(sv => sv == a.Id.ToString()))
                                                         .Select(a => a.Code)
                                                         .ToList(),
                Shape = GetTraitValueOrEmpty(entity.Shape,lngIdentifier),
                PrimaryType = GetTraitValueOrEmpty(entity.PrimaryTypes.FirstOrDefault() ?? string.Empty,lngIdentifier),
                OtherPrimaryTypes = entity.PrimaryTypes.Skip(1).Select(pt=> GetTraitValueOrEmpty(pt,lngIdentifier)).ToList(),
                SecondaryType = GetTraitValueOrEmpty(entity.SecondaryTypes.FirstOrDefault() ?? string.Empty, lngIdentifier),
                OtherSecondaryTypes = entity.SecondaryTypes.Skip(1).Select(st => GetTraitValueOrEmpty(st, lngIdentifier)).ToList(),
                Categories = entity.Categories.Select(c=> GetTraitValueOrEmpty(c,lngIdentifier)).ToList(),
                Series = GetTraitValueOrEmpty(entity.Series.FirstOrDefault() ?? string.Empty, lngIdentifier),
                OtherSeries = entity.Series.Skip(1).Select(series => GetTraitValueOrEmpty(series, lngIdentifier)).ToList(),
                MountingTypes = entity.MountingTypes.Select(mt=> GetTraitValueOrEmpty(mt,lngIdentifier)).ToList(),
                MountingVariationsCodes = AccessoriesEntities.Where(a => entity.MountingVariations.Any(mv => mv == a.Id.ToString()))
                                                             .Select(a => a.Code)
                                                             .ToList(),
                Dimensions = entity.Dimensions.Select(d=> new AccessoryDimension() { Dimension = GetTraitValueOrEmpty(d.Key, lngIdentifier, true) , DimensionValue = d.Value}).ToList(),
                //Select only the Prices that match the PriceGroupId
                Prices = entity.PricesInfo
                .Where(pi=> priceTraitsIdsWithPriceGroup.Any(pId=> pId.IdAsString == pi.PriceTraitId))
                .Select(pi => new AccessoryPriceMin()
                {
                    PriceValue = pi.PriceValue ,
                    Price = GetTraitValueOrEmpty(pi.PriceTraitId, lngIdentifier),
                    RefersTo = !string.IsNullOrWhiteSpace(pi.RefersToFinishId)
                        ? GetTraitValueOrEmpty(pi.RefersToFinishId, lngIdentifier, true)
                        : GetTraitGroupNameOrEmpty(pi.RefersToFinishGroupId, lngIdentifier, true),
                }
                ).ToList(),
            };
            return accessoryMin;
        }
        /// <summary>
        /// Converts a Trait Entity into a TraitMin
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="lngIdentifier"></param>
        /// <returns></returns>
        private TraitJson ConvertEntityToTraitMin(TraitEntity entity , string lngIdentifier)
        {
            TraitJson traitMin = new()
            {
                SortNo = entity.SortNo,
                TraitType = entity.TraitType.ToString(),
                Code = entity.Code,
                PhotoUrl = urlHelper.AppendContainerPathToUrl(entity.PhotoURL),
                TraitValue = GetTraitValueOrEmpty(entity.Id.ToString(),lngIdentifier),
                TraitTooltip = GetTraitTooltipOrEmpty(entity.Id.ToString(),lngIdentifier),
                TraitGroup = GetTraitGroupNameOrEmpty(entity.AssignedGroups.FirstOrDefault() ?? "", lngIdentifier)
            };
            return traitMin;
        }
        /// <summary>
        /// Converts a TraitClass Entity into a Trait Class Min
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="lngIdentifier"></param>
        /// <returns></returns>
        private TraitClassJson ConvertEntityToTraitClassMin(TraitClassEntity entity , string lngIdentifier)
        {
            TraitClassJson traitClassMin = new()
            {
                SortNo = entity.SortNo,
                TraitType = entity.TraitType.ToString(),
                PhotoUrl = urlHelper.AppendContainerPathToUrl(entity.PhotoURL),
                Name = TraitClassesEntities.FirstOrDefault(tc=> tc.Id == entity.Id)?.Name.GetLocalizedValue(lngIdentifier,true) ?? string.Empty,
                Description = TraitClassesEntities.FirstOrDefault(tc => tc.Id == entity.Id)?.Description.GetLocalizedValue(lngIdentifier, true) ?? string.Empty,
                ExtendedDescription = TraitClassesEntities.FirstOrDefault(tc => tc.Id == entity.Id)?.ExtendedDescription.GetLocalizedValue(lngIdentifier, true) ?? string.Empty,
                TraitValues = entity.Traits.Select(tId=> GetTraitValueOrEmpty(tId.ToString(),lngIdentifier)).ToList(),
            };
            return traitClassMin;
        }

        /// <summary>
        /// Returns the Trait Value from a Trait after searching in the traits Cache. Returns Empty or the Trait Id according to what is chosen
        /// </summary>
        /// <param name="traitId"></param>
        /// <param name="lngIdentifier"></param>
        /// <param name="returnIdInsteadOfEmpty">Used in Dimensions and Prices where the Values are Dictionary Keys and need to be unique instead of being empty strings</param>
        /// <returns></returns>
        private string GetTraitValueOrEmpty(string traitId, string lngIdentifier , bool returnIdInsteadOfEmpty = false)
        {
            if (traitId == string.Empty || traitId == ObjectId.Empty.ToString())
            {
                return returnIdInsteadOfEmpty ? traitId : string.Empty;
            }
            return TraitEntities.FirstOrDefault(te => te.Id.ToString() == traitId)?.Trait.GetLocalizedValue(lngIdentifier, true) ?? (returnIdInsteadOfEmpty ? traitId : string.Empty);
        }

        /// <summary>
        /// Returns the Trait Value from a Trait after searching in the traits Cache. Returns Empty or the Trait Id according to what is chosen
        /// </summary>
        /// <param name="traitId"></param>
        /// <param name="lngIdentifier"></param>
        /// <param name="returnIdInsteadOfEmpty">Used in Dimensions and Prices where the Values are Dictionary Keys and need to be unique instead of being empty strings</param>
        /// <returns></returns>
        private string GetTraitTooltipOrEmpty(string traitId, string lngIdentifier, bool returnIdInsteadOfEmpty = false)
        {
            if (traitId == string.Empty || traitId == ObjectId.Empty.ToString())
            {
                return returnIdInsteadOfEmpty ? traitId : string.Empty;
            }
            return TraitEntities.FirstOrDefault(te => te.Id.ToString() == traitId)?.TraitTooltip.GetLocalizedValue(lngIdentifier, true) ?? (returnIdInsteadOfEmpty ? traitId : string.Empty);
        }
        
        /// <summary>
        /// Returns the TraitGroup Name from a TraitGroup after searching in the traitsGroups Cache. Returns Name or Empty or the TraitGroup Id according to what is chosen
        /// </summary>
        /// <param name="traitGroupId"></param>
        /// <param name="lngIdentifier"></param>
        /// <param name="returnIdInsteadOfEmpty"></param>
        /// <returns></returns>
        private string GetTraitGroupNameOrEmpty(string traitGroupId, string lngIdentifier, bool returnIdInsteadOfEmpty = false)
        {
            if (traitGroupId == string.Empty || traitGroupId == ObjectId.Empty.ToString())
            {
                return returnIdInsteadOfEmpty ? traitGroupId : string.Empty;
            }
            return TraitGroupEntities.FirstOrDefault(tg => tg.Id.ToString() == traitGroupId)?.Name.GetLocalizedValue(lngIdentifier, true) ?? (returnIdInsteadOfEmpty ? traitGroupId : string.Empty);
        }

        /// <summary>
        /// Returns all the Repository of accessories converted to AccessoryWeb Items
        /// </summary>
        /// <returns></returns>
        public List<AccessoryWebItem> ConvertRepoToWebItem()
        {
            List<AccessoryWebItem> items = new();
            AccessoryWebItem.SetSerilizableProperties(webItemConversionOptions.WebItemSerilizableProperties);
            foreach (var e in AccessoriesEntities)
            {
                foreach (var af in e.AvailableFinishes)
                {
                    var item = Convert(e, af.FinishId);
                    items.Add(item);
                }
            }
            return items;
        }

        public AccessoryWebItem Convert(BathAccessoryEntity item,string selectedFinish)
        {
            TraitEntity selectedFinishEntity = TraitEntities.FirstOrDefault(e => e.Id.ToString() == selectedFinish)
                ?? throw new Exception($"Finish with id {selectedFinish} is not a valid finish");

            //Find which prices have the corresponding PriceGroup Id
            var validPriceTraits = TraitEntities.Where(t => t.TraitType == TypeOfTrait.PriceTrait && t.AssignedGroups.Any(g => g == webItemConversionOptions.PriceGroupId));
            //Get only those Prices from the Item
            var validPrices = item.PricesInfo.Where(pi => validPriceTraits.Any(p=> p.IdAsString == pi.PriceTraitId));
            
            //Find which price is needed based on the finish else based on the Finish Group
            var price = validPrices.FirstOrDefault(p => p.RefersToFinishId == selectedFinishEntity.IdAsString)?.PriceValue
                ?? validPrices.FirstOrDefault(p => selectedFinishEntity.AssignedGroups.Any(g => g == p.RefersToFinishGroupId))?.PriceValue 
                ?? 0;

            var webItem = new AccessoryWebItem()
            {
                Code = BathAccessoryEntity.GenerateSpecificCode(item.MainCode,selectedFinishEntity.Code,item.ExtraCode,item.UsesOnlyMainCode),
                Name = item.Name.GetLocalizedValue(webItemConversionOptions.LanguageIdentifier),
                Description = item.Description.GetLocalizedValue(webItemConversionOptions.LanguageIdentifier),
                ExtendedDescription = item.ExtendedDescription.GetLocalizedValue(webItemConversionOptions.LanguageIdentifier),
                Series = TraitEntities.FirstOrDefault(t=> t.Id.ToString() == item.Series.FirstOrDefault())?.Trait.GetLocalizedValue(webItemConversionOptions.LanguageIdentifier) ?? string.Empty,
                PrimaryType = TraitEntities.FirstOrDefault(t => t.Id.ToString() == item.PrimaryTypes.FirstOrDefault())?.Trait.GetLocalizedValue(webItemConversionOptions.LanguageIdentifier) ?? string.Empty,
                SecondaryType = TraitEntities.FirstOrDefault(t => t.Id.ToString() == item.SecondaryTypes.FirstOrDefault())?.Trait.GetLocalizedValue(webItemConversionOptions.LanguageIdentifier) ?? string.Empty,
                Finish = selectedFinishEntity.Trait.GetLocalizedValue(webItemConversionOptions.LanguageIdentifier) ?? string.Empty,
                Shape = TraitEntities.FirstOrDefault(t => t.Id.ToString() == item.Shape)?.Trait.GetLocalizedValue(webItemConversionOptions.LanguageIdentifier) ?? string.Empty,
                Size = TraitEntities.FirstOrDefault(t => t.Id.ToString() == item.Size)?.Trait.GetLocalizedValue(webItemConversionOptions.LanguageIdentifier) ?? string.Empty,
                MountingTypes = TraitEntities.Where(t=> item.MountingTypes.Any(mt=> mt == t.Id.ToString())).Select(mt=> mt.Trait.GetLocalizedValue(webItemConversionOptions.LanguageIdentifier)).ToList() ?? new(),
                Categories = TraitEntities.Where(t => item.Categories.Any(c => c == t.Id.ToString())).Select(mt => mt.Trait.GetLocalizedValue(webItemConversionOptions.LanguageIdentifier)).ToList() ?? new(),
                Dimensions = TraitEntities.Where(t=> item.Dimensions.ContainsKey(t.Id.ToString())).Select(d=> new AccessoryDimension() { Dimension = d.Trait.GetLocalizedValue(webItemConversionOptions.LanguageIdentifier),DimensionValue = item.Dimensions.TryGetValue(d.Id.ToString(),out double dimValue) ? dimValue : 0 }).ToList() ?? new(),
                ThumbnailGeneralPhotoUrl = urlHelper.GetPhotoOrDefault(item.MainPhotoURL,PhotoSize.Thumbnail,""),
                ThumbnailSpecificFinishPhotoUrl = urlHelper.GetPhotoOrDefault(item.AvailableFinishes.FirstOrDefault(af=> af.FinishId == selectedFinishEntity.IdAsString)?.PhotoUrl ?? "",PhotoSize.Thumbnail,""),
                SmallGeneralPhotoUrl = urlHelper.GetPhotoOrDefault(item.MainPhotoURL, PhotoSize.Small, ""),
                SmallSpecificFinishPhotoUrl = urlHelper.GetPhotoOrDefault(item.AvailableFinishes.FirstOrDefault(af => af.FinishId == selectedFinishEntity.IdAsString)?.PhotoUrl ?? "", PhotoSize.Small, ""),
                MediumGeneralPhotoUrl = urlHelper.GetPhotoOrDefault(item.MainPhotoURL, PhotoSize.Medium, ""),
                MediumSpecificFinishPhotoUrl = urlHelper.GetPhotoOrDefault(item.AvailableFinishes.FirstOrDefault(af => af.FinishId == selectedFinishEntity.IdAsString)?.PhotoUrl ?? "", PhotoSize.Medium, ""),
                LargeGeneralPhotoUrl = urlHelper.GetPhotoOrDefault(item.MainPhotoURL, PhotoSize.Large, ""),
                LargeSpecificFinishPhotoUrl = urlHelper.GetPhotoOrDefault(item.AvailableFinishes.FirstOrDefault(af => af.FinishId == selectedFinishEntity.IdAsString)?.PhotoUrl ?? "", PhotoSize.Large, ""),
                FullGeneralPhotoUrl = urlHelper.GetPhotoOrDefault(item.MainPhotoURL, PhotoSize.Full, ""),
                FullSpecificFinishPhotoUrl = urlHelper.GetPhotoOrDefault(item.AvailableFinishes.FirstOrDefault(af => af.FinishId == selectedFinishEntity.IdAsString)?.PhotoUrl ?? "", PhotoSize.Full, ""),
                CataloguePrice = price
            };
            return webItem;
        }
        public void SetConversionOptions(WebItemConversionOptions options)
        {
            webItemConversionOptions = options.GetDeepClone();
        }
        public void SetConversionOptions(Action<WebItemConversionOptions> action)
        {
            //Defaults
            webItemConversionOptions = new();
            //Change the Defaults according to the Passed Action
            action(webItemConversionOptions);
        }
    }

    public class AccessoriesJsonStash
    {
        public List<AccessoryJson> Accessories { get; set; } = new();
        public List<TraitJson> Traits { get; set; } = new();
        public List<TraitClassJson> TraitClasses { get; set; } = new();
    }

    public class AccessoriesMinStashConversionOptions
    {
        /// <summary>
        /// The Language to Use for all The Trait Classes - Traits and Trait Group Names
        /// </summary>
        public string LanguageIdentifier { get; set; } = "en-EN";
        /// <summary>
        /// The SelectedPrice Group from which to Include Prices in the Accessories
        /// </summary>
        public string PriceGroupId { get; set; } = string.Empty;
        /// <summary>
        /// The Accessory Properties that will be Included in the Serilization of the JSON or XML File
        /// </summary>
        public string[] AccessorySerilizableProps { get; set; } = typeof(AccessoryJson).GetProperties().Select(p => p.Name).ToArray();
        /// <summary>
        /// The Traits Properties that will be Included in the Serilization of the JSON or XML File
        /// </summary>
        public string[] TraitSeriliazableProps { get; set; } = typeof(TraitJson).GetProperties().Select(p => p.Name).ToArray();
        /// <summary>
        /// The Trait Classes Properties that will be Included in the Serilization of the JSON or XML File
        /// </summary>
        public string[] TraitClassSerilizableProps { get; set; } = typeof(TraitClassJson).GetProperties().Select(p => p.Name).ToArray();
    }

}

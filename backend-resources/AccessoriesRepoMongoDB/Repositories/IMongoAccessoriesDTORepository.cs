using BathAccessoriesModelsLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using CommonInterfacesBronze;
using AccessoriesRepoMongoDB.Entities;
using MongoDbCommonLibrary.CommonEntities;
using BathAccessoriesModelsLibrary.Services;
using MongoDbCommonLibrary.CommonExceptions;
using BathAccessoriesModelsLibrary.AccessoriesUserOptions;
using UsersRepoMongoDb;
using AccessoriesRepoMongoDB.Helpers;
using static MongoDbCommonLibrary.ExtensionMethods.MongoDbExtensions;

namespace AccessoriesRepoMongoDB.Repositories
{
    public interface IMongoAccessoriesDTORepository
    {
        public const string DefaultUserName = "DefaultUser";
        public const string ObjectIdClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        /// <summary>
        /// Returns the DTO Stash either from Memory or from the Db if the Cache has Expired or is not activated
        /// </summary>
        /// <param name="lngIdentifier">The Language Identifier</param>
        /// <param name="ignoreCache">Weather to ignore any present Cached Data and return directly from the Database</param>
        /// <returns>The Dto Stash for accessories</returns>
        Task<AccessoriesDtoStash> GetStashAsync(string lngIdentifier, bool ignoreCache = false);
    }

    /// <summary>
    /// Mongo Implementation of Accessories Memory Repo
    /// </summary>
    public class MongoAccessoriesDTORepository : IMongoAccessoriesDTORepository
    {
        /// <summary>
        /// Keeps the Dictionary Values Cached according to the Selected options during DI Container Registration
        /// </summary>
        private static readonly MemoryCache _cache = new(new MemoryCacheOptions());
        private readonly IAccessoryEntitiesRepository repo;
        private readonly ItemStockMongoRepository stockRepo;
        private readonly UserAccessoriesOptionsRepository accessoriesOptionsRepo;
        private readonly MongoPriceRuleEntityRepo priceRulesRepo;
        private readonly UsersRepositoryMongo usersRepo;
        private readonly AccessoriesUrlHelper urlHelper;
        private readonly ILogger<MongoAccessoriesDTORepository> logger;
        private readonly MongoAccessoriesRepositoryOptions options;
        public bool isCachingEnabled;

        public MongoAccessoriesDTORepository(
            IAccessoryEntitiesRepository repo,
            ItemStockMongoRepository stockRepo,
            UserAccessoriesOptionsRepository accessoriesOptionsRepo,
            MongoPriceRuleEntityRepo priceRulesRepo,
            UsersRepositoryMongo usersRepo,
            AccessoriesUrlHelper urlHelper,
            ILogger<MongoAccessoriesDTORepository> logger,
            IOptions<MongoAccessoriesRepositoryOptions> options)
        {
            this.repo = repo;
            this.stockRepo = stockRepo;
            this.accessoriesOptionsRepo = accessoriesOptionsRepo;
            this.priceRulesRepo = priceRulesRepo;
            this.usersRepo = usersRepo;
            this.urlHelper = urlHelper;
            this.logger = logger;
            this.options = options.Value;
        }

        /// <summary>
        /// Returns the DTO Stash either from Memory or from the Db if the Cache has Expired or is not activated
        /// </summary>
        /// <param name="lngIdentifier">The Language Identifier</param>
        /// <param name="ignoreCache">Weather to ignore any present Cached Data and return directly from the Database</param>
        /// <returns>The Dto Stash for accessories</returns>
        public async Task<AccessoriesDtoStash> GetStashAsync(string lngIdentifier, bool ignoreCache = false)
        {
            if (ignoreCache)
            {
                _cache.Remove(lngIdentifier);
            }
            return await GetFromCacheOrDbAsync(lngIdentifier, () => GetStashFromDb(lngIdentifier));
        }

        /// <summary>
        /// Retrieves the DTO stash from the Database
        /// </summary>
        /// <param name="lngIdentifier">The Language Identifier</param>
        /// <returns></returns>
        private async Task<AccessoriesDtoStash> GetStashFromDb(string lngIdentifier)
        {
            var accessoriesEntities = await repo.GetAccessoriesAsync(Builders<BathAccessoryEntity>.Filter.Eq(a => a.IsOnline, true));
            var traitEntities = await repo.Traits.GetTraitsAsync(Builders<TraitEntity>.Filter.Eq(t => t.IsEnabled, true));
            var traitClassesEntities = await repo.Traits.TraitClasses.GetTraitClassesAsync(Builders<TraitClassEntity>.Filter.Eq(tc => tc.IsEnabled, true));
            var traitGroupEntities = await repo.Traits.TraitGroups.GetGroupsAsync(Builders<TraitGroupEntity>.Filter.Eq(tg => tg.IsEnabled, true));
            var priceRulesEntities = await priceRulesRepo.GetEntitiesAsync(Builders<CustomPriceRuleEntity>.Filter.Eq(e=> e.IsEnabled, true));
            var accessoriesOptions = await accessoriesOptionsRepo.GetEntitiesAsync(Builders<UserAccessoriesOptionsEntity>.Filter.Eq(e=>e.IsEnabled,true));
            var userEntities = await usersRepo.GetEntitiesAsync(Builders<UserInfoEntity>.Filter.Eq(e => e.IsEnabled, true));
            var stockInfoEntities = await stockRepo.GetAllEntitiesAsync();
            AccessoriesDtoStash stash = new()
            {

                //Accessories Declared Below along with Prices in one pass of the entities
                Traits = traitEntities.Select(t => GetAccessoryTraitDTO(t, lngIdentifier)).ToList(),
                TraitClasses = traitClassesEntities.Select(tc => GetAccessoryTraitClassDTO(tc, lngIdentifier)).ToList(),
                TraitGroups = traitGroupEntities.Select(tg => GetAccessoryTraitGroup(tg, lngIdentifier)).ToList(),
                //Prices Declared Below along with accessories in one pass of the entities
                CustomPriceRules = priceRulesEntities.Select(e=> GetCustomPriceRule(e,lngIdentifier)).ToList(),
                AccessoriesOptions = accessoriesOptions.Select(e=> GetAccessoriesOptionsDTO(e,lngIdentifier)).ToList(),
                Users = userEntities.Select(e => GetUserInfoDTO(e)).ToList(),
                StockInfo = stockInfoEntities.ToDictionary(e => e.Code, e => e.Quantity),
            };
            foreach (var a in accessoriesEntities)
            {
                // Add each Accessory Without Price
                stash.Accessories.Add(GetAccessoryDTO(a, lngIdentifier));
                // Add the Price Seperately
                stash.PricesInfo.Add(a.IdAsString, a.PricesInfo.Select(pi => GetPriceInfoDTO(pi)).ToList());
            }

            return stash;
        }

        /// <summary>
        /// Gets a certain Cached item (For this Repository a Cached Dictionary)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey">The Cache Key of the Item</param>
        /// <param name="GetFromDbAsync">The Create Item Function</param>
        /// <returns>The Cached or Created Item</returns>
        private async Task<T> GetFromCacheOrDbAsync<T>(string cacheKey, Func<Task<T>> GetFromDbAsync)
        {
            // Try to get the Value of the Cache
            if (_cache.TryGetValue(cacheKey, out T? item) is false)
            {
                // If the cache does not contain this Key or is not Valid at the Moment and needs Reinitilization
                // Create the Item
                item = await GetFromDbAsync();

                // Put the Expiration Time for this cached Item
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = options.CacheExpirationRelativeToLastCache
                };
                // Set the Created item to the cache
                _cache.Set(cacheKey, item, cacheEntryOptions);
            }
            // item is never null here this is why its checked above (if the Db Does not return it throws Consumers of this class should try catch)
            return item!;
        }

        /// <summary>
        /// Gets the Accessory Data Transfer Object from an Accessory Entity and the Language Identifier
        /// </summary>
        /// <param name="entity">The Accessory Entity</param>
        /// <param name="langIdentifier">The Four Letter Iso Lnaguage Identifier (ex. el-GR)</param>
        /// <returns>The Data Transfer Object</returns>
        private BathroomAccessoryDTO GetAccessoryDTO(BathAccessoryEntity entity, string langIdentifier)
        {
            BathroomAccessoryDTO dto = new()
            {
                Id = entity.Id.ToString(),
                DescriptionInfo = entity.GetDescriptionInfo(langIdentifier, false),
                MainCode = entity.MainCode,
                ExtraCode = entity.ExtraCode,
                UsesOnlyMainCode = entity.UsesOnlyMainCode,
                SortNo = entity.SortNo,
                MainPhotoUrl = urlHelper.AppendContainerPathToUrl(entity.MainPhotoURL),
                DimensionsPhotoUrl = urlHelper.AppendContainerPathToUrl(entity.DimensionsPhotoUrl),
                ExtraPhotosUrl = entity.ExtraPhotosURL.Select(url => urlHelper.AppendContainerPathToUrl(url)).ToList(),
                PdfUrl = urlHelper.AppendContainerPathToUrl(entity.PdfURL),
                Finish = entity.Finish.ToString(),
                AvailableFinishes = entity.AvailableFinishes.Select(fi=> GetFinishInfoDTO(fi)).ToList(),
                Material = entity.Material.ToString(),
                Size = entity.Size.ToString(),
                SizeVariations = new(entity.SizeVariations),
                Shape = entity.Shape.ToString(),
                PrimaryType = entity.PrimaryTypes.FirstOrDefault() ?? string.Empty,
                OtherPrimaryTypes = entity.PrimaryTypes.Skip(1).ToList(),
                SecondaryType = entity.SecondaryTypes.FirstOrDefault() ?? string.Empty,
                OtherSecondaryTypes = entity.SecondaryTypes.Skip(1).ToList(),
                Categories = new(entity.Categories),
                Series = entity.Series.FirstOrDefault() ?? string.Empty,
                OtherSeries = entity.Series.Skip(1).ToList(),
                MountingTypes = new(entity.MountingTypes),
                MountingVariations = new(entity.MountingVariations),
                Dimensions = new(entity.Dimensions)
            };
            return dto;
        }
        private AccessoryFinishInfoDTO GetFinishInfoDTO(AccessoryFinishInfo finishInfo)
        {
            AccessoryFinishInfoDTO dto = new()
            {
                FinishId = finishInfo.FinishId,
                PhotoUrl = urlHelper.AppendContainerPathToUrl(finishInfo.PhotoUrl),
                DimensionsPhotoUrl = urlHelper.AppendContainerPathToUrl(finishInfo.DimensionsPhotoUrl),
                PdfUrl = urlHelper.AppendContainerPathToUrl(finishInfo.PdfUrl),
                ExtraPhotosUrl = new(finishInfo.ExtraPhotosUrl.Select(url=> urlHelper.AppendContainerPathToUrl(url))),
            };
            return dto;
        }
        private static PriceInfoDTO GetPriceInfoDTO(PriceInfo priceInfo)
        {
            PriceInfoDTO dto = new()
            {
                PriceId = priceInfo.PriceTraitId,
                FinishId = priceInfo.RefersToFinishId,
                FinishGroupId = priceInfo.RefersToFinishGroupId,
                PriceValue = priceInfo.PriceValue,
            };
            return dto;
        }
        private AccessoryTraitDTO GetAccessoryTraitDTO(TraitEntity entity, string langIdentifier)
        {
            AccessoryTraitDTO trait = new()
            {
                Id = entity.Id.ToString(),
                Code = entity.Code,
                SortNo = entity.SortNo,
                PhotoUrl = urlHelper.AppendContainerPathToUrl(entity.PhotoURL),
                Trait = entity.Trait.GetLocalizedValue(langIdentifier),
                TraitTooltip = entity.TraitTooltip.GetLocalizedValue(langIdentifier),
                TraitType = entity.TraitType,
                GroupsIds = new(entity.AssignedGroups),
                SecondaryTypes = entity is PrimaryTypeTraitEntity pt ? new(pt.AllowedSecondaryTypes.Select(st=>st.ToString())) : new()
            };
            return trait;
        }
        private AccessoryTraitClassDTO GetAccessoryTraitClassDTO(TraitClassEntity entity, string langIdentifier)
        {
            AccessoryTraitClassDTO traitClass = new()
            {
                SortNo = entity.SortNo,
                TraitType = entity.TraitType,
                PhotoUrl = urlHelper.AppendContainerPathToUrl(entity.PhotoURL),
                DescriptionInfo = entity.GetDescriptionInfo(langIdentifier),
                AccessoryTraitsIds = entity.Traits.Select(id => id.ToString()).ToList(),
            };
            return traitClass;
        }
        private AccessoryTraitGroup GetAccessoryTraitGroup(TraitGroupEntity entity, string langIdentifier)
        {
            AccessoryTraitGroup traitGroup = new()
            {
                Code = entity.Code,
                SortNo = entity.SortNo,
                DescriptionInfo = entity.GetDescriptionInfo(langIdentifier),
                Id = entity.IdAsString
            };
            return traitGroup;
        }
        private CustomPriceRule GetCustomPriceRule(CustomPriceRuleEntity entity,string langIdentifier)
        {
            CustomPriceRule rule = new()
            {
                Id = entity.IdAsString,
                DescriptionInfo = entity.GetDescriptionInfo(langIdentifier),
                SortNo = entity.SortNo,
                RuleValueType = entity.RuleValueType,
                RuleValue = entity.RuleValue,
                Conditions = new(entity.Conditions.Select(c=> c.GetDeepClone()))
            };
            return rule;
        }
        /// <summary>
        /// Transforms an Entity to the DTO options Object
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static UserAccessoriesOptionsDTO GetAccessoriesOptionsDTO(UserAccessoriesOptionsEntity entity,string langIdentifier)
        {
            UserAccessoriesOptionsDTO dto = new()
            {
                Id = entity.IdAsString,
                DescriptionInfo = entity.GetDescriptionInfo(langIdentifier),
                AppearingDimensionsGroupId = entity.AppearingDimensionsGroup,
                PricesGroupId = entity.PricesGroup,
                Discounts = GetUserAccessoriesDiscounts(entity.Discounts),
                CustomPriceRules = entity.CustomPriceRules
            };
            return dto;
        }
        private static UserAccessoriesDiscounts GetUserAccessoriesDiscounts(UserAccessoriesDiscountsDTO entityDTO)
        {
            UserAccessoriesDiscounts discounts = new()
            {
                MainDiscountDecimal = entityDTO.MainDiscount,
                SecondaryDiscountDecimal = entityDTO.SecondaryDiscount,
                TertiaryDiscountDecimal = entityDTO.TertiaryDiscount,
                QuantityDiscPrimary = entityDTO.QuantityDiscPrimary,
                QuantityDiscQuantityPrimary = entityDTO.QuantityDiscQuantityPrimary,
                QuantityDiscSecondary = entityDTO.QuantityDiscSecondary,
                QuantityDiscQuantitySecondary = entityDTO.QuantityDiscQuantitySecondary,
                QuantityDiscTertiary = entityDTO.QuantityDiscTertiary,
                QuantityDiscQuantityTertiary = entityDTO.QuantityDiscQuantityTertiary,
            };
            return discounts;
        }
        private static UserInfoDTO GetUserInfoDTO(UserInfoEntity entity)
        {
            UserInfoDTO userInfo = new()
            {
                UserName = entity.UserName,
                AccessoriesOptionsId = entity.AccessoriesOptionsId,
                GObjectId = entity.GraphUserObjectId,
                GUserDisplayName = entity.GraphUserDisplayName,
                IsGUser = entity.IsGraphUser,
                RM = entity.RegisteredMachine,
            };
            return userInfo;
        }
    }

    /// <summary>
    /// Options for the MongoAccessoriesRepository
    /// </summary>
    public class MongoAccessoriesRepositoryOptions
    {
        /// <summary>
        /// Weather Caching is Enabled
        /// </summary>
        public bool IsCachingEnabled { get; set; }
        /// <summary>
        /// How much time can Pass before an Item Expires from the Moment it gets set in the Cache
        /// </summary>
        public TimeSpan CacheExpirationRelativeToLastCache { get; set; }
    }
}

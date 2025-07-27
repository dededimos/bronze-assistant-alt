using AccessoriesRepoMongoDB.Entities;
using AccessoriesRepoMongoDB.Validators;
using BathAccessoriesModelsLibrary;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbCommonLibrary.CommonExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BathAccessoriesModelsLibrary.AccessoryTrait;

namespace AccessoriesRepoMongoDB.Repositories
{
    public interface IAccessoryEntitiesRepository
    {
        ITraitEntitiesRepository Traits { get; }

        bool IsCacheDirty { get; }
        /// <summary>
        /// Returns the Cahced Results , when the Cache is not Dirty
        /// </summary>
        IEnumerable<BathAccessoryEntity> Cache { get; }

        Task<string> InsertAccessoryAsync(BathAccessoryEntity accessory);
        Task<BathAccessoryEntity> GetAccessoryAsync(ObjectId id);
        Task<BathAccessoryEntity> GetAccessoryAsync(string code);
        Task<IEnumerable<BathAccessoryEntity>> GetAllAccessoriesAsync();
        /// <summary>
        /// Returns the Accessories of the specified Filter Definition
        /// </summary>
        /// <param name="filter">The Filter Definition</param>
        /// <returns></returns>
        Task<IEnumerable<BathAccessoryEntity>> GetAccessoriesAsync(FilterDefinition<BathAccessoryEntity> filter);
        /// <summary>
        /// Gets the Number of Accessories Entities in the Database
        /// </summary>
        /// <returns>The Number of Documents (accessories)</returns>
        Task<long> GetAccessoriesCountAsync();
        Task UpdateAccessoryAsync(BathAccessoryEntity accessory);
        Task DeleteAccessoryAsync(ObjectId id);

        /// <summary>
        /// Marks the Repositories Cache as Dirty
        /// </summary>
        void MarkCacheAsDirty();
        public event EventHandler? OnCacheBecomingDirty;
        /// <summary>
        /// Sets weather the Cache is Enabled or Not 
        /// </summary>
        /// <param name="isCacheEnabled"></param>
        void SetCaching(bool isCacheEnabled);
    }

    public class MongoAccessoryEntitiesRepository : IAccessoryEntitiesRepository
    {
        private readonly IMongoCollection<BathAccessoryEntity> accessories;
        private readonly ILogger<MongoAccessoryEntitiesRepository> logger;
        private readonly BathAccessoryEntityValidator validator = new();

        private bool isCacheEnabled = true;

        public ITraitEntitiesRepository Traits { get; }

        private List<BathAccessoryEntity> cache = new();

        private bool isCacheDirty = true;

        public event EventHandler? OnCacheBecomingDirty;

        public bool IsCacheDirty { get => isCacheDirty; }
        public IEnumerable<BathAccessoryEntity> Cache { get => IsCacheDirty ? Enumerable.Empty<BathAccessoryEntity>() : cache; }

        public MongoAccessoryEntitiesRepository(
            IMongoDbAccessoriesConnection accConnection,
            ITraitEntitiesRepository traitsRepo,
            ILogger<MongoAccessoryEntitiesRepository> logger)
        {
            this.accessories = accConnection.AccessoriesCollection;
            this.logger = logger;
            Traits = traitsRepo;

            // Whenever the Traits or TraitsClasses Cache becomes Dirty this Cache becomes also Dirty
            Traits.OnCacheBecomingDirty += (_, _) => MarkCacheAsDirty();
            Traits.TraitClasses.OnCacheBecomingDirty += (_, _) => MarkCacheAsDirty();
        }

        public async Task<string> InsertAccessoryAsync(BathAccessoryEntity accessory)
        {
            var currentDate = DateTime.Now.ToUniversalTime();
            //Generate a new Id if not already there
            if (accessory.Id == default)
            {
                accessory.Id = ObjectId.GenerateNewId(currentDate);
            }

            ValidateAccessory(accessory);
            await accessories.InsertOneAsync(accessory);

            logger.LogInformation("Inserted Accessory : {code}", accessory.Code);

            // Update Cache Accordingly (so that no need to mark it as Dirty)
            if (!IsCacheDirty && isCacheEnabled)
            {
                cache.Add(accessory.GetDeepClone());
            }

            return accessory.Id.ToString();
        }
        public async Task<BathAccessoryEntity> GetAccessoryAsync(ObjectId id)
        {
            // Filter to find the Accessory
            var filter = Builders<BathAccessoryEntity>.Filter.Eq(e => e.Id, id);

            // Call to retrieve it
            var result = await accessories.FindAsync(filter);
            // Take only one from the Cursor else throw 
            var accessory = await result.SingleAsync();
            // Return it
            return accessory;
        }
        public async Task<BathAccessoryEntity> GetAccessoryAsync(string code)
        {
            // Filter to find the Accessory
            var filter = Builders<BathAccessoryEntity>.Filter.Eq(e => e.Code, code);

            // Call to retrieve it
            var result = await accessories.FindAsync(filter);
            // Take only one from the Cursor else throw 
            var accessory = await result.SingleAsync();
            // Return it
            return accessory;
        }
        public async Task<IEnumerable<BathAccessoryEntity>> GetAllAccessoriesAsync()
        {
            if (isCacheEnabled && !isCacheDirty)
            {
                logger.LogInformation("Retriving Accessories from Cache...");
                return cache;
            }
            else
            {
                var filter = Builders<BathAccessoryEntity>.Filter.Empty;
                logger.LogInformation("Retrieving All Accessories...");
                var result = await accessories.FindAsync(filter);
                var found = await result.ToListAsync();
                if (isCacheEnabled)
                {
                    cache = found;
                    isCacheDirty = false;
                }
                return found;
            }
        }

        public async Task<IEnumerable<BathAccessoryEntity>> GetAccessoriesAsync(FilterDefinition<BathAccessoryEntity> filter)
        {
            var result = await accessories.FindAsync(filter);
            return await result.ToListAsync();
        }
        public async Task<long> GetAccessoriesCountAsync()
        {
            if (isCacheEnabled && !isCacheDirty)
            {
                return cache.Count;
            }
            else
            {
                var filter = Builders<BathAccessoryEntity>.Filter.Empty;
                return await accessories.CountDocumentsAsync(filter);
            }
        }
        public async Task UpdateAccessoryAsync(BathAccessoryEntity accessory)
        {
            ValidateAccessory(accessory);

            var filter = Builders<BathAccessoryEntity>.Filter.Eq(e => e.Id, accessory.Id);

            FindOneAndReplaceOptions<BathAccessoryEntity, BathAccessoryEntity> options =
                new() { IsUpsert = false, ReturnDocument = ReturnDocument.Before };

            var result = await accessories.FindOneAndReplaceAsync(filter, accessory, options)
                ?? throw new RecordNotFoundException($"Accessory with Id:{accessory.Id} was not Found and did not Update , Current Accessory Code : {accessory.Code}");

            if (isCacheEnabled && !IsCacheDirty)
            {
                var itemToReplace = cache.FirstOrDefault(a => a.Id == result.Id);
                if (itemToReplace is not null)
                {
                    var indexOfItemToReplace = cache.IndexOf(itemToReplace);
                    cache[indexOfItemToReplace] = accessory.GetDeepClone();
                }
                else
                {
                    // if the item is not found in the cache something is wrong so mark the cache as Dirty to be sure;
                    MarkCacheAsDirty();
                }
            }

            logger.LogInformation("Updated Accessory with Id: {id} , Code: {code}", accessory.Id, accessory.Code);
        }
        public async Task DeleteAccessoryAsync(ObjectId id)
        {
            //Check weather this Id is being used as a variation in any other Accessory
            await ThrowIfThereAreRelatedVariations(id);

            var filter = Builders<BathAccessoryEntity>.Filter.Eq(e => e.Id, id);
            var result = await accessories.DeleteOneAsync(filter);
            if (result.DeletedCount < 1)
            {
                throw new RecordNotFoundException($"Could not Delete ,Accessory with Id:{id} was not Found");
            }
            if (isCacheEnabled && !IsCacheDirty)
            {
                var itemToRemove = cache.FirstOrDefault(a => a.Id == id);
                if (itemToRemove is not null)
                {
                    cache.Remove(itemToRemove);
                }
                else
                {
                    // If the item is not included in the Cache Something is wrong , mark the Cache as Dirty
                    MarkCacheAsDirty();
                }
            }
            logger.LogInformation("Deleted Accessory with Id: {id}", id.ToString());
        }

        /// <summary>
        /// Throws an Exception if the Accessory Id Provided is a Variation of any other Accessory
        /// </summary>
        /// <param name="id">The Id of the Accessory to Check for</param>
        /// <returns></returns>
        private async Task ThrowIfThereAreRelatedVariations(ObjectId id)
        {
            string idToString = id.ToString();
            var filterAccessoriesWithThisVariation =
                Builders<BathAccessoryEntity>.Filter.AnyEq(a => a.MountingVariations, idToString) |
                Builders<BathAccessoryEntity>.Filter.AnyEq(a => a.SizeVariations, idToString);
            var result = await accessories.FindAsync(filterAccessoriesWithThisVariation);
            var found = await result.ToListAsync();
            var codesOfFound = found.Select(a => a.Code).ToList();
            if (codesOfFound.Count is not 0)
            {
                var allVariationsMsgs = codesOfFound.Count <= 10 ? codesOfFound.Select(v => $"{codesOfFound.IndexOf(v) + 1}) {v}") : codesOfFound.Take(10).Select(v => $"{codesOfFound.IndexOf(v) + 1}) {v}");
                var joinedMsg = string.Join(Environment.NewLine, allVariationsMsgs);
                throw new Exception($"The Accessory with Id {id} is already being used as a Variation in other Accessories{Environment.NewLine}{Environment.NewLine}{joinedMsg}");
            }
        }

        /// <summary>
        /// Marks the Cache as Dirty
        /// </summary>
        public void MarkCacheAsDirty()
        {
            isCacheDirty = true;
            OnCacheBecomingDirty?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Validates and Throws a Validation Exception if the Accessory is not Valid
        /// </summary>
        /// <param name="accessory">The Accessory to Validate</param>
        /// <exception cref="ValidationException"></exception>
        private void ValidateAccessory(BathAccessoryEntity accessory)
        {
            var valResult = validator.Validate(accessory);
            if (valResult.IsValid is false)
            {
                throw new ValidationException($"Accessory Validation Failed with Error Codes:{Environment.NewLine}{string.Join(Environment.NewLine, valResult.Errors.Select(e => e.ErrorCode))}");
            }
        }

        /// <summary>
        /// Sets weather caching is used or Not
        /// </summary>
        /// <param name="isCacheEnabled">Weather Caching is Enabled (Enabled by Default)</param>
        public void SetCaching(bool isCacheEnabled)
        {
            this.isCacheEnabled = isCacheEnabled;
            if (isCacheEnabled == false)
            {
                cache.Clear();
                MarkCacheAsDirty();
            }
            Traits.SetCaching(isCacheEnabled);
        }
    }



}

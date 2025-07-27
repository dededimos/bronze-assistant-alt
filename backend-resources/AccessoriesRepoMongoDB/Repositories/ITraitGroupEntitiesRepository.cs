using AccessoriesRepoMongoDB.Entities;
using AccessoriesRepoMongoDB.Validators;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbCommonLibrary.CommonExceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesRepoMongoDB.Repositories
{
    public interface ITraitGroupEntitiesRepository
    {
        /// <summary>
        /// Returns all the Trait Groups
        /// </summary>
        /// <returns></returns>
        Task<List<TraitGroupEntity>> GetAllGroupsAsync();
        /// <summary>
        /// Returns all the Groups of the Specified Filter Definition
        /// </summary>
        /// <param name="filter">The Filter</param>
        /// <returns></returns>
        Task<List<TraitGroupEntity>> GetGroupsAsync(FilterDefinition<TraitGroupEntity> filter);

        /// <summary>
        /// Returns the Group matching the specified Id
        /// </summary>
        /// <param name="id">The Id of the Needed Group</param>
        /// <returns></returns>
        Task<TraitGroupEntity> GetGroupAsync(ObjectId id);

        /// <summary>
        /// Updates the specified Trait Group , The Id of the newly updated group must match that of the old one
        /// </summary>
        /// <param name="traitGroup">The New Trait Group</param>
        /// <returns></returns>
        Task UpdateTraitGroupAsync(TraitGroupEntity traitGroup);

        /// <summary>
        /// Inserts the new Trait Group
        /// </summary>
        /// <param name="traitGroup">The New trait Group</param>
        /// <returns></returns>
        Task<string> InsertNewTraitGroupAsync(TraitGroupEntity traitGroup);
        /// <summary>
        /// Deletes the specified Trait Group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteTraitGroupAsync(ObjectId id);

        /// <summary>
        /// Returns the Cached Results when the Cache is not Dirty
        /// </summary>
        IEnumerable<TraitGroupEntity> Cache { get; }

        void MarkCacheAsDirty();
        public event EventHandler? OnCacheBecomingDirty;
        /// <summary>
        /// Sets weather the Cache is Enabled
        /// </summary>
        /// <param name="isCacheEnabled"></param>
        void SetCaching(bool isCacheEnabled);
    }

    public class MongoTraitGroupEntitiesRepository : ITraitGroupEntitiesRepository
    {
        private readonly IMongoCollection<TraitEntity> traits;
        private readonly IMongoCollection<BathAccessoryEntity> accessories;
        private readonly IMongoCollection<TraitGroupEntity> traitGroups;
        private readonly MongoClient client;
        private readonly ILogger<MongoTraitGroupEntitiesRepository> logger;
        private readonly TraitGroupEntityValidator validator = new();

        private List<TraitGroupEntity> cache = [];
        private bool isCacheDirty = true;
        private bool isCacheEnabled = true;
        public IEnumerable<TraitGroupEntity> Cache => isCacheDirty ? Enumerable.Empty<TraitGroupEntity>() : cache;
        public event EventHandler? OnCacheBecomingDirty;

        public MongoTraitGroupEntitiesRepository(IMongoDbAccessoriesConnection connection,
            ILogger<MongoTraitGroupEntitiesRepository> logger)
        {
            this.traits = connection.TraitsCollection;
            this.traitGroups = connection.TraitGroupsCollection;
            this.client = connection.Client;
            this.logger = logger;
            this.accessories = connection.AccessoriesCollection;
        }
        public async Task<List<TraitGroupEntity>> GetAllGroupsAsync()
        {
            if (isCacheEnabled && !isCacheDirty)
            {
                logger.LogInformation("Retrieving Trait Groups from Cache...");
                return cache;
            }
            else
            {
                logger.LogInformation("Retrieving Trait Groups from Database...");
                var filter = Builders<TraitGroupEntity>.Filter.Empty;
                var result = await traitGroups.FindAsync(filter);
                var found = await result.ToListAsync();
                if (isCacheEnabled)
                {
                    cache = found;
                    isCacheDirty = false;
                }
                return found;
            }
        }
        public async Task<List<TraitGroupEntity>> GetGroupsAsync(FilterDefinition<TraitGroupEntity> filter)
        {
            var result = await traitGroups.FindAsync(filter);
            return await result.ToListAsync();
        }
        public async Task<TraitGroupEntity> GetGroupAsync(ObjectId id)
        {
            var filter = Builders<TraitGroupEntity>.Filter.Eq(e => e.Id, id);
            var result = await traitGroups.FindAsync<TraitGroupEntity>(filter);
            return await result.SingleAsync();
        }
        public async Task UpdateTraitGroupAsync(TraitGroupEntity traitGroup)
        {
            ValidateTraitGroup(traitGroup);

            var filter = Builders<TraitGroupEntity>.Filter.Eq(e => e.Id, traitGroup.Id);

            FindOneAndReplaceOptions<TraitGroupEntity, TraitGroupEntity> options =
            new() { IsUpsert = false, ReturnDocument = ReturnDocument.Before };

            var result = await traitGroups.FindOneAndReplaceAsync(filter, traitGroup, options)
            ?? throw new RecordNotFoundException(
            $"Trait Group - {traitGroup.Description.DefaultValue} - with Id:{traitGroup.Id} and Code:{traitGroup.Code} was Not Found and did not Update");
            logger.LogInformation("Trait Group - {traitGroup} - with Id: {id} has been updated", traitGroup.Description.DefaultValue, traitGroup.Id.ToString());

            //Update the Cache Accordingly (if the item is not in the cache , something is wrong mark the cache as Dirty)
            if (isCacheEnabled && !isCacheDirty)
            {
                var itemToReplace = cache.FirstOrDefault(t => t.Id == result.Id);
                if (itemToReplace is not null)
                {
                    var indexOfItemToReplace = cache.IndexOf(itemToReplace);
                    cache[indexOfItemToReplace] = traitGroup.GetDeepClone();
                }
                else
                {
                    MarkCacheAsDirty();
                }
            }
        }
        public async Task<string> InsertNewTraitGroupAsync(TraitGroupEntity traitGroup)
        {
            var currentDate = DateTime.Now.ToUniversalTime();
            //Generate a new Id if not already there
            if (traitGroup.Id == default)
            {
                traitGroup.Id = ObjectId.GenerateNewId(currentDate);
            }

            ValidateTraitGroup(traitGroup);
            await traitGroups.InsertOneAsync(traitGroup);

            logger.LogInformation("Inserted TraitGroup : {code}", traitGroup.Code);

            // Update the Cache if its not already dirty
            if (isCacheEnabled && !isCacheDirty)
            {
                cache.Add(traitGroup.GetDeepClone());
            }

            return traitGroup.Id.ToString();
        }
        public async Task DeleteTraitGroupAsync(ObjectId id)
        {
            //Find the TraitGroup to Delete
            var traitGroup = await GetGroupAsync(id);

            var traitGroupIdToString = id.ToString();
            
            //Check if the Found Group is Assigned to any Traits
            FilterDefinition<TraitEntity> filterTraitEntities = Builders<TraitEntity>.Filter.AnyEq(t => t.AssignedGroups, traitGroupIdToString);
            var resultTraits = await traits.FindAsync(filterTraitEntities);
            var foundTraits = await resultTraits.ToListAsync();

            //if there are Any Found Traits Prevent Deletion
            if (foundTraits.Count != 0)
            {
                var traitsStrings = foundTraits.Count <= 10 ? foundTraits.Select(t => $"{foundTraits.IndexOf(t) + 1} {t.Trait.DefaultValue}({t.Code})") : foundTraits.Take(10).Select(t => $"{foundTraits.IndexOf(t) + 1} {t.Trait.DefaultValue}({t.Code})");
                var traitsJoinedStrings = string.Join(Environment.NewLine, traitsStrings);
                logger.LogInformation("There where Traits Using this TraitGroup with Id:{id}{newLine}{traits}", traitGroup.Id, Environment.NewLine, traitsJoinedStrings);
                throw new Exception($"Cannot Delete TraitGroup while its Assigned to Traits{Environment.NewLine}{traitsJoinedStrings}{Environment.NewLine}...");
            }

            //Check if the Found Group is Assigned to any Accessories Price Info
            FilterDefinition<BathAccessoryEntity> filterAccessories = Builders<BathAccessoryEntity>.Filter.ElemMatch(a=> a.PricesInfo,pi=> pi.RefersToFinishGroupId == traitGroupIdToString);
            var resultAccessories = await accessories.FindAsync(filterAccessories);
            var foundAccessories = await resultAccessories.ToListAsync();

            //if there are Any Found accessories Prevent Deletion
            if (foundAccessories.Count != 0)
            {
                var accessoriesStrings = foundAccessories.Count <= 10 ? foundAccessories.Select(a => $"{foundAccessories.IndexOf(a) + 1}. {a.Code}") : foundAccessories.Take(10).Select(a => $"{foundAccessories.IndexOf(a) + 1}. {a.Code}");
                var accessoriesJoinedStrings = string.Join(Environment.NewLine, accessoriesStrings);
                logger.LogInformation("There where Accessories Prices Using this TraitGroup with Id:{id}{newLine}{accessories}", traitGroup.Id, Environment.NewLine, accessoriesJoinedStrings);
                throw new Exception($"Cannot Delete TraitGroup while its Assigned to Prices of Accessories{Environment.NewLine}{accessoriesJoinedStrings}{Environment.NewLine}...");
            }

            //otherwise delete the TraitGroup
            var deleteTraitGroupFilter = Builders<TraitGroupEntity>.Filter.Eq(tg => tg.Id, traitGroup.Id);
            var deletionResult = await traitGroups.DeleteOneAsync(deleteTraitGroupFilter);
            if (deletionResult.DeletedCount < 1)
            {
                throw new RecordNotFoundException($"Requested TraitGroup to Delete Was Not Found , Id:{traitGroup.Id}-Code{traitGroup.Code}");
            }
            logger.LogInformation("TraitGroup with Id: {id} has been Deleted", traitGroup.Id.ToString());
            // Update the Cache also
            if (isCacheEnabled && !isCacheDirty)
            {
                var itemToRemoveFromCache = cache.FirstOrDefault(tg => tg.Id == traitGroup.Id);
                if (itemToRemoveFromCache is not null)
                {
                    cache.Remove(itemToRemoveFromCache);
                }
                else
                {
                    MarkCacheAsDirty();
                }
            }
        }
        public void MarkCacheAsDirty()
        {
            isCacheDirty = true;
            OnCacheBecomingDirty?.Invoke(this, EventArgs.Empty);
        }
        public void SetCaching(bool isCacheEnabled)
        {
            this.isCacheEnabled = isCacheEnabled;
            if (isCacheEnabled == false)
            {
                cache.Clear();
                MarkCacheAsDirty();
            }
        }

        /// <summary>
        /// Validates and Throws a Validation Exception if the Trait Group is not Valid
        /// </summary>
        /// <param name="traitGroup">The Trait To Validate</param>
        /// <exception cref="ValidationException"></exception>
        private void ValidateTraitGroup(TraitGroupEntity traitGroup)
        {
            var valResult = validator.Validate(traitGroup);
            if (valResult.IsValid is false)
            {
                throw new ValidationException($"Trait Validation Failed with Error Codes:{Environment.NewLine}{string.Join(Environment.NewLine, valResult.Errors.Select(e => e.ErrorCode))}");
            }
        }

    }


}

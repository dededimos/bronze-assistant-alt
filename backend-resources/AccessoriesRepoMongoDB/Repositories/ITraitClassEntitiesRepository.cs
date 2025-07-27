using AccessoriesRepoMongoDB.Entities;
using AccessoriesRepoMongoDB.Validators;
using BathAccessoriesModelsLibrary;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbCommonLibrary.CommonExceptions;

namespace AccessoriesRepoMongoDB.Repositories
{
    public interface ITraitClassEntitiesRepository
    {
        Task<List<TraitClassEntity>> GetAllTraitClassesAsync();
        Task<List<TraitClassEntity>> GetTraitClassesAsync(FilterDefinition<TraitClassEntity> filter);
        Task<TraitClassEntity> GetTraitClassAsync(ObjectId id);
        Task<TraitClassEntity> GetTraitClassAsync(TypeOfTrait traitType);
        Task<bool> AddTraitToTraitClassAsync(TraitEntity trait);
        Task<bool> RemoveTraitFromTraitClassAsync(TraitEntity trait);
        Task UpdateTraitClassAsync(TraitClassEntity traitClass);
        Task<string> InsertNewTraitClassAsync(TraitClassEntity traitClass);
        Task DeleteTraitClassAsync(ObjectId id);

        /// <summary>
        /// The Cache of the Trait Classes
        /// </summary>
        IEnumerable<TraitClassEntity> Cache { get; }

        void MarkCacheAsDirty();
        event EventHandler? OnCacheBecomingDirty;
        /// <summary>
        /// Sets weather the Cache is Enabled or Not
        /// </summary>
        /// <param name="isCacheEnabled"></param>
        void SetCaching(bool isCacheEnabled);
    }
    public class MongoTraitClassEntitiesRepository : ITraitClassEntitiesRepository
    {
        private readonly IMongoCollection<TraitClassEntity> traitClasses;
        private readonly ILogger<MongoTraitClassEntitiesRepository> logger;
        private readonly TraitClassEntityValidator validator = new();

        private List<TraitClassEntity> cache = [];
        private bool isCacheDirty = true;
        private bool isCacheEnabled = true;

        public IEnumerable<TraitClassEntity> Cache { get => isCacheDirty ? Enumerable.Empty<TraitClassEntity>() : cache; }

        public event EventHandler? OnCacheBecomingDirty;

        public MongoTraitClassEntitiesRepository(IMongoDbAccessoriesConnection connection,
            ILogger<MongoTraitClassEntitiesRepository> logger)
        {
            this.traitClasses = connection.TraitClassesCollection;
            this.logger = logger;
        }

        public async Task<List<TraitClassEntity>> GetAllTraitClassesAsync()
        {
            if (isCacheEnabled && !isCacheDirty)
            {
                logger.LogInformation("Retrieving Trait Classes from Cache...");
                return cache;
            }
            else
            {
                logger.LogInformation("Retrieving Trait Classes from Database...");
                var filter = Builders<TraitClassEntity>.Filter.Empty;
                var result = await traitClasses.FindAsync(filter);
                var found = await result.ToListAsync();
                if (isCacheEnabled)
                {
                    isCacheDirty = false;
                    cache = found;
                }
                return found;
            }
        }
        public async Task<List<TraitClassEntity>> GetTraitClassesAsync(FilterDefinition<TraitClassEntity> filter)
        {
            var result = await traitClasses.FindAsync(filter);
            return await result.ToListAsync();
        }
        public async Task<TraitClassEntity> GetTraitClassAsync(TypeOfTrait traitType)
        {
            var filter = Builders<TraitClassEntity>.Filter.Eq(e => e.TraitType, traitType);
            var result = await traitClasses.FindAsync<TraitClassEntity>(filter);
            var foundTraitClass = await result.SingleOrDefaultAsync() 
                ?? throw new RecordNotFoundException($"{nameof(TraitClassEntity)} for {nameof(TypeOfTrait)} :{traitType} was not Found"); ;
            return foundTraitClass;
        }
        public async Task<TraitClassEntity> GetTraitClassAsync(ObjectId id)
        {
            var filter = Builders<TraitClassEntity>.Filter.Eq(e => e.Id, id);
            var result = await traitClasses.FindAsync<TraitClassEntity>(filter);
            return await result.SingleAsync();
        }
        public async Task UpdateTraitClassAsync(TraitClassEntity traitClass)
        {
            ValidateTraitClass(traitClass);

            var filter = Builders<TraitClassEntity>.Filter.Eq(e => e.Id, traitClass.Id) &
                Builders<TraitClassEntity>.Filter.Eq(e => e.TraitType, traitClass.TraitType);

            FindOneAndReplaceOptions<TraitClassEntity, TraitClassEntity> options =
                new() { IsUpsert = false, ReturnDocument = ReturnDocument.Before };
            var result = await traitClasses.FindOneAndReplaceAsync(filter, traitClass, options)
                ?? throw new RecordNotFoundException($"Trait Class with Id:{traitClass.Id} and Type:{traitClass.TraitType} was EITHER{Environment.NewLine}{Environment.NewLine}1.Not Found and did not Update{Environment.NewLine}OR{Environment.NewLine}2.The Type {traitClass.TraitType} has changed from the Original.Trait Classes cannot change their Type after they have been created.");

            if (isCacheEnabled && !isCacheDirty)
            {
                var itemToReplace = cache.FirstOrDefault(tc => tc.Id == result.Id);
                if (itemToReplace is not null)
                {
                    var indexOfItemToReplace = cache.IndexOf(itemToReplace);
                    cache[indexOfItemToReplace] = traitClass.GetDeepClone();
                }
                else
                {
                    MarkCacheAsDirty();
                }
            }

            logger.LogInformation("Updated Trait Class with Type: {type}, Name:{name}", traitClass.TraitType, traitClass.Name.DefaultValue);
        }
        public async Task<string> InsertNewTraitClassAsync(TraitClassEntity traitClass)
        {
            var currentDate = DateTime.Now;
            if (traitClass.Id == default)
            {
                traitClass.Id = ObjectId.GenerateNewId(currentDate.Date.ToUniversalTime());
            }

            ValidateTraitClass(traitClass);
            await traitClasses.InsertOneAsync(traitClass);

            if (isCacheEnabled && !isCacheDirty)
            {
                cache.Add(traitClass.GetDeepClone());
            }

            logger.LogInformation("Inserted new Trait Class with Id: {id} , Type: {type} , Name:{name}", traitClass.Id.ToString(), traitClass.TraitType, traitClass.Name.DefaultValue);
            return traitClass.Id.ToString();
        }
        /// <summary>
        /// Adds a Trait to the List of Traits of a Trait Class
        /// </summary>
        /// <param name="traitId">The Id of the Trait to Add</param>
        /// <param name="traitType">The Type of the Trait to Add</param>
        /// <returns></returns>
        /// <exception cref="RecordNotFoundException"></exception>
        public async Task<bool> AddTraitToTraitClassAsync(TraitEntity trait)
        {

            TraitClassEntity traitClass = await GetTraitClassAsync(trait.TraitType);
            
            bool added = traitClass.Traits.Add(trait.Id);

            if (added)
            {
                await UpdateTraitClassAsync(traitClass);
                //No need to Manually add the Trait (the Update Method takes care of the Replacement)
                logger.LogInformation("Added Trait {trait} with Id {id} to Trait Class of Type {type}", trait.Trait.DefaultValue, trait.Id, trait.TraitType);
                return true;
            }
            else
            {
                logger.LogInformation("Trait Class of Type {type} contains already the Trait {trait} with Id {id}", trait.TraitType, trait.Trait.DefaultValue, trait.Id);
                return false;
            }
        }
        /// <summary>
        /// Removes a Trait from a Trait Class
        /// </summary>
        /// <param name="trait">The Trait to Remove</param>
        /// <returns></returns>
        public async Task<bool> RemoveTraitFromTraitClassAsync(TraitEntity trait)
        {
            TraitClassEntity traitClass = await GetTraitClassAsync(trait.TraitType);
            bool removed = traitClass.Traits.Remove(trait.Id);
            if (removed)
            {
                await UpdateTraitClassAsync(traitClass);
                //No need to Manually add the Trait (the Update Method takes care of the Replacement)
                logger.LogInformation("Removed Trait {trait} with Id {id} from Trait Class of Type {type}", trait.Trait.DefaultValue, trait.Id, trait.TraitType);
                return true;
            }
            else
            {
                logger.LogInformation("Trait Class of Type {type} does not contain the Trait {trait} with Id {id}", trait.TraitType, trait.Trait.DefaultValue, trait.Id);
                return false;
            }
        }

        public async Task DeleteTraitClassAsync(ObjectId id)
        {
            var filter = Builders<TraitClassEntity>.Filter.Eq(e => e.Id, id) &
                Builders<TraitClassEntity>.Filter.Size(e => e.Traits, 0);
            var result = await traitClasses.DeleteOneAsync(filter);
            if (result.DeletedCount < 1)
            {
                throw new RecordNotFoundException($"Could not Delete , TraitClass with Id:{id} EITHER :{Environment.NewLine}{Environment.NewLine}1.Trait Class was not Found{Environment.NewLine}OR{Environment.NewLine}2.The Trait Class is Using Traits and Cannot be Deleted until those traits are Deleted Also.");
            }

            if (isCacheEnabled && !isCacheDirty)
            {
                var itemToRemove = cache.FirstOrDefault(tc => tc.Id == id);
                if (itemToRemove is not null)
                {
                    cache.Remove(itemToRemove);
                }
                else
                {
                    MarkCacheAsDirty();
                }
            }

            logger.LogInformation("Deleted Trait Class with Id: {id}", id.ToString());
        }

        public void MarkCacheAsDirty()
        {
            isCacheDirty = true;
            OnCacheBecomingDirty?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Validates and Throws a Validation Exception if the Trait Class is not Valid
        /// </summary>
        /// <param name="traitClass">The Trait Class to Validate</param>
        /// <exception cref="ValidationException"></exception>
        private void ValidateTraitClass(TraitClassEntity traitClass)
        {
            var valResult = validator.Validate(traitClass);
            if (valResult.IsValid is false)
            {
                throw new ValidationException($"Trait Class Validation Failed with Error Codes:{Environment.NewLine}{string.Join(Environment.NewLine, valResult.Errors.Select(e => e.ErrorCode))}");
            }
        }

        /// <summary>
        /// Sets weather Cache is Enabled or Not (Enabled by Default)
        /// </summary>
        /// <param name="isCacheEnabled"></param>
        public void SetCaching(bool isCacheEnabled)
        {
            this.isCacheEnabled = isCacheEnabled;
            if (isCacheEnabled == false)
            {
                cache.Clear();
                MarkCacheAsDirty();
            }
        }
    }


}

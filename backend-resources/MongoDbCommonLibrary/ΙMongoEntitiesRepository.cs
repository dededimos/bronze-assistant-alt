using CommonInterfacesBronze;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbCommonLibrary.CommonEntities;
using MongoDbCommonLibrary.CommonExceptions;
using MongoDbCommonLibrary.CommonValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbCommonLibrary
{
    public interface IMongoEntitiesRepository<T>
        where T : DbEntity
    {
        IEnumerable<T> Cache { get; }

        Task<string> InsertEntityAsync(T entity);
        Task UpdateEntityAsync(T entity);
        Task DeleteEntityAsync(ObjectId id);
        Task<IEnumerable<T>> GetAllEntitiesAsync();
        Task<IEnumerable<T>> GetEntitiesAsync(FilterDefinition<T> filter);
        Task<T> GetEntityById(ObjectId id);
        Task<long> GetEntitiesCountAsync();

        void MarkCacheAsDirty();
        public event EventHandler? OnCacheBecomingDirty;
        public event EventHandler? OnCacheRefresh;
        void SetCaching(bool isCacheEnabled);
    }

    public class MongoEntitiesRepository<T> : IMongoEntitiesRepository<T>
        where T : DbEntity , IDeepClonable<T>
    {
        private readonly DbEntityValidator<T> validator;
        private readonly IMongoCollection<T> collection;
        protected readonly ILogger<MongoEntitiesRepository<T>> logger;

        public MongoEntitiesRepository(DbEntityValidator<T> validator , IMongoCollection<T> collection , ILogger<MongoEntitiesRepository<T>> logger)
        {
            this.validator = validator;
            this.collection = collection;
            this.logger = logger;
        }

        private List<T> cache = [];
        public IEnumerable<T> Cache { get => IsCacheDirty ? Enumerable.Empty<T>() : cache; }
        private bool isCacheEnabled = true;
        private bool isCacheDirty = true;
        public bool IsCacheDirty { get => isCacheDirty; }

        public event EventHandler? OnCacheBecomingDirty;
        public event EventHandler? OnCacheRefresh;

        public virtual async Task DeleteEntityAsync(ObjectId id)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, id);
            var result = await collection.DeleteOneAsync(filter);
            if (result.DeletedCount < 1)
            {
                throw new RecordNotFoundException($"Entity with id : {id} was not Found , Delete has failed");
            }
            if (isCacheEnabled && !isCacheDirty)
            {
                var itemToRemove = cache.FirstOrDefault(e => e.Id == id);
                if (itemToRemove != null)
                {
                    cache.Remove(itemToRemove);
                }
                else
                {
                    MarkCacheAsDirty();
                }
            }
            logger.LogInformation("Deleted Entity with Id: {id}", id.ToString());
        }

        public async Task<IEnumerable<T>> GetAllEntitiesAsync()
        {
            if (isCacheEnabled && !isCacheDirty)
            {
                logger.LogInformation("Retrieving Accessories from Cache...");
                return cache;
            }
            else
            {
                var filter = Builders<T>.Filter.Empty;
                logger.LogInformation("Retrieving All Entities...");
                var result = await collection.FindAsync(filter);
                var found = await result.ToListAsync();
                if (isCacheEnabled)
                {
                    cache = found;
                    isCacheDirty = false;
                    OnCacheRefresh?.Invoke(this, EventArgs.Empty);
                }
                return found;
            }
        }

        public async Task<IEnumerable<T>> GetEntitiesAsync(FilterDefinition<T> filter)
        {
            var result = await collection.FindAsync(filter);
            return await result.ToListAsync();
        }

        public async Task<T> GetEntityById(ObjectId id)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, id);
            var result = await collection.FindAsync(filter);
            var found = await result.SingleAsync();
            return found;
        }

        public async Task<string> InsertEntityAsync(T entity)
        {
            var currentDate = DateTime.Now.ToUniversalTime();
            if (entity.Id == default)
            {
                entity.Id = ObjectId.GenerateNewId(currentDate);
            }
            ValidateOrThrow(entity);
            await collection.InsertOneAsync(entity);
            logger.LogInformation("Inserted new {entity} with Id {id}", typeof(T).Name, entity.Id);
            if (!IsCacheDirty && isCacheEnabled)
            {
                cache.Add(entity.GetDeepClone());
            }
            return entity.IdAsString;
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

        public async Task UpdateEntityAsync(T entity)
        {
            ValidateOrThrow(entity);
            var filter = Builders<T>.Filter.Eq(e=> e.Id,entity.Id);
            FindOneAndReplaceOptions<T, T> options =
                new() { IsUpsert = false, ReturnDocument = ReturnDocument.Before };
            var result = await collection.FindOneAndReplaceAsync(filter, entity, options)
            ?? throw new RecordNotFoundException($"Entity with Id: {entity.Id} was not Found and did not Update");

            if (isCacheEnabled && !isCacheDirty)
            {
                var itemToReplace = cache.FirstOrDefault(e=> e.Id == result.Id);
                if (itemToReplace is not null)
                {
                    var indexOfItemToReplace = cache.IndexOf(itemToReplace);
                    cache[indexOfItemToReplace] = entity.GetDeepClone();
                }
                else
                {
                    MarkCacheAsDirty();
                }
            }

            logger.LogInformation("Updated Entity with Id:{id}", entity.Id);
        }

        private void ValidateOrThrow(T entity)
        {
            var valResult = validator.Validate(entity);
            if (valResult.IsValid is false)
            {
                throw new ValidationException($"Entity : {typeof(T).Name} , Validation Failed with Error Codes:{Environment.NewLine}{string.Join(Environment.NewLine, valResult.Errors.Select(e => e.ErrorCode))}");
            }
        }

        public async Task<long> GetEntitiesCountAsync()
        {
            if (isCacheEnabled && !isCacheDirty)
            {
                return cache.Count;
            }
            else
            {
                var filter = Builders<T>.Filter.Empty;
                return await collection.CountDocumentsAsync(filter);
            }
        }
    }
}

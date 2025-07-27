using CommonInterfacesBronze;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbCommonLibrary.CommonEntities;
using MongoDbCommonLibrary.CommonExceptions;
using MongoDbCommonLibrary.CommonInterfaces;
using MongoDbCommonLibrary.CommonValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbCommonLibrary
{
    public interface IMongoDatabaseEntityRepo<T>
        where T : IDatabaseEntity
    {
        event EventHandler<T>? EntityUpdated;
        event EventHandler<string>? EntityDeleted;
        event EventHandler<T>? EntityInserted;

        Task DeleteEntityAsync(string id);
        Task<IEnumerable<T>> GetAllEntitiesAsync();
        Task<IEnumerable<T>> GetEntitiesAsync(FilterDefinition<T> filter);
        IAsyncEnumerable<T> GetEntitiesAsync(FilterDefinition<T> filter, FindOptions<T>? options);
        Task<long> GetEntitiesCountAsync();
        Task<T> GetEntityById(string id);
        Task<string> InsertEntityAsync(T entity);
        Task UpdateEntityAsync(T entity);
    }

    /// <summary>
    /// A MongoDB Entities Repository with CRUD and CACHE of the Repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMongoDatabaseEntityRepoCache<T> : IMongoDatabaseEntityRepo<T>, IMongoEntitiesCache<T>
        where T : IDatabaseEntity
    {

    }
    /// <summary>
    /// An interface representing only the Cache of the Database (Without Any CRUD Operations)
    /// <para>Containing All the <see cref="T"/> objects</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMongoEntitiesCache<T> where T : IDatabaseEntity
    {
        IEnumerable<T> Cache { get; }
        /// <summary>
        /// Asynchronously Builds the Cache of the Repository 
        /// </summary>
        /// <returns></returns>
        Task BuildCacheAsync();
        /// <summary>
        /// Whenever the Cache is Built/Refreshed
        /// </summary>
        event EventHandler? CacheRefreshed;
        /// <summary>
        /// Whenever the Cache Changes
        /// </summary>
        event EventHandler? CacheChanged;
    }

    public class MongoDatabaseEntityRepoOptions
    {
        
    }


    /// <summary>
    /// A MongoDB Entities Repository with CRUD Operations
    /// </summary>
    /// <typeparam name="T">The type of Entity that is represented by each Document in the Database</typeparam>
    public class MongoDatabaseEntityRepo<T> : IMongoDatabaseEntityRepo<T>
        where T : IDatabaseEntity, IDeepClonable<T>
    {
        private readonly AbstractValidator<T> validator;
        protected readonly IMongoCollection<T> collection;
        protected readonly ILogger<MongoDatabaseEntityRepo<T>> logger;

        public event EventHandler<T>? EntityUpdated;
        public event EventHandler<string>? EntityDeleted;
        public event EventHandler<T>? EntityInserted;

        public MongoDatabaseEntityRepo(
            AbstractValidator<T> entityValidator,
            IMongoCollection<T> collection,
            IOptions<MongoDatabaseEntityRepoOptions> options,
            ILogger<MongoDatabaseEntityRepo<T>> logger)
        {
            this.validator = entityValidator;
            this.collection = collection;
            this.logger = logger;
        }


        public virtual async Task DeleteEntityAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, id);
            var result = await collection.DeleteOneAsync(filter);
            if (result.DeletedCount < 1)
            {
                throw new RecordNotFoundException($"Entity with id : {id} was not Found , Delete has failed");
            }
            logger.LogInformation("Deleted Entity with Id: {id}", id.ToString());
            EntityDeleted?.Invoke(this, id);
        }
        public virtual async Task<IEnumerable<T>> GetAllEntitiesAsync()
        {
            var filter = Builders<T>.Filter.Empty;
            logger.LogInformation("Retrieving All {T} of Repostitory...", typeof(T).Name);
            var result = await collection.FindAsync(filter);
            var found = await result.ToListAsync();
            return found;
        }
        public virtual async Task<IEnumerable<T>> GetEntitiesAsync(FilterDefinition<T> filter)
        {
            var result = await collection.FindAsync(filter);
            logger.LogInformation("Getting {T} , with Filter {filter} ", typeof(T).Name, filter.ToString());
            return await result.ToListAsync();
        }
        public virtual async IAsyncEnumerable<T> GetEntitiesAsync(FilterDefinition<T> filter,FindOptions<T>? options)
        {
            options ??= new FindOptions<T>();

            // Log the query
            logger.LogInformation("Getting {Type} Entities", typeof(T).Name);

            // Get an async cursor from the collection
            using var cursor = await collection.FindAsync(filter, options);

            // Iterate over batches returned by the cursor
            while (await cursor.MoveNextAsync())
            {
                if (cursor.Current != null)
                {
                    foreach (var document in cursor.Current)
                    {
                        yield return document;
                    }
                }
            }
        }
        public virtual async Task<T> GetEntityById(string id)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, id);
            logger.LogInformation("Getting {T} with ID : {id}", typeof(T).Name, id);
            var result = await collection.FindAsync(filter);
            var found = await result.SingleAsync();
            return found;
        }
        public virtual async Task<string> InsertEntityAsync(T entity)
        {
            var currentDate = DateTime.Now.ToUniversalTime();
            if (string.IsNullOrWhiteSpace(entity.Id))
            {
                var id = ObjectId.GenerateNewId(currentDate).ToString();
                entity.Id = id;
            }
            OnAfterIdAssignmentInsertOperation(entity);
            await ValidateOrThrowAsync(entity);
            await collection.InsertOneAsync(entity);
            logger.LogInformation("Inserted new {entity} with Id {id}", typeof(T).Name, entity.Id);
            EntityInserted?.Invoke(this, entity);
            return entity.Id;
        }
        /// <summary>
        /// Execute Code After the Id Has Been Assigned to an Insert Operation 
        /// <para>This method Executes before Validation and Before Insertion of the Entity to the Database</para>
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void OnAfterIdAssignmentInsertOperation(T entity) { }
        
        public virtual async Task UpdateEntityAsync(T entity)
        {
            await ValidateOrThrowAsync(entity);
            var currentDate = DateTime.Now.ToUniversalTime();
            entity.LastModified = currentDate;
            var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
            FindOneAndReplaceOptions<T, T> options =
                new() { IsUpsert = false, ReturnDocument = ReturnDocument.Before };
            var result = await collection.FindOneAndReplaceAsync(filter, entity, options)
            ?? throw new RecordNotFoundException($"Entity with Id: {entity.Id} was not Found and did not Update");
            logger.LogInformation("Updated {T} with ID:{id}", typeof(T).Name, entity.Id);
            EntityUpdated?.Invoke(this, entity);
        }
        protected virtual Task ValidateOrThrowAsync(T entity)
        {
            var valResult = validator.Validate(entity);
            if (valResult.IsValid is false)
            {
                throw new ValidationException($"Entity : {typeof(T).Name} , Validation Failed with Error Codes:{Environment.NewLine}{string.Join(Environment.NewLine, valResult.Errors.Select(e => e.ErrorCode))}");
            }
            //this way the overrides can use Async
            return Task.CompletedTask;
        }
        public virtual async Task<long> GetEntitiesCountAsync()
        {
            var filter = Builders<T>.Filter.Empty;
            return await collection.CountDocumentsAsync(filter);
        }

        /// <summary>
        /// Deletes all documents from the repository , Does not Trigger any Events consumer needs to handle propagation
        /// </summary>
        /// <returns>The number of documents deleted</returns>
        public virtual async Task<long> DeleteAllAsync()
        {
            var filter = Builders<T>.Filter.Empty;
            var result = await collection.DeleteManyAsync(filter);

            logger.LogInformation("Deleted all {Type} documents. Count: {count}", typeof(T).Name, result.DeletedCount);

            // Optionally: If we want to trigger the EntityDeleted event for each deleted entity,
            // we would need to first retrieve all entities, then delete them and trigger events.
            // This implementation prioritizes performance by using a single operation.does not trigger any events consumer needs to handle this
            //EntityDeleted?.Invoke(this, "");

            return result.DeletedCount;
        }

        /// <summary>
        /// Inserts multiple documents in bulk , DOES NOT TRIGGER ANY EVENTS CONSUMER NEEDS TO HANDLE PROPAGATION 
        /// </summary>
        /// <param name="items">Collection of entities to insert</param>
        /// <returns>Result containing successfully inserted items and failed items with their exceptions</returns>
        public virtual async Task<BulkInsertResult<T>> InsertBulkAsync(IEnumerable<T> items)
        {
            var result = new BulkInsertResult<T>();
            var currentDate = DateTime.Now.ToUniversalTime();
            var validItems = new List<T>();

            // Process each item - assign ID and validate
            foreach (var item in items)
            {
                try
                {
                    // Assign ObjectId if not present
                    if (string.IsNullOrWhiteSpace(item.Id))
                    {
                        var id = ObjectId.GenerateNewId(currentDate).ToString();
                        item.Id = id;
                    }

                    // Call hook method
                    OnAfterIdAssignmentInsertOperation(item);

                    // Validate
                    await ValidateOrThrowAsync(item);

                    // Add to valid items
                    validItems.Add(item);
                }
                catch (Exception ex)
                {
                    result.FailedItems.Add(new FailedItemInfo<T>(item, ex));
                }
            }

            // Insert valid items if any
            if (validItems.Count != 0)
            {
                try
                {
                    await collection.InsertManyAsync(validItems);

                    // Track successful items
                    result.SuccessfulItems.AddRange(validItems);

                    // Trigger events for each successfully inserted item
                    //DOES NOT TRIGGER ANY EVENTS IN BULK INSERTS CONSUMER NEEDS TO HANDLE THIS
                    //foreach (var item in validItems)
                    //{
                    //    EntityInserted?.Invoke(this, item);
                    //}

                    logger.LogInformation("Bulk inserted {count} {Type} documents", validItems.Count, typeof(T).Name);
                }
                catch (MongoBulkWriteException ex)
                {
                    // Handle bulk write exceptions
                    logger.LogError(ex, "Bulk insert operation failed for {Type}", typeof(T).Name);

                    // Move successfully processed items from validItems to result.SuccessfulItems
                    // based on the write errors
                    var failedIndexes = ex.WriteErrors.Select(e => e.Index).ToHashSet();

                    for (int i = 0; i < validItems.Count; i++)
                    {
                        if (failedIndexes.Contains(i))
                        {
                            // This was a failed item
                            result.FailedItems.Add(
                                new FailedItemInfo<T>(
                                    validItems[i], 
                                    new Exception($"Bulk write error: {ex.WriteErrors.FirstOrDefault(e => e.Index == i)?.Message}")));
                        }
                        else
                        {
                            // This was a successful item
                            result.SuccessfulItems.Add(validItems[i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle other exceptions during insertion
                    logger.LogError(ex, "Error during bulk insert of {Type} documents", typeof(T).Name);

                    // Move all valid items to failed items since we don't know which ones succeeded
                    foreach (var item in validItems)
                    {
                        result.FailedItems.Add(new FailedItemInfo<T>(item, ex));
                    }
                }
            }

            return result;
        }
    }

    /// <summary>
    /// A Database Repository Implementation with Caching
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MongoDatabaseEntityRepoCache<T> : IMongoDatabaseEntityRepoCache<T>, IMongoEntitiesCache<T>
        where T : IDatabaseEntity
    {
        private readonly IMongoDatabaseEntityRepo<T> entitiesRepo;

        private List<T> cache = [];
        public IEnumerable<T> Cache { get => cache; }

        public event EventHandler? CacheRefreshed;
        public event EventHandler? CacheChanged;
        public event EventHandler<T>? EntityUpdated;
        public event EventHandler<T>? EntityInserted;
        public event EventHandler<string>? EntityDeleted;

        public MongoDatabaseEntityRepoCache(IMongoDatabaseEntityRepo<T> entitiesRepo)
        {
            this.entitiesRepo = entitiesRepo;
        }

        public async Task BuildCacheAsync()
        {
            _ = await this.GetAllEntitiesAsync();
        }
        private void OnEntityUpdated(T entity)
        {
            var entityToBeReplaced = cache.FirstOrDefault(e => e.Id == entity.Id) ?? throw new CacheRecordNotFoundException();
            var indexOfEntity = cache.IndexOf(entityToBeReplaced);
            cache[indexOfEntity] = entity;
            EntityUpdated?.Invoke(this, entity);
            CacheChanged?.Invoke(this, EventArgs.Empty);
        }
        private void OnEntityInserted(T entity)
        {
            cache.Add(entity);
            EntityInserted?.Invoke(this, entity);
            CacheChanged?.Invoke(this, EventArgs.Empty);
        }
        private void OnEntityDeleted(string id)
        {
            var entityToDelete = cache.FirstOrDefault(e => e.Id == id) ?? throw new CacheRecordNotFoundException();
            cache.Remove(entityToDelete);
            EntityDeleted?.Invoke(this, id);
            CacheChanged?.Invoke(this, EventArgs.Empty);
        }
        public async Task DeleteEntityAsync(string id)
        {
            await entitiesRepo.DeleteEntityAsync(id);
            OnEntityDeleted(id);
        }
        public async Task<IEnumerable<T>> GetAllEntitiesAsync()
        {
            var result = await entitiesRepo.GetAllEntitiesAsync();
            cache = result.ToList();
            CacheRefreshed?.Invoke(this, EventArgs.Empty);
            CacheChanged?.Invoke(this, EventArgs.Empty);
            return cache;
        }
        public async Task<IEnumerable<T>> GetEntitiesAsync(FilterDefinition<T> filter)
        {
            return await entitiesRepo.GetEntitiesAsync(filter);
        }
        public async Task<long> GetEntitiesCountAsync()
        {
            return await entitiesRepo.GetEntitiesCountAsync();
        }
        public async Task<T> GetEntityById(string id)
        {
            return await entitiesRepo.GetEntityById(id);
        }
        public async Task<string> InsertEntityAsync(T entity)
        {
            var id = await entitiesRepo.InsertEntityAsync(entity);
            OnEntityInserted(entity);
            return id;
        }
        public async Task UpdateEntityAsync(T entity)
        {
            await entitiesRepo.UpdateEntityAsync(entity);
            OnEntityUpdated(entity);
        }
        public IAsyncEnumerable<T> GetEntitiesAsync(FilterDefinition<T> filter, FindOptions<T>? options)
        {
            return entitiesRepo.GetEntitiesAsync(filter,options);
        }
    }

    /// <summary>
    /// Information about a failed item in a bulk operation
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    public class FailedItemInfo<T> (T item , Exception exception)
    {
        public T Item { get; set; } = item;
        public Exception Exception { get; set; } = exception;
    }
    /// <summary>
    /// Result object for bulk insert operations
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    public class BulkInsertResult<T>
    {
        public List<T> SuccessfulItems { get; set; } = [];
        public List<FailedItemInfo<T>> FailedItems { get; set; } = [];

        public bool HasFailures => FailedItems.Count != 0;
        public int TotalProcessed => SuccessfulItems.Count + FailedItems.Count;
        public int SuccessCount => SuccessfulItems.Count;
        public int FailureCount => FailedItems.Count;
    }
}

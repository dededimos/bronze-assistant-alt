using AccessoriesRepoMongoDB.Entities;
using AccessoriesRepoMongoDB.Validators;
using BathAccessoriesModelsLibrary;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbCommonLibrary.CommonExceptions;
using System.ComponentModel.DataAnnotations;

namespace AccessoriesRepoMongoDB.Repositories
{
    public interface ITraitEntitiesRepository
    {
        Task<List<TraitEntity>> GetAllTraitsAsync();
        /// <summary>
        /// Returns the traits of teh Specified Filter Definition
        /// </summary>
        /// <param name="filter">The Filter Definition</param>
        /// <returns></returns>
        Task<List<TraitEntity>> GetTraitsAsync(FilterDefinition<TraitEntity> filter);
        Task<TraitEntity> GetTraitAsync(ObjectId id);
        Task UpdateTraitAsync(TraitEntity trait);
        Task<string> InsertNewTraitAsync(TraitEntity trait);
        Task DeleteTraitAsync(ObjectId id);

        ITraitClassEntitiesRepository TraitClasses { get; }
        ITraitGroupEntitiesRepository TraitGroups { get; }

        /// <summary>
        /// Returns the Cached Results when the Cache is not Dirty
        /// </summary>
        IEnumerable<TraitEntity> Cache { get; }

        void MarkCacheAsDirty();
        public event EventHandler? OnCacheBecomingDirty;
        /// <summary>
        /// Sets weather the Cache is Enabled
        /// </summary>
        /// <param name="isCacheEnabled"></param>
        void SetCaching(bool isCacheEnabled);
    }
    public class MongoTraitEntitiesRepository : ITraitEntitiesRepository
    {
        private readonly IMongoCollection<BathAccessoryEntity> accessories;
        private readonly IMongoCollection<TraitEntity> traits;
        private readonly MongoClient client;
        private readonly ILogger<MongoTraitEntitiesRepository> logger;
        private readonly TraitEntityValidator validator = new();

        private List<TraitEntity> cache = [];
        private bool isCacheDirty = true;
        private bool isCacheEnabled = true;

        public IEnumerable<TraitEntity> Cache => isCacheDirty ? Enumerable.Empty<TraitEntity>() : cache;

        public ITraitClassEntitiesRepository TraitClasses { get; }
        public ITraitGroupEntitiesRepository TraitGroups { get; }

        public event EventHandler? OnCacheBecomingDirty;

        public MongoTraitEntitiesRepository(IMongoDbAccessoriesConnection connection,
            ITraitClassEntitiesRepository traitClassesRepo,
            ITraitGroupEntitiesRepository traitsGroupsRepo,
            ILogger<MongoTraitEntitiesRepository> logger)
        {
            this.accessories = connection.AccessoriesCollection;
            this.traits = connection.TraitsCollection;
            this.client = connection.Client;
            TraitClasses = traitClassesRepo;
            TraitGroups = traitsGroupsRepo;
            this.logger = logger;
        }


        public async Task<List<TraitEntity>> GetAllTraitsAsync()
        {
            if (isCacheEnabled && !isCacheDirty)
            {
                logger.LogInformation("Retrieving Traits from Cache...");
                return cache;
            }
            else
            {
                logger.LogInformation("Retrieving Traits from Database...");
                var filter = Builders<TraitEntity>.Filter.Empty;
                var result = await traits.FindAsync(filter);
                var found = await result.ToListAsync();
                if (isCacheEnabled)
                {
                    cache = found;
                    isCacheDirty = false;
                }
                return found;
            }
        }
        public async Task<List<TraitEntity>> GetTraitsAsync(FilterDefinition<TraitEntity> filter)
        {
            var result = await traits.FindAsync(filter);
            return await result.ToListAsync();
        }
        public async Task<TraitEntity> GetTraitAsync(ObjectId id)
        {
            var filter = Builders<TraitEntity>.Filter.Eq(e => e.Id, id);
            var result = await traits.FindAsync<TraitEntity>(filter);
            return await result.SingleAsync();
        }
        public async Task UpdateTraitAsync(TraitEntity trait)
        {
            ValidateTrait(trait);

            using var session = await client.StartSessionAsync();

            try
            {
                // Start the Transaction
                session.StartTransaction();
                logger.LogInformation("Updating Trait Entity Transaction Started...");
                var filter = Builders<TraitEntity>.Filter.Eq(e => e.Id, trait.Id) &
                    Builders<TraitEntity>.Filter.Eq(e => e.TraitType, trait.TraitType);
                // WILL NOT UPDATE THE TRAIT TYPE OF AN EXISTING TRAIT

                FindOneAndReplaceOptions<TraitEntity, TraitEntity> options =
                    new() { IsUpsert = false, ReturnDocument = ReturnDocument.Before };

                var result = await traits.FindOneAndReplaceAsync(filter, trait, options)
                    ?? throw new RecordNotFoundException(
                        $"Trait - {trait.Trait.DefaultValue} - with Id:{trait.Id} and Type{trait.TraitType} was EITHER {Environment.NewLine}{Environment.NewLine} 1.Not Found and did not Update{Environment.NewLine}OR{Environment.NewLine}2.The Trait Type :{trait.TraitType} , was Changed From the Original . Traits Cannot Change Their Trait Type after they have been created");

                logger.LogInformation("Trait - {trait} - with Id: {id} has been updated", trait.Trait.DefaultValue, trait.Id.ToString());

                //Update the Cache Accordingly (if the item is not in the cache , something is wrong mark the cache as Dirty)
                if (isCacheEnabled && !isCacheDirty)
                {
                    var itemToReplace = cache.FirstOrDefault(t => t.Id == result.Id);
                    if (itemToReplace is not null)
                    {
                        var indexOfItemToReplace = cache.IndexOf(itemToReplace);
                        cache[indexOfItemToReplace] = trait.GetDeepClone();
                    }
                    else
                    {
                        MarkCacheAsDirty();
                    }
                }


                logger.LogInformation("Replacing Trait To Trait Classes List ...");

                // If the Trait Type being updated had changed ,
                // then the trait must be Removed from the Old Trait Class and Moved to the Correct One
                if (result.TraitType != trait.TraitType)
                {
                    // Remove the Old Trait from the Old Trait Class
                    await TraitClasses.RemoveTraitFromTraitClassAsync(result);
                    // Add the New Trait to the New Trait Class
                    await TraitClasses.AddTraitToTraitClassAsync(trait);
                }

                await session.CommitTransactionAsync();
                logger.LogInformation("Update Transaction Has been Commited...");
            }
            catch (Exception)
            {
                await session.AbortTransactionAsync();
                logger.LogWarning("Update Transaction has been aborted...");
                throw;
            }
        }
        public async Task<string> InsertNewTraitAsync(TraitEntity trait)
        {
            using var session = await client.StartSessionAsync();

            try
            {
                // Start the Transaction
                session.StartTransaction();
                logger.LogInformation("Insert Trait Entity Transaction Started...");

                var currentDate = DateTime.Now.ToUniversalTime();
                //Generate a new Id if not already there
                if (trait.Id == default)
                {
                    trait.Id = ObjectId.GenerateNewId(currentDate);
                }

                ValidateTrait(trait);
                await traits.InsertOneAsync(trait);
                logger.LogInformation("New Trait - {trait} - with Id: {id} , has been inserted", trait.Id, trait.Trait.DefaultValue);

                await TraitClasses.AddTraitToTraitClassAsync(trait);


                // Update the Cache if its not already dirty
                if (isCacheEnabled && !isCacheDirty)
                {
                    cache.Add(trait.GetDeepClone());
                }

                await session.CommitTransactionAsync();
                logger.LogInformation("Transaction has been commited...");
                return trait.Id.ToString();
            }
            catch (Exception)
            {
                await session.AbortTransactionAsync();
                logger.LogWarning("Update Transaction has been aborted...");
                throw;
            }
        }
        public async Task DeleteTraitAsync(ObjectId id)
        {
            using var session = await client.StartSessionAsync();
            try
            {
                // Start the Transaction
                session.StartTransaction();
                logger.LogInformation("Insert Trait Entity Transaction Started...");
                // Find the Trait
                var trait = await GetTraitAsync(id);

                // Check if the Trait is a Secondary Trait and there is already inside some Primary Trait
                if (trait.TraitType is TypeOfTrait.SecondaryTypeTrait)
                {
                    var filterPrimaryTypes = Builders<TraitEntity>.Filter.And(
                        Builders<TraitEntity>.Filter.Eq("_t", "PrimaryTypeTraitEntity"),
                        Builders<TraitEntity>.Filter.AnyEq("AllowedSecondaryTypes", trait.Id));
                    var primaryTypesContainingTrait = await traits.Find(filterPrimaryTypes).ToListAsync();
                    if (primaryTypesContainingTrait != null && primaryTypesContainingTrait.Count > 0)
                    {
                        string exceptionMessage = "There Are PrimaryType Traits Already Using this Secondary Trait , please delete it from there first";
                        string allContainedPrimaryTraitsMessage = string.Join(Environment.NewLine, primaryTypesContainingTrait.Select(pt => pt.Trait.DefaultValue));
                        throw new Exception($"{exceptionMessage}{Environment.NewLine}{Environment.NewLine}{allContainedPrimaryTraitsMessage}");
                    }
                }

                // Create a filter to Check if there are any Accessories Using the Trait

                // The Properties that are Dictionaries can have their Keys Queried
                // only with the Dot Notation as done below for the Price and Dimension Traits , with the Exists method
                // The string interpolation cannot be used inside the Filter Methods because they are Executed Serverside and they are not recognized ?!
                string dotNotationMongoFilter = "";
                var traitIdToString = trait.Id.ToString();
                if (trait.TraitType is TypeOfTrait.DimensionTrait)
                {
                    dotNotationMongoFilter = $"{nameof(BathAccessoryEntity.Dimensions)}.{traitIdToString}";
                }
                else if (trait.TraitType is TypeOfTrait.PriceTrait)
                {
                    dotNotationMongoFilter = $"{nameof(BathAccessoryEntity.PricesInfo)}.{nameof(PriceInfo.PriceTraitId)}";
                }

                FilterDefinition<BathAccessoryEntity> filterAccessories = trait.TraitType switch
                {
                    TypeOfTrait.FinishTrait => 
                        Builders<BathAccessoryEntity>.Filter.Eq(a=> a.Finish,traitIdToString) |
                        Builders<BathAccessoryEntity>.Filter.ElemMatch(a => a.AvailableFinishes,finish=> finish.FinishId == traitIdToString),
                    TypeOfTrait.MaterialTrait => Builders<BathAccessoryEntity>.Filter.Eq(a => a.Material, traitIdToString),
                    TypeOfTrait.CategoryTrait => Builders<BathAccessoryEntity>.Filter.AnyEq(a => a.Categories, traitIdToString),
                    TypeOfTrait.SizeTrait => Builders<BathAccessoryEntity>.Filter.Eq(a => a.Size, traitIdToString),
                    TypeOfTrait.DimensionTrait => Builders<BathAccessoryEntity>.Filter.Exists(dotNotationMongoFilter, true),
                    TypeOfTrait.SeriesTrait => Builders<BathAccessoryEntity>.Filter.AnyEq(a => a.Series, traitIdToString),
                    TypeOfTrait.ShapeTrait => Builders<BathAccessoryEntity>.Filter.Eq(a => a.Shape, traitIdToString),
                    TypeOfTrait.PrimaryTypeTrait => Builders<BathAccessoryEntity>.Filter.AnyEq(a => a.PrimaryTypes, traitIdToString),
                    TypeOfTrait.SecondaryTypeTrait => Builders<BathAccessoryEntity>.Filter.AnyEq(a => a.SecondaryTypes, traitIdToString),
                    TypeOfTrait.MountingTypeTrait => Builders<BathAccessoryEntity>.Filter.AnyEq(a => a.MountingTypes, traitIdToString),
                    TypeOfTrait.PriceTrait => Builders<BathAccessoryEntity>.Filter.ElemMatch(a=> a.PricesInfo,pi=> pi.PriceTraitId == traitIdToString),
                    _ => throw new NotSupportedException($"The Found Traits Type is not Supported -"),
                };

                //check if any accessories are using the Trait marked for Deletion.
                var resultAccessories = await accessories.FindAsync(filterAccessories);
                var foundAccessories = await resultAccessories.ToListAsync();

                // if there are any accessories that satisfy the Filter PREVENT DELETION
                if (foundAccessories.Count != 0)
                {
                    var accessoriesStrings = foundAccessories.Count <= 10 ? foundAccessories.Select(a => $"{foundAccessories.IndexOf(a) + 1}) {a.Code}") : foundAccessories.Take(10).Select(a => $"{foundAccessories.IndexOf(a) + 1}) {a.Code}");
                    var accessoriesJoinedStrings = string.Join(Environment.NewLine, accessoriesStrings);
                    logger.LogInformation("There where Accessories Using trait with Id:{id}{newLine}{accessories}", trait.Id, Environment.NewLine, accessoriesJoinedStrings);
                    throw new Exception($"Cannot Delete Trait while there are Accessories Using it{Environment.NewLine}{accessoriesJoinedStrings}{Environment.NewLine}...");
                }

                // otherwise delete the trait
                var deleteTraitFilter = Builders<TraitEntity>.Filter.Eq(e => e.Id, trait.Id);
                var deletionResult = await traits.DeleteOneAsync(deleteTraitFilter);
                if (deletionResult.DeletedCount < 1)
                {
                    throw new RecordNotFoundException($"Requested Trait to Delete Was Not Found , Id:{trait.Id}-TraitType{trait.TraitType}");
                }
                logger.LogInformation("Trait with Id: {id} has been Deleted", trait.Id.ToString());

                // Update the Cache also
                if (isCacheEnabled && !isCacheDirty)
                {
                    var itemToRemoveFromCache = cache.FirstOrDefault(t => t.Id == trait.Id);
                    if (itemToRemoveFromCache is not null)
                    {
                        cache.Remove(itemToRemoveFromCache);
                    }
                    else
                    {
                        MarkCacheAsDirty();
                    }
                }
                // Remove the Trait also from any TraitClasses 
                await TraitClasses.RemoveTraitFromTraitClassAsync(trait);

                await session.CommitTransactionAsync();
                logger.LogInformation("Transaction has been commited...");
            }
            catch (Exception)
            {
                await session.AbortTransactionAsync();
                logger.LogWarning("Update Transaction has been aborted...");
                throw;
            }
        }

        public void MarkCacheAsDirty()
        {
            isCacheDirty = true;
            OnCacheBecomingDirty?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Validates and Throws a Validation Exception if the Trait is not Valid
        /// </summary>
        /// <param name="trait">The Trait To Validate</param>
        /// <exception cref="ValidationException"></exception>
        private void ValidateTrait(TraitEntity trait)
        {
            var valResult = validator.Validate(trait);
            if (valResult.IsValid is false)
            {
                throw new ValidationException($"Trait Validation Failed with Error Codes:{Environment.NewLine}{string.Join(Environment.NewLine, valResult.Errors.Select(e => e.ErrorCode))}");
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
            TraitClasses.SetCaching(isCacheEnabled);
            TraitGroups.SetCaching(isCacheEnabled);
        }
    }


}

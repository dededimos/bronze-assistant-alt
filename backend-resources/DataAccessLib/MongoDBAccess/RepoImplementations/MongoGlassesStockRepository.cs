using DataAccessLib.NoSQLModels;
using GlassesOrdersModels.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLib.MongoDBAccess.RepoImplementations
{
    /// <summary>
    /// The MongoDB Implementation of the IGlassesStockRepository
    /// </summary>
    public class MongoGlassesStockRepository : IGlassesStockRepository
    {
    
        private readonly IMongoCollection<StockedGlassRowEntity> stockCollection;
        /// <summary>
        /// Filters Nothing (returns all Results)
        /// </summary>
        private readonly FilterDefinition<StockedGlassRowEntity> emptyFilter = Builders<StockedGlassRowEntity>.Filter.Empty;

        public MongoGlassesStockRepository(IMongoDbCabinsConnection connection)
        {
            stockCollection = connection.GlassesStockCollection;
        }

        /// <summary>
        /// Inserts a New Glass to the StockList
        /// </summary>
        /// <param name="glassToAdd">The Glass to Add</param>
        /// <returns>Returns the Inserted Id as a string</returns>
        public async Task<string> InsertNewGlassAsync(StockedGlassRow glassToAdd)
        {
            var currentDate = DateTime.Now;
            StockedGlassRowEntity entity = new(glassToAdd);
            //Generate a new Id if not already there
            if (entity.Id == default)
            {
                entity.Id = ObjectId.GenerateNewId(currentDate.Date.ToUniversalTime());
            }
            await stockCollection.InsertOneAsync(entity);
            return entity.Id.ToString();
        }
        
        public async Task<IEnumerable<StockedGlassRow>> GetAllStock()
        {
            var result = await stockCollection.Find(emptyFilter).ToListAsync();
            return result.Select(r => r.ToStockedGlassRow());
        }

        /// <summary>
        /// Removes a Glass from the StockList
        /// </summary>
        /// <param name="glassId">The id of the Glass to Remove</param>
        /// <returns></returns>
        public async Task RemoveFromStock(string glassId)
        {
            var id = ObjectId.TryParse(glassId, out ObjectId outId) ? outId : throw new MongoException($"Invalid ObjectId , the Id string could not be Parsed : {glassId}");
            var filter = Builders<StockedGlassRowEntity>.Filter.Eq(e => e.Id, id);
            var result = await stockCollection.DeleteOneAsync(filter);
            if (result.DeletedCount < 1)
            {
                throw new RecordNotFoundException($"Could not Delete , Glass with Id:{id} was not Found");
            }
        }
        /// <summary>
        /// Updates a Glass already in the Stock List
        /// </summary>
        /// <param name="updatedGlass"></param>
        /// <returns></returns>
        public async Task UpdateStockedGlass(StockedGlassRow updatedGlass)
        {
            var updatedEntity = new StockedGlassRowEntity(updatedGlass);
            var filter = Builders<StockedGlassRowEntity>.Filter.Eq(e => e.Id, updatedEntity.Id);
            //Replace the found in the database
            //Do not Insert a new one if the part is not there
            FindOneAndReplaceOptions<StockedGlassRowEntity, StockedGlassRowEntity> options =
                new() { IsUpsert = false, ReturnDocument = ReturnDocument.Before };

            var result = await stockCollection.FindOneAndReplaceAsync(filter, updatedEntity, options)
                ?? throw new RecordNotFoundException($"Glass with Id{updatedEntity.Id} was not Found and did not Update");
        }
    }
}

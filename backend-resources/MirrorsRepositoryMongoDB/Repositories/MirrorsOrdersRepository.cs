using CommonOrderModels;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MirrorsLib.Enums;
using MirrorsRepositoryMongoDB.Entities;
using MirrorsRepositoryMongoDB.Validators;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbCommonLibrary;
using MongoDbCommonLibrary.CommonExceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsRepositoryMongoDB.Repositories
{
    public class MirrorsOrdersRepository : MongoDatabaseEntityRepo<MirrorsOrderEntity>
    {
        public MirrorsOrdersRepository(MirrorsOrderEntityValidator entityValidator,
                                       IMongoDbMirrorsConnection connection,
                                       IOptions<MirrorsOrdersRepositoryOptions> options,
                                       ILogger<MirrorsOrdersRepository> logger)
            : base(entityValidator, connection.MirrorsOrdersCollection, options, logger)
        {
            this.options = options.Value;
        }
        private readonly MirrorsOrdersRepositoryOptions options;
        private readonly SortDefinition<MirrorsOrderEntity> sortOrderDescendingDate = Builders<MirrorsOrderEntity>.Sort.Descending(e => e.Created);
        private readonly FilterDefinition<MirrorsOrderEntity> emptyFilter = Builders<MirrorsOrderEntity>.Filter.Empty;
        private readonly MirrorOrderRowEntityValidator rowValidator = new(true);

        /// <summary>
        /// Gets the Number of Pages in a Certain Query , based on the PagedQuerySize
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<int> GetNumberOfPagesOfQuery(FilterDefinition<MirrorsOrderEntity> filter , int documentsPerPage = 20)
        {
            if (documentsPerPage <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(documentsPerPage), "The Documents Per Page must be greater than 0");
            }
            var totalDocuments = await collection.CountDocumentsAsync(filter);
            return (int)Math.Ceiling((double)totalDocuments / documentsPerPage);
        }
        /// <summary>
        /// Returns the number of Pages for the whole collection of Orders
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetNumberPagesOfAllOrders(int documentsPerPage = 20)
        {
            return await GetNumberOfPagesOfQuery(emptyFilter,documentsPerPage);
        }

        /// <summary>
        /// Gets the Orders of a certain Page from all Orders
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async IAsyncEnumerable<MirrorsOrderEntity> GetPageFromAllOrdersAsync(int page,int documentsPerPage = 20)
        {
            var pages = await GetNumberOfPagesOfQuery(emptyFilter);

            if (page < 0 || page > pages)
            {
                throw new ArgumentOutOfRangeException(nameof(page), $"The Page Number must be between 0 and {pages - 1}");
            }

            FindOptions<MirrorsOrderEntity> findOptions = new()
            {
                Sort = sortOrderDescendingDate,
                Skip = (page - 1) * documentsPerPage,
                Limit = documentsPerPage
            };
            var results = await collection.FindAsync(emptyFilter, findOptions);
            
            while (await results.MoveNextAsync())
            {
                if (results.Current != null)
                {
                    foreach (var order in results.Current)
                    {
                        yield return order;
                    }
                }
            }
        }
        /// <summary>
        /// Gets the order No of the Last Created Order 
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetLastOrderNoAsync() 
        {
            var found = await collection.Find(emptyFilter).Sort(sortOrderDescendingDate).Limit(1).FirstOrDefaultAsync();
            return found?.OrderNo ?? string.Empty;
        }

        /// <summary>
        /// Returns the Pending MirrorOrderRows from a certain Date Range
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<MirrorOrderRowEntity> GetPendingRows(DateTime from, DateTime to)
        {
            from = from.ToUniversalTime();
            to = to.ToUniversalTime();

            var pendingFilter = Builders<MirrorsOrderEntity>.Filter.Eq(e => e.Status, CommonOrderModels.OrderStatus.PendingOrderStatus)
                | Builders<MirrorsOrderEntity>.Filter.Eq(e=>e.Status,CommonOrderModels.OrderStatus.PartiallyFilledOrderStatus)
                | Builders<MirrorsOrderEntity>.Filter.Eq(e => e.Status, CommonOrderModels.OrderStatus.PartiallyFilledAndCancelledOrderStatus);
            var fromFilter = Builders<MirrorsOrderEntity>.Filter.Gte(e => e.Created, from);
            var toFilter = Builders<MirrorsOrderEntity>.Filter.Lte(e => e.Created, to);
            var filter = Builders<MirrorsOrderEntity>.Filter.And(pendingFilter, fromFilter, toFilter);

            FindOptions<MirrorsOrderEntity> findOptions = new()
            {
                Sort = sortOrderDescendingDate,
            };

            var cursor = await collection.FindAsync(filter, findOptions);

            while (await cursor.MoveNextAsync())
            {
                if (cursor.Current != null)
                {
                    foreach (var order in cursor.Current)
                    {
                        foreach (var row in order.Rows)
                        {
                            if (row.Status.HasFlag(OrderStatus.PendingOrderStatus))
                            {
                                yield return row;
                            }
                        }
                    }
                }
            }

        }

        public async Task UpdateMirrorRowAsync(MirrorOrderRowEntity update)
        {
            var valResult = rowValidator.Validate(update);
            if (valResult.IsValid is false)
            {
                throw new FluentValidation.ValidationException($"Entity : {update.GetType().Name} , Validation Failed with Error Codes:{Environment.NewLine}{string.Join(Environment.NewLine, valResult.Errors.Select(e => e.ErrorCode))}");
            }

            // The Filter will Return the Document (MirrorsOrder) with the certain orderNo
            // and will remember the item (Row) with the certain Id to update it
            update.LastModified = DateTime.Now;
            var filter = Builders<MirrorsOrderEntity>.Filter.Eq(o=> o.OrderNo,update.ParentOrderNo)
                & Builders<MirrorsOrderEntity>.Filter.Eq("Rows.Id", update.Id);

            // the $ operator practically tells to the Update method to Update the Item returned by the Filter (the row with entity.Id)
            var updateDefinition = Builders<MirrorsOrderEntity>.Update.Set("GlassRows.$", update);
            var options = new UpdateOptions() { IsUpsert = false };

            var result = await collection.UpdateOneAsync(filter, updateDefinition, options);
            //Server Aknowledged the operation and found only 1 document (the Modified Count could be 0 if the items fields match)
            if (!result.IsAcknowledged)
            {
                throw new OperationNotAknowledgedException("The operation was not Aknowledged by the Server");
            }
            if (result.MatchedCount != 1)
            {
                throw new RecordNotFoundException($"The Entity to Update was not Found - Id:{update.Id} , Mirrors-Order-Number{update.ParentOrderNo} , Mirror: {update.RowItem?.ShapeType.ToString() ?? "nullItem"}--{update.RowItem?.DimensionsInformation.TotalLength}x{update.RowItem?.DimensionsInformation.TotalHeight}mm");
            }
        }

        public override Task<string> InsertEntityAsync(MirrorsOrderEntity entity)
        {
            GenerateRowAndRowItemIdsIfNotThere(entity);
            return base.InsertEntityAsync(entity);
        }
        public override Task UpdateEntityAsync(MirrorsOrderEntity entity)
        {
            GenerateRowAndRowItemIdsIfNotThere(entity);
            return base.UpdateEntityAsync(entity);
        }

        /// <summary>
        /// Assigns Object Ids to Rows and Mirrors because they are entities and Mongo will throw if they remain empty
        /// </summary>
        /// <param name="entity"></param>
        private void GenerateRowAndRowItemIdsIfNotThere(MirrorsOrderEntity entity)
        {
            foreach (var row in entity.Rows)
            {
                if (string.IsNullOrEmpty(row.Id)) row.Id = ObjectId.GenerateNewId(DateTime.Now.ToUniversalTime()).ToString();
                if (row.RowItem is not null && string.IsNullOrEmpty(row.RowItem.Id)) row.RowItem.Id = ObjectId.GenerateNewId(DateTime.Now.ToUniversalTime()).ToString();
            }
        }


        public class MirrorsOrdersRepositoryOptions : MongoDatabaseEntityRepoOptions
        {
            
        }
    }
}

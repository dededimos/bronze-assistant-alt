using DataAccessLib.NoSQLModels;
using GlassesOrdersModels.Models;
using GlassesOrdersModels.Models.SubModels;
using MongoDB.Bson;
using MongoDB.Driver;
using SpecificationFilterLibrary;
using System.Linq.Expressions;

namespace DataAccessLib.MongoDBAccess.RepoImplementations
{
    /// <summary>
    /// A Glass Order Repository Implementation for MongoDB ,  Retrieving and Manipulating Orders
    /// </summary>
    public class MongoGlassOrderRepository : IGlassOrderRepository
    {
        /// <summary>
        /// The Number of Glasses Orders per page
        /// </summary>
        private readonly int pageSize = 3;
        /// <summary>
        /// Sorts the Orders by Ascending Date (cause ObjectId has first its Date Sorted)
        /// </summary>
        private readonly SortDefinition<GlassesOrderEntity> sortOrderAscending = Builders<GlassesOrderEntity>.Sort.Ascending(order => order.Id);
        /// <summary>
        /// Sorts the Orders by Descending Date (cause ObjectId has first its Date Sorted)
        /// </summary>
        private readonly SortDefinition<GlassesOrderEntity> sortOrderDescending = Builders<GlassesOrderEntity>.Sort.Descending(order => order.Id);
        /// <summary>
        /// Filters Nothing (returns all Results)
        /// </summary>
        private readonly FilterDefinition<GlassesOrderEntity> emptyFilter = Builders<GlassesOrderEntity>.Filter.Empty;

        private readonly IMongoCollection<GlassesOrderEntity> glassOrders;

        public MongoGlassOrderRepository(IMongoDbCabinsConnection connection)
        {
            glassOrders = connection.GlassOrdersCollection;
        }

        /// <summary>
        /// Retruns the Number of Pages for a Filtered Query
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        private async Task<int> GetQueryPages(FilterDefinition<GlassesOrderEntity> queryFilter)
        {
            var totalDocuments = await glassOrders.CountDocumentsAsync(queryFilter);
            return (int)Math.Ceiling((double)totalDocuments / pageSize);
        }

        /// <summary>
        /// Gets the Number of Pages for all The Orders
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetOrdersPagesAsync()
        {
            return await GetQueryPages(emptyFilter);
        }

        /// <summary>
        /// Returns all Orders of the selected page , one by one Asynchronously
        /// </summary>
        /// <returns>The Found Orders or throws if Nothing is Found</returns>
        /// <exception cref="RecordNotFoundException">When no records are found</exception>
        /// <exception cref="Exception">When it fails for any other reason</exception>
        public async IAsyncEnumerable<GlassesOrder> GetAllOrdersAsync(int page)
        {
            //Get the Number of Pages for all the Documents
            var pages = await GetQueryPages(emptyFilter);
            if (page > pages) throw new InvalidOperationException($"There where only {pages} pages , could not retrieve page No :{page}");

            //Sort by Descending , skip the pages until reaching the desired one then Limit the Results to the Page Number
            FindOptions<GlassesOrderEntity> options = new()
            {
                Sort = sortOrderDescending,
                Skip = (page - 1) * pageSize,
                Limit = pageSize
            };

            var results = await glassOrders.FindAsync(emptyFilter, options);
            while (await results.MoveNextAsync())
            {
                if (results.Current != null)
                {
                    foreach (var order in results.Current)
                    {
                        yield return order.ToGlassesOrder();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the Latest Order Id or an empty string if there are no Orders
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetLatestOrderIdAsync()
        {
            var found = await glassOrders.Find(emptyFilter).Sort(sortOrderDescending).Limit(1).SingleAsync();
            return found?.OrderId ?? string.Empty;
        }

        /// <summary>
        /// Updates an Existing Glasses Order , without changing its ObjectId
        /// Throws Exceptions if it fails or does not find an item to update
        /// </summary>
        /// <param name="orderUpdate">The Order to Update</param>
        /// <returns></returns>
        /// <exception cref="RecordNotFoundException">When the Record is Not Found</exception>
        /// <exception cref="Exception">When the operation Fails for any Other Reason</exception>
        public async Task UpdateOrderAsync(GlassesOrder orderUpdate)
        {
            var objectId = await FindOrderObjectIdAsync(orderUpdate.OrderId);
            //Construct the new Entity
            GlassesOrderEntity entity = new(orderUpdate)
            {
                Id = objectId,
                LastModified = DateTime.Now,
            };

            //We do not care for the Object Ids of The Glass and Cabin Rows 
            //If the whole order is replaced they may very well be changed as long as the OrderObjectId is not changed
            //and as long as all the fields of those rows where copies of before wherever they have not been changed
            foreach (var item in entity.GlassRows)
            {
                item.Id = item.Id != default ? item.Id : ObjectId.GenerateNewId(DateTime.Now.Date.ToUniversalTime());
                item.LastModified = DateTime.Now;
                item.OrderId = entity.OrderId;
            }
            foreach (var item in entity.CabinRows.Where(r => r.OrderId != entity.OrderId))
            {
                item.Id = item.Id != default ? item.Id : ObjectId.GenerateNewId(DateTime.Now.Date.ToUniversalTime());
                item.OrderId = entity.OrderId;
                item.LastModified = DateTime.Now;
            }

            //Create the Filter to Find the Item in the Database
            var filter = Builders<GlassesOrderEntity>.Filter.Eq("Id", entity.Id);

            //Replace the found in the database
            //Do not Insert a new one if the part is not there
            FindOneAndReplaceOptions<GlassesOrderEntity, GlassesOrderEntity> options =
                new() { IsUpsert = false, ReturnDocument = ReturnDocument.Before };

            GlassesOrderEntity? result = await glassOrders.FindOneAndReplaceAsync(filter, entity, options) 
                ?? throw new RecordNotFoundException($"Order with No{entity.OrderId} was not Found and did not Update");
        }

        /// <summary>
        /// Update a Specific Glass Row in an Order
        /// </summary>
        /// <param name="update">The Row to Update (Needs a Row Id to Update)</param>
        /// <returns></returns>
        /// <exception cref="Exception">When the RowId is not Set</exception>
        /// <exception cref="OperationNotAknowledgedException">When the Server does not Aknowledge the Operation</exception>
        /// <exception cref="RecordNotFoundException">When no Records are found for the selected Criteria</exception>
        public async Task UpdateGlassRowAsync(GlassOrderRow update)
        {
            if (!GlassesOrder.OrderIdRegex.IsMatch(update.OrderId))
            {
                throw new Exception("Invalid OrderId");
            }

            var entity = GlassOrderRowEntity.CreateEntity(update);
            entity.LastModified = DateTime.Now;
            if (entity.Id == default) throw new Exception($"{nameof(GlassOrderRow)}-Update , Passed an Invalid or Null ObjectId to its Entity form");

            // The Filter will Return the Document (Glass Order) with the certain orderId
            // and will remember the item (GlassRow) with the certain Id to update it
            var filter = Builders<GlassesOrderEntity>.Filter.Eq(o => o.OrderId, update.OrderId)
                & Builders<GlassesOrderEntity>.Filter.Eq("GlassRows.Id", entity.Id);

            // the $ operator practically tells to the Update method to Update the Item returned by the Filter (the row with entity.Id)
            var updateDefinition = Builders<GlassesOrderEntity>.Update.Set("GlassRows.$", entity);
            var options = new UpdateOptions() { IsUpsert = false };

            var result = await glassOrders.UpdateOneAsync(filter, updateDefinition, options);

            //Server Aknowledged the operation and found only 1 document (the Modified Count could be 0 if the items fields match)
            if (!result.IsAcknowledged)
            {
                throw new OperationNotAknowledgedException("The operation was not Aknowledged by the Server");
            }
            if (result.MatchedCount != 1)
            {
                throw new RecordNotFoundException($"The Entity to Update was not Found - Id:{entity.Id} , Glasses-Order-Number{entity.OrderId} , Glass: {entity.OrderedGlass.Draw}--{entity.OrderedGlass.Length}x{entity.OrderedGlass.Height}mm");
            }
        }

        /// <summary>
        /// Inserts a New Glass Order into the Database , Will throw if the Operation Fails
        /// </summary>
        /// <param name="newOrder">The New Order to Insert</param>
        /// <exception cref="Exception">When it fails</exception>
        public async Task InsertNewOrderAsync(GlassesOrder newOrder)
        {
            var currentDate = DateTime.Now;
            //The Unique OrderId key takes care of not duplicated OrderIds no need to check for it
            GlassesOrderEntity entity = new(newOrder)
            {
                LastModified = currentDate,
            };

            // Set ObjectIds for the nested Objects Otherwise it does not take them ...
            // Set the OrderIds also
            foreach (var item in entity.GlassRows)
            {
                item.Id = item.Id != default ? item.Id : ObjectId.GenerateNewId(currentDate.Date.ToUniversalTime());
                item.LastModified = currentDate;
                item.OrderId = entity.OrderId;
            }
            foreach (var item in entity.CabinRows)
            {
                item.Id = item.Id != default ? item.Id : ObjectId.GenerateNewId(currentDate.Date.ToUniversalTime());
                item.LastModified = currentDate;
                item.OrderId = entity.OrderId;
            }

            //Insert the new Order , if the insertion fails this will throw an Exception
            //So there is no need to check if it succeeded by searching the document again.
            await glassOrders.InsertOneAsync(entity);
        }

        /// <summary>
        /// USE ONLY TO INSERT OLD DATA - Inserts a New Glass Order into the Database , Will throw if the Operation Fails
        /// </summary>
        /// <param name="newOrder">The New Order to Insert</param>
        /// <exception cref="Exception">When it fails</exception>
        public async Task InsertNewOrderAsync(GlassesOrderEntity newOrder)
        {
            // Set ObjectIds for the nested Objects Otherwise it does not take them ...
            // Set the OrderIds also
            // All the Glass and Cabin Rows take the Date of the New Order inside their object Id.
            foreach (var item in newOrder.GlassRows)
            {
                item.Id = item.Id != default ? item.Id : ObjectId.GenerateNewId(newOrder.Id.CreationTime);
                item.LastModified = newOrder.Created;
                item.OrderId = newOrder.OrderId;
            }
            foreach (var item in newOrder.CabinRows)
            {
                item.Id = item.Id != default ? item.Id : ObjectId.GenerateNewId(newOrder.Id.CreationTime);
                item.LastModified = newOrder.Created;
                item.OrderId = newOrder.OrderId;
            }

            //Insert the new Order , if the insertion fails this will throw an Exception
            //So there is no need to check if it succeeded by searching the document again.
            await glassOrders.InsertOneAsync(newOrder);
        }

        /// <summary>
        /// Returns the Object Id of a certain Order (Searching by OrderId)
        /// </summary>
        /// <param name="orderId">The OrderId as is in the System</param>
        /// <returns>The Object Id or throws</returns>
        /// <exception cref="RecordNotFoundException">When the order Id is not Found</exception>
        private async Task<ObjectId> FindOrderObjectIdAsync(string orderId)
        {
            //Find the Entity with an Order Id as the orderId we are searching for
            var filter = Builders<GlassesOrderEntity>.Filter.Eq("OrderId", orderId);

            //Define to what to Extract from the Results (Project the item into something else). Here we only need the ObjectId of the Document
            var projection = Builders<GlassesOrderEntity>.Projection.Expression(o => o.Id);
            //Pass the Projection into the Find Options
            var options = new FindOptions<GlassesOrderEntity, ObjectId>() { Projection = projection };

            IAsyncCursor<ObjectId> result = await glassOrders.FindAsync(filter, options);
            ObjectId retrivedItem = result.SingleOrDefault();

            if (retrivedItem == default) throw new RecordNotFoundException($"Failed to Retrieve {nameof(ObjectId)} , order with Id:{orderId} was not Found");
            return retrivedItem;
        }

        /// <summary>
        /// Deletes an Order from the Database
        /// </summary>
        /// <param name="orderId">The OrderID of the order to Delete</param>
        /// <returns></returns>
        /// <exception cref="RecordNotFoundException">When the record to Delete is not Found</exception>
        /// <exception cref="Exception">When it fails for any other Reason</exception>
        public async Task DeleteOrderAsync(string orderId)
        {
            var filter = Builders<GlassesOrderEntity>.Filter.Eq("OrderId", orderId);
            var result = await glassOrders.DeleteOneAsync(filter);
            if (result.DeletedCount != 1)
            {
                throw new RecordNotFoundException($"Could not Delete , GlassOrder with No:{orderId} was not Found");
            }
        }

        /// <summary>
        /// Returns All the Pending Glasses Asyncronously in batches
        /// </summary>
        /// <param name="from">From which date</param>
        /// <param name="to">Up to Which Date</param>
        /// <returns>Async batches of GlassOrderRows</returns>
        /// <exception cref="RecordNotFoundException">When there are no pending orders</exception>
        public async IAsyncEnumerable<IEnumerable<GlassOrderRow>> GetPendingGlassesAsync(DateTime from, DateTime to)
        {
            from = from.ToUniversalTime().Date;
            to = to.ToUniversalTime().Date;

            var fromFilter = Builders<GlassesOrderEntity>.Filter.Gte(o => o.Created, from);
            var toFilter = Builders<GlassesOrderEntity>.Filter.Lte(o => o.Created, to);
            var filter = Builders<GlassesOrderEntity>.Filter.And(fromFilter, toFilter);

            FindOptions<GlassesOrderEntity> options = new()
            {
                Sort = sortOrderDescending
            };
            var cursor = await glassOrders.FindAsync(filter, options);

            //Store if any documents where yielded (if cursor is checked the cursor will get disposed)
            var cursorYieldedDocuments = false;

            while (await cursor.MoveNextAsync())
            {
                if (cursor.Current != null)
                {
                    foreach (var order in cursor.Current)
                    {
                        //Transform the Glass Rows Entities to GlassRows Only where the filters apply (the document contains all its glasses)
                        var glassRows = order.GlassRows
                            .Where(gr => gr.FilledQuantity < gr.Quantity)
                            .Select(g => g.ToGlassOrderRow());
                        var cabinKeys = glassRows.Select(r => r.CabinRowKey).Distinct();

                        //Keep the Cabins only that have matching keys with the Glasses then Tranform those Cabins To Rows
                        var cabinRows = order.CabinRows.Where(cr => cabinKeys.Any(k => k == cr.CabinKey))
                            .Select(c => c.ToCabinOrderRow(glassRows.Where(gr => gr.CabinRowKey == c.CabinKey)));

                        var yieldedRows = glassRows.Select(gr =>
                        {
                            gr.ParentCabinRow = cabinRows.FirstOrDefault(cr => cr.CabinKey == gr.CabinRowKey);
                            return gr;
                        });

                        //Yield return those Rows and Assign their Parent Cabins (Must be done like this otherwise the Glass rows return null (lose reference))
                        if (yieldedRows.Any())
                        {
                            cursorYieldedDocuments = true;
                            yield return yieldedRows;
                        }
                    }
                }
            }

            if (!cursorYieldedDocuments) throw new RecordNotFoundException($"No Records Found with Pending Glasses");
        }

        /// <summary>
        /// Returns the Glass rows found Asynchronously ,returns batches of glass rows per orderDocument
        /// </summary>
        /// <param name="glassSpec">The Speccification/Filtering for the Glass Rows</param>
        /// <param name="page">Retrieves the Page requested</param>
        /// <returns></returns>
        public async IAsyncEnumerable<IEnumerable<GlassOrderRow>> QueryGlassesAsync(DateTime from , DateTime to , Specification<GlassOrderRowEntity> glassSpec , int page)
        {
            //Store the Spec Expression so it can be used inside the Filters (otherwise serverside cannot run the custom method Get Expression)
            Expression<Func<GlassOrderRowEntity, bool>> glassExpression = glassSpec.GetExpression();
            from = from.ToUniversalTime().Date;
            to = to.ToUniversalTime().Date;
            //Produce the Filter for Both Cabin and Glass , so to get Documents that will only contain glasses and Cabins of the SpecifiedType
            var glassFilter = Builders<GlassesOrderEntity>.Filter.ElemMatch(o => o.GlassRows, glassExpression);
            var fromFilter = Builders<GlassesOrderEntity>.Filter.Gte(o=> o.Created,from);
            var toFilter = Builders<GlassesOrderEntity>.Filter.Lte(o => o.Created ,to);

            var filter = Builders<GlassesOrderEntity>.Filter.And(fromFilter, toFilter,glassFilter);

            //Check if the Requested Page is Valid else Throw
            var pages = await GetQueryPages(filter);
            if (page > pages) throw new InvalidOperationException($"There where only {pages} pages , could not retrieve page No :{page}");
            
            FindOptions<GlassesOrderEntity> options = new()
            {
                Sort = sortOrderDescending,
                Skip = (page - 1) * pageSize, //Skip documents of previous pages
                Limit = pageSize //Limit the Results to the PageSize
            };

            //Get all the Documents that contain at least one glass or one Cabin of the Above expressions (the ones that where inserted in the Filters)
            var cursor = await glassOrders.FindAsync(filter, options);

            //Store if any documents where yielded (if cursor is checked the cursor will get disposed)
            var cursorYieldedDocuments = false;
            while (await cursor.MoveNextAsync())
            {
                if (cursor.Current != null)
                {
                    foreach (var order in cursor.Current)
                    {
                        //Transform the Glass Rows Entities to GlassRows Only where the filters apply (the document contains all its glasses)
                        var glassRows = order.GlassRows
                            .Where(gr => glassSpec.IsSatisfiedBy(gr))
                            .Select(g => g.ToGlassOrderRow());
                        var cabinKeys = glassRows.Select(r => r.CabinRowKey).Distinct();

                        //Keep the Cabins only that have matching keys with the Glasses then Tranform those Cabins To Rows
                        var cabinRows = order.CabinRows.Where(cr => cabinKeys.Any(k => k == cr.CabinKey))
                            .Select(c => c.ToCabinOrderRow(glassRows.Where(gr => gr.CabinRowKey == c.CabinKey)));

                        var yieldedRows = glassRows.Select(gr =>
                        {
                            gr.ParentCabinRow = cabinRows.FirstOrDefault(cr => cr.CabinKey == gr.CabinRowKey);
                            return gr;
                        });

                        //Yield return those Rows and Assign their Parent Cabins (Must be done like this otherwise the Glass rows return null (lose reference))
                        if (yieldedRows.Any()) 
                        {
                            cursorYieldedDocuments = true;
                            yield return yieldedRows;
                        }
                    }
                }
            }

            if(!cursorYieldedDocuments) throw new RecordNotFoundException($"No Records Found for the Selected Filters{Environment.NewLine}{Environment.NewLine}{glassExpression}");
        }

        /// <summary>
        /// Gets the Number of Pages for a Glass Query
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetGlassQueryPagesAsync(DateTime from, DateTime to, Specification<GlassOrderRowEntity> glassSpec)
        {
            //Store the Spec Expression so it can be used inside the Filters (otherwise serverside cannot run the custom method Get Expression)
            var glassExpression = glassSpec.GetExpression();
            from = from.ToUniversalTime().Date;
            to = to.ToUniversalTime().Date;
            //Produce the Filter for Both Cabin and Glass , so to get Documents that will only contain glasses and Cabins of the SpecifiedType
            var glassFilter = Builders<GlassesOrderEntity>.Filter.ElemMatch(o => o.GlassRows, glassExpression);
            var fromFilter = Builders<GlassesOrderEntity>.Filter.Gte(o => o.Created, from);
            var toFilter = Builders<GlassesOrderEntity>.Filter.Lte(o => o.Created, to);
            var filter = Builders<GlassesOrderEntity>.Filter.And(fromFilter, toFilter, glassFilter);
            return await GetQueryPages(filter);
        }

        /// <summary>
        /// Returns the Cabin rows found Asynchronously ,returns batches of cabin rows per orderDocument
        /// </summary>
        /// <param name="cabinSpec">The Speccification/Filtering for the Cabin Rows</param>
        /// <param name="page">Retrieves the Page requested</param>
        /// <returns></returns>
        public async IAsyncEnumerable<IEnumerable<CabinOrderRow>> QueryCabinsAsync(DateTime from , DateTime to ,Specification<CabinRowEntity> cabinSpec,int page)
        {
            //Store the Spec Expression so it can be used inside the Filters (otherwise serverside cannot run the custom method Get Expression)
            var cabinExpression = cabinSpec.GetExpression();
            from = from.ToUniversalTime().Date;
            to = to.ToUniversalTime().Date;
            //Produce the Filter for Both Cabin and Glass , so to get Documents that will only contain glasses and Cabins of the SpecifiedType
            var cabinFilter = Builders<GlassesOrderEntity>.Filter.ElemMatch(o => o.CabinRows, cabinExpression);
            var fromFilter = Builders<GlassesOrderEntity>.Filter.Gte(o => o.Created, from);
            var toFilter = Builders<GlassesOrderEntity>.Filter.Lte(o => o.Created, to);
            var filter = Builders<GlassesOrderEntity>.Filter.And(fromFilter, toFilter, cabinFilter);
            //Check if the Requested Page is Valid else Throw
            var pages = await GetQueryPages(filter);
            if (page > pages) throw new InvalidOperationException($"There where only {pages} pages , could not retrieve page No :{page}");

            FindOptions<GlassesOrderEntity> options = new()
            {
                Sort = sortOrderDescending,
                Skip = (page - 1) * pageSize, //Skip documents of previous pages
                Limit = pageSize //Limit the Results to the PageSize
            };

            //Get all the Documents that contain at least one glass or one Cabin of the Above expressions (the ones that where inserted in the Filters)
            var cursor = await glassOrders.FindAsync(filter, options);

            //Store if any documents where yielded (if cursor is checked the cursor will get disposed)
            var cursorYieldedDocuments = false;
            while (await cursor.MoveNextAsync())
            {
                if (cursor.Current != null)
                {
                    foreach (var order in cursor.Current)
                    {
                        //Transform the Found Cabins that Satisfy the Filter and return them
                        var cabinRows = order.CabinRows.Where(cr=> cabinSpec.IsSatisfiedBy(cr))
                            .Select(c => c.ToCabinOrderRow(order.GlassRows
                                            .Where(gre => gre.CabinRowKey == c.CabinKey)
                                            .Select(gre=> gre.ToGlassOrderRow())));
                        if (cabinRows.Any())
                        {
                            cursorYieldedDocuments = true;
                            yield return cabinRows;
                        }
                    }
                }
            }

            if (!cursorYieldedDocuments) throw new RecordNotFoundException($"No Records Found for the Selected Filters{Environment.NewLine}{Environment.NewLine}{cabinExpression}");
        }
        /// <summary>
        /// Gets the Number of Pages for a Cabin Query
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetCabinQueryPagesAsync(DateTime from , DateTime to, Specification<CabinRowEntity> cabinSpec)
        {
            //Store the Spec Expression so it can be used inside the Filters (otherwise serverside cannot run the custom method Get Expression)
            Expression<Func<CabinRowEntity, bool>> cabinExpression = cabinSpec.GetExpression();
            from = from.ToUniversalTime().Date;
            to = to.ToUniversalTime().Date;
            //Produce the Filter for Both Cabin and Glass , so to get Documents that will only contain glasses and Cabins of the SpecifiedType
            var cabinFilter = Builders<GlassesOrderEntity>.Filter.ElemMatch(o => o.CabinRows, cabinExpression);
            var fromFilter = Builders<GlassesOrderEntity>.Filter.Gte(o => o.Created, from);
            var toFilter = Builders<GlassesOrderEntity>.Filter.Lte(o => o.Created, to);
            var filter = Builders<GlassesOrderEntity>.Filter.And(fromFilter, toFilter, cabinFilter);
            return await GetQueryPages(filter);
        }

        /// <summary>
        /// Returns all the Found orders in a smaller Format
        /// </summary>
        /// <param name="maxResults">The maximum Number of Results to retrieve</param>
        /// <param name="ignoreCache">If it should ignore the cache and reload all from DB</param>
        /// <returns></returns>
        /// <exception cref="RecordNotFoundException"></exception>
        public async Task<IEnumerable<GlassesOrderSmall>> GetOrdersSmallAsync(int maxResults)
        {
            //The Projection expression does not allow to convert ObjectId to Date so we have
            //to create a new object to get what we need and convert afterwards in memory to the desired object with Date
            var projection = Builders<GlassesOrderEntity>.Projection.Expression(o =>
            new GlassesOrderSmallProjection()
            {
                CabinsCount = o.CabinsCount,
                GlassesCount = o.GlassesCount,
                Notes = o.Notes,
                OrderId = o.OrderId,
                PA0Count = o.PA0Count,
                Status = o.Status,
                Id = o.Id
            });

            var result = await glassOrders.Find(emptyFilter)
                                          .Sort(sortOrderAscending) //to effectivly skip older ones
                                          .Sort(sortOrderDescending) //to effectivly limit older ones and keep newer ones
                                          .Limit(maxResults)
                                          .Project(projection)
                                          .ToListAsync();

            //Return the Retrieved along with cached
            var finalResult = result.Select(proj => proj.ToSmallGlassesOrder());

            return finalResult.Any() ? finalResult : throw new RecordNotFoundException("No Orders Found");
        }
    }

    public class GlassesOrderSmallProjection
    {
        public string OrderId { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public OrderStatus Status { get; set; } = OrderStatus.Undefined;
        public int GlassesCount { get; set; }
        public int CabinsCount { get; set; }
        public int PA0Count { get; set; }

        //To retrieve the Date Only because new driver cannot read the property
        public ObjectId Id { get; set; }

        public GlassesOrderSmall ToSmallGlassesOrder()
        {
            return new GlassesOrderSmall()
            {
                OrderId = OrderId,
                Notes = Notes,
                Status = Status,
                GlassesCount = GlassesCount,
                CabinsCount = CabinsCount,
                PA0Count = PA0Count,
                Created = Id.CreationTime.ToLocalTime()
            };
        }
    }
}

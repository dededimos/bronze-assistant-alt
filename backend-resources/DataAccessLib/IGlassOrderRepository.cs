using CommonHelpers;
using GlassesOrdersModels.Models;
using MongoDB.Bson;
using ShowerEnclosuresModelsLibrary.Enums.ShowerDrawEnums;
using DataAccessLib.NoSQLModels;
using GlassesOrdersModels.Models.SubModels;
using SpecificationFilterLibrary;

namespace DataAccessLib;

public interface IGlassOrderRepository
{
    /// <summary>
    /// Deletes an Order from the Database
    /// </summary>
    /// <param name="orderId">The OrderID of the order to Delete</param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException">When the record to Delete is not Found</exception>
    /// <exception cref="Exception">When it fails for any other Reason</exception>
    Task DeleteOrderAsync(string orderId);
    /// <summary>
    /// Returns the Number of pages of All The Orders
    /// </summary>
    /// <returns></returns>
    Task<int> GetOrdersPagesAsync();
    /// <summary>
    /// Returns all Orders of the Selected Page , One By One Asynchronously.
    /// </summary>
    /// <param name="page">The Page of the Results to Return</param>
    /// <returns>The Found Orders or throws if Nothing is Found</returns>
    /// <exception cref="RecordNotFoundException">When no records are found</exception>
    /// <exception cref="Exception">When it fails for any other reason</exception>
    IAsyncEnumerable<GlassesOrder> GetAllOrdersAsync(int page);
    /// <summary>
    /// Returns the Latest Order-Id
    /// </summary>
    /// <returns></returns>
    Task<string> GetLatestOrderIdAsync();
    /// <summary>
    /// Inserts a New Glass Order into the Database , Will throw if the Operation Fails
    /// </summary>
    /// <param name="newOrder">The New Order to Insert</param>
    /// <exception cref="Exception">When it fails</exception>
    Task InsertNewOrderAsync(GlassesOrder newOrder);
    /// <summary>
    /// USE ONLY TO INSERT OLD DATA - Inserts a New Glass Order into the Database , Will throw if the Operation Fails
    /// </summary>
    /// <param name="newOrder">The New Order ENTITY to Insert</param>
    /// <exception cref="Exception">When it fails</exception>
    Task InsertNewOrderAsync(GlassesOrderEntity newOrder);
    /// <summary>
    /// Updates an Existing Glasses Order , without changing its ObjectId
    /// Throws Exceptions if it fails or does not find an item to update
    /// </summary>
    /// <param name="orderUpdate">The Order to Update</param>
    /// <returns></returns>
    /// <exception cref="RecordNotFoundException">When the Record is Not Found</exception>
    /// <exception cref="Exception">When the operation Fails for any Other Reason</exception>
    Task UpdateOrderAsync(GlassesOrder orderUpdate);
    /// <summary>
    /// Update a Specific Glass Row in an Order
    /// </summary>
    /// <param name="update">The Row to Update (Needs a Row Id to Update)</param>
    /// <returns></returns>
    /// <exception cref="Exception">When the RowId is not Set</exception>
    /// <exception cref="OperationNotAknowledgedException">When the Server does not Aknowledge the Operation</exception>
    /// <exception cref="RecordNotFoundException">When no Records are found for the selected Criteria</exception>
    Task UpdateGlassRowAsync(GlassOrderRow update);

    /// <summary>
    /// Returns the Glass rows found Asynchronously ,returns batches of glass rows per orderDocument
    /// </summary>
    /// <param name="from">Query from this Date Onwards</param>
    /// <param name="to">Query up to This Date</param>
    /// <param name="glassSpec">The Speccification/Filtering for the Glass Rows</param>
    /// <param name="page">Retrieves the Page requested</param>
    /// <returns>Yield returns an Enumerable of Glass Rows for each retrieved Order Document</returns>
    IAsyncEnumerable<IEnumerable<GlassOrderRow>> QueryGlassesAsync(DateTime from , DateTime to ,Specification<GlassOrderRowEntity> glassSpec , int page);
    /// <summary>
    /// Returns All the Pending Glasses Asyncronously in batches
    /// </summary>
    /// <param name="from">From which date</param>
    /// <param name="to">Up to Which Date</param>
    /// <returns>Async batches of GlassOrderRows</returns>
    /// <exception cref="RecordNotFoundException">When there are no pending orders</exception>
    IAsyncEnumerable<IEnumerable<GlassOrderRow>> GetPendingGlassesAsync(DateTime from, DateTime to);

    /// <summary>
    /// Gets the Number of Pages for a Glass Query
    /// </summary>
    /// <param name="from">Query from this Date Onwards</param>
    /// <param name="to">Query up to This Date</param>
    /// <param name="glassSpec">The Specification Filter if any</param>
    /// <returns>The number of Pages for the Selected Filters</returns>
    Task<int> GetGlassQueryPagesAsync(DateTime from , DateTime to ,Specification<GlassOrderRowEntity> glassSpec);

    /// <summary>
    /// Returns the Cabin rows found Asynchronously ,returns batches of cabin rows per orderDocument
    /// </summary>
    /// <param name="from">Query from this Date Onwards</param>
    /// <param name="to">Query up to this Date</param>
    /// <param name="cabinSpec">The Speccification/Filtering for the Cabin Rows</param>
    /// <param name="page">Retrieves the Page requested</param>
    /// <returns></returns>
    IAsyncEnumerable<IEnumerable<CabinOrderRow>> QueryCabinsAsync(DateTime from , DateTime to , Specification<CabinRowEntity> cabinSpec, int page);
    /// <summary>
    /// Gets the Number of Pages for a Cabin Query
    /// </summary>
    /// <param name="from">Query from this Date Onwards</param>
    /// <param name="to">Query up to This Date</param>
    /// <param name="cabinSpec">The Specification Filter if any</param>
    /// <returns>The number of Pages for the Selected Filters</returns>
    Task<int> GetCabinQueryPagesAsync(DateTime from , DateTime to, Specification<CabinRowEntity> cabinSpec);

    /// <summary>
    /// Gets all the GlassOrders in a small Format
    /// </summary>
    /// <param name="maxResults">The max result to be retrieved</param>
    /// <returns>The retrieved Orders</returns>
    Task<IEnumerable<GlassesOrderSmall>> GetOrdersSmallAsync(int maxResults);
}

using GlassesOrdersModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLib
{
    /// <summary>
    /// A Repository for the Stocked Glasses
    /// </summary>
    public interface IGlassesStockRepository
    {
        /// <summary>
        /// Inserts a New Glass to the StockList
        /// </summary>
        /// <param name="glassToAdd">The Glass to Add</param>
        /// <returns>The id of the Inserted as a string</returns>
        public Task<string> InsertNewGlassAsync(StockedGlassRow glassToAdd);
        /// <summary>
        /// Removes a Glass from the StockList
        /// </summary>
        /// <param name="glassId">The id of the Glass to Remove</param>
        /// <returns></returns>
        public Task RemoveFromStock(string glassId);
        /// <summary>
        /// Updates a Glass already in the Stock List
        /// </summary>
        /// <param name="updatedGlass"></param>
        /// <returns></returns>
        public Task UpdateStockedGlass(StockedGlassRow updatedGlass);
        /// <summary>
        /// Returns all the Stock
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<StockedGlassRow>> GetAllStock();
    }
}

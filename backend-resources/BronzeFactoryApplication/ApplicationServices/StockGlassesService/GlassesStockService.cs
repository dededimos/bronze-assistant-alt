
using MongoDB.Driver;
using System.Collections.ObjectModel;

namespace BronzeFactoryApplication.ApplicationServices.StockGlassesService
{
    public class GlassesStockService
    {
        private readonly IGlassesStockRepository stockRepo;

        public bool HasPopulated { get; set; }

        public ObservableCollection<StockedGlassRow> StockList { get; set; } = new();

        public GlassesStockService(IGlassesStockRepository stockRepo)
        {
            this.stockRepo = stockRepo;
        }

        /// <summary>
        /// Adds a Glass to The Stock , Inserts a New Glass or adds to the Quantity of a previous One
        /// </summary>
        /// <param name="glassRow">The Stocked Glass Row to Add</param>
        /// <returns></returns>
        public async Task AddGlassToStock(StockedGlassRow glassRow, int quantityToAdd)
        {
            var similarGlassInStock = StockList.FirstOrDefault(g => g.Glass.Equals(glassRow.Glass) && g.Notes == glassRow.Notes);
            int previousQty = similarGlassInStock?.Quantity ?? 0;
            string previousNotes = similarGlassInStock?.Notes ?? "";
            try
            {
                //If a similar glass was found just update its quantity
                if (similarGlassInStock is not null)
                {
                    await stockRepo.UpdateStockedGlass(similarGlassInStock);
                    similarGlassInStock.Quantity += quantityToAdd;
                    similarGlassInStock.Notes += string.IsNullOrEmpty(similarGlassInStock.Notes) ? glassRow.Notes : $"{Environment.NewLine}{glassRow.Notes}";
                }
                //else insert the new Glass
                else
                {
                    glassRow.Quantity = quantityToAdd;
                    var insertedId = await stockRepo.InsertNewGlassAsync(glassRow);
                    glassRow.RowId = insertedId;
                    StockList.Add(glassRow);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Removes a Glass from the Stock either completely or by subtracting only quantity
        /// </summary>
        /// <param name="rowToRemove">The Glass Row to Remove</param>
        /// <returns></returns>
        public async Task RemoveFromStock(StockedGlassRow rowToRemove, int quantityToRemove)
        {
            try
            {
                if (quantityToRemove < rowToRemove.Quantity)
                {
                    rowToRemove.Quantity -= quantityToRemove;
                    await stockRepo.UpdateStockedGlass(rowToRemove);
                }
                else
                {
                    await stockRepo.RemoveFromStock(rowToRemove.RowId);
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        var localRowToRemove = StockList.FirstOrDefault(r => r.RowId == rowToRemove.RowId) 
                            ?? throw new Exception($"Row was not Found on the Locals List... {rowToRemove.RowId}");

                        StockList.Remove(localRowToRemove);
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Populates the Stock List
        /// </summary>
        /// <returns></returns>
        public async Task PopulateStockList()
        {
            try
            {
                //If the GetAllStock Fails its fine will throw and will not execute the rest of the code
                //Otherwise it will clear and execute which is fine
                var stock = await stockRepo.GetAllStock();
                StockList.Clear();
                foreach (var item in stock)
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        StockList.Add(item);
                    });
                }
                HasPopulated = true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Initilizes the Service
        /// </summary>
        /// <returns></returns>
        public async Task InitilizeService()
        {
            await PopulateStockList();
        }
    }
}

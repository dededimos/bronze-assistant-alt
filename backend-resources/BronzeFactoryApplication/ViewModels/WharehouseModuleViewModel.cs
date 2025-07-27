using BronzeFactoryApplication.ViewModels.HelperViewModels;
using AccessoriesRepoMongoDB.Repositories;
using GalaxyStockHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccessoriesRepoMongoDB.Entities;
using MongoDbCommonLibrary.CommonEntities;

namespace BronzeFactoryApplication.ViewModels
{
    public partial class WharehouseModuleViewModel : BaseViewModel
    {
        public WharehouseModuleViewModel(GalaxyStockService stockService,
            OperationProgressViewModel progressVm,
            AccessoriesEntitiesModuleViewModel accessoriesModule,
            IAccessoryEntitiesRepository accRepo,
            ItemStockMongoRepository stockRepo)
        {
            this.stockService = stockService;
            this.progressVm = progressVm;
            this.accessoriesModule = accessoriesModule;
            this.accRepo = accRepo;
            this.stockRepo = stockRepo;
        }

        private GalaxyStockService stockService;
        private readonly OperationProgressViewModel progressVm;
        private readonly AccessoriesEntitiesModuleViewModel accessoriesModule;
        private readonly IAccessoryEntitiesRepository accRepo;
        private readonly ItemStockMongoRepository stockRepo;
        private Dictionary<string, WharehouseItem> stockInfoDictionary = [];

        public ObservableCollection<WharehouseItem> Items { get; } = [];

        [RelayCommand]
        private async Task GetStock()
        {
            IsBusy = true;
            try
            {
                progressVm.SetNewOperation("Loading Stock ...", 1);
                stockInfoDictionary = await stockService.GetStockInfoDictionaryAsync();

                Items.Clear();
                progressVm.SetNewOperation($"Adding Items 0/{stockInfoDictionary.Count}", stockInfoDictionary.Count);
                int count = 0;
                foreach (var item in stockInfoDictionary.Values)
                {
                    Items.Add(item);
                    count++;
                    progressVm.ReduceCount($"Adding Items {progressVm.RemainingCount}/{progressVm.CountOfItems}");
                    if (count % 100 == 0)
                    {
                        await Task.Delay(4); // Yield to UI thread
                    }
                }
            }
            catch (Exception ex) { MessageService.LogAndDisplayException(ex); progressVm.MarkAllOperationsFinished(); }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private void AnyCodeHasGreek()
        {
            var codesWithGreek = stockInfoDictionary.Keys.Where(k => k.Any(c => c.IsCharachterGreek()));
            if (codesWithGreek.Any())
            {
                var message = $"The following codes have Greek letters: {Environment.NewLine}{Environment.NewLine}{string.Join(Environment.NewLine, codesWithGreek)}";
                MessageService.Warning(message, "Codes With Greek in the Code");
            }
            else
            {
                MessageService.Info("No codes with Greek letters found.", "Codes Are Clean");
            }
        }

        [RelayCommand]
        private async Task TransferAccessoriesStockInfo()
        {
            if (stockInfoDictionary.Count == 0)
            {
                await GetStock();
            }
            if (accessoriesModule.IsCacheDirty || accessoriesModule.BathAccessories.Count == 0)
            {
                accessoriesModule.SelectedEntity = null; //
                accessoriesModule.RetrieveAccessoriesCommand.Execute(null);
            }

            //finish Traits
            var finishTraits = accRepo.Traits.Cache.Where(t => t.TraitType == BathAccessoriesModelsLibrary.TypeOfTrait.FinishTrait);
            
            List<ItemStockEntity> stockEntities = [];
            //Create ItemStockEntities
            foreach (var acc in accRepo.Cache)
            {
                //Get all the code combinations for each accessory
                var codes = BathAccessoryEntity.GetAllAvailableSpecificCodes(acc, finishTraits);

                //For each code add a stock entry
                foreach (var code in codes)
                {
                    ItemStockEntity entity = new() { Code = code };
                    if (stockInfoDictionary.TryGetValue(code, out var stockInfo))
                    {
                        entity.Quantity = stockInfo.TotalStockWithoutRedunduncies;
                    }
                    stockEntities.Add(entity);
                }
            }

            //Save to DB (Clear Old stock and assign new)
            try
            {
                progressVm.SetNewOperation("Deleting Old Stock Entries...", 1);
                await stockRepo.DeleteAllAsync();
                progressVm.SetNewOperation("Inserting new Entries...", 1);
                var result = await stockRepo.InsertBulkAsync(stockEntities);

                if (result.HasFailures)
                {
                    MessageService.Info(
                    $"The Stock was transferred to the database with Errors {Environment.NewLine}{Environment.NewLine}" +
                    $"Failures : {result.FailureCount}{Environment.NewLine}" +
                    $"Failed Items :{Environment.NewLine}{string.Join(" , ", result.FailedItems.Select(i => i.Item.Code))}" +
                    $"{Environment.NewLine}{Environment.NewLine}If there where failed codes please check log for more information"
                    , "Operation Finished");
                }
                else { MessageService.Info("Operation completed Successfully", "Operation Finished"); }
            }
            catch (Exception ex)
            {
                MessageService.Error("The Operation stopped due to an error , please check logs for more details , the stock might not be correctly saved", "Operation failed");
                MessageService.LogAndDisplayException(ex);
            }
            finally
            {
                IsBusy = false;
                progressVm.MarkAllOperationsFinished();
            }
        }
    }
}

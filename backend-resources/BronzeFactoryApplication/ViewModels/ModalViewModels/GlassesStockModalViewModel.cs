using BronzeFactoryApplication.ApplicationServices.ExcelXlsService;
using BronzeFactoryApplication.ApplicationServices.StockGlassesService;
using BronzeFactoryApplication.ViewModels.ComponentsUCViewModels.DrawsViewModels;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class GlassesStockModalViewModel : ModalViewModel
    {
        private readonly GlassesStockService stockService;
        private readonly ValidatorGlass validator = new();

        public ObservableCollection<StockedGlassViewModel> StockList { get; } = new();

        public GlassDrawViewModel GlassDraw { get;} = new();
        [ObservableProperty]
        private bool isDrawToList;

        [ObservableProperty]
        private GlassViewModel glassRowToAdd = new() { Draw= GlassDrawEnum.DrawF,Height=2000,Length=800,GlassType=GlassTypeEnum.FixedGlass,Finish=GlassFinishEnum.Transparent,Thickness=GlassThicknessEnum.Thick8mm };
        [ObservableProperty]
        private string notes = string.Empty;
        [ObservableProperty]
        private int quantity;
        
        private StockedGlassViewModel? selectedRow;
        public StockedGlassViewModel? SelectedRow 
        {
            get => selectedRow;
            set
            {
                if (value != selectedRow)
                {
                    selectedRow = value;
                    
                    //If the SelectedRow is not null pass the Draw of the Glass it has (otherwise do Nothing)
                    if (selectedRow is not null)
                    {
                        GlassDraw.SetGlassDraw(selectedRow.Glass);
                        IsDrawToList = true; //Pass the Draw to the Stock List
                    }
                    OnPropertyChanged(nameof(SelectedRow));
                }
            }
        }


        #region Constructor
        public GlassesStockModalViewModel(GlassesStockService stockService)
        {
            Title = "GlassesStock".TryTranslateKey();
            this.stockService = stockService;
            GlassDraw.SetGlassDraw(glassRowToAdd);
            stockService.StockList.CollectionChanged += StockList_CollectionChanged;

        }

        private void StockList_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && e.NewItems?.Count > 0)
            {
                foreach (var item in e.NewItems)
                {
                    StockedGlassRow stockedGlassRow = (StockedGlassRow)item;
                    StockList.Add(new(stockedGlassRow));
                }
            }
        }
        #endregion

        private void GlassRowToAdd_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            GlassDraw.SetGlassDraw(GlassRowToAdd);
            //Pass the Draw to the Added Glass
            IsDrawToList = false;
        }

        protected override async Task InitilizeAsync()
        {
            if (Initilized)
            {
                return;
            }

            IsBusy = true;
            //Retrieve from Service
            if (!stockService.HasPopulated)
            {
                await stockService.PopulateStockList(); //Try Catch is inside Service no need to redo it
            }
            //Repopulate the List
            StockList.Clear();
            foreach (var item in stockService.StockList)
            {
                StockList.Add(new StockedGlassViewModel(item));
            }

            Initilized = true;
            IsBusy = false;
            //Initilized = true; (Will trigger from stock Service)
        }

        /// <summary>
        /// Initilizes the Stock List again
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task RefreshList()
        {
            //Set the Service to not populate it to Repopulate
            this.Initilized = false;
            stockService.HasPopulated = false;
            await InitilizeAsync();
        }
        [RelayCommand]
        private async Task AddGlassToStock()
        {
            if (Quantity < 1)
            {
                MessageService.Warning("lngInsertValidQuantityAbove1".TryTranslateKey(), "lngInvalidQuantity".TryTranslateKey());
                return;
            }
            if (validator.Validate(GlassRowToAdd.GetGlass()) is ValidationResult res && res.IsValid is false)
            {
                StringBuilder builder = new();
                int incrementor = 1;
                foreach (var error in res.Errors)
                {
                    builder.Append(incrementor++);
                    builder.Append(". ");
                    builder.AppendLine(error.ErrorCode.TryTranslateKey());
                }
                MessageService.Warning(builder.ToString(), "lngFailure".TryTranslateKey());
                return;
            }

            try
            {
                IsBusy = true;
                BusyPrompt = "Adding...";
                StockedGlassRow stockedGlass = new(string.Empty, GlassRowToAdd.GetGlass(), Quantity, Notes, DateTime.Now, DateTime.Now);
                //Find the same glass if there and add the quantity of the Addition quantity
                if (StockList.Any(r=> r.Glass.GetGlass().IsEqualGlass(stockedGlass.Glass) && r.Notes == stockedGlass.Notes))
                {
                    var itemToChange = StockList.FirstOrDefault(r => r.Glass.GetGlass().IsEqualGlass(stockedGlass.Glass) && r.Notes == stockedGlass.Notes);
                    if (itemToChange is not null) itemToChange.Quantity += stockedGlass.Quantity; 
                }
                await stockService.AddGlassToStock(stockedGlass,stockedGlass.Quantity);
                //Else it will be added by the Subscription to Colelction Changed of Stock Service... fail...
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally 
            {
                IsBusy = false;
                BusyPrompt = "Loading...";
            }
        }
        [RelayCommand]
        private async Task RemoveGlassFromStock(StockedGlassViewModel rowToRemove)
        {
            if (MessageService.Question("Are you Sure you want to Remove all the Quantity ?", "Remove Glass", "Ok", "Cancel") == MessageBoxResult.Cancel) return;

            try
            {
                IsBusy = true;
                BusyPrompt = "Removing...";
                SelectedRow = null; //Reset Selection
                await stockService.RemoveFromStock(rowToRemove.GetStockedGlass(), rowToRemove.Quantity);
                StockList.Remove(rowToRemove);
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally
            {
                IsBusy = false;
                BusyPrompt = "Loading...";
            }
        }
        [RelayCommand]
        private async Task AddQuantityToStock(StockedGlassViewModel rowToAddStock)
        {
            IsBusy = true;
            BusyPrompt = "Adding 1...";
            try
            {
                await stockService.AddGlassToStock(rowToAddStock.GetStockedGlass(), 1);
                rowToAddStock.Quantity++;
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally
            {
                IsBusy = false;
                BusyPrompt = LOADING;
            }
        }
        [RelayCommand]
        private async Task RemoveQuantityFromStock(StockedGlassViewModel rowToRemoveStock)
        {
            IsBusy = true;
            BusyPrompt = "Removing 1...";
            try
            {
                await stockService.RemoveFromStock(rowToRemoveStock.GetStockedGlass(), 1);
                rowToRemoveStock.Quantity--;
                if (rowToRemoveStock.Quantity == 0)
                {
                    SelectedRow = null; //Reset Selection
                    StockList.Remove(rowToRemoveStock);
                }
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally 
            { 
                IsBusy = false;
                BusyPrompt = LOADING;
            }
        }


        [RelayCommand]
        private async Task SaveListToExcelAsync()
        {
            BusyPrompt = SAVING;
            IsBusy = true;
            try
            {
                if (StockList.Any())
                {
                    var fileName = await Task.Run(() =>
                    {
                        var fileName = ExcelService.ReportXls.SaveAsXlsReport(StockList.Select(vm=> vm.GetStockedGlass()));
                        return fileName;
                    });
                    if (MessageService.Questions.ExcelSavedAskOpenFile(fileName) == MessageBoxResult.OK)
                    {
                        //Open the file if users reply is positive
                        Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = true });
                    }
                }
                else MessageService.Info("No Rows To Save", "Empty Rows");
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
            }
            finally
            {
                IsBusy = false;
                BusyPrompt = LOADING;
            }
        }

        //IF MADE TRANSIENT WE HAVE TO DISPOSE ON CLOSING WINDOW . CURRENTLY ITS SINGLETON DOES NOT NEED DISPOSING
        private bool _disposed;

        public override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                stockService.StockList.CollectionChanged -= StockList_CollectionChanged;
                foreach (var item in StockList)
                {
                    item.Dispose();
                }
                StockList.Clear();
                GlassRowToAdd.Dispose();
                GlassDraw.Dispose();
                SelectedRow?.Dispose();
                SelectedRow = null;
            }

            //object has been disposed
            _disposed = true;
            base.Dispose(disposing);
            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            //base.Dispose(disposing);
        }
    }
}

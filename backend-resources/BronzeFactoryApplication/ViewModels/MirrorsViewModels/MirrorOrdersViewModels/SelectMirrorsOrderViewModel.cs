using BronzeFactoryApplication.ViewModels.MirrorsViewModels.MirrorOrdersViewModels;
using MirrorsLib.MirrorsOrderModels;
using MirrorsLib.Repositories;
using MirrorsRepositoryMongoDB.Entities;
using MirrorsRepositoryMongoDB.Repositories;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.MirrorOrdersViewModels
{
    public partial class SelectMirrorsOrderViewModel : BaseViewModel , IModableViewModel
    {
        public SelectMirrorsOrderViewModel(MirrorsOrdersRepository mirrorsOrdersRepo,
                                           IMirrorsDataProvider dataProvider)
        {
            BusyPrompt = "Loading Orders...";
            this.mirrorsOrdersRepo = mirrorsOrdersRepo;
            mirrorsOrdersRepo.EntityDeleted += MirrorsOrdersRepo_Deleted;
            mirrorsOrdersRepo.EntityUpdated += MirrorsOrdersRepo_Upserted;
            mirrorsOrdersRepo.EntityInserted += MirrorsOrdersRepo_Upserted;
            this.dataProvider = dataProvider;
        }

        private void MirrorsOrdersRepo_Upserted(object? sender, MirrorsOrderEntity e)
        {
            //Mark it as not Initilized (Whenever it reruns it will RePULL the repository to refresh)
            Initilized = false;
        }
        private void MirrorsOrdersRepo_Deleted(object? sender, string e)
        {
            //Mark it as not Initilized (Whenever it reruns it will RePULL the repository to refresh)
            Initilized = false;
        }

        private readonly MirrorsOrdersRepository mirrorsOrdersRepo;
        private readonly IMirrorsDataProvider dataProvider;

        public event EventHandler<MirrorsOrder>? OrderSelected;

        [RelayCommand]
        private void SelectOrder(MirrorsOrder order)
        {
            OrderSelected?.Invoke(this, order);
        }

        public ObservableCollection<MirrorsOrder> LoadedOrders { get; } = [];

        [ObservableProperty]
        private MirrorsOrder? selectedOrder;

        [ObservableProperty]
        private int orderToLoadPerFetch = 20;

        public int TotalOrdersLoaded { get => LoadedOrders.Count; }
        public string ModalId { get; } = Guid.NewGuid().ToString();
        public bool IsWrappedInModal { get; } = true;

        [RelayCommand]
        protected override async Task InitilizeAsync()
        {
            //Load at the begining some orders
            if (Initilized) return;
            SelectedOrder = null;
            LoadedOrders.Clear();
            OnPropertyChanged(nameof(TotalOrdersLoaded));
            var loaded = await LoadOrders();
            Initilized = loaded;
        }

        /// <summary>
        /// Refreshes the Orders Count and Reloads orders from scratch
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task RefreshLoadedOrders()
        {
            SelectedOrder = null;
            LoadedOrders.Clear();
            OnPropertyChanged(nameof(TotalOrdersLoaded));
            await LoadOrders();
        }

        /// <summary>
        /// Loads the next batch of Orders from the Database
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task<bool> LoadOrders()
        {
            IsBusy = true;
            try
            {
                var options = new FindOptions<MirrorsOrderEntity>()
                {
                    Sort = Builders<MirrorsOrderEntity>.Sort.Descending(e => e.Created),
                    Skip = TotalOrdersLoaded,
                    Limit = OrderToLoadPerFetch,
                };
                var filter = Builders<MirrorsOrderEntity>.Filter.Empty;

                await foreach (var item in mirrorsOrdersRepo.GetEntitiesAsync(filter, options))
                {
                    var order = item.ToMirrorsOrder(dataProvider);
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        LoadedOrders.Add(order);
                        OnPropertyChanged(nameof(TotalOrdersLoaded));
                    });
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
                return false;
            }
            finally
            {
                IsBusy = false;
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
                mirrorsOrdersRepo.EntityDeleted -= MirrorsOrdersRepo_Deleted;
                mirrorsOrdersRepo.EntityUpdated -= MirrorsOrdersRepo_Upserted;
                mirrorsOrdersRepo.EntityInserted -= MirrorsOrdersRepo_Upserted;
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            base.Dispose(disposing);
        }

    }
}

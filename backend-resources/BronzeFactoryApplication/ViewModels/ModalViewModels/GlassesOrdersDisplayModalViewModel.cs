using BronzeFactoryApplication.ApplicationServices.MessangerService;
using BronzeFactoryApplication.ViewModels.ModalViewModels;
using DataAccessLib;
using GlassesOrdersModels.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels;

public partial class GlassesOrdersDisplayModalViewModel : ModalViewModel
{
    private readonly IGlassOrderRepository ordersRepo;
    private readonly IMessenger messenger;
    private readonly CloseModalService closeModalService;
    [ObservableProperty]
    private ObservableCollection<GlassesOrder> orders = new();
    [ObservableProperty]
    private GlassesOrder? selectedOrder;
    [ObservableProperty]
    private bool isBusy;

    /// <summary>
    /// The Number of Pages of the GetOrders Query
    /// </summary>
    [ObservableProperty]
    private int pagesNumber;

    /// <summary>
    /// The PageNumber for the Retrieved Results
    /// </summary>
    private int nextPageToRetrieve = 1;

    public GlassesOrdersDisplayModalViewModel(IGlassOrderRepository ordersRepo ,
                                        IMessenger messenger,
                                        CloseModalService closeModalService)
    {
        Title = "lngPickOrder".TryTranslateKey();
        this.ordersRepo = ordersRepo;
        this.messenger = messenger;
        this.closeModalService = closeModalService;
    }

    /// <summary>
    /// Iniitilizes the Modal , Retrieves the First Page of Orders and The Number of Pages Available
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    protected override async Task InitilizeAsync()
    {
        if (Initilized) return;
        
        IsBusy = true;
        try
        {
            PagesNumber = await ordersRepo.GetOrdersPagesAsync();
            Orders = new();
            await RetrieveOrdersPage(nextPageToRetrieve);
            nextPageToRetrieve++;
            Initilized = true;
        }
        catch (Exception ex)
        {
            MessageService.LogAndDisplayException(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Retrieve Orders based on the Next Page to Be Retrieved
    /// </summary>
    /// <returns></returns>
    private async Task RetrieveOrdersPage(int page)
    {
        await foreach (var order in ordersRepo.GetAllOrdersAsync(page))
        {
            //Marshal this back to the UI Thread
            await Application.Current.Dispatcher.InvokeAsync(() => Orders.Add(order));
        }
    }

    [RelayCommand]
    private async Task RetrieveMoreOrders()
    {
        try
        {
            if (nextPageToRetrieve <= PagesNumber)
            {
                await RetrieveOrdersPage(nextPageToRetrieve);
                nextPageToRetrieve++;
            }            
        }
        catch (Exception ex)
        {
            MessageService.LogAndDisplayException(ex);
        }
    }


    /// <summary>
    /// Sends a Message an Order Has been Selected 
    /// </summary>
    /// <param name="order">The Selected Order</param>
    [RelayCommand]
    private void SelectOrder(GlassesOrder order)
    {
        IsBusy = true;
        //Return if the order selected is not in the results (should not be possible)
        if (order is null || !Orders.Contains(order))
        {
            IsBusy = false;
            return;
        }
        if (!order.CanBeEdited)
        {
            MessageService.Warning($"This Order is either 'OLD'{Environment.NewLine}or{Environment.NewLine}It has already some of its glasses Collected", "Failure".TryTranslateKey());
            IsBusy = false;
            return;
        }

        GlassesOrderSelectedMessage message = new(order, typeof(GlassesOrdersDisplayModalViewModel));
        messenger.Send(message);
        IsBusy = false;
        closeModalService.CloseModal(this);
    }

}

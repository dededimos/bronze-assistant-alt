using BronzeFactoryApplication.ApplicationServices.MessangerService;
using DataAccessLib;
using GlassesOrdersModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class EditGlassesOrderDetailsModalViewModel : ModalViewModel
    {
        private GlassesOrder? _undoStore;
        private readonly IMessenger messenger;
        private readonly CloseModalService closeModalService;
        private readonly IGlassOrderRepository glassRepo;

        /// <summary>
        /// If this is a New Order that will be saved to the Database or an Old One // Old orders cannot have their Ids Changed
        /// </summary>
        [ObservableProperty]
        private bool isNewOrder;
        [ObservableProperty]
        private string lastOrderId = string.Empty;
        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string orderId = string.Empty;
        [ObservableProperty]
        private string notes = string.Empty;

        /// <summary>
        /// Weather it has already Saved
        /// </summary>
        public bool HasSaved { get; private set; }


        public int GlassesCount { get => _undoStore?.GlassesCount ?? 0; }
        public int CabinsCount { get => _undoStore?.CabinsCount ?? 0; }
        public int PA0Count { get => _undoStore?.PA0Count ?? 0; }

        public EditGlassesOrderDetailsModalViewModel(
            IMessenger messenger, 
            CloseModalService closeModalService,
            IGlassOrderRepository glassRepo)
        {
            Title = "lngEditGlassesOrderDetailsModalTitle".TryTranslateKey();
            this.messenger = messenger;
            this.closeModalService = closeModalService;
            this.glassRepo = glassRepo;
            closeModalService.ModalClosing += OnModalClosing;
        }

        private void OnModalClosing(object? sender, ModalClosingEventArgs e)
        {
            if (
                !HasSaved
                && (Notes != _undoStore?.Notes || OrderId != _undoStore?.OrderId)
                && MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel
               )

                e.ShouldCancelClose = true;
        }

        public void SetGlassesOrder(GlassesOrder order,bool isNew)
        {
            this._undoStore = order;
            IsNewOrder = isNew;
            Notes = order.Notes;
            OrderId = order.OrderId;
            OnPropertyChanged(nameof(GlassesCount));
            OnPropertyChanged(nameof(CabinsCount));
            OnPropertyChanged(nameof(PA0Count));
            HasSaved = false;
        }

        [RelayCommand]
        private void UndoEdits()
        {
            Notes = _undoStore?.Notes ?? string.Empty;
            OrderId = _undoStore?.OrderId ?? string.Empty;
        }

        [RelayCommand]
        private void SaveAndClose()
        {
            if (GlassesOrder.OrderIdRegex.IsMatch(OrderId))
            {
                messenger.Send(new GlassesOrderDetailsEditMessage(OrderId, Notes));
                HasSaved = true;
                MessageService.Information.SaveSuccess();
                closeModalService.CloseModal(this);
            }
            else
            {
                MessageService.Warnings.InvalidOrderId();
            }
        }

        [RelayCommand]
        private async Task GetLatestIdAsync()
        {
            IsBusy = true;
            try
            {
                LastOrderId = await glassRepo.GetLatestOrderIdAsync();
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
                closeModalService.ModalClosing -= OnModalClosing;
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

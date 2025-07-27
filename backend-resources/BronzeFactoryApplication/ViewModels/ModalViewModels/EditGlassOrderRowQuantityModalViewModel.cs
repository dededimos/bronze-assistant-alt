using BronzeFactoryApplication.ViewModels.OrderRelevantViewModels;
using DataAccessLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class EditGlassOrderRowQuantityModalViewModel : ModalViewModel
    {
        private readonly IGlassOrderRepository glassRepo;
        private readonly CloseModalService closeModalService;
        private int _undoStoreFilledQuantity;
        private bool hasSaved;

        [ObservableProperty]
        private GlassOrderRowViewModel? row;
        [ObservableProperty]
        private bool isBusy;

        public EditGlassOrderRowQuantityModalViewModel(IGlassOrderRepository glassRepo,
            CloseModalService closeModalService)
        {
            this.Title = "lngPartialReceipt".TryTranslateKey();
            this.glassRepo = glassRepo;
            this.closeModalService = closeModalService;
            closeModalService.ModalClosing += CloseModalService_ModalClosing;
        }

        /// <summary>
        /// Prompts to cancel closing when there are unsaved changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseModalService_ModalClosing(object? sender, ModalClosingEventArgs e)
        {
            if (Row?.FilledQuantity != _undoStoreFilledQuantity && !hasSaved)
            {
                if (MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
                {
                    e.ShouldCancelClose = true;
                }
            }
        }

        public void InitilizeViewModel(GlassOrderRowViewModel row)
        {
            Row = row;
            _undoStoreFilledQuantity = row.FilledQuantity;
        }

        /// <summary>
        /// Saves changes to the Database and Closes the Modal
        /// If it failed it restores to the previous Filled quantity 
        /// and prompts the failure without closing the modal
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task TrySaveAndClose()
        {
            if (Row is null) return;
            IsBusy = true;
            try
            {
                await glassRepo.UpdateGlassRowAsync(Row.GetModel());
                hasSaved = true;
                closeModalService.CloseModal(this);
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);
                //Restore the Quantity
                Row!.FilledQuantity = _undoStoreFilledQuantity;
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private void Undo()
        {
            if (Row is null) return;
            Row!.FilledQuantity = _undoStoreFilledQuantity;
        }

        private bool _disposed;
        public override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                closeModalService.ModalClosing -= CloseModalService_ModalClosing;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class GlassMatchingModalViewModel : ModalViewModel
    {
        private readonly CloseModalService closeModalService;

        [ObservableProperty]
        private CabinsGlassMatchesViewModel? matchesVm;

        public GlassMatchingModalViewModel(CloseModalService closeModalService)
        {
            Title = "GlassSwapFromStock".TryTranslateKey();
            this.closeModalService = closeModalService;
            this.closeModalService.ModalClosing += CloseModalService_ModalClosing;
        }

        private void CloseModalService_ModalClosing(object? sender, ModalClosingEventArgs e)
        {
            if (e.ClosingModal == this)
            {
                MatchesVm?.ScrapSwaps();
            }
        }

        public void SetMatchesVm(CabinsGlassMatchesViewModel matchesVm)
        {
            this.MatchesVm = matchesVm;
        }

        [RelayCommand]
        private void SaveAndClose()
        {
            MatchesVm?.ConfirmSwaps();
            closeModalService.CloseModal(this);
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
                this.closeModalService.ModalClosing -= CloseModalService_ModalClosing;
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

using BronzeFactoryApplication.ViewModels.ComponentsUCViewModels.DrawsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenPrintPreviewGlassModalService
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly Func<PrintPriviewGlassDrawModalViewModel> vmFactory;
        private readonly IDialogService dialogService;

        public OpenPrintPreviewGlassModalService(ModalsContainerViewModel modalsContainer, Func<PrintPriviewGlassDrawModalViewModel> vmFactory, IDialogService dialogService)
        {
            this.modalsContainer = modalsContainer;
            this.vmFactory = vmFactory;
            this.dialogService = dialogService;
        }

        public void OpenPreviewDialog(GlassDrawViewModel glassDraw , string? specialDrawString = null , int? specialDrawNumber = null)
        {
            var existingWindow = Application.Current.Windows.OfType<DialogWindow>().FirstOrDefault(w => w.DataContext is PrintPriviewGlassDrawModalViewModel);
            existingWindow?.Close();
            
            var modal = vmFactory.Invoke();
            modal.SetDraw(glassDraw, specialDrawString, specialDrawNumber);
            dialogService.OpenDialogAsWindow(modal);
        }
        public void OpenModal(GlassDrawViewModel glassDraw, string? specialDrawString = null, int? specialDrawNumber = null)
        {
            var modal = vmFactory.Invoke();
            modal.SetDraw(glassDraw, specialDrawString, specialDrawNumber);
            modalsContainer.OpenModal(modal);
        }
    }
}

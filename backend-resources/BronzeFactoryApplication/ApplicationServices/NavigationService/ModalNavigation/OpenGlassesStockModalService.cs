using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenGlassesStockModalService
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly IDialogService dialogService;
        private readonly Func<GlassesStockModalViewModel> vmFactory;

        public OpenGlassesStockModalService(
            Func<GlassesStockModalViewModel> vmFactory, 
            ModalsContainerViewModel modalsContainer,
            IDialogService dialogService)
        {
            this.vmFactory = vmFactory;
            this.modalsContainer = modalsContainer;
            this.dialogService = dialogService;
        }

        public void OpenModal()
        {
            var vm = vmFactory.Invoke();
            modalsContainer.OpenModal(vm);
        }

        public void OpenModalAsWindow()
        {
            var vm = vmFactory.Invoke();
            dialogService.OpenDialogAsWindow(vm);
        }
    }
}

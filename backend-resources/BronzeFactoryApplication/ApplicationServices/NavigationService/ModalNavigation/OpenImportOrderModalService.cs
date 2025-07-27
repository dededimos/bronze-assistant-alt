using BronzeFactoryApplication.ViewModels.ModalViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenImportOrderModalService
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly Func<ImportOrderModalViewModel> vmFactory;
        private readonly IDialogService dialogService;

        public OpenImportOrderModalService(ModalsContainerViewModel modalsContainer, 
            Func<ImportOrderModalViewModel> vmFactory,
            IDialogService dialogService)
        {
            this.modalsContainer = modalsContainer;
            this.vmFactory = vmFactory;
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

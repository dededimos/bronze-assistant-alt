using BronzeFactoryApplication.ViewModels.AccessoriesViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenEditLocalizedStringModalService
    {
        private ModalsContainerViewModel modalsContainer;
        private readonly Func<LocalizedStringEditModalViewModel> vmFactory;

        public OpenEditLocalizedStringModalService(ModalsContainerViewModel modalsContainer, Func<LocalizedStringEditModalViewModel> vmFactory)
        {
            this.modalsContainer = modalsContainer;
            this.vmFactory = vmFactory;
        }

        public void OpenModal(LocalizedStringViewModel localizedStringToEdit , string modalTitle)
        {
            var modalVm = vmFactory.Invoke();
            modalVm.InitilizeModal(localizedStringToEdit, modalTitle);
            modalsContainer.OpenModal(modalVm);
        }
    }
}

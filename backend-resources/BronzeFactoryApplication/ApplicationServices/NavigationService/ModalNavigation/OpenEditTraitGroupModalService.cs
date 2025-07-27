using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenEditTraitGroupModalService
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly Func<EditTraitGroupModalViewModel> modalVmFactory;

        public OpenEditTraitGroupModalService(ModalsContainerViewModel modalsContainer, Func<EditTraitGroupModalViewModel> modalVmFactory)
        {
            this.modalsContainer = modalsContainer;
            this.modalVmFactory = modalVmFactory;
        }

        public void OpenModal()
        {
            var vm = modalVmFactory.Invoke();
            modalsContainer.OpenModal(vm);
        }
    }
}

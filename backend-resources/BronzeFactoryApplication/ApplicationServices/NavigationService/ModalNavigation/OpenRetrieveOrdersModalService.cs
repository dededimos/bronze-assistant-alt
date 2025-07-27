using BronzeFactoryApplication.ViewModels.ModalViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenRetrieveOrdersModalService
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly Func<GlassesOrdersDisplayModalViewModel> vmFactory;

        public OpenRetrieveOrdersModalService(ModalsContainerViewModel modalsContainer , Func<GlassesOrdersDisplayModalViewModel> vmFactory)
        {
            this.modalsContainer = modalsContainer;
            this.vmFactory = vmFactory;
        }

        public void OpenModal()
        {
            var vm = vmFactory.Invoke();
            modalsContainer.OpenModal(vm);
        }
    }
}

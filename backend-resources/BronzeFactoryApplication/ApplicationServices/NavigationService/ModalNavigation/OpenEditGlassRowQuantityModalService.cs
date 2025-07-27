using BronzeFactoryApplication.ViewModels.ModalViewModels;
using BronzeFactoryApplication.ViewModels.OrderRelevantViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenEditGlassRowQuantityModalService
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly Func<EditGlassOrderRowQuantityModalViewModel> vmFactory;

        public OpenEditGlassRowQuantityModalService(Func<EditGlassOrderRowQuantityModalViewModel> vmFactory, ModalsContainerViewModel modalsContainer)
        {
            this.vmFactory = vmFactory;
            this.modalsContainer = modalsContainer;
        }

        public void OpenModal(GlassOrderRowViewModel row)
        {
            var vm = vmFactory.Invoke();
            vm.InitilizeViewModel(row);
            modalsContainer.OpenModal(vm);
        }
    }
}

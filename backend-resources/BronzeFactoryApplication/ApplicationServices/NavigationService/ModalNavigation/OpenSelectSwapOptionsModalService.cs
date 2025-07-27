using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenSelectSwapOptionsModalService
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly Func<SelectSwapOptionsModalViewModel> vmFactory;

        public OpenSelectSwapOptionsModalService(ModalsContainerViewModel modalsContainer, Func<SelectSwapOptionsModalViewModel> vmFactory)
        {
            this.modalsContainer = modalsContainer;
            this.vmFactory = vmFactory;
        }

        public void OpenModal(CabinSynthesis synthesisToSwap,CabinSynthesisModel modelUnderSwap , GlassSwap swapGlass)
        {
            var vm = vmFactory.Invoke();
            vm.ConfigureSwapProperties(synthesisToSwap, modelUnderSwap, swapGlass);
            modalsContainer.OpenModal(vm);
        }
    }
}

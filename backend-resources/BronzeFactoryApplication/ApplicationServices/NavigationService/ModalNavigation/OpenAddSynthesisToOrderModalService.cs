using BronzeFactoryApplication.ViewModels.ModalViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenAddSynthesisToOrderModalService
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly Func<AddSynthesisToOrderModalViewModel> vmFactory;

        public OpenAddSynthesisToOrderModalService(Func<AddSynthesisToOrderModalViewModel> vmFactory, ModalsContainerViewModel modalsContainer)
        {
            this.vmFactory = vmFactory;
            this.modalsContainer = modalsContainer;
        }

        public void OpenModal(CabinSynthesis synthesis,string refPA0Number,GlassSwap? swap)
        {
            var viewModel = vmFactory.Invoke();
            viewModel.Initilize(synthesis,refPA0Number,swap);
            modalsContainer.OpenModal(viewModel);
        }

    }
}

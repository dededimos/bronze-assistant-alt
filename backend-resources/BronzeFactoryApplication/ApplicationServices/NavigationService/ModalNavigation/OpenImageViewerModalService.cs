using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenImageViewerModalService
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly Func<ImageViewerModalViewModel> vmFactory;

        public OpenImageViewerModalService(ModalsContainerViewModel modalsContainer , Func<ImageViewerModalViewModel> vmFactory)
        {
            this.modalsContainer = modalsContainer;
            this.vmFactory = vmFactory;
        }

        public void OpenModal(string imageUrl)
        {
            var vm = vmFactory.Invoke();
            vm.SetUrl(imageUrl);
            modalsContainer.OpenModal(vm);
        }
    }
}

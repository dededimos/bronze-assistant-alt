using BronzeFactoryApplication.ViewModels.ModalViewModels;
using GlassesOrdersModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenEditGlassesOrderDetailsModalService
    {
        private ModalsContainerViewModel modalsContainer;
        private readonly Func<EditGlassesOrderDetailsModalViewModel> vmFactory;

        public OpenEditGlassesOrderDetailsModalService(ModalsContainerViewModel modalsContainer, Func<EditGlassesOrderDetailsModalViewModel> vmFactory)
        {
            this.modalsContainer = modalsContainer;
            this.vmFactory = vmFactory;
        }

        public void OpenModal(GlassesOrder orderToEdit,bool isNew)
        {
            var vm = vmFactory.Invoke();
            vm.SetGlassesOrder(orderToEdit,isNew);
            modalsContainer.OpenModal(vm);
        }
    }
}

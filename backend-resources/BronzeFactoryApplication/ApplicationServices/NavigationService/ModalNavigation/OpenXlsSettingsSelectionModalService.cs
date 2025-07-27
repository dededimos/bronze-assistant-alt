using BronzeFactoryApplication.ApplicationServices.ExcelXlsService;
using BronzeFactoryApplication.ApplicationServices.SettingsService;
using BronzeFactoryApplication.Properties;
using BronzeFactoryApplication.ViewModels.ModalViewModels;
using BronzeFactoryApplication.ViewModels.SettingsViewModels.XlsSettingsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenXlsSettingsSelectionModalService
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly Func<XlsSettingsModalViewModel> vmFactory;

        public OpenXlsSettingsSelectionModalService(ModalsContainerViewModel modalsContainer , Func<XlsSettingsModalViewModel> vmFactory)
        {
            this.modalsContainer = modalsContainer;
            this.vmFactory = vmFactory;
        }

        public async Task OpenModal()
        {
            var vm = vmFactory.Invoke();
            modalsContainer.OpenModal(vm);
            await vm.InitilizeViewModel();
        }
    }
}

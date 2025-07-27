using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenPrintCabinBomModalService
    {
        private ModalsContainerViewModel modalsContainer;
        private readonly Func<PrintCabinBomModalViewModel> vmFactory;
        public OpenPrintCabinBomModalService(ModalsContainerViewModel modalsContainer, Func<PrintCabinBomModalViewModel> vmFactory)
        {
            this.modalsContainer = modalsContainer;
            this.vmFactory = vmFactory;
        }

        public void OpenModal(IEnumerable<CabinOrderRow> rows)
        {
            var vm = vmFactory.Invoke();
            foreach (var row in rows)
            {
                vm.AddCabinBom(row);
            }
            modalsContainer.OpenModal(vm);
        }

    }
}

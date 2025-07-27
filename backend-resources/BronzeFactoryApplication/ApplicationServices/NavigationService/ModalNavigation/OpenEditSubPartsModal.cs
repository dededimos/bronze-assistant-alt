using BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels.Parts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenEditSubPartsModal
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly Func<EditSubPartsModalViewModel> editVmFactory;
        public OpenEditSubPartsModal(ModalsContainerViewModel modalsContainer, Func<EditSubPartsModalViewModel> editVmFactory)
        {
            this.modalsContainer = modalsContainer;
            this.editVmFactory = editVmFactory;
        }

        public void OpenModal(EditPartViewModel editPartVm)
        {
            var editSubPartsVm = editVmFactory.Invoke();
            editSubPartsVm.SetEditPartViewModel(editPartVm);
            modalsContainer.OpenModal(editSubPartsVm);
        }

    }
}

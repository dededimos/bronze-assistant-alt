using BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels.DefaultPartsLists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenEditPartSetsModalService
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly Func<EditPartSetsModalViewModel> editVmFactory;

        public OpenEditPartSetsModalService(ModalsContainerViewModel modalsContainer, Func<EditPartSetsModalViewModel> editVmFactory)
        {
            this.modalsContainer = modalsContainer;
            this.editVmFactory = editVmFactory;
        }

        /// <summary>
        /// Opens a Modal to Edit the Part Sets of a Default List
        /// </summary>
        /// <param name="editPartsVm">The Edition ViewModel for the Default List</param>
        public void OpenModal(EditDefaultPartsViewModel editPartsVm)
        {
            var modalVm = editVmFactory.Invoke();
            modalVm.SetEditDefaultPartsViewModel(editPartsVm);
            modalsContainer.OpenModal(modalVm);
        }
    }
}

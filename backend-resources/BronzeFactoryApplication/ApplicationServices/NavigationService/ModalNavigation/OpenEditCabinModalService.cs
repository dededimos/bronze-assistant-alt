using BronzeFactoryApplication.ViewModels.ModalViewModels;
using GlassesOrdersModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenEditCabinModalService
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly Func<EditCabinModalViewModel> editVmFactory;

        public OpenEditCabinModalService(ModalsContainerViewModel modalsContainer , Func<EditCabinModalViewModel> editVmFactory)
        {
            this.modalsContainer = modalsContainer;
            this.editVmFactory = editVmFactory;
        }

        /// <summary>
        /// Opens a modal to Edit the Properties of the Selected Cabin
        /// </summary>
        /// <param name="cabinRow">The Cabin Row to Edit</param>
        public void OpenModal(CabinOrderRow cabinRow)
        {
            EditCabinModalViewModel editVm = editVmFactory.Invoke();
            editVm.SetCabinRow(cabinRow);
            modalsContainer.OpenModal(editVm);
        }
    }
}

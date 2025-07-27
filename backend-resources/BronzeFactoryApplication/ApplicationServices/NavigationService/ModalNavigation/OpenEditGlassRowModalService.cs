using BronzeFactoryApplication.ViewModels.ComponentsUCViewModels;
using BronzeFactoryApplication.ViewModels.ModalViewModels;
using GlassesOrdersModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    /// <summary>
    /// The Service that Opens an Edit GlassRow Modal
    /// </summary>
    public class OpenEditGlassRowModalService
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly Func<GlassRowEditViewModel> glassRowEditVM;

        public OpenEditGlassRowModalService(ModalsContainerViewModel modalsContainer, Func<GlassRowEditViewModel> glassRowEditVM)
        {
            this.modalsContainer = modalsContainer;
            this.glassRowEditVM = glassRowEditVM;
        }

        public void OpenModal(GlassOrderRow glassRow)
        {
            GlassRowEditViewModel vm = glassRowEditVM.Invoke();
            vm.SetGlassRow(glassRow);
            modalsContainer.OpenModal(vm);
        }

    }
}

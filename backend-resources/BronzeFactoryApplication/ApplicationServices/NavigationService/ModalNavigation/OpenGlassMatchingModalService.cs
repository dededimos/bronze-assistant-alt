using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenGlassMatchingModalService
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly Func<GlassMatchingModalViewModel> modalFactory;

        public OpenGlassMatchingModalService(ModalsContainerViewModel modalsContainer, Func<GlassMatchingModalViewModel> modalFactory)
        {
            this.modalsContainer = modalsContainer;
            this.modalFactory = modalFactory;
        }

        public void OpenModal(CabinsGlassMatchesViewModel matchesVm)
        {
            var modalVm = modalFactory.Invoke();
            modalVm.SetMatchesVm(matchesVm);
            modalsContainer.OpenModal(modalVm);
        }
    }
}

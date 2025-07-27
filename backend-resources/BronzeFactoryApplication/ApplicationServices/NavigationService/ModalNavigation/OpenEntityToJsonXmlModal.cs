using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenEntityToJsonXmlModal
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly Func<EntityToJsonXmlModalViewModel> modalFactory;

        public OpenEntityToJsonXmlModal(ModalsContainerViewModel modalsContainer, Func<EntityToJsonXmlModalViewModel> modalFactory)
        {
            this.modalsContainer = modalsContainer;
            this.modalFactory = modalFactory;
        }

        public void OpenModal()
        {
            var modal = modalFactory.Invoke();
            modalsContainer.OpenModal(modal);
        }
    }
}

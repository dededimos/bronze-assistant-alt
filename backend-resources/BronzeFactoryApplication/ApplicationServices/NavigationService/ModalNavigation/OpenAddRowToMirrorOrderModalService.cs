using BronzeFactoryApplication.ViewModels.MirrorsViewModels.MirrorOrdersViewModels;
using MirrorsLib;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation
{
    public class OpenAddRowToMirrorOrderModalService
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly Func<AddRowToMirrorsOrderModalViewModel> vmFactory;

        public OpenAddRowToMirrorOrderModalService(ModalsContainerViewModel modalsContainer, Func<AddRowToMirrorsOrderModalViewModel> vmFactory)
        {
            this.modalsContainer = modalsContainer;
            this.vmFactory = vmFactory;
        }

        public void OpenModal(MirrorSynthesis synthesisToAdd , string refPaoPam = "")
        {
            var vm = vmFactory.Invoke();
            vm.SetMirrorToAdd(synthesisToAdd);
            vm.Row.RefPAOPAM = refPaoPam;
            modalsContainer.OpenModal(vm);
        }
    }
}

using BronzeFactoryApplication.ViewModels.ModalViewModels;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation;

public class OpenModalAdvancedCabinPropsService
{
    private readonly ModalsContainerViewModel modalsContainer;
    private readonly Func<EditCabinConstraintsModalViewModel> modalVmFactory;

    public OpenModalAdvancedCabinPropsService(ModalsContainerViewModel modalsContainer , Func<EditCabinConstraintsModalViewModel> modalVmFactory)
    {
        this.modalsContainer = modalsContainer;
        this.modalVmFactory = modalVmFactory;
    }

    public void OpenModal(CabinViewModel cabinVm)
    {
        var vm = modalVmFactory.Invoke();
        vm.SetConstraints(cabinVm);
        modalsContainer.OpenModal(vm);
    }
}


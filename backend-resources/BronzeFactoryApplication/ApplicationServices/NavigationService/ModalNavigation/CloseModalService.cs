using BronzeFactoryApplication.ViewModels.ModalViewModels;

namespace BronzeFactoryApplication.ApplicationServices.NavigationService.ModalNavigation;

public class CloseModalService
{

    private readonly ModalsContainerViewModel modalsContainer;
    public CloseModalService(ModalsContainerViewModel modalsContainer)
    {
        this.modalsContainer = modalsContainer;
    }

    public void CloseModal(IModalViewModel closingModal)
    {
        modalsContainer.CloseModal(closingModal);
    }
    /// <summary>
    /// Closes the Modal with the given Id
    /// <para>If the modal is not found an exception will be thrown</para>
    /// </summary>
    /// <param name="modalId"></param>
    public void CloseModal(string modalId)
    {
        modalsContainer.CloseModal(modalId);
    }

    public event EventHandler<ModalClosingEventArgs>? ModalClosing
    {
        add { this.modalsContainer.ModalClosing += value; }
        remove { this.modalsContainer.ModalClosing -= value; }
    }
    public event EventHandler<ModalClosedEventArgs>? ModalClosed
    {
        add { this.modalsContainer.ModalClosed += value; }
        remove { this.modalsContainer.ModalClosed -= value; }
    }
}








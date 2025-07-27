using System.Collections.ObjectModel;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class ModalsContainerViewModel : BaseViewModel
    {
        public ObservableCollection<IModalViewModel> OpenedModals { get;} = [];
        public IModalViewModel? FrontModal { get => OpenedModals.OrderByDescending(m => m.LayerNo).FirstOrDefault(); }

        public void OpenModal(IModalViewModel modal)
        {
            // Put the Opened Modal in Front
            modal.LayerNo += (FrontModal?.LayerNo ?? 0) + 1;
            OpenedModals.Add(modal);
            OnPropertyChanged(nameof(FrontModal));
        }

        [RelayCommand]
        public void CloseModal(IModalViewModel modal)
        {
            if (OnClosingModal(modal).ShouldCancelClose)
            {
                return;
            }
            // Remove the Modal from view
            OpenedModals.Remove(modal);
            // Inform front Modal Changed
            OnPropertyChanged(nameof(FrontModal));
            // Inform which type of modal closed
            RaiseModalClosed(modal.GetType());
            // Dispose the closed modal
            modal.Dispose();
        }

        public void CloseModal(string modalId)
        {
            IModalViewModel? modal = OpenedModals.FirstOrDefault(m => m.ModalId == modalId) 
                ?? throw new Exception($"Modal with id '{modalId}' not Found");
            CloseModal(modal);
        }

        [RelayCommand]
        private void CloseFrontModal()
        {
            if (FrontModal is null)
            {
                return;
            }
            else
            {
                CloseModal(FrontModal);
            }
        }

        public event EventHandler<ModalClosingEventArgs>? ModalClosing;
        protected ModalClosingEventArgs OnClosingModal(IModalViewModel closingModal)
        {
            ModalClosingEventArgs closingArgs = new(closingModal);
            ModalClosing?.Invoke(this, closingArgs);
            return closingArgs;
        }

        public event EventHandler<ModalClosedEventArgs>? ModalClosed;
        protected void RaiseModalClosed(Type modalType)
        {
            ModalClosedEventArgs args = new(modalType);
            ModalClosed?.Invoke(this, args);
        }
    }

    public class ModalClosingEventArgs : EventArgs
    {
        public bool ShouldCancelClose { get; set; } = false;
        public IModalViewModel ClosingModal { get; set; }

        public ModalClosingEventArgs(IModalViewModel closingModal)
        {
            ClosingModal = closingModal;
        }
    }

    public class ModalClosedEventArgs : EventArgs
    {
        public Type TypeOfClosedModal { get; set; }

        public ModalClosedEventArgs(Type typeOfClosedModal)
        {
            TypeOfClosedModal = typeOfClosedModal;
        }
    }

}

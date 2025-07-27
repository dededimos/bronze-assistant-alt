using CommonInterfacesBronze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ApplicationServices.ModalsDirectorService
{
    public interface IWrappedViewsModalsGenerator
    {
        /// <summary>
        /// A Generator Opener of Wrapped ViewModels into a Modal
        /// </summary>
        /// <param name="wrappedViewModel">The ViewModel that is wrapped into a Modal</param>
        /// <param name="title">The Language KEY!!! of the Title of the Modal </param>
        /// <param name="customAction">A custom Action to be executed by a Button of the Modal and then close if null no button will be visible</param>
        /// <param name="customButtonFunction">The Custom Button Function to show in the UI</param>
        /// <param name="preventCloseIfFalseFunction">A Function that will prevent the Modal from closing if it returns false</param>
        void OpenModal(IBaseViewModel wrappedViewModel,
                       string title = "",
                       Action? customAction = null,
                       WrappedModalCustomActionButtonOption customButtonFunction = WrappedModalCustomActionButtonOption.None,
                       bool shouldDisposeWrappedVmOnClose = false,
                       Func<bool>? preventCloseIfFalseFunction = null);
    }

    public class WrappedViewsModalsGenerator : IWrappedViewsModalsGenerator
    {
        private readonly ModalsContainerViewModel modalsContainer;
        private readonly Func<IWrappedViewModelModalViewModel> modalVmFactory;

        public WrappedViewsModalsGenerator(Func<IWrappedViewModelModalViewModel> modalVmFactory, ModalsContainerViewModel modalsContainer)
        {
            this.modalVmFactory = modalVmFactory;
            this.modalsContainer = modalsContainer;
        }

        public void OpenModal(IBaseViewModel wrappedViewModel,
                              string title = "",
                              Action? customAction = null,
                              WrappedModalCustomActionButtonOption customButtonFunction = WrappedModalCustomActionButtonOption.None,
                              bool shouldDisposeWrappedVmOnClose = false,
                              Func<bool>? preventCloseIfFalseFunction = null)
        {
            var modalVm = modalVmFactory.Invoke();

            //Set a new Id if this a ModalbleViewModel
            if (wrappedViewModel is IModableViewModel modable)
            {
                modalVm.SetNewModalId(modable.ModalId);
            }
            modalVm.ShouldDisposeWrappedViewModel = shouldDisposeWrappedVmOnClose;
            modalVm.InitilizeModal(wrappedViewModel, title,customAction, customButtonFunction, preventCloseIfFalseFunction);
            modalsContainer.OpenModal(modalVm);
        }
    }
}

using BronzeFactoryApplication.ApplicationServices.NavigationService;
using BronzeFactoryApplication.ViewModels.AccessoriesViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels;
using CommonInterfacesBronze;
using Microsoft.AspNetCore.Components.Forms;
using ShowerEnclosuresModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using TasCustomControlsLibrary.CustomControls;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class ModalViewModel : BaseViewModel , IModalViewModel
    {

        public override bool IsDisposable => true;
        public string ModalId { get; private set; } = Guid.NewGuid().ToString();

        [ObservableProperty]
        private int layerNo;
                
        public ModalViewModel()
        {
            
        }

        public void SetNewModalId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new Exception("Id cannot be Empty or Null on a Modal Id");
            }
            ModalId = id;
        }
    }

    /// <summary>
    /// The ViewModel of a Modal
    /// </summary>
    public interface IModalViewModel : IBaseViewModel
    {
        public int LayerNo { get; set; }
        public string ModalId { get; }
        /// <summary>
        /// Set a new Id to this Modal
        /// </summary>
        /// <param name="id"></param>
        void SetNewModalId(string id);
    }
    /// <summary>
    /// A <see cref="IModalViewModel"/> that wraps a <typeparamref name="T"/> ViewModel
    /// This way any view can be created as a Modal
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IWrappedViewModelModalViewModel : IModalViewModel
    {
        /// <summary>
        /// The ViewModel that is wrapped in the Modal to be shown
        /// </summary>
        IBaseViewModel? WrappedViewModel { get; }
        bool HasCustomAction { get; }
        bool HasCustomSaveAndCloseAction { get; }
        bool HasCustomAddAndCloseAction { get; }
        bool ShouldDisposeWrappedViewModel { get; set; }

        /// <summary>
        /// Initilizes the Modal ViewModel with the Wrapped ViewModel and its title
        /// </summary>
        /// <param name="wrappedViewModel"></param>
        /// <param name="title"></param>
        /// <param name="customAction">A custom action Command of this wrapped ViewModel</param>
        /// <param name="customButtonFunction">The Function of the Custom Button</param>
        /// <param name="preventCloseIfFalseFunction">A Function that will prevent the Modal from Closing if it returns False</param>
        void InitilizeModal(IBaseViewModel wrappedViewModel,
                            string title = "",
                            Action? customAction = null,
                            WrappedModalCustomActionButtonOption customButtonFunction = WrappedModalCustomActionButtonOption.None,
                            Func<bool>? preventCloseIfFalseFunction = null);
    }
    /// <summary>
    /// A ViewModel that can be wrapped in a Modal
    /// </summary>
    public interface IModableViewModel : IBaseViewModel
    {
        /// <summary>
        /// The Id of the Modal that will wrapp this viewmodel
        /// </summary>
        string ModalId { get; }
        /// <summary>
        /// Weather this is wrapped in a Modal or not
        /// </summary>
        bool IsWrappedInModal { get; }
    }
  

    public partial class WrappedViewModelModalViewModel : ModalViewModel , IWrappedViewModelModalViewModel
    {
        public WrappedViewModelModalViewModel(CloseModalService closeModalService)
        {
            this.closeModalService = closeModalService;
        }

        private void CloseModalService_ModalClosing(object? sender, ModalClosingEventArgs e)
        {
            if (e.ClosingModal == this 
                && preventCloseIfFalseFunction is not null 
                && preventCloseIfFalseFunction.Invoke() == false)
            {
                e.ShouldCancelClose = true;
            }
        }

        private readonly CloseModalService closeModalService;

        public IBaseViewModel? WrappedViewModel { get; private set; }
        public bool HasCustomSaveAndCloseAction { get; private set; }
        public bool HasCustomAddAndCloseAction { get; private set; }
        public bool ShouldDisposeWrappedViewModel { get; set; }

        public bool HasCustomAction { get => customAction is not null; }
        private Action? customAction;
        private Func<bool>? preventCloseIfFalseFunction;

        public virtual void InitilizeModal(IBaseViewModel wrappedViewModel,
                                           string title = "",
                                           Action? customAction = null,
                                           WrappedModalCustomActionButtonOption customButtonFunction = WrappedModalCustomActionButtonOption.None,
                                           Func<bool>? preventCloseIfFalseFunction = null)
        {
            WrappedViewModel = wrappedViewModel;
            OnPropertyChanged(nameof(WrappedViewModel));
            
            this.customAction = customAction;
            OnPropertyChanged(nameof(HasCustomAction));


            if (customAction != null) 
            {
                switch (customButtonFunction)
                {
                    case WrappedModalCustomActionButtonOption.SaveAndClose:
                        HasCustomAddAndCloseAction = false;
                        HasCustomSaveAndCloseAction = true;
                        break;
                    case WrappedModalCustomActionButtonOption.AddAndClose:
                        HasCustomAddAndCloseAction = true;
                        HasCustomSaveAndCloseAction = false;
                        break;
                    default:
                        throw new Exception($"Custom action has no Option Determined Or is not Supported - Option Selected-- {nameof(WrappedModalCustomActionButtonOption)}:{customButtonFunction}");
                }
            }

            //Subscribe to prevent Closure with the given Function when false is returned
            this.preventCloseIfFalseFunction = preventCloseIfFalseFunction;
            if (this.preventCloseIfFalseFunction != null)
            {
                closeModalService.ModalClosing += CloseModalService_ModalClosing;
            }

            Title = string.IsNullOrWhiteSpace(title)
                ? $"Modal for {WrappedViewModel.GetType().Name}"
                : title;
        }

        [RelayCommand]
        private void CustomAction()
        {
            try
            {
                customAction?.Invoke();
                closeModalService.CloseModal(this);
            }
            catch (Exception ex)
            {
                MessageService.Error(ex.Message, "Error");
                return;
            }
        }

        [RelayCommand]
        private void CloseModal()
        {
            closeModalService.CloseModal(this);
        }

        //IF MADE TRANSIENT WE HAVE TO DISPOSE ON CLOSING WINDOW . CURRENTLY ITS SINGLETON DOES NOT NEED DISPOSING
        private bool _disposed;
        

        public override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                closeModalService.ModalClosing -= CloseModalService_ModalClosing;
                //DO NOT DISPOSE WRAPPED VIEWMODEL HERE , ITS ALWAYS PASSED AROUND ONLY FOR CHANGES . CONSUMERS SHOULD DISPOSE IT IF NEEDED
                //JUST SET THE WRAPPED VIEWMODEL TO NULL ONCE THIS IS DISPOSED
                if (ShouldDisposeWrappedViewModel) WrappedViewModel?.Dispose();
                //The Wrapped Viewmodel leaves more than the ModalViewModel
                WrappedViewModel = null;
                customAction = null;
                //if (WrappedViewModel?.IsDisposable is true)
                //{
                //    WrappedViewModel.Dispose();
                //}
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            base.Dispose(disposing);
        }
    }

    public enum WrappedModalCustomActionButtonOption
    {
        None = 0,
        /// <summary>
        /// The Button of the Modal wrapping the Vm will be a Save and Close Button
        /// Any Exceptions thrown by the Action will be displayed and the Modal will not Close
        /// </summary>
        SaveAndClose = 1,
        /// <summary>
        /// The Button of the Modal wrapping the Vm will be an Add and Close Button
        /// Any Exceptions thrown by the Action will be displayed and the Modal will not Close
        /// </summary>
        AddAndClose = 2
    }
}

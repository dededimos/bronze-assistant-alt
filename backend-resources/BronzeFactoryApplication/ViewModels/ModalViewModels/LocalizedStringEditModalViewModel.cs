using BronzeFactoryApplication.ViewModels.AccessoriesViewModels;
using CommonInterfacesBronze;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class LocalizedStringEditModalViewModel : ModalViewModel
    {
        private readonly CloseModalService closeModalService;
        private readonly EditItemContext<LocalizedString> editContext = new(new LocalizedStringEqualityComparer());

        [ObservableProperty]
        private LocalizedStringViewModel? localizedStringVm;

        public bool HasMadeChanges { get; set; } = false;

        public LocalizedStringEditModalViewModel(CloseModalService closeModalService)
        {
            this.closeModalService = closeModalService;
            closeModalService.ModalClosing += OnModalClosing;
        }

        private void OnModalClosing(object? sender, ModalClosingEventArgs e)
        {
            if (e.ClosingModal == this && editContext.HasUnsavedChanges())
            {
                if (LocalizedStringVm != null &&
                    MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.OK)
                {
                    LocalizedStringVm.PropertyChanged -= LocalizedStringVm_PropertyChanged;
                    LocalizedStringVm?.SetModel(editContext.GetLastSave()!);
                    HasMadeChanges = false;
                }
                else
                {
                    e.ShouldCancelClose = true;
                }
            }
        }

        public void InitilizeModal(LocalizedStringViewModel localizedString, string modalTitle)
        {
            this.LocalizedStringVm = localizedString;
            editContext.SetUndoStore(localizedString.GetModel());
            Title = modalTitle;
            LocalizedStringVm.PropertyChanged += LocalizedStringVm_PropertyChanged;
        }

        /// <summary>
        /// Save any Edits Made to the LocalizedString ViewModel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocalizedStringVm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (LocalizedStringVm is not null)
            {
                editContext.PushEdit(LocalizedStringVm.GetModel());
            }
        }

        [RelayCommand]
        private void Undo()
        {
            try
            {
                LocalizedStringVm?.SetModel(editContext.Undo());
            }
            catch (Exception ex)
            {
                MessageService.DisplayException(ex);
            }
        }

        [RelayCommand]
        private void FullUndo()
        {
            LocalizedStringVm?.SetModel(editContext.FullUndo() ?? new());
        }

        [RelayCommand]
        private void SaveAndClose()
        {
            editContext.SaveCurrentState();
            HasMadeChanges = true;
            closeModalService.CloseModal(this);
        }


        private bool _disposed;
        public override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)//Managed Resources
            {
                closeModalService.ModalClosing -= OnModalClosing;
                if (LocalizedStringVm is not null)
                {
                    LocalizedStringVm.PropertyChanged -= LocalizedStringVm_PropertyChanged;
                }
            }

            //object has been disposed
            _disposed = true;
            base.Dispose(disposing);

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            //base.Dispose(disposing);
        }
    }
}

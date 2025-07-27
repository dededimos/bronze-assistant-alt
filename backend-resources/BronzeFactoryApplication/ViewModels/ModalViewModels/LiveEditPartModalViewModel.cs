using BronzeFactoryApplication.ViewModels.CabinsViewModels.PartsViewModels.Parts;

namespace BronzeFactoryApplication.ViewModels.ModalViewModels
{
    public partial class LiveEditPartModalViewModel : ModalViewModel
    {
        private readonly CloseModalService closeModalService;

        /// <summary>
        /// The Part being Edited
        /// </summary>
        [ObservableProperty]
        private LiveEditPartViewModel partEditViewModel = new();
        /// <summary>
        /// The Spot being edited
        /// </summary>
        [ObservableProperty]
        private PartSpot spotUnderEdit;
        /// <summary>
        /// The object that has requested the Edit of the Part
        /// </summary>
        [ObservableProperty]
        private PartsViewModel? editSender;

        public LiveEditPartModalViewModel(CloseModalService closeModalService)
        {
            this.closeModalService = closeModalService;
            this.closeModalService.ModalClosing += CloseModalService_ModalClosing; 
        }

        private void CloseModalService_ModalClosing(object? sender, ModalClosingEventArgs e)
        {
            //Inform the sender vm , the certain spot has been changed
            if(e.ClosingModal == this && PartEditViewModel.HasChanges())
            {
                EditSender?.InformSpotPartChanged(SpotUnderEdit);
            }
        }

        public void SetPart(LiveEditPartViewModel partVm,PartSpot spotUnderEdit,PartsViewModel editSender)
        {
            Title = "lngEditPartModalTitle".TryTranslateKey();
            PartEditViewModel = partVm;
            SpotUnderEdit = spotUnderEdit;
            EditSender = editSender;
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
                this.closeModalService.ModalClosing -= CloseModalService_ModalClosing;
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

using BronzeFactoryApplication.ApplicationServices.MessangerService;
using CommunityToolkit.Diagnostics;
using MirrorsLib;
using MirrorsLib.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.MirrorOrdersViewModels
{
    public partial class AddRowToMirrorsOrderModalViewModel : ModalViewModel
    {

        public AddRowToMirrorsOrderModalViewModel(MirrorsOrderViewModel order,
            Func<MirrorSynthesisEditorWithDrawViewModel> mirrorFactory,
            Func<MirrorOrderRowViewModel> rowFactory,
            CloseModalService closeModalService,
            IMessenger messenger)
        {
            this.order = order;
            this.closeModalService = closeModalService;
            this.messenger = messenger;
            Row = rowFactory.Invoke();
            MirrorToAdd = mirrorFactory.Invoke();
            Title = "lngAddMirrorToOrder".TryTranslateKeyWithoutError();
        }
        private readonly MirrorsOrderViewModel order;
        private readonly CloseModalService closeModalService;
        private readonly IMessenger messenger;
        private readonly MirrorSynthesisValidator mirrorValidator = new();

        public MirrorOrderRowViewModel Row { get; set; }
        public MirrorSynthesisEditorWithDrawViewModel MirrorToAdd { get; set; }

        [RelayCommand]
        private void TryAddRowAndClose()
        {
            var mirror = MirrorToAdd.MirrorEditor.GetModel();
            var validationResult = mirrorValidator.Validate(mirror);
            if (!validationResult.IsValid)
            {
                MessageService.Warning($"Mirror is not Valid with Errors:{Environment.NewLine}{Environment.NewLine}{string.Join(Environment.NewLine, validationResult.Errors.Select(e=> $"{validationResult.Errors.IndexOf(e) + 1}.{e.ErrorCode}"))}", 
                    "Mirror Validation Failed");
                return;
            }
            if (Row.Quantity <= 0)
            {
                MessageService.Warning($"Quantity cannot be zero or less","Invalid Quantity");
                return;
            }
            if (Row.Quantity > 9)
            {
                var questionResult = MessageService.Question(
                    $"The Quantity inserted is bigger than usual {Environment.NewLine}{Environment.NewLine}{Row.Quantity}pcs{Environment.NewLine}{Environment.NewLine} Are you sure you want to Continue ?",
                    "Bigger quantity than usual",
                    "Continue",
                    "Cancel");

                if (questionResult == MessageBoxResult.Cancel) return;
            }
            if (string.IsNullOrEmpty(Row.RefPAOPAM))
            {
                MessageService.Warning("The Reference PAO - PAM is Empty", "Invalid PAO - PAM");
                return;
            }

            var mirrorToAdd = MirrorToAdd.MirrorEditor.GetModel();
            order.AddRow(mirrorToAdd, Row.Notes, Row.Quantity, Row.RefPAOPAM);
            //inform listeners that a mirror was added to the order
            messenger.Send(new MirrorAddedToOrderMessage(mirrorToAdd.GetDeepClone()));
            closeModalService.CloseModal(this);
        }

        /// <summary>
        /// Set the Mirror that will be added
        /// </summary>
        /// <param name="mirror"></param>
        public void SetMirrorToAdd(MirrorSynthesis mirror)
        {
            MirrorToAdd.MirrorEditor.SetModel(mirror);
            //Default Quantity
            Row.Quantity = 1;
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
                MirrorToAdd.Dispose();
                Row.Dispose();
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            base.Dispose(disposing);
        }
    }
}

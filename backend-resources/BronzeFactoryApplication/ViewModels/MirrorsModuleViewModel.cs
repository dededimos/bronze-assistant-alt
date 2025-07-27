using BronzeFactoryApplication.ApplicationServices.MessangerService;
using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using BronzeFactoryApplication.ViewModels.DrawingsViewModels;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.MirrorOrdersViewModels;
using CommonHelpers.Utilities;
using DocumentFormat.OpenXml.Presentation;
using MirrorsLib;
using MirrorsLib.MirrorsOrderModels;
using MirrorsLib.Repositories;
using MirrorsLib.Services.CodeBuldingService;
using MirrorsLib.Validators;
using MirrorsRepositoryMongoDB.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels
{
    public partial class MirrorsModuleViewModel : BaseViewModel, IRecipient<MirrorAddedToOrderMessage>
    {
        public MirrorsModuleViewModel(
            IMessenger messenger,
            IWrappedViewsModalsGenerator wrappedViewsModalsGenerator,
            MirrorsEntitiesManagmentViewModel mirrorsEntitiesManager,
            IMongoMirrorsRepository mirrorsRepo,
            OperationProgressViewModel progressVm,
            Func<MirrorSynthesisEditorWithDrawViewModel> mirrorEditorVmFactory,
            MirrorsOrdersRepository mirrorsOrdersRepo,
            MirrorsOrderViewModel mirrorsOrderEditor,
            SelectMirrorsOrderViewModel mirrorsOrderSelector,
            OpenAddRowToMirrorOrderModalService openAddRowToMirrorsOrderModalService,
            MirrorCodesParser parser,
            CloseModalService closeModalService)
        {
            this.messenger = messenger;
            this.messenger.RegisterAll(this);
            this.wrappedViewsModalsGenerator = wrappedViewsModalsGenerator;
            this.mirrorsEntitiesManager = mirrorsEntitiesManager;
            this.mirrorsRepo = mirrorsRepo;
            this.progressVm = progressVm;
            this.mirrorsOrdersRepo = mirrorsOrdersRepo;

            MirrorsOrderEditor = mirrorsOrderEditor;
            MirrorsOrderEditor.SetModel(new());

            MirrorsOrderSelector = mirrorsOrderSelector;
            this.openAddRowToMirrorsOrderModalService = openAddRowToMirrorsOrderModalService;
            this.parser = parser;
            this.closeModalService = closeModalService;
            this.MirrorEditor = mirrorEditorVmFactory.Invoke();

            MirrorsOrderSelector.OrderSelected += MirrorsOrderSelector_OrderSelected;
        }

        private void MirrorsOrderSelector_OrderSelected(object? sender, MirrorsOrder e)
        {
            if (MirrorsOrderEditor.HasUnsavedChanges && MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel) return;
            MirrorsOrderEditor.SetModel(e);
            closeModalService.CloseModal(MirrorsOrderSelector.ModalId);
        }

        private readonly IMessenger messenger;
        private readonly IWrappedViewsModalsGenerator wrappedViewsModalsGenerator;
        private readonly MirrorsEntitiesManagmentViewModel mirrorsEntitiesManager;
        private readonly OperationProgressViewModel progressVm;
        private readonly MirrorsOrdersRepository mirrorsOrdersRepo;
        private readonly OpenAddRowToMirrorOrderModalService openAddRowToMirrorsOrderModalService;
        private readonly MirrorCodesParser parser;
        private readonly CloseModalService closeModalService;
        private readonly IMongoMirrorsRepository mirrorsRepo;
        private readonly MirrorSynthesisValidator mirrorValidator = new();
        private MirrorSynthesis? lastMirrorAddedToOrder;
        public override bool IsDisposable => false;

        [ObservableProperty]
        private string enteredCode = string.Empty;

        [RelayCommand]
        private void TryParseEnteredCode()
        {
            try
            {
                var parsedMirror = parser.ParseCodeToMirror(EnteredCode);
                MirrorEditor.MirrorEditor.SetModel(parsedMirror);
            }
            catch (Exception ex)
            {
                MessageService.DisplayException(ex);
            }
        }


        [ObservableProperty]
        private string refPaoPamMirrorRow = string.Empty;

        /// <summary>
        /// The Editor takes control of how the Mirror is Changed and how its Drawn along with all the Drawing Options
        /// </summary>
        public MirrorSynthesisEditorWithDrawViewModel MirrorEditor { get; set; }
        public bool CanRemakePreviousMirror { get => lastMirrorAddedToOrder is not null; }
        public MirrorsOrderViewModel MirrorsOrderEditor { get; }
        public SelectMirrorsOrderViewModel MirrorsOrderSelector { get; }

        /// <summary>
        /// Once a Mirror is Added to an Order , the MirrorEditor is Reset and the latest Mirror is stored here for retrieval if needed
        /// </summary>
        /// <param name="message"></param>
        public void Receive(MirrorAddedToOrderMessage message)
        {
            lastMirrorAddedToOrder = message.MirrorAddedToOrder;
            OnPropertyChanged(nameof(CanRemakePreviousMirror));
            RemakeLastAddedMirrorCommand.NotifyCanExecuteChanged();
            MirrorEditor.MirrorEditor.ResetMirrorCommand.Execute(null);
        }

        [RelayCommand]
        private void OpenEntitiesManagment()
        {
            wrappedViewsModalsGenerator.OpenModal(mirrorsEntitiesManager, $"{nameof(MirrorsEntitiesManagmentViewModel).TryTranslateKey()}");
        }
        [RelayCommand]
        private void OpenSelectMirrorsOrder()
        {
            wrappedViewsModalsGenerator.OpenModal(MirrorsOrderSelector,
                                                  "lngSelectMirrorsOrder".TryTranslateKey());
        }
        [RelayCommand]
        private void OpenAddRowToOrder()
        {
            var mirror = MirrorEditor.MirrorEditor.GetModel();
            var validationResult = mirrorValidator.Validate(mirror);
            if (!validationResult.IsValid)
            {
                MessageService.Warning($"Mirror is not Valid with Errors:{Environment.NewLine}{Environment.NewLine}{string.Join(Environment.NewLine, validationResult.Errors.Select(e => $"{validationResult.Errors.IndexOf(e) + 1}.{e.ErrorCode}"))}",
                    "Mirror Validation Failed");
                return;
            }
            openAddRowToMirrorsOrderModalService.OpenModal(mirror, RefPaoPamMirrorRow);
        }
        [RelayCommand]
        private void NewOrder()
        {
            if (MirrorsOrderEditor.HasUnsavedChanges &&
                MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
            {
                return;
            }
            else
            {
                MirrorsOrderEditor.SetModel(new());
                //Open the modal for the Order Details
                MirrorsOrderEditor.EditOrderDetailsCommand.Execute(null);
            }
        }
        [RelayCommand(CanExecute =nameof(CanRemakePreviousMirror))]
        private void RemakeLastAddedMirror()
        {
            if (lastMirrorAddedToOrder is null)
            {
                return;
            }
            MirrorEditor.MirrorEditor.SetModel(lastMirrorAddedToOrder);
        }

        [RelayCommand]
        private async Task BuildMirrorsCache()
        {
            IsBusy = true;
            progressVm.SetNewOperation("Building Mirrors Cache", 10);
            try
            {
                //Subscribe to the Progress Class to send retrieve the message whenever a cache building finishes
                //this in turn changes the message and progress on the ProgressViewModel
                var progress = new Progress<string>(progressMessage => progressVm.ReduceCount(progressMessage));
                await mirrorsRepo.BuildCachesAsync();
            }
            catch (Exception ex)
            {
                MessageService.LogAndDisplayException(ex);

            }
            finally
            {
                IsBusy = false;

                progressVm.MarkAllOperationsFinished();
            }
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
                this.messenger.UnregisterAll(this);
                MirrorEditor.Dispose();
                MirrorsOrderSelector.OrderSelected -= MirrorsOrderSelector_OrderSelected;
                MirrorsOrderSelector.Dispose();
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

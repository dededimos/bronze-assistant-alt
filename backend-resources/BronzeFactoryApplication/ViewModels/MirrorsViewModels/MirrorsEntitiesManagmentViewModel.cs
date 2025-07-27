using BronzeFactoryApplication.ViewModels.HelperViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels;
using CommunityToolkit.Diagnostics;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using MirrorsRepositoryMongoDB.Entities;
using MirrorsRepositoryMongoDB.Repositories;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels
{
    public partial class MirrorsEntitiesManagmentViewModel : BaseViewModel
    {
        private readonly ConstraintsRepoManagerViewModel constraintsManager;
        private readonly CustomMirrorElementsRepoManagerViewModel customElementsManager;
        private readonly MirrorSupportsRepoManagerViewModel supportsManager;
        private readonly MirrorSandblastsRepoManagerViewModel sandblastsManager;
        private readonly MirrorLightElementsRepoManagerViewModel lightElementsManager;
        private readonly MirrorModulesRepoManagerViewModel modulesRepoManager;
        private readonly MirrorSeriesRepoManagerViewModel seriesManager;
        private readonly MirrorPositionsRepoManagerViewModel positionsManager;
        private readonly PositionOptionsRepoManagerViewModel positionOptionsManager;
        private readonly MirrorFinishesRepoManagerViewModel finishesRepoManager;
        private readonly MirrorCustomTraitsRepoManagerViewModel customTraitsRepoManager;
        private readonly MirrorAplicationOptionsRepoManagerViewModel mirrorOptionsRepoManager;
        private readonly CloseModalService closeModalService;

        public override bool IsDisposable => false;
        public bool NoManagerActive => SelectedManager is null;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NoManagerActive))]
        private BaseViewModel? selectedManager;

        [RelayCommand]
        private void SelectManager(EntitiesManagerType managerType)
        {
            //The ViewModels themselves do not know about the change of manager or modal closure
            if (SelectedManager is IMirrorsRepoManagerViewModel manager)
            {
                if (manager.EntityEditor?.HasUnsavedChanges == true && MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
                {
                    return;
                }
                else
                {
                    //Return it to its previous state (scrapping any changes) , otherwise if manager is set to null ,
                    //The Ui Grid connected to Selected Entity get their selection to null and the HasUnsavedChanges retriggers
                    manager.EntityEditor?.FullUndoCommand.Execute(null);
                }
            }

            SelectedManager = managerType switch
            {
                EntitiesManagerType.ConstraintsManagerType => constraintsManager,
                EntitiesManagerType.CustomElementsManagerType => customElementsManager,
                EntitiesManagerType.SupportsManagerType => supportsManager,
                EntitiesManagerType.SandblastsManagerType => sandblastsManager,
                EntitiesManagerType.LightElementsManagerType => lightElementsManager,
                EntitiesManagerType.ModulesManagerType => modulesRepoManager,
                EntitiesManagerType.SeriesManagerType => seriesManager,
                EntitiesManagerType.PositionsManagerType => positionsManager,
                EntitiesManagerType.PositionOptionsManagerType => positionOptionsManager,
                EntitiesManagerType.CustomMirrorTraitsManagerType => customTraitsRepoManager,
                EntitiesManagerType.MirrorFinishElementsManagerType => finishesRepoManager,
                EntitiesManagerType.MirrorApplicationOptionsManagerType => mirrorOptionsRepoManager,
                _ => null,
            };
        }

        public MirrorsEntitiesManagmentViewModel(
            ConstraintsRepoManagerViewModel constraintsManager,
            CustomMirrorElementsRepoManagerViewModel customMirrorElementsManager,
            MirrorSupportsRepoManagerViewModel mirrorSupportsManager,
            MirrorSandblastsRepoManagerViewModel mirrorSandblastsManager,
            MirrorLightElementsRepoManagerViewModel mirrorLightElementsManager,
            MirrorPositionsRepoManagerViewModel mirrorPositionsManager,
            MirrorSeriesRepoManagerViewModel mirrorSeriesManager,
            MirrorModulesRepoManagerViewModel mirrorModuleRepoManager,
            PositionOptionsRepoManagerViewModel mirrorPositionOptionsManager,
            CloseModalService closeModalService,
            MirrorFinishesRepoManagerViewModel finishesRepoManager,
            MirrorCustomTraitsRepoManagerViewModel customTraitsRepoManager,
            MirrorAplicationOptionsRepoManagerViewModel mirrorOptionsRepoManager)
        {
            this.constraintsManager = constraintsManager;
            this.customElementsManager = customMirrorElementsManager;
            this.supportsManager = mirrorSupportsManager;
            this.sandblastsManager = mirrorSandblastsManager;
            this.lightElementsManager = mirrorLightElementsManager;
            this.positionsManager = mirrorPositionsManager;
            this.seriesManager = mirrorSeriesManager;
            this.modulesRepoManager = mirrorModuleRepoManager;
            this.positionOptionsManager = mirrorPositionOptionsManager;
            this.finishesRepoManager = finishesRepoManager;
            this.customTraitsRepoManager = customTraitsRepoManager;
            this.closeModalService = closeModalService;
            this.mirrorOptionsRepoManager = mirrorOptionsRepoManager;
            closeModalService.ModalClosing += CloseModalService_ModalClosing;
        }

        private void CloseModalService_ModalClosing(object? sender, ModalClosingEventArgs e)
        {
            //Reset the Selection of the Manager when a modal wrapping this viewmodel has been closed
            //This way if the modal opens again the Selection is Reset
            if (e.ClosingModal is IWrappedViewModelModalViewModel wrappedModal && wrappedModal.WrappedViewModel == this)
            {
                //The ViewModels themselves do not know about the change of manager or modal closure
                if (SelectedManager is IMirrorsRepoManagerViewModel manager)
                {
                    if (manager.EntityEditor?.HasUnsavedChanges == true && MessageService.Questions.UnsavedChangesContinue() == MessageBoxResult.Cancel)
                    {
                        e.ShouldCancelClose = true;
                        return;
                    }
                    else
                    {
                        //Return it to its previous state (scrapping any changes) , otherwise if manager is set to null ,
                        //The Ui Grid connectd to Selected Entity get their selection to null and the HasUnsavedChanges retriggers
                        manager.EntityEditor?.FullUndoCommand.Execute(null);
                    }
                }
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
                closeModalService.ModalClosing -= CloseModalService_ModalClosing;
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            base.Dispose(disposing);
        }
    }
    public enum EntitiesManagerType
    {
        NoneManagerType = 0,
        ConstraintsManagerType = 1,
        CustomElementsManagerType = 2,
        SupportsManagerType = 3,
        SandblastsManagerType = 4,
        LightElementsManagerType = 5,
        ModulesManagerType = 6,
        SeriesManagerType = 7,
        PositionsManagerType = 8,
        PositionOptionsManagerType = 9,
        MirrorFinishElementsManagerType = 10,
        CustomMirrorTraitsManagerType = 11,
        MirrorApplicationOptionsManagerType = 12,
    }
}

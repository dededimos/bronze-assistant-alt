using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements.Sandblasts;
using MirrorsRepositoryMongoDB.Entities;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels
{
    public partial class MirrorSandblastEntityEditorViewModel : MirrorElementEntityBaseEditorViewModel<MirrorSandblastEntity>, IMirrorEntityEditorViewModel<MirrorSandblastEntity>
    {
        private readonly SandblastEditorViewModelsFactory sandblastVmFactory;

        private MirrorSandblastType? sandblastType;
        public MirrorSandblastType? SandblastType 
        {
            get => sandblastType;
            set
            {
                if (value != sandblastType)
                {
                    sandblastType = value;
                    //Do not pass the change into the undo stack . The Sandblast ModelGetter will change and it will pass changes to it.
                    StopTrackingUndoChanges();
                    //Inform the Vm and Ui that this has changes but without pushing edits into the undo stack
                    OnPropertyChanged(nameof(SandblastType));
                    StartTrackingUndoChanges();
                    Sandblast = sandblastVmFactory.CreateNew(sandblastType ?? MirrorSandblastType.Undefined);
                }
            }
        }

        private IModelGetterViewModel<MirrorSandblastInfo> sandblast = IModelGetterViewModel<MirrorSandblastInfo>.EmptyGetter();
        public IModelGetterViewModel<MirrorSandblastInfo> Sandblast
        {
            get => sandblast;
            private set
            {
                if (sandblast != value)
                {
                    sandblast.PropertyChanged -= Sandblast_PropertyChanged;
                    sandblast.Dispose();
                    sandblast = value;
                    sandblast.PropertyChanged += Sandblast_PropertyChanged;
                    OnPropertyChanged(nameof(Sandblast));
                }
            }
        }

        public MirrorSandblastEntityEditorViewModel(
            SandblastEditorViewModelsFactory sandblastVmFactory,
            Func<MongoDatabaseEntityEditorViewModel> baseEntityFactory,
            IEditModelModalsGenerator editModalsGenerator) : base(baseEntityFactory, editModalsGenerator)
        {
            this.sandblastVmFactory = sandblastVmFactory;
        }

        public override MirrorSandblastEntity CopyPropertiesToModel(MirrorSandblastEntity model)
        {
            base.CopyPropertiesToModel(model);
            model.Sandblast = Sandblast.GetModel();
            return model;
        }
        protected override void SetModelWithoutUndoStore(MirrorSandblastEntity model)
        {
            //Call the MirrorElementEntity base to set the Db Entity and the MirrorElements Properties
            base.SetModelWithoutUndoStore(model);
            //Generate the Correct Editor ModuleViewModel (the factory takes care of the SetModel method)
            var vm = sandblastVmFactory.Create(model.Sandblast);

            //Hack!!!! (change the backing field so that it does not trigger change of the SandblastInfo Vm...)
            sandblastType = model.Sandblast.SandblastType == MirrorSandblastType.Undefined ? null : model.Sandblast.SandblastType;
            OnPropertyChanged(nameof(SandblastType));

            //Set the New ViewModel to the Module Property which is also and IModelGetterViewModel<MirrorModuleInfo>
            Sandblast = vm;
        }


        private void Sandblast_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Sandblast));
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
                Sandblast.PropertyChanged -= Sandblast_PropertyChanged;
                Sandblast.Dispose();
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

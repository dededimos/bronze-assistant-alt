using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements;
using MirrorsRepositoryMongoDB.Entities;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels
{
    public partial class MirrorModuleEntityEditorViewModel : MirrorElementEntityBaseEditorViewModel<MirrorModuleEntity>, IMirrorEntityEditorViewModel<MirrorModuleEntity>
    {
        public MirrorModuleEntityEditorViewModel(
            ModuleEditorViewModelsFactory moduleVmFactory,
            Func<MongoDatabaseEntityEditorViewModel> baseEntityFactory, IEditModelModalsGenerator editModalsGenerator)
            : base(baseEntityFactory, editModalsGenerator)
        {
            this.moduleVmFactory = moduleVmFactory;
        }

        private readonly ModuleEditorViewModelsFactory moduleVmFactory;

        private MirrorModuleType? moduleType;
        public MirrorModuleType? ModuleType
        {
            get => moduleType;
            set
            {
                if (value != moduleType)
                {
                    moduleType = value;
                    //Do not pass the change into the undo stack . The Sandblast ModelGetter will change and it will pass changes to it.
                    StopTrackingUndoChanges();
                    //Inform the Vm and Ui that this has changes but without pushing edits into the undo stack
                    OnPropertyChanged(nameof(ModuleType));
                    StartTrackingUndoChanges();
                    Module = moduleVmFactory.CreateNew(moduleType ?? MirrorModuleType.Undefined);
                }
            }
        }

        private IModelGetterViewModel<MirrorModuleInfo> module = IModelGetterViewModel<MirrorModuleInfo>.EmptyGetter();
        public IModelGetterViewModel<MirrorModuleInfo> Module
        {
            get => module;
            private set
            {
                if (module != value)
                {
                    module.PropertyChanged -= Module_PropertyChanged;
                    module.Dispose();
                    module = value;
                    module.PropertyChanged += Module_PropertyChanged;
                    OnPropertyChanged(nameof(Module));
                }
            }
        }

        public override MirrorModuleEntity CopyPropertiesToModel(MirrorModuleEntity model)
        {
            base.CopyPropertiesToModel(model);
            model.ModuleInfo = Module.GetModel();
            return model;
        }
        protected override void SetModelWithoutUndoStore(MirrorModuleEntity model)
        {
            //Call the MirrorElementEntity base to set the Db Entity and the MirrorElements Properties
            base.SetModelWithoutUndoStore(model);
            //Generate the Correct Editor ModuleViewModel (the factory takes care of the SetModel method)
            var vm = moduleVmFactory.Create(model.ModuleInfo);

            //Hack!!!! (change the backing field so that it does not trigger change of the SandblastInfo Vm...)
            moduleType = model.ModuleInfo.ModuleType == MirrorModuleType.Undefined ? null : model.ModuleInfo.ModuleType;
            OnPropertyChanged(nameof(ModuleType));

            //Set the New ViewModel to the Module Property which is also and IModelGetterViewModel<MirrorModuleInfo>
            Module = vm;
        }

        private void Module_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Module));
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
                module.PropertyChanged -= Module_PropertyChanged;
                module.Dispose();
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

using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Services.PositionService;
using MirrorsRepositoryMongoDB.Entities;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels
{
    public partial class MirrorElementPositionEntityEditorViewModel : MirrorElementEntityBaseEditorViewModel<MirrorElementPositionEntity>, IMirrorEntityEditorViewModel<MirrorElementPositionEntity>
    {
        private readonly PositionInstructionsEditorViewModelsFactory instructionsVmFactory;

        private PositionInstructionsType? instructionsType;
        public PositionInstructionsType? InstructionsType
        {
            get => instructionsType;
            set
            {
                if (value != instructionsType)
                {
                    instructionsType = value;
                    //Do not pass the change into the undo stack . The Sandblast ModelGetter will change and it will pass changes to it.
                    StopTrackingUndoChanges();
                    //Inform the Vm and Ui that this has changes but without pushing edits into the undo stack
                    OnPropertyChanged(nameof(InstructionsType));
                    StartTrackingUndoChanges();
                    Instructions = instructionsVmFactory.CreateNew(instructionsType ?? PositionInstructionsType.UndefinedInstructions);
                }
            }
        }

        private IModelGetterViewModel<PositionInstructionsBase> instructions = IModelGetterViewModel<PositionInstructionsBase>.EmptyGetter();
        public IModelGetterViewModel<PositionInstructionsBase> Instructions
        {
            get => instructions;
            private set
            {
                if (instructions != value)
                {
                    instructions.PropertyChanged -= Instructions_PropertyChanged;
                    instructions.Dispose();
                    instructions = value;
                    instructions.PropertyChanged += Instructions_PropertyChanged;
                    OnPropertyChanged(nameof(Instructions));
                }
            }
        }

        public MirrorElementPositionEntityEditorViewModel(
            PositionInstructionsEditorViewModelsFactory instructionsVmFactory,
            Func<MongoDatabaseEntityEditorViewModel> baseEntityFactory,
            IEditModelModalsGenerator editModalsGenerator) : base(baseEntityFactory, editModalsGenerator)
        {
            this.instructionsVmFactory = instructionsVmFactory;
        }

        public override MirrorElementPositionEntity CopyPropertiesToModel(MirrorElementPositionEntity model)
        {
            base.CopyPropertiesToModel(model);
            model.Instructions = Instructions.GetModel();
            return model;
        }
        protected override void SetModelWithoutUndoStore(MirrorElementPositionEntity model)
        {
            //Call the MirrorElementEntity base to set the Db Entity and the MirrorElements Properties
            base.SetModelWithoutUndoStore(model);
            //Generate the Correct Editor ModuleViewModel (the factory takes care of the SetModel method)
            var vm = instructionsVmFactory.Create(model.Instructions);

            //Hack!!!! (change the backing field ,so that it does not trigger change of the SupportInfo Vm...)
            instructionsType = model.Instructions.InstructionsType == PositionInstructionsType.UndefinedInstructions ? null : model.Instructions.InstructionsType;
            OnPropertyChanged(nameof(InstructionsType));

            //Set the New ViewModel to the Module Property which is also and IModelGetterViewModel<MirrorModuleInfo>
            Instructions = vm;
        }


        private void Instructions_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Instructions));
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
                Instructions.PropertyChanged -= Instructions_PropertyChanged;
                Instructions.Dispose();
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

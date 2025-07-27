using BronzeFactoryApplication.ViewModels.MirrorsViewModels.ModulesViewModels;
using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.MirrorModules;
using MirrorsLib.Repositories;
using MirrorsLib.Services.PositionService;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.PositionInstructionsViewModels
{
    public partial class MirrorElementPositionEditorViewModel : BaseViewModel , IEditorViewModel<MirrorElementPosition>
    {
        public MirrorElementPositionEditorViewModel(Func<MirrorElementInfoEditorViewModel> elementInfoVmFactory,
            PositionInstructionsEditorViewModelsFactory positionInstructionsVmFactory,
            IMirrorsDataProvider dataProvider)
        {
            elementInfo = elementInfoVmFactory.Invoke();
            ElementInfo.PropertyChanged += ElementInfo_PropertyChanged;
            Instructions.PropertyChanged += Instructions_PropertyChanged;
            this.elementInfoVmFactory = elementInfoVmFactory;
            this.positionInstructionsVmFactory = positionInstructionsVmFactory;
        }

        private readonly Func<MirrorElementInfoEditorViewModel> elementInfoVmFactory;
        private readonly PositionInstructionsEditorViewModelsFactory positionInstructionsVmFactory;
        private readonly PositionInstructionsBaseEqualityComparer comparer = new();

        private MirrorElementInfoEditorViewModel elementInfo;
        public MirrorElementInfoEditorViewModel ElementInfo 
        {
            get => elementInfo;
            set
            {
                if (elementInfo is not null) elementInfo.PropertyChanged -= ElementInfo_PropertyChanged;
                var oldValue = elementInfo;
                elementInfo = value;
                oldValue?.Dispose();
                if (elementInfo is not null) elementInfo.PropertyChanged += ElementInfo_PropertyChanged;
                OnPropertyChanged(nameof(ElementInfo));
            }
        }

        private IModelGetterViewModel<PositionInstructionsBase> instructions = IModelGetterViewModel<PositionInstructionsBase>.EmptyGetter();
        public IModelGetterViewModel<PositionInstructionsBase> Instructions 
        {
            get => instructions;
            set
            {
                instructions.PropertyChanged -= Instructions_PropertyChanged;
                var oldValue = instructions;
                instructions = value;
                oldValue.Dispose();
                instructions.PropertyChanged += Instructions_PropertyChanged;
                OnPropertyChanged(nameof(Instructions));
            }
        }

        private MirrorElementPosition? defaultInstructions;

        /// <summary>
        /// Informs consumers that the Element info  have changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ElementInfo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ElementInfo));
        }
        /// <summary>
        /// Informs consumers that the Instructions have changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Instructions_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //Change the Element Info as custom if the instructions where the default and changed
            if (!ElementInfo.IsOverriddenElement && defaultInstructions is not null && !comparer.Equals(defaultInstructions.Instructions,Instructions.GetModel()))
            {
                ElementInfo.MarkElementInfoAsCustom();
            }
            OnPropertyChanged(nameof(Instructions));
        }

        public void SetModel(MirrorElementPosition model)
        {
            SuppressPropertyNotifications();
            var elementInfoVm = elementInfoVmFactory.Invoke();
            elementInfoVm.SetModel(model);
            ElementInfo = elementInfoVm;
            var instructionsVm = positionInstructionsVmFactory.Create(model.Instructions);

            //Save the model instructions if they are default to Compare with the Edited ones in case they change
            if (!model.IsOverriddenElement)
            {
                defaultInstructions = model;
            }
            else
            {
                defaultInstructions = null;
            }

            Instructions = instructionsVm;

            ResumePropertyNotifications();
            OnPropertyChanged("");
        }


        public MirrorElementPosition CopyPropertiesToModel(MirrorElementPosition model)
        {
            throw new NotSupportedException($"{nameof(MirrorElementPositionEditorViewModel)} does not Support Copy Properties to Model");
        }

        public MirrorElementPosition GetModel()
        {
            return new(ElementInfo.GetModel(), Instructions.GetModel());
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
                ElementInfo.PropertyChanged -= ElementInfo_PropertyChanged;
                ElementInfo.Dispose();
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


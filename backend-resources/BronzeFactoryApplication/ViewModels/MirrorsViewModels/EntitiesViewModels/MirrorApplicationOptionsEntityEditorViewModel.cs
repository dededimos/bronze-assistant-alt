using BronzeFactoryApplication.ViewModels.HelperViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.MirrorOptionsViewModels;
using ClosedXML;
using CommonInterfacesBronze;
using MirrorsLib;
using MirrorsLib.Enums;
using MirrorsLib.Services.CodeBuldingService;
using MirrorsRepositoryMongoDB.Entities;
using MongoDbCommonLibrary.CommonEntities;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels
{
    public partial class MirrorApplicationOptionsEntityEditorViewModel : MongoEntityBaseUndoEditorViewModel<MirrorApplicationOptionsEntity>, IMirrorEntityEditorViewModel<MirrorApplicationOptionsEntity>
    {
        public MirrorApplicationOptionsEntityEditorViewModel(Func<MongoDatabaseEntityEditorViewModel> baseEntityVmFactory,
            MirrorApplicationOptionsViewModelsFactory optionsVmFactory)
            : base(baseEntityVmFactory)
        {
            this.optionsVmFactory = optionsVmFactory;
        }

        private readonly MirrorApplicationOptionsViewModelsFactory optionsVmFactory;

        [ObservableProperty]
        private BronzeApplicationType concerningApplication;
        public List<OptionsTranslationHelper> OptionsTypes { get; } = [new(nameof(MirrorCodesBuilderOptions)),];

        private OptionsTranslationHelper? selectedOptionsType;
        public OptionsTranslationHelper? SelectedOptionsType
        {
            get => selectedOptionsType;
            set
            {
                if (selectedOptionsType != value)
                {
                    //Do not pass the change into the undo stack . The ModelGetter will change and it will pass changes to it.
                    selectedOptionsType = value;
                    StopTrackingUndoChanges();
                    //Inform the Vm and Ui that this has changes but without pushing edits into the undo stack
                    OnPropertyChanged(nameof(SelectedOptionsType));
                    StartTrackingUndoChanges();
                    Options = value?.OptionsType switch
                    {
                        nameof(MirrorCodesBuilderOptions) => optionsVmFactory.Create(MirrorCodesBuilderOptions.EmptyCodeBuilderOptions()),
                        nameof(MirrorApplicationOptionsBase) => optionsVmFactory.Create(MirrorApplicationOptionsBase.Empty()),
                        null => optionsVmFactory.Create(MirrorApplicationOptionsBase.Empty()),
                        _ => throw new NotSupportedException($"{value} is not a Supported Options Type  of : {nameof(MirrorApplicationOptionsBase)}"),
                    };
                }
            }
        }

        private void Options_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //Do not trigger it if the Property is an Affix Presenter (just for efficiency :-P )
            if (e.PropertyName != nameof(MirrorCodesBuilderOptionsEditorViewModel.AffixPresenters))
            {
                OnPropertyChanged(nameof(Options));
            }
        }

        private IModelGetterViewModel<MirrorApplicationOptionsBase> options = IModelGetterViewModel<MirrorApplicationOptionsBase>.EmptyGetter();
        public IModelGetterViewModel<MirrorApplicationOptionsBase> Options
        {
            get => options;
            private set
            {
                if (options != value)
                {
                    options.PropertyChanged -= Options_PropertyChanged;
                    options.Dispose();
                    options = value;
                    options.PropertyChanged += Options_PropertyChanged;
                    OnPropertyChanged(nameof(Options));
                }
            }
        }

        public override MirrorApplicationOptionsEntity CopyPropertiesToModel(MirrorApplicationOptionsEntity model)
        {
            base.CopyPropertiesToModel(model);
            model.ConcerningApplication = this.ConcerningApplication;
            model.OptionsType = SelectedOptionsType?.OptionsType ?? nameof(MirrorApplicationOptionsBase);
            model.Options = Options.GetModel();
            return model;
        }

        protected override void SetModelWithoutUndoStore(MirrorApplicationOptionsEntity model)
        {
            base.SetModelWithoutUndoStore(model);
            //Generate the Correct EditorViewModel (the factory takes care of the SetModel method)
            var vm = optionsVmFactory.Create(model.Options);

            //Hack!!!! (change the backing field so that it does not trigger change of the Options Vm...)
            selectedOptionsType = OptionsTypes.FirstOrDefault(o=> o.OptionsType == model.OptionsType); //select one from the list !
            OnPropertyChanged(nameof(SelectedOptionsType));

            this.ConcerningApplication = model.ConcerningApplication;

            Options = vm;
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
                Options.PropertyChanged -= Options_PropertyChanged;
                Options.Dispose();
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Helper to have translated Values of OptionTypes also available
    /// </summary>
    /// <param name="optionsType"></param>
    public class OptionsTranslationHelper(string optionsType)
    {
        public string OptionsType { get; } = optionsType;
        public string OptionsTranslation { get => OptionsType.TryTranslateKeyWithoutError(); }
    }
}

using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.Services.CodeBuldingService;
using MongoDB.Driver;
using System.Collections.ObjectModel;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.MirrorOptionsViewModels
{
    public partial class MirrorCodesBuilderOptionsEditorViewModel : MirrorApplicationOptionsBaseViewModel, IEditorViewModel<MirrorCodesBuilderOptions>
    {
        public MirrorCodesBuilderOptionsEditorViewModel(Func<ElementCodeAffixOptionsEditorViewModel> elementAffixOptionsVmFactory,
            IWrappedViewsModalsGenerator modalsGenerator)
        {
            SelectedAffixOptionsOfElement = elementAffixOptionsVmFactory.Invoke();
            SelectedAffixOptionsOfModule = elementAffixOptionsVmFactory.Invoke();
            OptionsType = nameof(MirrorCodesBuilderOptions);
            this.modalsGenerator = modalsGenerator;
        }

        private readonly IWrappedViewsModalsGenerator modalsGenerator;

        /// <summary>
        /// The Affix Presenters to Help on Presenting the Code Structure
        /// </summary>
        public List<AffixPresenter> AffixPresenters { get => this.GetModel().GetPresenterAffixes(); }

        public ObservableCollection<SeparatorPositionHelper> Separators { get; } = [];
        [ObservableProperty]
        private int selectedSeparatorPositionToAdd  = 0;
        [ObservableProperty]
        private string selectedSeparatorToAdd = "-";

        [RelayCommand]
        private void AddSeperator()
        {
            if (Separators.Any(s=> s.Position == SelectedSeparatorPositionToAdd))
            {
                MessageService.Warning($"There is already a Seperator in Position:{SelectedSeparatorPositionToAdd}{Environment.NewLine}{Environment.NewLine}Please Select another Position","Position already filled");
                return;
            }
            else if (string.IsNullOrEmpty(SelectedSeparatorToAdd))
            {
                MessageService.Warning($"A Seperator cannot be Empty{Environment.NewLine}{Environment.NewLine}Please Define the Seperator to be Added", "Undefined Seperator");
                return;
            }
            else
            {
                Separators.Add(new(SelectedSeparatorToAdd, SelectedSeparatorPositionToAdd));
                OnPropertyChanged(nameof(Separators));
                OnPropertyChanged(nameof(AffixPresenters));
            } 
        }
        [RelayCommand]
        private void RemoveSeparator(SeparatorPositionHelper separatorPosition)
        {
            bool removed = Separators.Remove(separatorPosition);
            if (!removed) MessageService.Warning($"Unexpected Error Seperator-Position Combo not Found ... {separatorPosition.Position}--{separatorPosition.Separator}", "Unexpected Error");
            OnPropertyChanged(nameof(Separators));
            OnPropertyChanged(nameof(AffixPresenters));
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AffixPresenters))]
        private MirrorCodeDimensionsUnit dimensionUnit;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AffixPresenters))]
        private int lengthAffixPosition;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AffixPresenters))]
        private int heightAffixPosition;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsComplexCodeOptionsEditable))]
        private bool glassOnlyOptions;
        partial void OnGlassOnlyOptionsChanged(bool value)
        {
            if(value is true)
            {
                ComplexCodeOptions = false;
            }
        }

        [ObservableProperty]
        private bool complexCodeOptions;
        public bool IsComplexCodeOptionsEditable { get => !GlassOnlyOptions; }

        [ObservableProperty]
        private string truncatedTrailingCharachter = string.Empty;

        public ObservableCollection<CodeOptionsAffixHelper> MirrorPropertiesCodeAffixes { get; set; } = [];
        [ObservableProperty]
        private MirrorCodeOptionsElementType elementTypeToAddAffixOptions;
        public ElementCodeAffixOptionsEditorViewModel SelectedAffixOptionsOfElement { get; }

        /// <summary>
        /// Copies an ElementCodeAffix Options into the Editor of SelectedAffixOptionsOfElement 
        /// </summary>
        /// <param name="selectedAffixFromList"></param>
        [RelayCommand]
        private void CopyListAffixToSelectedAffixOfElement(CodeOptionsAffixHelper selectedAffixFromList) 
        {
            ElementTypeToAddAffixOptions = selectedAffixFromList.ElementType;
            SelectedAffixOptionsOfElement.SetModel(selectedAffixFromList.AffixOptions);
        }
        [RelayCommand]
        private void AddMirrorPropertyCodeAffix()
        {
            if (MirrorPropertiesCodeAffixes.Any(a=> a.ElementType == ElementTypeToAddAffixOptions))
            {
                MessageService.Warning($"There is already an Element of Type {ElementTypeToAddAffixOptions} in the List of Mirror Properties' Affixes","Element Already Present");
                return;
            }
            var affix = SelectedAffixOptionsOfElement.GetModel();
            if (affix.CodeType == MirrorElementAffixCodeType.NoneCode && string.IsNullOrWhiteSpace(affix.OverrideCodeString))
            {
                MessageService.Warning($"The Affix cannot have both {nameof(ElementCodeAffixOptions.OverrideCodeString)} : Empty and {nameof(ElementCodeAffixOptions.CodeType)} : {MirrorElementAffixCodeType.NoneCode}","Affix without Code String Instructions");
                return;
            }

            MirrorPropertiesCodeAffixes.Add(new(ElementTypeToAddAffixOptions, affix));
            OnPropertyChanged(nameof(MirrorPropertiesCodeAffixes));
            OnPropertyChanged(nameof(AffixPresenters));
        }
        [RelayCommand]
        private void RemoveMirrorPropertyCodeAffix(CodeOptionsAffixHelper codeOptionsAffixHelper)
        {
            bool removed = MirrorPropertiesCodeAffixes.Remove(codeOptionsAffixHelper);
            if (!removed) MessageService.Warning($"Unexpected Error Element-Affix Combo not Found for : {codeOptionsAffixHelper.ElementType}", "Unexpected Error");
            OnPropertyChanged(nameof(MirrorPropertiesCodeAffixes));
            OnPropertyChanged(nameof(AffixPresenters));
        }
        [RelayCommand]
        private void OpenEditSelectedMirrorPropertyAffix()
        {
            modalsGenerator.OpenModal(SelectedAffixOptionsOfElement, "lngEditCodeAffix".TryTranslateKeyWithoutError(),()=> { },WrappedModalCustomActionButtonOption.SaveAndClose);
        }

        public ObservableCollection<ModuleTypeAffixHelper> SpecificModulesCodeAffixes { get; } = [];
        [ObservableProperty]
        private MirrorModuleType moduleTypeToAddAffixOptions;
        public ElementCodeAffixOptionsEditorViewModel SelectedAffixOptionsOfModule { get; }
        /// <summary>
        /// Copies an ElementCodeAffix Options into the Editor of SelectedAffixOptionsOfModule 
        /// </summary>
        /// <param name="selectedAffixFromList"></param>
        [RelayCommand]
        private void CopyListAffixToSelectedAffixOfModule(ModuleTypeAffixHelper selectedAffixFromList)
        {
            ModuleTypeToAddAffixOptions = selectedAffixFromList.ModuleType;
            SelectedAffixOptionsOfModule.SetModel(selectedAffixFromList.AffixOptions);
        }
        [RelayCommand]
        private void AddModuleTypeCodeAffix()
        {
            if (ModuleTypeToAddAffixOptions == MirrorModuleType.Undefined)
            {
                MessageService.Warning($"Cannot add an Undefined Module Type", "Module Type Undefined");
                return;
            }
            if (SpecificModulesCodeAffixes.Any(a => a.ModuleType == ModuleTypeToAddAffixOptions))
            {
                MessageService.Warning($"There is already a Module of Type {ModuleTypeToAddAffixOptions} in the List of Specific Modules' Affixes", "Module Type Already Present");
                return;
            }
            var affix = SelectedAffixOptionsOfModule.GetModel();
            if (affix.CodeType == MirrorElementAffixCodeType.NoneCode && string.IsNullOrWhiteSpace(affix.OverrideCodeString))
            {
                MessageService.Warning($"The Affix cannot have both {nameof(ElementCodeAffixOptions.OverrideCodeString)} : Empty and {nameof(ElementCodeAffixOptions.CodeType)} : {MirrorElementAffixCodeType.NoneCode}", "Affix without Code String Instructions");
                return;
            }

            SpecificModulesCodeAffixes.Add(new(ModuleTypeToAddAffixOptions, affix));
            OnPropertyChanged(nameof(SpecificModulesCodeAffixes));
            OnPropertyChanged(nameof(AffixPresenters));
        }
        [RelayCommand]
        private void RemoveModuleTypeCodeAffix(ModuleTypeAffixHelper moduleTypeAffixHelper)
        {
            bool removed = SpecificModulesCodeAffixes.Remove(moduleTypeAffixHelper);
            if (!removed) MessageService.Warning($"Unexpected Error Module-Affix Combo not Found for : {moduleTypeAffixHelper.ModuleType}", "Unexpected Error");
            OnPropertyChanged(nameof(SpecificModulesCodeAffixes));
            OnPropertyChanged(nameof(AffixPresenters));
        }
        [RelayCommand]
        private void OpenEditSpecificModulePropertyAffix()
        {
            modalsGenerator.OpenModal(SelectedAffixOptionsOfModule, "lngEditCodeAffix".TryTranslateKeyWithoutError(), () => { }, WrappedModalCustomActionButtonOption.SaveAndClose);
        }

        [RelayCommand]
        private void SetDefaultGlassCodeOptions()
        {
            if (MessageService.Question($"This will change all the Current Options with the :{Environment.NewLine}{Environment.NewLine}'Default Glass Code Options'{Environment.NewLine}{Environment.NewLine}whould you like to Proceed ?",
                                        "Set Default Glass Code Options",
                                        "Ok",
                                        "Cancel") == MessageBoxResult.OK)
            {
                SetModel(MirrorCodesBuilderOptions.DefaultGlassCodeOptions());
            }
        }
        [RelayCommand]
        private void SetDefaultMirrorCodeOptions()
        {
            if (MessageService.Question($"This will change all the Current Options with the :{Environment.NewLine}{Environment.NewLine}'Default Mirror Code Options'{Environment.NewLine}{Environment.NewLine}whould you like to Proceed ?",
                                        "Set Default Mirror Code Options",
                                        "Ok",
                                        "Cancel") == MessageBoxResult.OK)
            {
                SetModel(MirrorCodesBuilderOptions.DefaultMirrorCodeOptions());
            }
        }

        public MirrorCodesBuilderOptions CopyPropertiesToModel(MirrorCodesBuilderOptions model)
        {
            model.Separators = this.Separators.ToDictionary(s => s.Position, s => s.Separator);
            model.DimensionsUnit = this.DimensionUnit;
            model.LengthAffixPosition = this.LengthAffixPosition;
            model.HeightAffixPosition = this.HeightAffixPosition;
            model.GlassOnlyOptions = this.GlassOnlyOptions;
            model.ComplexCodeOptions = this.ComplexCodeOptions;
            model.TruncatedTrailingCharachter = this.TruncatedTrailingCharachter.FirstOrDefault();
            model.MirrorPropertiesCodeAffix = this.MirrorPropertiesCodeAffixes.ToDictionary(mp => mp.ElementType, mp => mp.AffixOptions.GetDeepClone());
            model.SpecificModuleCodeAffix = this.SpecificModulesCodeAffixes.ToDictionary(sm => sm.ModuleType, sm => sm.AffixOptions.GetDeepClone());
            return model;
        }

        public MirrorCodesBuilderOptions GetModel()
        {
            return CopyPropertiesToModel(new());
        }

        public void SetModel(MirrorCodesBuilderOptions model)
        {
            SuppressPropertyNotifications();
            Separators.Clear();
            foreach (var s in model.Separators) Separators.Add(new(s.Value, s.Key));

            this.DimensionUnit = model.DimensionsUnit;
            this.LengthAffixPosition = model.LengthAffixPosition;
            this.HeightAffixPosition = model.HeightAffixPosition;
            this.ComplexCodeOptions = model.ComplexCodeOptions;
            this.GlassOnlyOptions = model.GlassOnlyOptions;
            this.TruncatedTrailingCharachter = model.TruncatedTrailingCharachter.ToString();

            MirrorPropertiesCodeAffixes.Clear();
            foreach (var a in model.MirrorPropertiesCodeAffix) MirrorPropertiesCodeAffixes.Add(new(a.Key, a.Value.GetDeepClone()));

            SpecificModulesCodeAffixes.Clear();
            foreach (var m in model.SpecificModuleCodeAffix) SpecificModulesCodeAffixes.Add(new(m.Key, m.Value.GetDeepClone()));
            
            ResumePropertyNotifications();
            OnPropertyChanged("");
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

            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            base.Dispose(disposing);
        }
    }
    public class SeparatorPositionHelper(string separator, int position)
    {
        public string Separator { get; } = separator;
        public int Position { get; } = position;
    }
    public class CodeOptionsAffixHelper(MirrorCodeOptionsElementType elementType, ElementCodeAffixOptions affixOptions)
    {
        public MirrorCodeOptionsElementType ElementType { get; } = elementType;
        public ElementCodeAffixOptions AffixOptions { get; } = affixOptions;
    }
    public class ModuleTypeAffixHelper(MirrorModuleType moduleType, ElementCodeAffixOptions affixOptions)
    {
        public MirrorModuleType ModuleType { get; } = moduleType;
        public ElementCodeAffixOptions AffixOptions { get; } = affixOptions;
    }
    public class CodeOptionsStringHelper(MirrorCodeOptionsElementType elementType, string replacementCodeForEmpty)
    {
        public MirrorCodeOptionsElementType ElementType { get; } = elementType;
        public string ReplacementCodeForEmpty { get; } = replacementCodeForEmpty;
    }
    public class ModuleTypeStringHelper(MirrorModuleType moduleType, string replacementCodeForEmpty)
    {
        public MirrorModuleType ModuleType { get; } = moduleType;
        public string ReplacementCodeForEmpty { get; } = replacementCodeForEmpty;
    }
}

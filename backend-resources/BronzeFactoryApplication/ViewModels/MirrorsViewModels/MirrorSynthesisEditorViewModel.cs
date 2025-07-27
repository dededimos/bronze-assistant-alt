using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.ModulesViewModels.MirrorModuleWithElementInfoVms;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.PositionInstructionsViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.SandblastViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.ShapeInfoViewModels;
using BronzeFactoryApplication.ViewModels.MirrorsViewModels.SupportsViewModels;
using CommonInterfacesBronze;
using MirrorsLib;
using MirrorsLib.Enums;
using MirrorsLib.Helpers;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.MirrorModules;
using MirrorsLib.MirrorElements.Sandblasts;
using MirrorsLib.MirrorElements.Supports;
using MirrorsLib.Repositories;
using MirrorsLib.Services;
using MirrorsLib.Services.PositionService;
using ShapesLibrary;
using ShapesLibrary.Enums;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels
{
    public partial class MirrorSynthesisEditorViewModel : BaseViewModel, IEditorViewModel<MirrorSynthesis>
    {
        public MirrorSynthesisEditorViewModel(ShapeInfoVmsFactory shapesVmFactory,
                                          IMirrorsDataProvider dataProvider,
                                          IWrappedViewsModalsGenerator wrappedViewModelModalsGenerator,
                                          PositionInstructionsEditorViewModelsFactory positionInstructionsVmFactory,
                                          Func<MirrorSandblastEditorViewModel> sandblastEditorFactory,
                                          Func<MirrorModuleEditorVmBase> modulesEditorsFactory,
                                          Func<MirrorElementPositionEditorViewModel> positionsEditorsFactory,
                                          Func<MirrorSupportEditorViewModel> supportEditorFactory,
                                          Func<MirrorSynthesisBuilder> mirrorBuilderFactory)
        {
            this.shapesVmFactory = shapesVmFactory;
            this.dataProvider = dataProvider;
            this.wrappedViewModelModalsGenerator = wrappedViewModelModalsGenerator;
            this.sandblastEditorFactory = sandblastEditorFactory;
            this.modulesEditorsFactory = modulesEditorsFactory;
            this.positionsEditorsFactory = positionsEditorsFactory;
            this.supportEditorFactory = supportEditorFactory;
            this.positionInstructionsVmFactory = positionInstructionsVmFactory;
            this.mirrorBuilder = mirrorBuilderFactory.Invoke();
            this.dataProvider.ProviderDataChanged += OnMirrorsDataProviderDataChanged;
        }

        private readonly ShapeInfoVmsFactory shapesVmFactory;
        private readonly IMirrorsDataProvider dataProvider;
        private readonly IWrappedViewsModalsGenerator wrappedViewModelModalsGenerator;
        private readonly Func<MirrorSandblastEditorViewModel> sandblastEditorFactory;
        private readonly Func<MirrorModuleEditorVmBase> modulesEditorsFactory;
        private readonly Func<MirrorElementPositionEditorViewModel> positionsEditorsFactory;
        private readonly Func<MirrorSupportEditorViewModel> supportEditorFactory;
        private readonly PositionInstructionsEditorViewModelsFactory positionInstructionsVmFactory;
        private readonly MirrorSynthesisBuilder mirrorBuilder;
        private readonly PositionInstructionsBaseEqualityComparer instructionsComparer = new();

        /// <summary>
        /// An event signalling the Mirror has changed
        /// </summary>
        public event EventHandler? MirrorChanged;
        private void OnMirrorChanged()
        {
            OnPropertyChanged(nameof(GlassCode));
            OnPropertyChanged(nameof(MirrorCode));
            MirrorChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Alerts the Selectable Items have changed
        /// </summary>
        /// <param name="type"></param>
        private void OnMirrorsDataProviderDataChanged(object? sender, Type type)
        {
            if (type == typeof(MirrorSandblast)) OnPropertyChanged(nameof(SelectableSandblasts));
            else if (type == typeof(MirrorModule)) 
            {
                OnPropertyChanged(nameof(SelectableSandblastedMagnifiers));
                OnPropertyChanged(nameof(SelectableSimpleMagnifiers));
                OnPropertyChanged(nameof(SelectableProcesses));
            }
            else if (type == typeof(MirrorSupport)) OnPropertyChanged(nameof(SelectableSupports));
        }

        [RelayCommand]
        private void ResetMirror()
        {
            Shape = BronzeMirrorShape.UndefinedMirrorShape;
            IsMirrorCodeEditable = false;
        }
        [RelayCommand]
        private void ResetMirrorWithShape(BronzeMirrorShape shape)
        {
            Shape = BronzeMirrorShape.UndefinedMirrorShape; //Reset First;
            Shape = shape; // the Set the Shape;
            IsMirrorCodeEditable = false;
        }
                
        public MirrorConstraints Constraints { get => mirrorBuilder.Constraints; }
        public string GlassCode { get => mirrorBuilder.FormulatedMirror.GlassCode; }
        public string MirrorCode 
        { 
            get => mirrorBuilder.FormulatedMirror.Code; 
            set
            {
                if (mirrorBuilder.FormulatedMirror.OverriddenCode != value)
                {
                    mirrorBuilder.OverrideCode(value);
                    OnPropertyChanged(nameof(MirrorCode));
                }
                
            }
        }
        
        [ObservableProperty]
        private bool isMirrorCodeEditable;
        partial void OnIsMirrorCodeEditableChanged(bool value)
        {
            if (value is false)
            {
                mirrorBuilder.StopOverridingCode();
                OnPropertyChanged(nameof(MirrorCode));
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanEditMirror))]
        private BronzeMirrorShape shape;
        partial void OnShapeChanged(BronzeMirrorShape value)
        {
            var shapeInfoType = value.ToShapeInfoType();
            if (shapeInfoType != ShapeInfoType.Undefined)
            {
                var newMirrorShape = shapesVmFactory.Create(shapeInfoType);
                DimensionsInformation = newMirrorShape;
                //if the produced shape is null there is something wrong...
                if (DimensionsInformation == null) throw new Exception("Unexpected Null Reference on DimensionsInformation Property");
            }
            else
            {
                DimensionsInformation = null;
            }
        }
        public bool CanEditMirror { get => Shape != BronzeMirrorShape.UndefinedMirrorShape; }

        public MirrorOrientedShape OrientedShape { get => mirrorBuilder.FormulatedMirror.ShapeType; }

        [ObservableProperty]
        private ShapeInfoBaseVm? dimensionsInformation;
        partial void OnDimensionsInformationChanged(ShapeInfoBaseVm? oldValue, ShapeInfoBaseVm? newValue)
        {
            if (oldValue is not null)
            {
                oldValue.PropertyChanged -= DimensionsInformation_PropertyChanged;
                oldValue.Dispose();
            }

            if (newValue is not null)
            {
                mirrorBuilder.SetDimensions(newValue.GetModel()).FormulateMirror();
                newValue.PropertyChanged += DimensionsInformation_PropertyChanged;
            }
            else
            {
                mirrorBuilder.SetDimensions(ShapeInfo.Undefined()).FormulateMirror();
            }
            OnPropertyChanged(string.Empty);
            OnMirrorChanged();
        }

        /// <summary>
        /// Alerts the Dimensions have changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DimensionsInformation_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //Informs external subscribers as well as the builder that the Dimensions Information have changed;
            mirrorBuilder.SetDimensions(DimensionsInformation?.GetModel() ?? ShapeInfo.Undefined()).FormulateMirror();
            OnPropertyChanged(nameof(DimensionsInformation));
            OnPropertyChanged(nameof(OrientedShape));
            OnPropertyChanged(nameof(CanHaveRoundedCorners));
            OnPropertyChanged(nameof(HasRoundedCorners));
            OnPropertyChanged(nameof(GlassDimensions));
            OnPropertyChanged(nameof(Modules)); //For the Rounded Corners Module
            OnMirrorChanged();
        }

        public ShapeInfo GlassDimensions { get => mirrorBuilder.FormulatedMirror.MirrorGlassShape; }


        [ObservableProperty]
        private MirrorGlassType glassType;
        partial void OnGlassTypeChanged(MirrorGlassType value)
        {
            mirrorBuilder.SetGlassType(value);
            OnMirrorChanged();
        }

        [ObservableProperty]
        private MirrorGlassThickness glassThickness;
        partial void OnGlassThicknessChanged(MirrorGlassThickness value)
        {
            mirrorBuilder.SetGlassThickness(value);
            OnMirrorChanged();
        }

        #region Sandblast
        public MirrorPlacedSandblast? Sandblast
        {
            get => mirrorBuilder.FormulatedMirror.Sandblast;
        }
        public bool CanEditSandblast => Sandblast != null;
        public IEnumerable<MirrorSandblast> SelectableSandblasts { get => dataProvider.GetSandblasts([.. Constraints.AllowedSandblasts]); }

        private MirrorSandblastEditorViewModel? sandblastEditor;
        public MirrorSandblastEditorViewModel? SandblastEditor
        {
            get => sandblastEditor;
            set
            {
                if (sandblastEditor != value)
                {
                    if (sandblastEditor is not null) sandblastEditor.PropertyChanged -= SandblastEditor_PropertyChanged;
                    var oldValue = sandblastEditor;
                    sandblastEditor = value;
                    oldValue?.Dispose();
                    if (sandblastEditor is not null) sandblastEditor.PropertyChanged += SandblastEditor_PropertyChanged;
                    OnPropertyChanged(nameof(SandblastEditor));
                }
            }

        }
        private void SandblastEditor_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (SandblastEditor is null) throw new Exception($"Unexpected Error , Sandblast Editor was Null...");
            //Pass a new Sandblast whenever Editor Cahnges it
            mirrorBuilder.SetSandblast(SandblastEditor.GetModel()).FormulateMirror();
            OnPropertyChanged(nameof(Sandblast));
            OnPropertyChanged(nameof(Support));
            OnPropertyChanged(nameof(Modules));
            OnMirrorChanged();
        }

        [RelayCommand(CanExecute =nameof(CanEditSandblast))]
        private void EditSandblast()
        {
            if (Sandblast is null) throw new Exception($"Unexpected Error , Cannot Edit Null Sandblast...");
            var sandblastVm = sandblastEditorFactory.Invoke();
            sandblastVm.SetModel(new(Sandblast, Sandblast.SandblastInfo));
            SandblastEditor = sandblastVm;
            wrappedViewModelModalsGenerator.OpenModal(SandblastEditor, $"{"lngEdit".TryTranslateKeyWithoutError()} {Sandblast.SandblastInfo.SandblastType.ToString().TryTranslateKeyWithoutError()}");
        }
        [RelayCommand(CanExecute = nameof(CanEditSandblast))]
        private void RemoveSandblast()
        {
            mirrorBuilder.RemoveSandblast().FormulateMirror();
            OnPropertyChanged(nameof(Sandblast));
            EditSandblastCommand.NotifyCanExecuteChanged();
            RemoveSandblastCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(Support));
            OnPropertyChanged(nameof(Modules));
            OnMirrorChanged();
        }
        [RelayCommand]
        private void SelectSandblast(MirrorSandblast sandblast)
        {
            mirrorBuilder.SetSandblast(sandblast).FormulateMirror();
            OnPropertyChanged(nameof(Sandblast));
            EditSandblastCommand.NotifyCanExecuteChanged();
            RemoveSandblastCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(Support));
            OnPropertyChanged(nameof(Modules));
            OnMirrorChanged();
        }

        #endregion

        #region Support

        public MirrorPlacedSupport? Support
        {
            get => mirrorBuilder.FormulatedMirror.Support;
        }
        public bool CanEditSupport => Support != null;
        public IEnumerable<MirrorSupport> SelectableSupports => dataProvider.GetSupports([.. Constraints.AllowedSupports]);

        private MirrorSupportEditorViewModel? supportEditor;
        public MirrorSupportEditorViewModel? SupportEditor
        {
            get => supportEditor;
            set
            {
                if (supportEditor != value)
                {
                    if (supportEditor is not null) supportEditor.PropertyChanged -= SupportEditor_PropertyChanged;
                    var oldValue = supportEditor;
                    supportEditor = value;
                    oldValue?.Dispose();
                    if (supportEditor is not null) supportEditor.PropertyChanged += SupportEditor_PropertyChanged;
                    OnPropertyChanged(nameof(SupportEditor));
                    OnPropertyChanged(nameof(GlassDimensions));
                }
            }

        }
        private void SupportEditor_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (SupportEditor is null) throw new Exception($"Unexpected Error , Support Editor was Null...");
            //Pass a new Support whenever Editor Cahnges it
            mirrorBuilder.SetSupport(SupportEditor.GetModel()).FormulateMirror();
            OnPropertyChanged(nameof(Support));
            OnPropertyChanged(nameof(Sandblast));
            OnPropertyChanged(nameof(Modules));
            OnPropertyChanged(nameof(GlassDimensions));
            OnMirrorChanged();
        }

        [RelayCommand(CanExecute =nameof(CanEditSupport))]
        private void EditSupport()
        {
            if (Support is null) throw new Exception($"Unexpected Error , Cannot Edit Null Support...");
            var supportVm = supportEditorFactory.Invoke();
            supportVm.SetModel(new(Support, Support.SupportInfo,Support.Finish));
            SupportEditor = supportVm;
            wrappedViewModelModalsGenerator.OpenModal(SupportEditor, $"{"lngEdit".TryTranslateKeyWithoutError()} {Support.SupportInfo.SupportType.ToString().TryTranslateKeyWithoutError()}");
        }

        [RelayCommand(CanExecute = nameof(CanEditSupport))]
        private void RemoveSupport()
        {
            mirrorBuilder.RemoveSupport().FormulateMirror();
            OnPropertyChanged(nameof(Support));
            EditSupportCommand.NotifyCanExecuteChanged();
            RemoveSupportCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(Sandblast));
            OnPropertyChanged(nameof(Modules));
            OnPropertyChanged(nameof(GlassDimensions));
            OnMirrorChanged();
        }
        [RelayCommand]
        private void SelectSupport(MirrorSupport support)
        {
            mirrorBuilder.SetSupport(support).FormulateMirror();
            OnPropertyChanged(nameof(Support));
            EditSupportCommand.NotifyCanExecuteChanged();
            RemoveSupportCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(Sandblast));
            OnPropertyChanged(nameof(Modules));
            OnPropertyChanged(nameof(GlassDimensions));
            OnMirrorChanged();
        }

        #endregion

        #region Modules
        public bool HasMagnifierWithLight
        {
            get => mirrorBuilder.FormulatedMirror.ModulesInfo.HasModuleOfType(MirrorModuleType.MagnifierSandblastedModuleType);
            set
            {
                var magnifiers = mirrorBuilder.FormulatedMirror.ModulesInfo.ModulesOfType(MirrorModuleType.MagnifierSandblastedModuleType);
                var has = magnifiers.Count != 0;
                if (has != value)
                {
                    //User Selected not to have and value is opposite so it currently does have so remove all
                    if (value is false) mirrorBuilder.RemoveAllModulesOfType(MirrorModuleType.MagnifierSandblastedModuleType).FormulateMirror();
                    else //Add Default
                    {
                        mirrorBuilder.AddDefaultModule(MirrorModuleType.MagnifierSandblastedModuleType).FormulateMirror();
                    }
                    OnPropertyChanged(nameof(HasMagnifierWithLight));
                    OnPropertyChanged(nameof(Modules));
                    OnMirrorChanged();
                }
            }
        }
        public bool CanHaveMagnifierWithLight { get => mirrorBuilder.CanHaveModuleOfType(MirrorModuleType.MagnifierSandblastedModuleType); }
        public bool HasSimpleMagnifier
        {
            get => mirrorBuilder.FormulatedMirror.ModulesInfo.HasModuleOfType(MirrorModuleType.MagnifierModuleType);
            set
            {
                //TODO MAKE SPECIFIC METHODS FOR ADDING REMOVING MAGNIFIERS
                var magnifiersWithLight = mirrorBuilder.FormulatedMirror.ModulesInfo.ModulesOfType(MirrorModuleType.MagnifierModuleType);
                var has = magnifiersWithLight.Count != 0;
                if (has != value)
                {
                    //User Selected not to have and value is opposite so it currently does have so remove all
                    if (value is false) mirrorBuilder.RemoveAllModulesOfType(MirrorModuleType.MagnifierModuleType).FormulateMirror();
                    else //Add Default
                    {
                        mirrorBuilder.AddDefaultModule(MirrorModuleType.MagnifierModuleType).FormulateMirror();
                    }
                    OnPropertyChanged(nameof(HasSimpleMagnifier));
                    OnPropertyChanged(nameof(Modules));
                    OnMirrorChanged();
                }
            }
        }
        public bool CanHaveSimpleMagnifier { get => mirrorBuilder.CanHaveModuleOfType(MirrorModuleType.MagnifierModuleType); }
        public bool HasProcess
        {
            get => mirrorBuilder.FormulatedMirror.ModulesInfo.HasModuleOfType(MirrorModuleType.ProcessModuleType);
            set
            {
                var processes = mirrorBuilder.FormulatedMirror.ModulesInfo.ModulesOfType(MirrorModuleType.ProcessModuleType);
                var has = processes.Count != 0;
                if (has != value)
                {
                    //User Selected not to have and value is opposite so it currently does have so remove all
                    if (value is false) mirrorBuilder.RemoveAllModulesOfType(MirrorModuleType.ProcessModuleType).FormulateMirror();
                    else //Add Default
                    {
                        mirrorBuilder.AddDefaultModule(MirrorModuleType.ProcessModuleType).FormulateMirror();
                    }
                    OnPropertyChanged(nameof(HasProcess));
                    OnPropertyChanged(nameof(Modules));
                    OnMirrorChanged();
                }
            }
        }
        //Not used Currently - All Mirrors can get a process 
        public bool CanHaveProcess { get => mirrorBuilder.CanHaveModuleOfType(MirrorModuleType.ProcessModuleType); }
        public bool CanHaveRoundedCorners { get => mirrorBuilder.CanHaveModuleOfType(MirrorModuleType.RoundedCornersModuleType); }
        public bool HasRoundedCorners
        {
            get => mirrorBuilder.FormulatedMirror.ModulesInfo.HasModuleOfType(MirrorModuleType.RoundedCornersModuleType);
            set
            {
                var corners = mirrorBuilder.FormulatedMirror.ModulesInfo.ModulesOfType(MirrorModuleType.RoundedCornersModuleType);
                var has = corners.Count != 0;
                if (has != value)
                {
                    //User Selected not to have and value is opposite so it currently does have so remove all
                    if (value is false && DimensionsInformation is RectangleInfoVm rect) rect.SetRadius(RoundedCornersModuleInfo.ZeroCorners());
                    else if (DimensionsInformation is RectangleInfoVm vm) //Add Default
                    {
                        //Find the default and assign the values the Builder will refind it and assign the Element info needed . 
                        //This is clunky but the Dimensions info here are synced with those on the builder , but the builders are not synced back with
                        //the VMs Dimensions info so we have to do this
                        var defaultCorners = dataProvider.GetModulesOfType(MirrorModuleType.RoundedCornersModuleType).FirstOrDefault()
                            ?? throw new Exception("No Default Rounded Corners Module Found for this kind of Mirror...");
                        var defaultCornersModuleInfo = (RoundedCornersModuleInfo)(defaultCorners.ModuleInfo);
                        vm.SetRadius(defaultCornersModuleInfo);
                    }
                    //HasRoundedCorners , Mirror Changed and Foromulation will get Triggered By the Dimensions Information object
                }
            }
        }
        public List<ModuleWithPosition> Modules { get => [..mirrorBuilder.FormulatedMirror.ModulesInfo.GetAllModulesWithPosition(),..mirrorBuilder.FormulatedMirror.ModulesInfo.GetNonPositionableModules().Select(m=> new ModuleWithPosition(m,MirrorElementPosition.NAPositionElement()))]; }

        /// <summary>
        /// Current module Selected in the Datagrid
        /// </summary>
        [ObservableProperty]
        private ModuleWithPosition? currentSelectedModule;

        public IEnumerable<MirrorModule> SelectableSandblastedMagnifiers { get => mirrorBuilder.GetSelectableModules(MirrorModuleType.MagnifierSandblastedModuleType); }
        public IEnumerable<MirrorModule> SelectableSimpleMagnifiers { get => mirrorBuilder.GetSelectableModules(MirrorModuleType.MagnifierModuleType); }
        public IEnumerable<MirrorModule> SelectableProcesses { get => mirrorBuilder.GetSelectableModules(MirrorModuleType.ProcessModuleType); }
        public IEnumerable<MirrorModule> SelectableRoundedCorners { get => mirrorBuilder.GetSelectableModules(MirrorModuleType.RoundedCornersModuleType); }

        private MirrorModuleEditorVmBase? moduleEditor;
        public MirrorModuleEditorVmBase? ModuleEditor
        {
            get => moduleEditor;
            set
            {
                if (moduleEditor != value)
                {
                    if (moduleEditor is not null) moduleEditor.PropertyChanged -= ModuleEditor_PropertyChanged;
                    var oldValue = moduleEditor;
                    moduleEditor = value;
                    oldValue?.Dispose();
                    if (moduleEditor is not null) moduleEditor.PropertyChanged += ModuleEditor_PropertyChanged;
                    OnPropertyChanged(nameof(ModuleEditor));
                }
            }
        }
        /// <summary>
        /// Informs builder and VM about changes in a Module and reformulates the Mirror
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void ModuleEditor_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //The Module Editor Prop Changes are tracked Only when its not Null
            if (ModuleEditor is null) throw new Exception($"Unexpected Error Module Editor Was Null while tracking Changes...");

            //Pass the Module to the Builder
            var editedModule = ModuleEditor.GetModel();
            
            mirrorBuilder.ModifyModuleWithSameUniqueId(editedModule).FormulateMirror();
            //If the edited Module is a Rounded Corners Module Change also the Dimensions Info Object accordingly
            //change the backing field (otherwise the Dimensions Information Vm Will Change the module again)
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable MVVMTK0034 // Direct field reference to [ObservableProperty] backing field
            if ( editedModule.ModuleInfo is RoundedCornersModuleInfo roundedCorners
                && dimensionsInformation is RectangleInfoVm rect)
            {
                //Change the backing field (otherwise the Dimensions Information Vm Will Change the module again)
                rect.SetRadius(roundedCorners);
                OnPropertyChanged(nameof(HasRoundedCorners));
                OnPropertyChanged(nameof(GlassDimensions));
            }
#pragma warning restore MVVMTK0034 // Direct field reference to [ObservableProperty] backing field
#pragma warning restore IDE0079 // Remove unnecessary suppression

            //Live Recreate the Modules List whenever a Module is Edited
            OnPropertyChanged(nameof(Modules));
            OnMirrorChanged();
        }

        [RelayCommand]
        private void EditModule(ModuleWithPosition moduleWithPosition)
        {
            var editor = modulesEditorsFactory.Invoke();
            editor.SetModel(moduleWithPosition.Module);
            ModuleEditor = editor;
            wrappedViewModelModalsGenerator.OpenModal(ModuleEditor, $"{"lngEdit".TryTranslateKeyWithoutError()} {(moduleWithPosition.Module.ModuleInfo.ModuleType).ToString().TryTranslateKeyWithoutError()}");
        }
        [RelayCommand]
        private void RemoveModule(ModuleWithPosition moduleWithPosition)
        {
            //The Dimensions Information Vm changes will trigger the Builder Removal for the Rounded Corners
            if (moduleWithPosition.Module.ModuleInfo.ModuleType == MirrorModuleType.RoundedCornersModuleType
                && DimensionsInformation is RectangleInfoVm rect) rect.SetRadius(RoundedCornersModuleInfo.ZeroCorners());
            //Else Remove the Module with the Builder
            else mirrorBuilder.RemoveModule(moduleWithPosition.Module.ItemUniqueId).FormulateMirror();
            
            OnPropertyChanged(nameof(Modules));
            OnPropertyChanged(nameof(HasMagnifierWithLight));
            OnPropertyChanged(nameof(HasSimpleMagnifier));
            OnPropertyChanged(nameof(HasRoundedCorners));
            OnPropertyChanged(nameof(HasProcess));
            OnMirrorChanged();
        }
        [RelayCommand]
        private void SelectModule(MirrorModule module)
        {
            var clone = module.GetDeepClone();
            clone.AssignNewUniqueId();
            mirrorBuilder.AddModule(clone, null).FormulateMirror();
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable MVVMTK0034 // Direct field reference to [ObservableProperty] backing field
            if (module.ModuleInfo is RoundedCornersModuleInfo roundedCorners 
                && dimensionsInformation is RectangleInfoVm rect)
            {
                //Change the backing field (otherwise the Dimensions Information Vm Will Change the module again)
                rect.SetRadius(roundedCorners);
                OnPropertyChanged(nameof(HasRoundedCorners));
                OnPropertyChanged(nameof(GlassDimensions));
                //Pass The Module
            }
#pragma warning restore MVVMTK0034 // Direct field reference to [ObservableProperty] backing field
#pragma warning restore IDE0079 // Remove unnecessary suppression
            OnPropertyChanged(nameof(Modules));
            OnPropertyChanged(nameof(HasMagnifierWithLight));
            OnPropertyChanged(nameof(HasSimpleMagnifier));
            OnPropertyChanged(nameof(HasProcess));
            
            OnMirrorChanged();
        }

        /// <summary>
        /// Adds an additionabl Module of the specified Type
        /// </summary>
        /// <param name="type">The type of Module to Add</param>
        [RelayCommand]
        private void AddAnotherModule(MirrorModuleType type)
        {
            try
            {
                mirrorBuilder.AddDefaultModule(type).FormulateMirror();
                OnPropertyChanged(nameof(Modules));
                OnPropertyChanged(nameof(HasMagnifierWithLight));
                OnPropertyChanged(nameof(HasSimpleMagnifier));
                OnPropertyChanged(nameof(HasProcess));
                OnMirrorChanged();
            }
            catch (Exception ex)
            {
                MessageService.DisplayException(ex);
            }
        }
        #endregion

        #region Positions

        private MirrorElementPositionEditorViewModel? positionEditor;
        public MirrorElementPositionEditorViewModel? PositionEditor
        {
            get => positionEditor;
            set
            {
                if (positionEditor != value)
                {
                    if (positionEditor is not null) positionEditor.PropertyChanged -= PositionEditor_PropertyChanged;
                    var oldValue = positionEditor;
                    positionEditor = value;
                    oldValue?.Dispose();
                    if (positionEditor is not null) positionEditor.PropertyChanged += PositionEditor_PropertyChanged;
                    OnPropertyChanged(nameof(PositionEditor));
                }
            }
        }
        /// <summary>
        /// The Id of the Module that its position is being currently changed
        /// </summary>
        private string moduleUniqueIdHavingPositionEdited = string.Empty;

        /// <summary>
        /// Informs builder and VM about changes in a Modules Position and reformulates the Mirror
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void PositionEditor_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //The Module Editor Prop Changes are tracked Only when its not Null
            if (PositionEditor is null) throw new Exception($"Unexpected Error Position Editor Was Null while tracking Changes...");

            //Pass the Module to the Builder
            var editedPosition = PositionEditor.GetModel();
            mirrorBuilder.ModifyModulePosition(moduleUniqueIdHavingPositionEdited, editedPosition).FormulateMirror();

            //Live Recreate the Modules List whenever a Position is Edited
            OnPropertyChanged(nameof(Modules));
            OnMirrorChanged();
        }

        [RelayCommand]
        private void EditModulePosition(ModuleWithPosition moduleWithPosition)
        {
            if (moduleWithPosition.Position.Instructions.InstructionsType == PositionInstructionsType.UndefinedInstructions)
            {
                MessageService.Info($"Module with Code {moduleWithPosition.Module.Code} is not Positionable and cannot set any Position Information", "Not Positionable Module");
                return;
            }
            var vm = positionsEditorsFactory.Invoke();
            vm.SetModel(moduleWithPosition.Position);
            PositionEditor = vm;
            //Set the Id of the module having its position changed
            moduleUniqueIdHavingPositionEdited = moduleWithPosition.Module.ItemUniqueId;

            wrappedViewModelModalsGenerator.OpenModal(PositionEditor, $"{"lngEdit".TryTranslateKeyWithoutError()} {"lngPosition".TryTranslateKeyWithoutError()} {(moduleWithPosition.Module.ModuleInfo.ModuleType).ToString().TryTranslateKeyWithoutError()}");
        }
        [RelayCommand]
        private void SelectPositionOfModule(MirrorElementPosition position)
        {
            if (CurrentSelectedModule is null)
            {
                Log.Warning("Current Selected Module Appears Null , Cannot Select new Position for the Module");
                return;
            }
            
            mirrorBuilder.ModifyModulePosition(CurrentSelectedModule.Module.ItemUniqueId, position).FormulateMirror();
            OnPropertyChanged(nameof(Modules));
            OnMirrorChanged();
        }

        #endregion

        public MirrorSynthesis CopyPropertiesToModel(MirrorSynthesis model)
        {
            throw new NotSupportedException($"{nameof(MirrorSynthesisEditorViewModel)} does not support {nameof(CopyPropertiesToModel)} Method");
        }
        public MirrorSynthesis GetModel()
        {
            return mirrorBuilder.FormulatedMirror.GetDeepClone();
        }
        public void SetModel(MirrorSynthesis model)
        {
            //Set Shape First , them DimensionsInfo because Shape Change will trigger a DimensionsInfo Change
            Shape = model.GeneralShapeType;
            DimensionsInformation = shapesVmFactory.Create(model.DimensionsInformation);
            //Set Mirror Builder Last As Shape and DimensionsInfo are seperate and only synced with those of the Builder
            mirrorBuilder.SetFormulatedMirror(model.GetDeepClone());
            OnMirrorChanged();
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
                this.dataProvider.ProviderDataChanged -= OnMirrorsDataProviderDataChanged;
                if (DimensionsInformation != null)
                {
                    DimensionsInformation.PropertyChanged -= DimensionsInformation_PropertyChanged;
                    DimensionsInformation.Dispose();
                }
                if (positionEditor is not null)
                {
                    positionEditor.PropertyChanged -= PositionEditor_PropertyChanged;
                    positionEditor.Dispose();
                }
                if (moduleEditor is not null)
                {
                    moduleEditor.PropertyChanged -= ModuleEditor_PropertyChanged;
                    moduleEditor.Dispose();
                }
                if (sandblastEditor is not null)
                {
                    sandblastEditor.PropertyChanged -= SandblastEditor_PropertyChanged;
                    sandblastEditor.Dispose();
                }
                if (supportEditor is not null)
                {
                    supportEditor.PropertyChanged -= SupportEditor_PropertyChanged;
                    supportEditor.Dispose();
                }
                mirrorBuilder.Dispose();
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

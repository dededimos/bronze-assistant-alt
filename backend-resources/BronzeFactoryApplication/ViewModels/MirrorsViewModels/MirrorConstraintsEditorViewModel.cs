using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using BronzeFactoryApplication.ViewModels.AccessoriesViewModels;
using CommonInterfacesBronze;
using CommunityToolkit.Diagnostics;
using MirrorsLib;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements;
using MirrorsLib.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels
{
    public partial class MirrorConstraintsEditorViewModel : BaseViewModel, IEditorViewModel<MirrorConstraints>
    {
        public MirrorConstraintsEditorViewModel(IEditModelModalsGenerator editModalsGenerator,
            IMirrorsDataProvider dataProvider)
        {
            this.editModalsGenerator = editModalsGenerator;
            this.dataProvider = dataProvider;
        }

        private readonly IEditModelModalsGenerator editModalsGenerator;
        private readonly IMirrorsDataProvider dataProvider;

        [ObservableProperty]
        private BronzeMirrorShape? concerningMirrorShape;

        public ObservableCollection<MirrorGlassType> SelectedAllowedGlassTypes { get; } = [];
        public IEnumerable<MirrorGlassType> SelectableAllowedGlassTypes { get => Enum.GetValues<MirrorGlassType>().Except(SelectedAllowedGlassTypes).ToList(); }
        [ObservableProperty]
        private MirrorGlassType? selectedGlassTypeToAdd;

        public ObservableCollection<MirrorGlassThickness> SelectedAllowedGlassThicknesses { get; } = [];
        public IEnumerable<MirrorGlassThickness> SelectableAllowedGlassThicknesses { get => Enum.GetValues<MirrorGlassThickness>().Except(SelectedAllowedGlassThicknesses).ToList(); }
        [ObservableProperty]
        private MirrorGlassThickness? selectedGlassThicknessToAdd;

        [ObservableProperty]
        private double maxMirrorLength;

        [ObservableProperty]
        private double minMirrorLength;

        [ObservableProperty]
        private double minMirrorHeight;

        [ObservableProperty]
        private double maxMirrorHeight;

        [ObservableProperty]
        private double maxAllowedWattage;

        [ObservableProperty]
        private MirrorSandblast? selectedSandblastToAdd;
        public IEnumerable<MirrorSandblast> SelectableSandblasts { get => dataProvider.GetAllSandblasts().Except(SelectedSandblasts).ToList(); }
        public ObservableCollection<MirrorSandblast> SelectedSandblasts { get; } = [];

        [ObservableProperty]
        private MirrorSupport? selectedSupportToAdd;
        public IEnumerable<MirrorSupport> SelectableSupports { get => dataProvider.GetAllSupports().Except(SelectedSupports).ToList(); }
        public ObservableCollection<MirrorSupport> SelectedSupports { get; } = [];

        [ObservableProperty]
        private MirrorLightElement? selectedLightToAdd;
        public IEnumerable<MirrorLightElement> SelectableLights { get => dataProvider.GetAllLights().Except(SelectedLights).ToList(); }
        public ObservableCollection<MirrorLightElement> SelectedLights { get; } = [];

        [ObservableProperty]
        private MirrorModule? selectedModuleToAdd;
        [ObservableProperty]
        private bool isSelectedModuleObligatory;
        public IEnumerable<MirrorModule> SelectableModules { get => dataProvider.GetAllModules().Where(m => SelectedModules.Any(am => am.Module.ElementId == m.ElementId) == false).ToList(); }
        public ObservableCollection<AllowedMirrorModule> SelectedModules { get; } = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectableCustomElements))]
        private LocalizedString? selectedCustomElementType;
        partial void OnSelectedCustomElementTypeChanged(LocalizedString? value)
        {
            //Nullify the selection when the type changes
            SelectedCustomElementToAdd = null;
        }

        public IEnumerable<LocalizedString> CustomElementTypes { get => dataProvider.GetCustomElementsTypes().ToList(); }


        [ObservableProperty]
        private CustomMirrorElement? selectedCustomElementToAdd;
        [ObservableProperty]
        private bool isSelectedCustomElementObligatory;
        public IEnumerable<CustomMirrorElement> SelectableCustomElements
        {
            get => SelectedCustomElementType is not null
                ? dataProvider.GetCustomElementsOfType(SelectedCustomElementType.DefaultValue).Where(ce => SelectedCustomElements.Any(ace => ace.CustomElement.ElementId == ce.ElementId) == false).ToList()
                : [];
        }
        public ObservableCollection<AllowedCustomMirrorElement> SelectedCustomElements { get; } = [];

        [ObservableProperty]
        private bool canHaveLight;

        [ObservableProperty]
        private bool acceptsMirrorsWithoutSandblast;
        [ObservableProperty]
        private bool acceptsMirrorsWithoutSupport;
        [ObservableProperty]
        private bool acceptsMirrorsWithoutLight;

        [ObservableProperty]
        private IMirrorElement? selectedExclusiveElement1;
        [ObservableProperty]
        private IMirrorElement? selectedExclusiveElement2;
        [ObservableProperty]
        private bool areSelectedElementsExclusive;
        public IEnumerable<IMirrorElement> SelectableMirrorElements { get => dataProvider.GetSelectableExclusiveSetElements().ToList(); }
        public ObservableCollection<ExclusiveSetHelper> ExclusiveSets { get; } = [];

        [ObservableProperty]
        private bool perimeterIllumination;
        [ObservableProperty]
        private bool sandblastIllumination;
        [ObservableProperty]
        private bool magnifierIllumination;
        [ObservableProperty]
        private bool magnifier2Illumination;
        [ObservableProperty]
        private bool topIllumination;
        [ObservableProperty]
        private bool bottomIllumination;
        [ObservableProperty]
        private bool leftIllumination;
        [ObservableProperty]
        private bool rightIllumination;


        [RelayCommand]
        private void AddExclusiveSet()
        {
            if (SelectedExclusiveElement1 is null || SelectedExclusiveElement2 is null)
            {
                MessageService.Warning("Please Select Two Elements to make Exclusive", "Elements not Selected");
            }
            else if (SelectedExclusiveElement1.ElementId == SelectedExclusiveElement2.ElementId)
            {
                MessageService.Warning("Please Select Two Different Elements to make Exclusive", "Elements are the Same");
            }
            else
            {
                ExclusiveSets.Add(new(SelectedExclusiveElement1, SelectedExclusiveElement2, AreSelectedElementsExclusive));
                //Reset the Selection once Added
                SelectedExclusiveElement1 = null;
                SelectedExclusiveElement2 = null;
                AreSelectedElementsExclusive = false;
                OnPropertyChanged(nameof(ExclusiveSets));
            }
        }
        [RelayCommand]
        private void RemoveExclusiveSet(ExclusiveSetHelper exclusiveSet)
        {
            var removed = ExclusiveSets.Remove(exclusiveSet);
            if (!removed) MessageService.Error("The Exclusive Set you are trying to Remove was not Found...", "Unexpected Error");
            OnPropertyChanged(nameof(ExclusiveSets));
        }

        [RelayCommand]
        private void EditLocalizedString(LocalizedString localizedStringToEdit)
        {
            EditModelMessage<LocalizedString> message = new(localizedStringToEdit, this);
            editModalsGenerator.OpenEditModal(message);
        }
        [RelayCommand]
        private void AddGlassType()
        {
            var glassType = SelectedGlassTypeToAdd;

            if (glassType is null)
            {
                MessageService.Warning($"Please select a Glass Type First", "Glass Type to Add not Selected");
            }
            else if (SelectedAllowedGlassTypes.Any(t => t == glassType))
            {
                MessageService.Warning($"The Glass Type you are Trying to Add is already in the List", "Glass Type already in the List");
            }
            else
            {
                //Reset the Selection once Added
                SelectedGlassTypeToAdd = null;
                SelectedAllowedGlassTypes.Add((MirrorGlassType)glassType);
                OnPropertyChanged(nameof(SelectedAllowedGlassTypes));
                OnPropertyChanged(nameof(SelectableAllowedGlassTypes));
            }
        }
        [RelayCommand]
        private void RemoveGlassType(MirrorGlassType glassType)
        {
            var removed = SelectedAllowedGlassTypes.Remove(glassType);
            if (!removed) MessageService.Error("The Type you are trying to Remove was not Found...", "Unexpected Error");

            OnPropertyChanged(nameof(SelectedAllowedGlassTypes));
            OnPropertyChanged(nameof(SelectableAllowedGlassTypes));
        }
        [RelayCommand]
        private void AddGlassThickness()
        {
            var glassThickness = SelectedGlassThicknessToAdd;
            if (glassThickness is null)
            {
                MessageService.Warning($"Please select a Thickness First", "Thickness to Add not Selected");
            }
            else if (SelectedAllowedGlassThicknesses.Any(t => t == glassThickness))
            {
                MessageService.Warning($"The Thickness you are Trying to Add is already in the List", "Thickness already in the List");
            }
            else
            {
                //Reset the Selection once Added
                SelectedGlassThicknessToAdd = null;
                SelectedAllowedGlassThicknesses.Add((MirrorGlassThickness)glassThickness);
                OnPropertyChanged(nameof(SelectedAllowedGlassThicknesses));
                OnPropertyChanged(nameof(SelectableAllowedGlassThicknesses));
            }
        }
        [RelayCommand]
        private void RemoveGlassThickness(MirrorGlassThickness glassThickness)
        {
            var removed = SelectedAllowedGlassThicknesses.Remove(glassThickness);
            if (!removed) MessageService.Error("The Type you are trying to Remove was not Found...", "Unexpected Error");

            OnPropertyChanged(nameof(SelectedAllowedGlassThicknesses));
            OnPropertyChanged(nameof(SelectableAllowedGlassThicknesses));
        }
        [RelayCommand]
        private void AddSandblast()
        {
            var sandblast = SelectedSandblastToAdd;
            if (sandblast is null)
            {
                MessageService.Warning($"Please select a Sandblast First", "Sandblast to Add not Selected");
            }
            else if (SelectedSandblasts.Any(s => s.ElementId == sandblast.ElementId))
            {
                MessageService.Warning($"The Sandblast you are Trying to Add is already in the List", "Sandblast already in the List");
            }
            else
            {
                //Reset the Selection once Added
                SelectedSandblastToAdd = null;
                SelectedSandblasts.Add(sandblast);
                OnPropertyChanged(nameof(SelectedSandblasts));
                OnPropertyChanged(nameof(SelectableSandblasts));
            }
        }
        [RelayCommand]
        private void RemoveSandblast(MirrorSandblast sandblast)
        {
            var removed = SelectedSandblasts.Remove(sandblast);
            if (!removed) MessageService.Error("The Type you are trying to Remove was not Found...", "Unexpected Error");

            OnPropertyChanged(nameof(SelectedSandblasts));
            OnPropertyChanged(nameof(SelectableSandblasts));
        }
        [RelayCommand]
        private void AddSupport()
        {
            var support = SelectedSupportToAdd;
            if (support is null)
            {
                MessageService.Warning($"Please select a Support First", "Support to Add not Selected");
            }
            else if (SelectedSupports.Any(s => s.ElementId == support.ElementId))
            {
                MessageService.Warning($"The Support you are Trying to Add is already in the List", "Support already in the List");
            }
            else
            {
                //Reset the Selection once Added
                SelectedSupportToAdd = null;
                SelectedSupports.Add(support);
                OnPropertyChanged(nameof(SelectedSupports));
                OnPropertyChanged(nameof(SelectableSupports));
            }
        }
        [RelayCommand]
        private void RemoveSupport(MirrorSupport support)
        {
            var removed = SelectedSupports.Remove(support);
            if (!removed) MessageService.Error("The Type you are trying to Remove was not Found...", "Unexpected Error");

            OnPropertyChanged(nameof(SelectedSupports));
            OnPropertyChanged(nameof(SelectableSupports));
        }
        [RelayCommand]
        private void AddLight()
        {
            var light = SelectedLightToAdd;
            if (light is null)
            {
                MessageService.Warning($"Please select a Light First", "Light to Add not Selected");
            }
            else if (SelectedLights.Any(l => l.ElementId == light.ElementId))
            {
                MessageService.Warning($"The Light you are Trying to Add is already in the List", "Light already in the List");
            }
            else
            {
                //Reset the Selection once Added
                SelectedLightToAdd = null;
                SelectedLights.Add(light);
                OnPropertyChanged(nameof(SelectedLights));
                OnPropertyChanged(nameof(SelectableLights));
            }
        }
        [RelayCommand]
        private void RemoveLight(MirrorLightElement light)
        {
            var removed = SelectedLights.Remove(light);
            if (!removed) MessageService.Error("The Type you are trying to Remove was not Found...", "Unexpected Error");

            OnPropertyChanged(nameof(SelectedLights));
            OnPropertyChanged(nameof(SelectableLights));
        }
        [RelayCommand]
        private void AddModule()
        {
            var module = SelectedModuleToAdd;
            if (module is null)
            {
                MessageService.Warning($"Please select a Module First", "Module to Add not Selected");
            }
            else if (SelectedModules.Any(m => m.Module.ElementId == module.ElementId))
            {
                MessageService.Warning($"The Module you are Trying to Add is already in the List", "Module already in the List");
            }
            else
            {
                SelectedModules.Add(new(module, IsSelectedModuleObligatory));
                //Reset the Selection once Added
                IsSelectedModuleObligatory = false;
                SelectedModuleToAdd = null;
                OnPropertyChanged(nameof(SelectedModules));
                OnPropertyChanged(nameof(SelectableModules));
            }
        }
        [RelayCommand]
        private void RemoveModule(AllowedMirrorModule module)
        {
            var removed = SelectedModules.Remove(module);
            if (!removed) MessageService.Error("The Type you are trying to Remove was not Found...", "Unexpected Error");

            OnPropertyChanged(nameof(SelectedModules));
            OnPropertyChanged(nameof(SelectableModules));
        }
        [RelayCommand]
        private void AddCustomElement()
        {
            var element = SelectedCustomElementToAdd;
            if (element is null)
            {
                MessageService.Warning($"Please select a Custom Element First", "Custom Element not Selected");
            }
            else if (SelectedCustomElements.Any(m => m.CustomElement.ElementId == element.ElementId))
            {
                MessageService.Warning($"The Element you are Trying to Add is already in the List", "Element already in the List");
            }
            else
            {
                //Reset the Selection once Added
                SelectedCustomElements.Add(new(element, IsSelectedCustomElementObligatory));
                SelectedCustomElementToAdd = null;
                IsSelectedCustomElementObligatory = false;
                OnPropertyChanged(nameof(SelectedCustomElements));
                OnPropertyChanged(nameof(SelectableCustomElements));
            }
        }
        [RelayCommand]
        private void RemoveCustomElement(AllowedCustomMirrorElement element)
        {
            var removed = SelectedCustomElements.Remove(element);
            if (!removed) MessageService.Error("The Type you are trying to Remove was not Found...", "Unexpected Error");

            OnPropertyChanged(nameof(SelectedCustomElements));
            OnPropertyChanged(nameof(SelectableCustomElements));
        }

        public MirrorConstraints CopyPropertiesToModel(MirrorConstraints model)
        {
            model.ConcerningMirrorShape = this.ConcerningMirrorShape ?? BronzeMirrorShape.UndefinedMirrorShape;
            model.AllowedGlassTypes = new(this.SelectedAllowedGlassTypes);
            model.AllowedGlassThicknesses = new(this.SelectedAllowedGlassThicknesses);

            model.MaxMirrorLength = this.MaxMirrorLength;
            model.MinMirrorLength = this.MinMirrorLength;
            model.MinMirrorHeight = this.MinMirrorHeight;
            model.MaxMirrorHeight = this.MaxMirrorHeight;
            model.MaxAllowedWattage = this.MaxAllowedWattage;

            model.AllowedSandblasts = new(this.SelectedSandblasts.Select(s => s.ElementId));
            model.AllowedSupports = new(this.SelectedSupports.Select(s => s.ElementId));
            model.AllowedLights = new(this.SelectedLights.Select(l => l.ElementId));
            model.AllowedModules = new(this.SelectedModules.Select(m => m.Module.ElementId));
            model.AllowedCustomElements = new(this.SelectedCustomElements.Select(e => e.CustomElement.ElementId));

            model.AcceptsMirrorsWithoutLight = this.AcceptsMirrorsWithoutLight;
            model.AcceptsMirrorsWithoutSandblast = this.AcceptsMirrorsWithoutSandblast;
            model.AcceptsMirrorsWithoutSupport = this.AcceptsMirrorsWithoutSupport;
            model.ObligatoryCustomElements = new(this.SelectedCustomElements.Where(e => e.IsObligatory).Select(e => e.CustomElement.ElementId));
            model.ObligatoryModules = new(this.SelectedModules.Where(m => m.IsObligatory).Select(m => m.Module.ElementId));
            model.ExclusiveSets = new(this.ExclusiveSets.Select(es => new ExclusiveSet(es.Element1.ElementId, es.Element2.ElementId, es.AreExclusive)));

            model.CanHaveLight = this.CanHaveLight;

            //Each line checks a boolean property (e.g., this.PerimeterIllumination, this.SandblastIllumination, etc.).
            //If the boolean is true, it adds a corresponding IlluminationOption enum value (e.g., IlluminationOption.MirrorPerimeterIllumination) to model.AllowedIllumination.
            //If the boolean is false, it adds 0 (no change).
            model.AllowedIllumination = 0;
            model.AllowedIllumination |= this.PerimeterIllumination ? IlluminationOption.MirrorPerimeterIllumination : 0;
            model.AllowedIllumination |= this.SandblastIllumination ? IlluminationOption.SandblastIllumination : 0;
            model.AllowedIllumination |= this.MagnifierIllumination ? IlluminationOption.Magnifyer1Illumination : 0;
            model.AllowedIllumination |= this.Magnifier2Illumination ? IlluminationOption.Magnifyer2Illumination : 0;
            model.AllowedIllumination |= this.TopIllumination ? IlluminationOption.MirrorTopIllumination : 0;
            model.AllowedIllumination |= this.BottomIllumination ? IlluminationOption.MirrorBottomIllumination : 0;
            model.AllowedIllumination |= this.LeftIllumination ? IlluminationOption.MirrorLeftIllumination : 0;
            model.AllowedIllumination |= this.RightIllumination ? IlluminationOption.MirrorRightIllumination : 0;
            return model;
        }
        public MirrorConstraints GetModel()
        {
            return CopyPropertiesToModel(new());
        }
        public void SetModel(MirrorConstraints model)
        {
            SuppressPropertyNotifications();
            this.ConcerningMirrorShape = model.ConcerningMirrorShape != BronzeMirrorShape.UndefinedMirrorShape ? model.ConcerningMirrorShape : null;
            this.SelectedAllowedGlassTypes.Clear();
            foreach (var glassType in model.AllowedGlassTypes) this.SelectedAllowedGlassTypes.Add(glassType);

            this.SelectedAllowedGlassThicknesses.Clear();
            foreach (var thickness in model.AllowedGlassThicknesses) this.SelectedAllowedGlassThicknesses.Add(thickness);

            this.MaxMirrorLength = model.MaxMirrorLength;
            this.MinMirrorLength = model.MinMirrorLength;
            this.MaxMirrorHeight = model.MaxMirrorHeight;
            this.MinMirrorHeight = model.MinMirrorHeight;
            this.MaxAllowedWattage = model.MaxAllowedWattage;

            this.SelectedSandblasts.Clear();
            var sandblastsToAdd = dataProvider.GetSandblasts(model.AllowedSandblasts.ToArray());
            foreach (var sandblast in sandblastsToAdd) SelectedSandblasts.Add(sandblast);

            this.SelectedSupports.Clear();
            var supportsToAdd = dataProvider.GetSupports(model.AllowedSupports.ToArray());
            foreach (var support in supportsToAdd) SelectedSupports.Add(support);

            this.SelectedLights.Clear();
            var lightsToAdd = dataProvider.GetLights(model.AllowedLights.ToArray());
            foreach (var light in lightsToAdd) SelectedLights.Add(light);

            this.SelectedModules.Clear();
            IEnumerable<string> moduleIdsToGet = [.. model.AllowedModules, .. model.ObligatoryModules];
            var modulesToAdd = dataProvider.GetModules(moduleIdsToGet.Distinct().ToArray());
            foreach (var module in modulesToAdd) SelectedModules.Add(new(module, model.ObligatoryModules.Any(m => m == module.ElementId)));

            this.SelectedCustomElements.Clear();
            IEnumerable<string> customElementIdsToGet = [.. model.AllowedCustomElements, .. model.ObligatoryCustomElements];
            var elementsToAdd = dataProvider.GetCustomElements(customElementIdsToGet.Distinct().ToArray());
            foreach (var element in elementsToAdd) SelectedCustomElements.Add(new(element, model.ObligatoryCustomElements.Any(e => e == element.ElementId)));

            this.ExclusiveSets.Clear();
            var selectableExclusiveElements = dataProvider.GetSelectableExclusiveSetElements();
            foreach (var exclusiveSet in model.ExclusiveSets)
            {
                var element1 = selectableExclusiveElements.FirstOrDefault(e => e.ElementId == exclusiveSet.ItemId1);
                var element2 = selectableExclusiveElements.FirstOrDefault(e => e.ElementId == exclusiveSet.ItemId2);
                if (element1 is null || element2 is null)
                {
                    MessageService.Error($"One of the Elements with Ids : {element1} ---- {element2} was not present in the Exclusive Elements List of the DataProvider", "Unexpected Error - Invalid Exclusive Set");
                    continue; // donot add the set if one of the elements is not found
                }
                this.ExclusiveSets.Add(new(element1, element2, exclusiveSet.MutuallyExclusive));
            }

            this.CanHaveLight = model.CanHaveLight;

            this.AcceptsMirrorsWithoutLight = model.AcceptsMirrorsWithoutLight;
            this.AcceptsMirrorsWithoutSandblast = model.AcceptsMirrorsWithoutSandblast;
            this.AcceptsMirrorsWithoutSupport = model.AcceptsMirrorsWithoutSupport;

            this.PerimeterIllumination = model.AllowedIllumination.HasFlag(IlluminationOption.MirrorPerimeterIllumination);
            this.SandblastIllumination = model.AllowedIllumination.HasFlag(IlluminationOption.SandblastIllumination);
            this.MagnifierIllumination = model.AllowedIllumination.HasFlag(IlluminationOption.Magnifyer1Illumination);
            this.Magnifier2Illumination = model.AllowedIllumination.HasFlag(IlluminationOption.Magnifyer2Illumination);
            this.TopIllumination = model.AllowedIllumination.HasFlag(IlluminationOption.MirrorTopIllumination);
            this.BottomIllumination = model.AllowedIllumination.HasFlag(IlluminationOption.MirrorBottomIllumination);
            this.LeftIllumination = model.AllowedIllumination.HasFlag(IlluminationOption.MirrorLeftIllumination);
            this.RightIllumination = model.AllowedIllumination.HasFlag(IlluminationOption.MirrorRightIllumination);

            ResumePropertyNotifications();
            OnPropertyChanged("");
        }
    }

    public class AllowedCustomMirrorElement
    {
        public AllowedCustomMirrorElement(CustomMirrorElement customElement, bool isObligatory)
        {
            CustomElement = customElement;
            IsObligatory = isObligatory;
        }
        public CustomMirrorElement CustomElement { get; set; }
        public bool IsObligatory { get; set; }

        public override string ToString()
        {
            return CustomElement.ToString();
        }
    }
    public class AllowedMirrorModule
    {
        public AllowedMirrorModule(MirrorModule module, bool isObligatory)
        {
            Module = module;
            IsObligatory = isObligatory;
        }
        public MirrorModule Module { get; set; }
        public bool IsObligatory { get; set; }
        public override string ToString()
        {
            return Module.ToString();
        }
    }
    public class ExclusiveSetHelper
    {
        public ExclusiveSetHelper(IMirrorElement element1, IMirrorElement element2, bool areExclusive)
        {
            Element1 = element1;
            Element2 = element2;
            AreExclusive = areExclusive;
        }
        public IMirrorElement Element1 { get; set; }
        public IMirrorElement Element2 { get; set; }
        public bool AreExclusive { get; set; }

        public override string ToString()
        {
            return $"{Element1}---{Element2} {(AreExclusive ? "Exclusive" : "Inclusive")}";
        }
    }


}

using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using CommonInterfacesBronze;
using Microsoft.Graph.Models;
using MirrorsLib;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.Sandblasts;
using MirrorsLib.MirrorElements.Supports;
using MirrorsLib.Repositories;
using MirrorsRepositoryMongoDB.Entities;
using System.Collections.ObjectModel;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels
{
    public partial class MirrorElementTraitEntityEditorBaseViewModel<TEntity> : MirrorElementEntityBaseEditorViewModel<TEntity>
        where TEntity : MirrorElementTraitEntity, IDeepClonable<TEntity>, IEqualityComparerCreator<TEntity>, new()
    {
        private readonly IMirrorsDataProvider dataProvider;
        private readonly Lazy<List<string>> selectableTypes;
        private Lazy<List<MirrorElementBase>> selectableElements;

        [ObservableProperty]
        private bool isAssignableToAll;
        public List<string> SelectableTypes { get => selectableTypes.Value; }
        public List<MirrorElementBase> SelectableElements { get => selectableElements.Value; }
        
        [ObservableProperty]
        private MirrorElementBase? selectedElementToAdd;
        [ObservableProperty]
        private string? selectedTypeToAdd;

        public ObservableCollection<string> TargetTypes { get; set; } = [];
        public ObservableCollection<MirrorElementBase> TargetElements { get; set; } = [];


        public MirrorElementTraitEntityEditorBaseViewModel(
            Func<MongoDatabaseEntityEditorViewModel> baseEntityFactory,
            IMirrorsDataProvider dataProvider,
            IEditModelModalsGenerator editModalsGenerator) : base(baseEntityFactory, editModalsGenerator)
        {
            this.dataProvider = dataProvider;
            dataProvider.ProviderDataChanged += DataProvider_ProviderDataChanged;
            TargetTypes.CollectionChanged += TargetTypes_CollectionChanged;
            TargetElements.CollectionChanged += TargetElements_CollectionChanged;
            selectableTypes = new(() => GetSelectableTypes());
            selectableElements = new(() => GetAllElements());
        }

        private void DataProvider_ProviderDataChanged(object? sender, Type e)
        {
            //reset the Lazy initilization of Elements when provider changes
            selectableElements = new(() => GetAllElements());
            OnPropertyChanged(nameof(SelectableElements));
        }

        [RelayCommand]
        private void AddSelectedType()
        {
            if (string.IsNullOrEmpty(SelectedTypeToAdd))
            {
                MessageService.Warning($"Please select a Type First", "Type not Selected");
                return;
            }
            if (TargetTypes.Contains(SelectedTypeToAdd))
            {
                MessageService.Warning($"Type: {SelectedTypeToAdd} has already been Added", "Type already Added");
                return;
            }
            TargetTypes.Add(SelectedTypeToAdd);
            SelectedTypeToAdd = null;
        }
        [RelayCommand]
        private void RemoveTargetType(string typeToRemove)
        {
            bool removed = TargetTypes.Remove(typeToRemove);
            if (!removed)
            {
                MessageService.Error($"Could not Execute Remove Type ,{Environment.NewLine}{typeToRemove} was not Found in the List of Target Types...", "Unexpected Error");
            }
        }
        [RelayCommand]
        private void AddSelectedElement()
        {
            if (SelectedElementToAdd is null)
            {
                MessageService.Warning($"Please select an Element First", "Element not Selected");
                return;
            }
            if (TargetElements.Any(e=> e.ElementId == SelectedElementToAdd.ElementId))
            {
                MessageService.Warning($"Element with Code {SelectedElementToAdd.Code} has already been Added", "Type already Added");
                return;
            }
            TargetElements.Add(SelectedElementToAdd);
            SelectedElementToAdd = null;
        }
        [RelayCommand]
        private void RemoveTargetElement(MirrorElementBase targetElement)
        {
            var removed = TargetElements.Remove(targetElement);
            if (!removed)
            {
                MessageService.Error($"Could not Execute Remove Element ,{Environment.NewLine}{targetElement.Code} was not Found in the List of Target Elements...", "Unexpected Error");
            }
        }


        /// <summary>
        /// Returns the Selectable Types  for the Assigning of the Traits
        /// </summary>
        /// <returns></returns>
        private List<string> GetSelectableTypes()
        {
            List<string> selectableTypes = [];
            //Get all the Types of the MirrorElementClass
            var assembly = typeof(MirrorElementEntity).Assembly;
            var allTypes = assembly.GetTypes();
            var types = allTypes.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(MirrorElementEntity)));
            selectableTypes.AddRange(types.Select(t => t.Name));
            var sandblastTypes = allTypes.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(MirrorSandblastInfo)));
            selectableTypes.AddRange(sandblastTypes.Select(t => t.Name));
            var supportTypes = allTypes.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(MirrorSupportInfo)));
            selectableTypes.AddRange(supportTypes.Select(t => t.Name));
            var moduleTypes = typeof(MirrorModuleInfo).Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(MirrorModuleInfo)));
            selectableTypes.AddRange(moduleTypes.Select(t => t.Name));
            selectableTypes.Add(nameof(MirrorSynthesis));
            SelectedTypeToAdd = null;
            return selectableTypes;
        }
        private List<MirrorElementBase> GetAllElements()
        {
            var modules = dataProvider.GetAllModules();
            var lights = dataProvider.GetAllLights();
            var series = dataProvider.GetAllSeries();
            var customElements = dataProvider.GetAllCustomElements();
            var supports = dataProvider.GetAllSupports();
            var sandblasts = dataProvider.GetAllSandblasts();
            var positions = dataProvider.GetAllPositions();
            SelectedElementToAdd = null;
            return [.. modules, .. lights, .. series, .. customElements, .. supports, .. sandblasts, .. positions];
        }

        private void TargetElements_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TargetElements));
        }
        private void TargetTypes_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TargetTypes));
        }

        public override TEntity CopyPropertiesToModel(TEntity model)
        {
            base.CopyPropertiesToModel(model);
            model.TargetTypes = new(this.TargetTypes);
            model.TargetElementIds = new(this.TargetElements.Select(e => e.ElementId));
            model.IsAssignableToAll = this.IsAssignableToAll;
            return model;
        }
        protected override void SetModelWithoutUndoStore(TEntity model)
        {
            //Call the MirrorElementEntity base to set the Db Entity and the MirrorElements Properties
            base.SetModelWithoutUndoStore(model);
            TargetTypes.Clear();
            foreach (var type in model.TargetTypes)
            {
                TargetTypes.Add(type);
            }

            //Add the Elements of the model matching them with the Found SelectableElements . If there is no match (null) do not add anything...
            TargetElements.Clear();
            var elements = model.TargetElementIds.Select(id => SelectableElements.FirstOrDefault(e => e.ElementId == id));
            foreach (var item in elements)
            {
                if (item != null) TargetElements.Add(item);
                else MessageService.Error($"Unexpected Error... One of the Assigned Items of the Model was not found in SelectableElementsList ...", "Unexpected");
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
                dataProvider.ProviderDataChanged -= DataProvider_ProviderDataChanged;
                TargetTypes.CollectionChanged -= TargetTypes_CollectionChanged;
                TargetElements.CollectionChanged -= TargetElements_CollectionChanged;
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

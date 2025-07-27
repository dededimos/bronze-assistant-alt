using CommonInterfacesBronze;
using MirrorsLib.Interfaces;
using MirrorsLib.MirrorElements;
using MirrorsLib.Repositories;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.ModulesViewModels.MirrorModuleWithElementInfoVms
{
    public partial class MirrorModuleEditorVmBase : BaseViewModel , IEditorViewModel<MirrorModule>
    {
        public MirrorModuleEditorVmBase(Func<MirrorElementInfoEditorViewModel> elementInfoVmFactory,
            ModuleEditorViewModelsFactory modulesVmFactory,
            IMirrorsDataProvider dataProvider)
        {
            ElementInfo = elementInfoVmFactory.Invoke();
            //Do not create Module unti Model is Set from Consumer

            this.modulesVmFactory = modulesVmFactory;
            this.dataProvider = dataProvider;
            this.dataProvider.ProviderDataChanged += DataProvider_ProviderDataChanged;
            InitilizeDefaultElements();
        }

        protected string uniqueId = string.Empty;
        protected readonly ModuleEditorViewModelsFactory modulesVmFactory;
        private readonly IMirrorsDataProvider dataProvider;
        private IEnumerable<MirrorModule> defaultElements = [];
        protected MirrorModuleInfoEqualityComparer comparer = new(true);

        private void DataProvider_ProviderDataChanged(object? sender, Type e)
        {
            //If the provider changed any of the Mirror Modules reInitilize the Default items for comparisons
            if (e == typeof(MirrorModule))
            {
                InitilizeDefaultElements();
            }
        }
        private void InitilizeDefaultElements()
        {
            defaultElements = dataProvider.GetAllModules();
        }

        public MirrorElementInfoEditorViewModel ElementInfo { get; set; }
        public IModelGetterViewModel<MirrorModuleInfo> MirrorModule { get; set; } = IModelGetterViewModel<MirrorModuleInfo>.EmptyGetter();

        /// <summary>
        /// Informs consumers that the Element info of the magnfier have changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ElementInfo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ElementInfo));
        }

        public void SetModel(MirrorModule model)
        {
            SuppressPropertyNotifications();
            ElementInfo.PropertyChanged -= ElementInfo_PropertyChanged;
            ElementInfo.SetModel(model);
            ElementInfo.PropertyChanged += ElementInfo_PropertyChanged;

            MirrorModule.PropertyChanged -= MirrorModule_PropertyChanged;
            MirrorModule = modulesVmFactory.Create(model.ModuleInfo);
            uniqueId = model.ItemUniqueId;
            MirrorModule.PropertyChanged += MirrorModule_PropertyChanged;
            ResumePropertyNotifications();
            OnPropertyChanged("");
        }

        private void MirrorModule_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //If its a positionable disregard the changes of the below properties for determining default elements
            if (e.PropertyName != nameof(IMirrorPositionable.MinDistanceFromOtherModules) &&
                e.PropertyName != nameof(IMirrorPositionable.MinDistanceFromSupport) &&
                e.PropertyName != nameof(IMirrorPositionable.MinDistanceFromSandblast))
            {
                var currentModule = MirrorModule.GetModel();
                var potentialEqualDefault = defaultElements.FirstOrDefault(def => comparer.Equals(def.ModuleInfo, currentModule));
                if (potentialEqualDefault is null && !ElementInfo.IsOverriddenElement) //only if they have not been already set to custom
                {
                    ElementInfo.MarkElementInfoAsCustom();
                }
                else if(potentialEqualDefault is not null)
                {
                    ElementInfo.SetModel(potentialEqualDefault);
                }
            }
            OnPropertyChanged(nameof(MirrorModule));
        }

        public virtual MirrorModule GetModel()
        {
            MirrorModule model = new(ElementInfo.GetModel(), MirrorModule.GetModel());
            model.AssignNewUniqueId(uniqueId);
            return model;
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
                MirrorModule.PropertyChanged -= MirrorModule_PropertyChanged;
                MirrorModule.Dispose();
                dataProvider.ProviderDataChanged -= DataProvider_ProviderDataChanged;
            }

            //object has been disposed
            _disposed = true;

            //If this comes from inheritance Where the parent implement IDisposable the must call base Dispose and the Dispose() method is only in the Parent
            //The subclasses only implement the virtual method and a field '_disposed'
            //Call the base Dispose(bool)
            base.Dispose(disposing);
        }

        public MirrorModule CopyPropertiesToModel(MirrorModule model)
        {
            throw new NotSupportedException($"{nameof(MirrorModuleEditorVmBase)} does not Support Copy Properties to Model");
        }
    }
}

using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.MirrorModules;
using MirrorsLib.MirrorElements.Sandblasts;
using MirrorsLib.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.SandblastViewModels
{
    public partial class MirrorSandblastEditorViewModel : BaseViewModel, IEditorViewModel<MirrorSandblast>
    {
        public MirrorSandblastEditorViewModel(SandblastEditorViewModelsFactory sandblastInfoVmsFactory,
                                              Func<MirrorElementInfoEditorViewModel> elementInfoVmFactory,
                                              IMirrorsDataProvider dataProvider)
        {
            this.sandblastInfoVmsFactory = sandblastInfoVmsFactory;
            this.dataProvider = dataProvider;
            //This changes whenever a new Sandblast Type is picked
            sandblastInfo = sandblastInfoVmsFactory.CreateNew(MirrorsLib.Enums.MirrorSandblastType.Undefined);

            //To notify consumers that sandblast info changed
            SandblastInfo.PropertyChanged += SandblastInfo_PropertyChanged;

            //This does not change during the lifetime of the Editor
            ElementInfo = elementInfoVmFactory.Invoke();
            //To notify consumers that elemnt info changed
            ElementInfo.PropertyChanged += ElementInfo_PropertyChanged;

            this.dataProvider.ProviderDataChanged += DataProvider_ProviderDataChanged;
            InitilizeDefaultSandblasts();
        }
        /// <summary>
        /// All the default Sandblasts to check for equalities
        /// </summary>
        private IEnumerable<MirrorSandblast> defaultSandblasts = [];
        private readonly SandblastEditorViewModelsFactory sandblastInfoVmsFactory;
        private readonly IMirrorsDataProvider dataProvider;

        /// <summary>
        /// A comparer to check weather the sandblast info has changed from the default , to change code
        /// </summary>
        private readonly MirrorSandblastInfoEqualityComparer sandblastInfoComparer = new(true);

        private IModelGetterViewModel<MirrorSandblastInfo> sandblastInfo;
        public IModelGetterViewModel<MirrorSandblastInfo> SandblastInfo
        {
            get => sandblastInfo;
            private set
            {
                if (sandblastInfo != value)
                {
                    var oldValue = sandblastInfo;
                    sandblastInfo = value;
                    oldValue?.Dispose();
                    OnPropertyChanged(nameof(SandblastInfo));
                }
            }
        }

        public MirrorElementInfoEditorViewModel ElementInfo { get; set; }

        /// <summary>
        /// Initilizes the Enumerable List of the available default Sandblasts from the Data Provider
        /// </summary>
        private void InitilizeDefaultSandblasts()
        {
            defaultSandblasts = dataProvider.GetAllSandblasts();
        }
        /// <summary>
        /// If the provider changed any of the Sandblasts reInitilize the Default items for comparisons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataProvider_ProviderDataChanged(object? sender, Type e)
        {
            if (e == typeof(MirrorSandblast))
            {
                InitilizeDefaultSandblasts();
            }
        }
        private void ElementInfo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ElementInfo));
        }
        private void SandblastInfo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SandblastInfo));

            //Every time the sandblast Info Changes other than distances properties
            //Check weather the one produced by the user matches one of the defaults or not
            //If it matches the Default change the ElementInfo to those , otherwise mark them for Custom , if they have not already been marked
            if (e.PropertyName != nameof(MirrorSandblastInfoBaseViewModel.MinDistanceFromOtherModules) &&
                e.PropertyName != nameof(MirrorSandblastInfoBaseViewModel.MinDistanceFromSupport))
            {
                var currentSandblastInfo = SandblastInfo.GetModel();
                var equalDefault = defaultSandblasts.FirstOrDefault(defaultSandblast => sandblastInfoComparer.Equals(defaultSandblast.SandblastInfo, currentSandblastInfo));
                if (equalDefault == null && !ElementInfo.IsOverriddenElement) //only if they have not been already set to custom
                {
                    ElementInfo.MarkElementInfoAsCustom();
                }
                else if (equalDefault != null)
                {
                    ElementInfo.SetModel(equalDefault);
                }
            }
        }

        public MirrorSandblast CopyPropertiesToModel(MirrorSandblast model)
        {
            throw new NotSupportedException($"{nameof(MirrorSandblastEditorViewModel)} does not Support Copy Properties to Model");
        }

        public MirrorSandblast GetModel()
        {
            return new MirrorSandblast(ElementInfo.GetModel(), SandblastInfo.GetModel());
        }
        public void SetModel(MirrorSandblast model)
        {
            SuppressPropertyNotifications();
            //unsubscribe from previous 
            SandblastInfo.PropertyChanged -= SandblastInfo_PropertyChanged;
            SandblastInfo = sandblastInfoVmsFactory.Create(model.SandblastInfo);
            //subscribe to new One
            SandblastInfo.PropertyChanged += SandblastInfo_PropertyChanged;

            ElementInfo.SetModel(model);
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
                this.dataProvider.ProviderDataChanged -= DataProvider_ProviderDataChanged;
                SandblastInfo.PropertyChanged -= SandblastInfo_PropertyChanged;
                SandblastInfo.Dispose();

                ElementInfo.PropertyChanged -= ElementInfo_PropertyChanged;
                ElementInfo.Dispose();
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

using BronzeFactoryApplication.ViewModels.MirrorsViewModels.SandblastViewModels;
using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.Supports;
using MirrorsLib.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.SupportsViewModels
{
    public partial class MirrorSupportEditorViewModel : BaseViewModel, IEditorViewModel<MirrorSupport>
    {
        public MirrorSupportEditorViewModel(SupportsEditorViewModelsFactory supportInfoVmsFactory,
                                            Func<MirrorElementInfoEditorViewModel> elementInfoVmFactory,
                                            IMirrorsDataProvider dataProvider)
        {
            this.supportInfoVmsFactory = supportInfoVmsFactory;
            this.dataProvider = dataProvider;
            //This changes whenever a new Sandblast Type is picked
            supportInfo = supportInfoVmsFactory.CreateNew(MirrorsLib.Enums.MirrorSupportType.Undefined);

            //To notify consumers that sandblast info changed
            SupportInfo.PropertyChanged += SupportInfo_PropertyChanged;

            //This does not change during the lifetime of the Editor
            ElementInfo = elementInfoVmFactory.Invoke();
            //To notify consumers that elemnt info changed
            ElementInfo.PropertyChanged += ElementInfo_PropertyChanged;

            this.dataProvider.ProviderDataChanged += DataProvider_ProviderDataChanged;
            InitilizeDefaultSupports();
            InitilizeSelectableFinishes();
        }

        /// <summary>
        /// All the Default Supports to check for Equalities
        /// </summary>
        private IEnumerable<MirrorSupport> defaultSupports = [];
        private readonly SupportsEditorViewModelsFactory supportInfoVmsFactory;
        private readonly IMirrorsDataProvider dataProvider;
        private readonly MirrorSupportInfoEqualityComparer supportInfoComparer = new(true);

        private IModelGetterViewModel<MirrorSupportInfo> supportInfo;
        public IModelGetterViewModel<MirrorSupportInfo> SupportInfo
        {
            get => supportInfo;
            private set
            {
                if (supportInfo != value)
                {
                    var oldValue = supportInfo;
                    supportInfo = value;
                    oldValue?.Dispose();
                    OnPropertyChanged(nameof(SupportInfo));
                }
            }
        }

        public MirrorElementInfoEditorViewModel ElementInfo { get; set; }

        public MirrorFinishElement Finish { get => SelectableFinishes.FirstOrDefault(f => f.ElementId == SelectedFinishId) ?? MirrorFinishElement.EmptyFinish(); }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Finish))]
        private string? selectedFinishId = string.Empty;

        public List<MirrorFinishElement> SelectableFinishes { get; private set; } = [];

        /// <summary>
        /// Initilizes the Enumerable List of the available default Supports from the Data Provider
        /// </summary>
        private void InitilizeDefaultSupports()
        {
            defaultSupports = dataProvider.GetAllSupports();
        }
        private void InitilizeSelectableFinishes()
        {
            SelectableFinishes = dataProvider.GetAllFinishElements().ToList();
            OnPropertyChanged(nameof(SelectableFinishes));
        }

        /// <summary>
        /// If the provider changed any of the Supports reInitilize the Default items for comparisons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataProvider_ProviderDataChanged(object? sender, Type e)
        {
            if (e == typeof(MirrorSupport))
            {
                InitilizeDefaultSupports();
            }
            else if (e == typeof(MirrorFinishElement))
            {
                InitilizeSelectableFinishes();
            }
        }
        private void ElementInfo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ElementInfo));
        }
        private void SupportInfo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SupportInfo));

            //Every time the support Info Changes other than distances properties
            //Check weather the one produced by the user matches one of the defaults or not
            //If it matches the Default change the ElementInfo to those , otherwise mark them for Custom , if they have not already been marked
            if (e.PropertyName != nameof(MirrorSupportInfoBaseViewModel.MinDistanceFromOtherModules) &&
                e.PropertyName != nameof(MirrorSupportInfoBaseViewModel.MinDistanceFromSandblast))
            {
                var currentSupportInfo = SupportInfo.GetModel();
                var equalDefault = defaultSupports.FirstOrDefault(defaultSupport => supportInfoComparer.Equals(defaultSupport.SupportInfo, currentSupportInfo));
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
        public MirrorSupport CopyPropertiesToModel(MirrorSupport model)
        {
            throw new NotSupportedException($"{nameof(MirrorSupportEditorViewModel)} does not Support Copy Properties to Model");
        }

        public MirrorSupport GetModel()
        {
            return new MirrorSupport(ElementInfo.GetModel(), SupportInfo.GetModel(),Finish.GetDeepClone());
        }

        public void SetModel(MirrorSupport model)
        {
            SuppressPropertyNotifications();
            //unsubscribe from previous 
            SupportInfo.PropertyChanged -= SupportInfo_PropertyChanged;
            SupportInfo = supportInfoVmsFactory.Create(model.SupportInfo);
            //subscribe to new One
            SupportInfo.PropertyChanged += SupportInfo_PropertyChanged;

            ElementInfo.SetModel(model);

            SelectedFinishId = SelectableFinishes.FirstOrDefault(f => f.ElementId == model.Finish.ElementId)?.ElementId ?? string.Empty;
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
                SupportInfo.PropertyChanged -= SupportInfo_PropertyChanged;
                SupportInfo.Dispose();

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

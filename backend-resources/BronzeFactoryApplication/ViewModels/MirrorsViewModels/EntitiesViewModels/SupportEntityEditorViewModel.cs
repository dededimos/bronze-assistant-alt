using BronzeFactoryApplication.ApplicationServices.ModalsDirectorService;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using MirrorsLib.Enums;
using MirrorsLib.MirrorElements;
using MirrorsLib.MirrorElements.Supports;
using MirrorsLib.Repositories;
using MirrorsRepositoryMongoDB.Entities;
using System.Collections.ObjectModel;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.EntitiesViewModels
{
    public partial class MirrorSupportEntityEditorViewModel : MirrorElementEntityBaseEditorViewModel<MirrorSupportEntity>, IMirrorEntityEditorViewModel<MirrorSupportEntity>
    {
        public MirrorSupportEntityEditorViewModel(
            SupportsEditorViewModelsFactory supportsVmFactory,
            Func<MongoDatabaseEntityEditorViewModel> baseEntityFactory,
            IMirrorsDataProvider dataProvider,
            IEditModelModalsGenerator editModalsGenerator) : base(baseEntityFactory, editModalsGenerator)
        {
            this.supportsVmFactory = supportsVmFactory;
            this.dataProvider = dataProvider;
        }

        private readonly SupportsEditorViewModelsFactory supportsVmFactory;
        private readonly IMirrorsDataProvider dataProvider;

        private MirrorSupportType? supportType;
        public MirrorSupportType? SupportType
        {
            get => supportType;
            set
            {
                if (value != supportType)
                {
                    supportType = value;
                    //Do not pass the change into the undo stack . The Sandblast ModelGetter will change and it will pass changes to it.
                    StopTrackingUndoChanges();
                    //Inform the Vm and Ui that this has changes but without pushing edits into the undo stack
                    OnPropertyChanged(nameof(SupportType));
                    StartTrackingUndoChanges();
                    Support = supportsVmFactory.CreateNew(supportType ?? MirrorSupportType.Undefined);
                }
            }
        }


        private IModelGetterViewModel<MirrorSupportInfo> support = IModelGetterViewModel<MirrorSupportInfo>.EmptyGetter();
        public IModelGetterViewModel<MirrorSupportInfo> Support
        {
            get => support;
            private set
            {
                if (support != value)
                {
                    support.PropertyChanged -= Support_PropertyChanged;
                    support.Dispose();
                    support = value;
                    support.PropertyChanged += Support_PropertyChanged;
                    OnPropertyChanged(nameof(Support));
                }
            }
        }

        public IEnumerable<MirrorFinishElement> SelectableFinishes { get => dataProvider.GetAllFinishElements();}

        [ObservableProperty]
        private MirrorFinishElement? selectedDefaultFinish;

        
        public IEnumerable<MirrorFinishElement> SelectableAllowedFinishes { get => dataProvider.GetAllFinishElements().Where(af=> AllowedFinishes.All(f=> f.ElementId != af.ElementId)); }
        public ObservableCollection<MirrorFinishElement> AllowedFinishes { get; set; } = [];
        
        [ObservableProperty]
        private MirrorFinishElement? selectedAllowedFinishToAdd;

        [RelayCommand]
        private void AddAllowedFinish()
        {
            if (SelectedAllowedFinishToAdd is null)
            {
                MessageService.Warning($"Please select a Finish to add First", "Finsih to Add not Selected");
                return;
            }
            else if(AllowedFinishes.Any(af=> af.ElementId == SelectedAllowedFinishToAdd.ElementId))
            {
                MessageService.Warning($"The Finish you are trying to add is Already in the List{Environment.NewLine}Finish:{SelectedAllowedFinishToAdd.Code}--{SelectedAllowedFinishToAdd.LocalizedDescriptionInfo.Name.DefaultValue}", "Finish Already in the List");
                return;
            }
            else
            {
                AllowedFinishes.Add(SelectedAllowedFinishToAdd.GetDeepClone());
                SelectedAllowedFinishToAdd = null; //Reset Selection
                OnPropertyChanged(nameof(AllowedFinishes));
                OnPropertyChanged(nameof(SelectableAllowedFinishes));
            }
        }
        [RelayCommand]
        private void RemoveAllowedFinish(MirrorFinishElement finishToRemove)
        {
            var removed = AllowedFinishes.Remove(finishToRemove);
            if (removed)
            {
                OnPropertyChanged(nameof(AllowedFinishes));
                OnPropertyChanged(nameof(SelectableAllowedFinishes));
            }
            else
            {
                MessageService.Warning($"Unexpected Error , The Finish {finishToRemove.Code} -- {finishToRemove.LocalizedDescriptionInfo.Name.DefaultValue} was not Found for Removal", "Failed to Remove Finish");
                return;
            }
        }

        public override MirrorSupportEntity CopyPropertiesToModel(MirrorSupportEntity model)
        {
            base.CopyPropertiesToModel(model);
            model.Support = Support.GetModel();
            model.DefaultSelectedFinishId = this.SelectedDefaultFinish?.ElementId ?? string.Empty;
            model.SelectableFinishes.Clear();
            foreach (var item in this.AllowedFinishes)
            {
                model.SelectableFinishes.Add(item.ElementId);
            }
            return model;
        }
        protected override void SetModelWithoutUndoStore(MirrorSupportEntity model)
        {
            //Call the MirrorElementEntity base to set the Db Entity and the MirrorElements Properties
            base.SetModelWithoutUndoStore(model);
            //Generate the Correct Editor ModuleViewModel (the factory takes care of the SetModel method)
            var vm = supportsVmFactory.Create(model.Support);

            //Hack!!!! (change the backing field ,so that it does not trigger change of the SupportInfo Vm...)
            supportType = model.Support.SupportType == MirrorSupportType.Undefined ? null : model.Support.SupportType;
            OnPropertyChanged(nameof(SupportType));

            var modelAllowedFinishes = dataProvider.GetFinishElements([.. model.SelectableFinishes]);
            AllowedFinishes.Clear();
            foreach (var allowedFinish in modelAllowedFinishes)
            {
                AllowedFinishes.Add(allowedFinish);
            }
            SelectedDefaultFinish = SelectableFinishes.FirstOrDefault(f=> f.ElementId == model.DefaultSelectedFinishId);
            OnPropertyChanged(nameof(SelectableAllowedFinishes));

            //Set the New ViewModel to the Module Property which is also and IModelGetterViewModel<MirrorModuleInfo>
            Support = vm;
        }


        private void Support_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Support));
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
                Support.PropertyChanged -= Support_PropertyChanged;
                Support.Dispose();
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

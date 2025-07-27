using AccessoriesRepoMongoDB.Entities;
using BathAccessoriesModelsLibrary;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.AccessoriesViewModels.EntitiesViewModels
{
    public partial class TraitGroupEntityViewModel : BaseViewModel, IEditorViewModel<TraitGroupEntity>
    {
        [ObservableProperty]
        private DescriptiveEntityViewModel baseDescriptiveEntity;
        [ObservableProperty]
        private int sortNo;
        [ObservableProperty]
        private bool isEnabled;
        [ObservableProperty]
        private string code = string.Empty;
        [ObservableProperty]
        private ObservableCollection<TypeOfTrait> permittedTraitTypes = new();

        [ObservableProperty]
        private TypeOfTrait? selectedPermittedType;

        public List<TypeOfTrait> AllPerimitedTypes { get; }

        public TraitGroupEntityViewModel(Func<DescriptiveEntityViewModel> descriptiveEntityVmFactory)
        {
            baseDescriptiveEntity = descriptiveEntityVmFactory.Invoke();
            AllPerimitedTypes = Enum.GetValues(typeof(TypeOfTrait)).Cast<TypeOfTrait>().Where(type => type != TypeOfTrait.Undefined).ToList();
        }

        [RelayCommand]
        private void AddPermittedTraitType()
        {
            if (SelectedPermittedType is null)
            {
                MessageService.Warning("Please first Select a Type to Add", "No Type Selected");
            }
            else if(PermittedTraitTypes.Any(t=> t == SelectedPermittedType))
            {
                MessageService.Warning("The Selected Type you are Trying to Add is already in the List of Permitted Types","Type already in the List");
            }
            else
            {
                PermittedTraitTypes.Add((TypeOfTrait)SelectedPermittedType);
                //Push PropertyChanges , because the ObservableCollection need subscription from outside this class
                //By notifying the Collection Has Cahnged any Listeners to the PropChanged Event will also be notified without having to subscribe to the ObservableCollection.CollectionChanged Event
                OnPropertyChanged(nameof(PermittedTraitTypes));
            }
        }

        [RelayCommand]
        private void RemovePermittedTraitType(TypeOfTrait typeToRemove)
        {
            if (PermittedTraitTypes.Remove(typeToRemove))
            {
                //Push PropertyChanges , because the ObservableCollection need subscription from outside this class
                //By notifying the Collection Has Cahnged any Listeners to the PropChanged Event will also be notified without having to subscribe to the ObservableCollection.CollectionChanged Event
                OnPropertyChanged(nameof(PermittedTraitTypes));
            };
        }


        public TraitGroupEntity CopyPropertiesToModel(TraitGroupEntity model)
        {
            BaseDescriptiveEntity.CopyPropertiesToModel(model);
            model.SortNo = this.SortNo;
            model.IsEnabled = this.IsEnabled;
            model.Code = this.Code;
            model.PermittedTraitTypes = new(this.PermittedTraitTypes);
            return model;
        }

        public TraitGroupEntity GetModel()
        {
            TraitGroupEntity entity = new();
            return this.CopyPropertiesToModel(entity);
        }

        public void SetModel(TraitGroupEntity model)
        {
            BaseDescriptiveEntity.SetModel(model);
            this.SortNo = model.SortNo;
            this.IsEnabled = model.IsEnabled;
            this.Code = model.Code;
            this.PermittedTraitTypes = new(model.PermittedTraitTypes);
        }
    }
}

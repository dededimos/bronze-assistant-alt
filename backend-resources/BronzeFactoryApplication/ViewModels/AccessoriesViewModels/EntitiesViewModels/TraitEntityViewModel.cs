using AccessoriesRepoMongoDB.Entities;
using AccessoriesRepoMongoDB.Repositories;
using BathAccessoriesModelsLibrary;
using BronzeFactoryApplication.ViewModels.HelperViewModels;
using CommonInterfacesBronze;
using MongoDB.Bson;
using MongoDbCommonLibrary.CommonEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.AccessoriesViewModels
{
    public partial class TraitEntityViewModel : BaseViewModel, IEditorViewModel<TraitEntity>
    {
        private readonly OpenImageViewerModalService openImageViewerService;
        private readonly IAccessoryEntitiesRepository repo;
        [ObservableProperty]
        private DbEntityViewModel baseEntity;
        [ObservableProperty]
        private bool isEnabled;
        [ObservableProperty]
        private int sortNo;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsPrimaryTypeTrait))]
        private TypeOfTrait traitType;

        public bool IsPrimaryTypeTrait { get => TraitType == TypeOfTrait.PrimaryTypeTrait; }

        [ObservableProperty]
        private string photoURL = string.Empty;
        [ObservableProperty]
        private string code = string.Empty;
        [ObservableProperty]
        private LocalizedStringViewModel trait;
        [ObservableProperty]
        private LocalizedStringViewModel traitTooltip;

        public ObservableCollection<TraitEntity> SecondaryTypes { get; set; } = new();
        [ObservableProperty]
        private TraitEntity? selectedSecondaryType;

        public ObservableCollection<TraitGroupEntity> AssignedGroups { get; set; } = new();
        [ObservableProperty]
        private TraitGroupEntity? selectedGroup;

        public TraitEntityViewModel(Func<DbEntityViewModel> baseEntityVmFactory,
            Func<LocalizedStringViewModel> localizedStringVmFactory,
            OpenImageViewerModalService openImageViewerService,
            IAccessoryEntitiesRepository repo)
        {
            baseEntity = baseEntityVmFactory.Invoke();
            trait = localizedStringVmFactory.Invoke();
            traitTooltip = localizedStringVmFactory.Invoke();
            this.openImageViewerService = openImageViewerService;
            this.repo = repo;
        }

        private IEnumerable<TraitEntity> GetRepoSecondaryTypeTraits()
        {
            return repo.Traits.Cache.Where(t => t.TraitType == TypeOfTrait.SecondaryTypeTrait);
        }
        private IEnumerable<TraitGroupEntity> GetRepoTraitGroups()
        {
            return repo.Traits.TraitGroups.Cache;
        }

        public TraitEntity CopyPropertiesToModel(TraitEntity model)
        {
            BaseEntity.CopyPropertiesToModel(model);
            model.SortNo = this.SortNo;
            model.IsEnabled = this.IsEnabled;
            model.TraitType = this.TraitType;
            model.PhotoURL = this.PhotoURL;
            model.Code = this.Code;
            model.Trait = this.Trait.GetModel();
            model.TraitTooltip = this.TraitTooltip.GetModel();
            model.AssignedGroups = new(this.AssignedGroups.Select(g => g.Id.ToString()));

            if (this.TraitType == TypeOfTrait.PrimaryTypeTrait && model is PrimaryTypeTraitEntity pt)
            {
                foreach (var secondType in this.SecondaryTypes)
                {
                    pt.AllowedSecondaryTypes.Add(secondType.Id);
                }
                return pt;
            }
            else
            {
                return model;
            }
        }

        public TraitEntity GetModel()
        {
            return this.CopyPropertiesToModel(this.TraitType == TypeOfTrait.PrimaryTypeTrait ? new PrimaryTypeTraitEntity() : new TraitEntity());
        }

        public void SetModel(TraitEntity model)
        {
            BaseEntity.SetModel(model);
            SortNo = model.SortNo;
            IsEnabled = model.IsEnabled;
            TraitType = model.TraitType;
            PhotoURL = model.PhotoURL;
            Code = model.Code;
            Trait.SetModel(model.Trait);
            TraitTooltip.SetModel(model.TraitTooltip);
            SecondaryTypes.Clear();
            if (model is PrimaryTypeTraitEntity pt)
            {
                foreach (var secondTypeId in pt.AllowedSecondaryTypes)
                {
                    var secondTypeToAdd = GetRepoSecondaryTypeTraits().FirstOrDefault(st => st.Id == secondTypeId);
                    if (secondTypeToAdd != null) SecondaryTypes.Add(secondTypeToAdd);
                    else MessageService.Warning($"SecondaryType Id {secondTypeId} was not found in the List of SecondaryTypes", "Secondary Type not Found");
                }
            }
            AssignedGroups.Clear();
            foreach (var groupId in model.AssignedGroups)
            {
                var groupToAdd = GetRepoTraitGroups().FirstOrDefault(g => g.Id.ToString() == groupId);
                if (groupToAdd != null) AssignedGroups.Add(groupToAdd);
                else MessageService.Warning($"TraitGroup Id {groupId} was not found in the List of TraitGroups", "TraitGroup not Found");
            }
        }

        [RelayCommand]
        private void ChangePhotoUrl()
        {
            var filePath = GeneralHelpers.SelectImageFile();
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }
            else
            {
                PhotoURL = filePath;
            }
        }
        [RelayCommand]
        private void OpenImageViewer(string imageUrl)
        {
            openImageViewerService.OpenModal(imageUrl);
        }
        [RelayCommand]
        private void RemoveSecondaryType(TraitEntity secondaryType)
        {
            if (SecondaryTypes.Remove(secondaryType))
            {
                //Push PropertyChanges , because the ObservableCollection need subscription from outside this class
                //By notifying the Collection Has Cahnged any Listeners to the PropChanged Event will also be notified without having to subscribe to the ObservableCollection.CollectionChanged Event
                OnPropertyChanged(nameof(SecondaryTypes));
            };
        }
        [RelayCommand]
        private void AddSecondaryType()
        {
            if (SelectedSecondaryType is null)
            {
                MessageService.Warning("Please first Select a Type to Add", "No Type Selected");
            }
            else if (SecondaryTypes.Any(t => t.Id == SelectedSecondaryType.Id))
            {
                MessageService.Warning("The Secondary Type you are Trying to Add is Already in the List", "Secondary Type already in the List");
            }
            else
            {
                SecondaryTypes.Add(SelectedSecondaryType);
                //Push PropertyChanges , because the ObservableCollection need subscription from outside this class
                //By notifying the Collection Has Cahnged any Listeners to the PropChanged Event will also be notified without having to subscribe to the ObservableCollection.CollectionChanged Event
                OnPropertyChanged(nameof(SecondaryTypes));
            }
        }

        [RelayCommand]
        private void RemoveTraitGroup(TraitGroupEntity traitGroup)
        {
            if (AssignedGroups.Remove(traitGroup))
            {
                //Push PropertyChanges , because the ObservableCollection need subscription from outside this class
                //By notifying the Collection Has Cahnged any Listeners to the PropChanged Event will also be notified without having to subscribe to the ObservableCollection.CollectionChanged Event
                OnPropertyChanged(nameof(AssignedGroups));
            };
        }
        [RelayCommand]
        private void AddTraitGroup()
        {
            if (SelectedGroup is null)
            {
                MessageService.Warning("Please first Select a Group to Add", "No Group Selected");
            }
            else if (AssignedGroups.Any(g => g.Id == SelectedGroup.Id))
            {
                MessageService.Warning("The Group you are Trying to Add is Already in the List", "Group already in the List");
            }
            else
            {
                AssignedGroups.Add(SelectedGroup);
                //Push PropertyChanges , because the ObservableCollection need subscription from outside this class
                //By notifying the Collection Has Cahnged any Listeners to the PropChanged Event will also be notified without having to subscribe to the ObservableCollection.CollectionChanged Event
                OnPropertyChanged(nameof(AssignedGroups));
            }
        }
    }
}

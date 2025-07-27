using AccessoriesRepoMongoDB.Entities;
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
    public partial class TraitClassEntityViewModel : BaseViewModel, IEditorViewModel<TraitClassEntity>
    {
        [ObservableProperty]
        private DescriptiveEntityViewModel baseDescriptiveEntity;
        [ObservableProperty]
        public TypeOfTrait traitType;
        [ObservableProperty]
        private int sortNo;
        [ObservableProperty]
        private bool isEnabled;
        [ObservableProperty]
        private string photoURL = string.Empty;
        [ObservableProperty]
        private ObservableCollection<ObjectId> traits = new();
        private readonly OpenImageViewerModalService openImageViewerService;

        public TraitClassEntityViewModel(Func<DescriptiveEntityViewModel> descriptiveEntityVmFactory, OpenImageViewerModalService openImageViewerService)
        {
            baseDescriptiveEntity = descriptiveEntityVmFactory.Invoke();
            this.openImageViewerService = openImageViewerService;
        }

        public void SetModel(TraitClassEntity model)
        {
            BaseDescriptiveEntity.SetModel(model);
            this.SortNo = model.SortNo;
            this.TraitType = model.TraitType;
            this.IsEnabled = model.IsEnabled;
            this.PhotoURL = model.PhotoURL;
            Traits.Clear();
            foreach (var id in model.Traits)
            {
                Traits.Add(id);
            }
        }

        public TraitClassEntity GetModel()
        {
            TraitClassEntity model = new();
            return this.CopyPropertiesToModel(model);
        }

        public TraitClassEntity CopyPropertiesToModel(TraitClassEntity model)
        {
            BaseDescriptiveEntity.CopyPropertiesToModel(model);
            model.SortNo = this.SortNo;
            model.TraitType = this.TraitType;
            model.IsEnabled = this.IsEnabled;
            model.PhotoURL = this.PhotoURL;
            model.Traits = new(this.Traits);
            return model;
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

    }
}

using AccessoriesRepoMongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BronzeFactoryApplication.ViewModels.AccessoriesViewModels.EntitiesViewModels
{
    public partial class AccessoryFinishInfoViewModel : ObservableObject
    {
        [ObservableProperty]
        private TraitEntity finish;
        [ObservableProperty]
        private string photoUrl = string.Empty;
        [ObservableProperty]
        private ObservableCollection<string> extraPhotosUrl = new();
        [ObservableProperty]
        private string pdfUrl = string.Empty;
        [ObservableProperty]
        private string dimensionsPhotoUrl = string.Empty;

        public AccessoryFinishInfoViewModel(TraitEntity finish)
        {
            this.finish = finish;
        }
        public AccessoryFinishInfoViewModel(TraitEntity finish ,AccessoryFinishInfo finishInfo)
        {
            this.finish = finish;
            this.photoUrl = finishInfo.PhotoUrl;
            this.extraPhotosUrl = new(finishInfo.ExtraPhotosUrl);
            this.pdfUrl = finishInfo.PdfUrl;
            this.dimensionsPhotoUrl = finishInfo.DimensionsPhotoUrl;
        }

        public AccessoryFinishInfo GetFinishInfoObject()
        {
            return new AccessoryFinishInfo()
            {
                FinishId = Finish.Id.ToString(),
                ExtraPhotosUrl = new(ExtraPhotosUrl),
                PdfUrl = this.PdfUrl,
                PhotoUrl = this.PhotoUrl,
                DimensionsPhotoUrl = this.DimensionsPhotoUrl,
            };
        }
    }
}

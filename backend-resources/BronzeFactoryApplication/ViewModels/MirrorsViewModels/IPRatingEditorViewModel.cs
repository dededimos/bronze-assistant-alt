using CommonInterfacesBronze;
using MirrorsLib;
using static MirrorsLib.IPRating;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels
{
    public partial class IPRatingEditorViewModel : BaseViewModel, IEditorViewModel<IPRating>
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IPRatingLettering))]
        [NotifyPropertyChangedFor(nameof(IPRatingFullDescription))]
        [NotifyPropertyChangedFor(nameof(SolidsRatingDescription))]
        private IPSolidsRating solidsRating;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IPRatingLettering))]
        [NotifyPropertyChangedFor(nameof(IPRatingFullDescription))]
        [NotifyPropertyChangedFor(nameof(WaterRatingDescription))]
        private IPWaterRating waterRating;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IPRatingLettering))]
        [NotifyPropertyChangedFor(nameof(IPRatingFullDescription))]
        [NotifyPropertyChangedFor(nameof(AdditionalLetterDescription))]
        private IPAdditionalLetter additionalLetter;

        public string IPRatingLettering { get => GetModel().GetIPRating(); }
        public string IPRatingFullDescription { get => GetModel().GetIPDescription(); }
        public string SolidsRatingDescription { get => IPRating.GetSolidsDescription(SolidsRating); }
        public string WaterRatingDescription { get => IPRating.GetWaterDescription(WaterRating); }
        public string AdditionalLetterDescription { get => IPRating.GetAdditionalLetterDescription(AdditionalLetter); }

        public IPRating CopyPropertiesToModel(IPRating model)
        {
            model.SolidsRating = this.SolidsRating;
            model.WaterRating = this.WaterRating;
            model.AdditionalLetter = this.AdditionalLetter;
            return model;
        }

        public IPRating GetModel()
        {
            return CopyPropertiesToModel(new());
        }

        public void SetModel(IPRating model)
        {
            this.SolidsRating = model.SolidsRating;
            this.WaterRating = model.WaterRating;
            this.AdditionalLetter = model.AdditionalLetter;
        }
    }
}

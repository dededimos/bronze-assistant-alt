using BronzeFactoryApplication.ViewModels.AccessoriesViewModels;
using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels
{
    public partial class MirrorElementInfoViewModel : BaseViewModel, IEditorViewModel<MirrorElementBase>
    {
        [ObservableProperty]
        private string code = string.Empty;
        [ObservableProperty]
        private LocalizedString name = LocalizedString.Undefined();
        [ObservableProperty]
        private LocalizedString description = LocalizedString.Undefined();
        [ObservableProperty]
        private LocalizedString extendedDescription = LocalizedString.Undefined();


        [ObservableProperty]
        private string photoUrl = string.Empty;
        [ObservableProperty]
        private string photoUrl2 = string.Empty;
        [ObservableProperty]
        private string iconUrl = string.Empty;

        public MirrorElementBase CopyPropertiesToModel(MirrorElementBase model)
        {
            model.Code = this.Code;
            model.LocalizedDescriptionInfo = LocalizedDescription.Create(Name.GetDeepClone(), Description.GetDeepClone(), ExtendedDescription.GetDeepClone());
            model.PhotoUrl = this.PhotoUrl;
            model.PhotoUrl2 = this.PhotoUrl2;
            model.IconUrl = this.IconUrl;
            return model;
        }

        public MirrorElementBase GetModel()
        {
            return CopyPropertiesToModel(new());
        }

        public void SetModel(MirrorElementBase model)
        {
            this.Code = model.Code;
            this.Name = model.LocalizedDescriptionInfo.Name.GetDeepClone();
            this.Description = model.LocalizedDescriptionInfo.Description.GetDeepClone();
            this.ExtendedDescription = model.LocalizedDescriptionInfo.ExtendedDescription.GetDeepClone();
            this.PhotoUrl = model.PhotoUrl;
            this.PhotoUrl2 = model.PhotoUrl2;
            this.IconUrl = model.IconUrl;
        }
    }
}

using CommonInterfacesBronze;
using MirrorsLib.MirrorElements;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels
{
    public partial class MirrorAdditionalLightInfoEditorViewModel : BaseViewModel, IEditorViewModel<MirrorAdditionalLightInfo>
    {
        [ObservableProperty]
        private IlluminationOption illumination;
        [ObservableProperty]
        private double lengthMeters;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsPoweredExternally))]
        private bool needsTouchButton;
        public bool IsPoweredExternally 
        {
            get => !NeedsTouchButton;
            set
            {
                NeedsTouchButton = !value;
            }
        }

        public MirrorAdditionalLightInfo CopyPropertiesToModel(MirrorAdditionalLightInfo model)
        {
            model.Illumination = this.Illumination;
            model.LengthMeters = this.LengthMeters;
            model.NeedsTouchButton = this.NeedsTouchButton;
            return model;
        }

        public MirrorAdditionalLightInfo GetModel()
        {
            return CopyPropertiesToModel(new());
        }

        public void SetModel(MirrorAdditionalLightInfo model)
        {
            this.Illumination = model.Illumination;
            this.LengthMeters = model.LengthMeters;
            this.NeedsTouchButton = model.NeedsTouchButton;
        }
    }
}

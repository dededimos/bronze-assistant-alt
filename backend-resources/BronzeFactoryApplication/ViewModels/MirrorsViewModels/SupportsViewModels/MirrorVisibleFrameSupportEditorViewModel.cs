using CommonInterfacesBronze;
using MirrorsLib.MirrorElements.Supports;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.SupportsViewModels
{
    public partial class MirrorVisibleFrameSupportEditorViewModel : MirrorSupportInfoBaseViewModel, IEditorViewModel<MirrorVisibleFrameSupport>
    {
        [ObservableProperty]
        private double frontThickness;
        [ObservableProperty]
        private double depth;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(GlassShrink))]
        private double rearThickness1;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(GlassShrink))]
        private double rearThickness2;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(GlassShrink))]
        private double glassInProfile;

        public double GlassShrink { get => MirrorVisibleFrameSupport.GetGlassShrink(RearThickness1, GlassInProfile); }
        
        public MirrorVisibleFrameSupport CopyPropertiesToModel(MirrorVisibleFrameSupport model)
        {
            CopyBasePropertiesToModel(model);
            model.FrontThickness = this.FrontThickness;
            model.Depth = this.Depth;
            model.RearThickness1 = this.RearThickness1;
            model.RearThickness2 = this.RearThickness2;
            model.GlassInProfile = this.GlassInProfile;
            return model;
        }

        public MirrorVisibleFrameSupport GetModel()
        {
            return CopyPropertiesToModel(new());
        }

        public void SetModel(MirrorVisibleFrameSupport model)
        {
            SetBaseProperties(model);
            this.FrontThickness = model.FrontThickness;
            this.Depth = model.Depth;
            this.RearThickness1 = model.RearThickness1;
            this.RearThickness2 = model.RearThickness2;
            this.GlassInProfile = model.GlassInProfile;
        }
    }
}

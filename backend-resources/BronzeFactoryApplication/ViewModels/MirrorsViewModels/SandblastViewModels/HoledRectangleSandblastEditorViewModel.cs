using CommonInterfacesBronze;
using MirrorsLib.MirrorElements.Sandblasts;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.SandblastViewModels
{
    public partial class HoledRectangleSandblastEditorViewModel : MirrorSandblastInfoBaseViewModel, IEditorViewModel<HoledRectangleSandblast>
    {
        [ObservableProperty]
        private double edgeDistance;
        [ObservableProperty]
        private double cornerRadius;
        [ObservableProperty]
        private bool followsRectangleGlassCornerRadius;

        public HoledRectangleSandblastEditorViewModel()
        {
            SandblastType = MirrorsLib.Enums.MirrorSandblastType.HoledRectangleSandblast;
        }

        public HoledRectangleSandblast CopyPropertiesToModel(HoledRectangleSandblast model)
        {
            CopyBasePropertiesToModel(model);
            model.EdgeDistance = this.EdgeDistance;
            model.CornerRadius = this.CornerRadius;
            model.FollowsRectangleGlassCornerRadius = this.FollowsRectangleGlassCornerRadius;
            return model;
        }

        public HoledRectangleSandblast GetModel()
        {
            return CopyPropertiesToModel(new());
        }

        public void SetModel(HoledRectangleSandblast model)
        {
            SuppressPropertyNotifications();
            SetBaseProperties(model);
            this.EdgeDistance = model.EdgeDistance;
            this.CornerRadius = model.CornerRadius;
            this.FollowsRectangleGlassCornerRadius = model.FollowsRectangleGlassCornerRadius;
            ResumePropertyNotifications();
            OnPropertyChanged("");
        }
    }
    
}

using CommonInterfacesBronze;
using MirrorsLib.MirrorElements.Sandblasts;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.SandblastViewModels
{
    public partial class TwoLineSandblastEditorViewModel : MirrorSandblastInfoBaseViewModel, IEditorViewModel<TwoLineSandblast>
    {
        [ObservableProperty]
        private double verticalEdgeDistance;
        [ObservableProperty]
        private double horizontalEdgeDistance;
        [ObservableProperty]
        private double cornerRadius;
        [ObservableProperty]
        private bool isVertical;
        public TwoLineSandblastEditorViewModel()
        {
            SandblastType = MirrorsLib.Enums.MirrorSandblastType.TwoLineSandblast;
        }
        public TwoLineSandblast CopyPropertiesToModel(TwoLineSandblast model)
        {
            CopyBasePropertiesToModel(model);
            model.VerticalEdgeDistance = this.VerticalEdgeDistance;
            model.HorizontalEdgeDistance = this.HorizontalEdgeDistance;
            model.IsVertical = this.IsVertical;
            model.CornerRadius = this.CornerRadius;
            return model;
        }

        public TwoLineSandblast GetModel()
        {
            return CopyPropertiesToModel(new());
        }

        public void SetModel(TwoLineSandblast model)
        {
            SuppressPropertyNotifications();
            SetBaseProperties(model);
            this.VerticalEdgeDistance = model.VerticalEdgeDistance;
            this.HorizontalEdgeDistance = model.HorizontalEdgeDistance;
            this.CornerRadius = model.CornerRadius;
            this.IsVertical = model.IsVertical;
            ResumePropertyNotifications();
            OnPropertyChanged("");
        }
    }
    
}

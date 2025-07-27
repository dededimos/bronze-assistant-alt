using CommonInterfacesBronze;
using MirrorsLib.MirrorElements.Sandblasts;

namespace BronzeFactoryApplication.ViewModels.MirrorsViewModels.SandblastViewModels
{
    public partial class CircularSandblastEditorViewModel : MirrorSandblastInfoBaseViewModel, IEditorViewModel<CircularSandblast>
    {
        [ObservableProperty]
        private double? distanceFromEdge;
        [ObservableProperty]
        private double? distanceFromCenter;
        public CircularSandblastEditorViewModel()
        {
            SandblastType = MirrorsLib.Enums.MirrorSandblastType.CircularSandblast;
        }
        public CircularSandblast CopyPropertiesToModel(CircularSandblast model)
        {
            CopyBasePropertiesToModel(model);
            model.DistanceFromEdge = this.DistanceFromEdge;
            model.DistanceFromCenter = this.DistanceFromCenter;
            return model;
        }

        public CircularSandblast GetModel()
        {
            return CopyPropertiesToModel(new());
        }

        public void SetModel(CircularSandblast model)
        {
            SetBaseProperties(model);
            this.DistanceFromEdge = model.DistanceFromEdge;
            this.DistanceFromCenter = model.DistanceFromCenter;
        }
    }
    
}

using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using MirrorsLib.DrawingElements.SandblastDrawingOptions;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels.DimesnsionsPresOptionsVms
{
    public partial class CircularSandblastInfoDimensionOptionsVm : BaseViewModel, IEditorViewModel<CircularSandblastInfoDimensionOptions>
    {
        public CircularSandblastInfoDimensionOptionsVm(Func<CircleRingDimensionsPresentationOptionsVm> circleRingDimOptionsVmFactory,
            Func<DimensionLineOptionsVm> lineOptionsVmFactory,
            Func<DrawingPresentationOptionsVm> presOptionsVmFactory)
        {
            SandblastBodyDimensionOptions = circleRingDimOptionsVmFactory.Invoke();
            DistanceFromEdgeLineOptions = lineOptionsVmFactory.Invoke();
            DistanceFromEdgePresentationOptions = presOptionsVmFactory.Invoke();
            SetModel(model);
        }

        private CircularSandblastInfoDimensionOptions model = CircularSandblastInfoDimensionOptions.Defaults(false);

        public CircleRingDimensionsPresentationOptionsVm SandblastBodyDimensionOptions { get; }
        public bool ShowDistanceFromEdge
        {
            get => model.ShowDistanceFromEdge;
            set => SetProperty(model.ShowDistanceFromEdge, value, model, (m, v) => m.ShowDistanceFromEdge = v);
        }

        public CircularSandblastEdgeDimensionPosition EdgeDistanceDimensionPosition
        {
            get => model.EdgeDistanceDimensionPosition;
            set => SetProperty(model.EdgeDistanceDimensionPosition, value, model, (m, v) => m.EdgeDistanceDimensionPosition = v);
        }

        public DimensionLineOptionsVm DistanceFromEdgeLineOptions { get; }
        public DrawingPresentationOptionsVm DistanceFromEdgePresentationOptions { get; }


        public CircularSandblastInfoDimensionOptions CopyPropertiesToModel(CircularSandblastInfoDimensionOptions model)
        {
            throw new NotImplementedException();
        }

        public CircularSandblastInfoDimensionOptions GetModel()
        {
            return model;
        }

        public void SetModel(CircularSandblastInfoDimensionOptions model)
        {
            if (this.model != model) this.model = model;
            SandblastBodyDimensionOptions.SetModel(model.SandblastBodyDimensionOptions);
            DistanceFromEdgeLineOptions.SetModel(model.DistanceFromEdgeLineOptions);
            DistanceFromEdgePresentationOptions.SetModel(model.DistanceFromEdgePresentationOptions);
            OnPropertyChanged("");
        }
    }

}

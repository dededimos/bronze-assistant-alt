using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using MirrorsLib.DrawingElements.SandblastDrawingOptions;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels.DimesnsionsPresOptionsVms
{
    public partial class TwoLineSandblastInfoDimensionOptionsVm : BaseViewModel, IEditorViewModel<TwoLineSandblastInfoDimensionOptions>
    {
        public TwoLineSandblastInfoDimensionOptionsVm(Func<RectangleDimensionsPresentationOptionsVm> rectDimOptionsVmFactory,
                                                      Func<DimensionLineOptionsVm> lineOptionsVmFactory,
                                                      Func<DrawingPresentationOptionsVm> presOptionsVmFactory)
        {
            SandblastBodyDimensionOptions = rectDimOptionsVmFactory.Invoke();
            DistanceFromEdgeLineOptions = lineOptionsVmFactory.Invoke();
            DistanceFromEdgePresentationOptions = presOptionsVmFactory.Invoke();
            SetModel(model);
        }

        private TwoLineSandblastInfoDimensionOptions model = TwoLineSandblastInfoDimensionOptions.Defaults(false);

        public RectangleDimensionsPresentationOptionsVm SandblastBodyDimensionOptions { get; }
        public LineSandblastVerticalDistancePosition VerticalDistanceFromEdgePosition
        {
            get => model.VerticalDistanceFromEdgePosition;
            set => SetProperty(model.VerticalDistanceFromEdgePosition, value, model, (m, v) => m.VerticalDistanceFromEdgePosition = v);
        }

        public LineSandblastHorizontalDistancePosition HorizontalDistanceFromEdgePosition
        {
            get => model.HorizontalDistanceFromEdgePosition;
            set => SetProperty(model.HorizontalDistanceFromEdgePosition, value, model, (m, v) => m.HorizontalDistanceFromEdgePosition = v);
        }

        public bool ShowHorizontalDistance
        {
            get => model.ShowHorizontalDistance;
            set => SetProperty(model.ShowHorizontalDistance, value, model, (m, v) => m.ShowHorizontalDistance = v);
        }

        public bool ShowVerticalDistance
        {
            get => model.ShowVerticalDistance;
            set => SetProperty(model.ShowVerticalDistance, value, model, (m, v) => m.ShowVerticalDistance = v);
        }

        public DimensionLineOptionsVm DistanceFromEdgeLineOptions { get; }
        public DrawingPresentationOptionsVm DistanceFromEdgePresentationOptions { get; }

        public TwoLineSandblastInfoDimensionOptions CopyPropertiesToModel(TwoLineSandblastInfoDimensionOptions model)
        {
            throw new NotImplementedException();
        }

        public TwoLineSandblastInfoDimensionOptions GetModel()
        {
            return model;
        }

        public void SetModel(TwoLineSandblastInfoDimensionOptions model)
        {
            if (this.model != model) this.model = model;
            SandblastBodyDimensionOptions.SetModel(model.SandblastBodyDimensionOptions);
            DistanceFromEdgeLineOptions.SetModel(model.DistanceFromEdgeLineOptions);
            DistanceFromEdgePresentationOptions.SetModel(model.DistanceFromEdgePresentationOptions);
            OnPropertyChanged("");
        }
    }

}

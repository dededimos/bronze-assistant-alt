using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using MirrorsLib.DrawingElements.SandblastDrawingOptions;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels.DimesnsionsPresOptionsVms
{
    public partial class HoledRectangleSandblastInfoDimensionOptionsVm : BaseViewModel, IEditorViewModel<HoledRectangleSandblastInfoDimensionOptions>
    {
        public HoledRectangleSandblastInfoDimensionOptionsVm(Func<RectangleRingDimensionsPresentationOptionsVm> rectRingDimOptionsVmFactory,
            Func<DimensionLineOptionsVm> lineOptionsVmFactory,
            Func<DrawingPresentationOptionsVm> presOptionsVmFactory)
        {
            SandblastBodyDimensionOptions = rectRingDimOptionsVmFactory.Invoke();
            DistanceFromEdgeLineOptions = lineOptionsVmFactory.Invoke();
            DistanceFromEdgePresentationOptions = presOptionsVmFactory.Invoke();
            SetModel(model);
        }

        private HoledRectangleSandblastInfoDimensionOptions model = HoledRectangleSandblastInfoDimensionOptions.Defaults(false);

        public RectangleRingDimensionsPresentationOptionsVm SandblastBodyDimensionOptions { get; }

        public RectangleEdgeDistanceDimensionAnchorPoint EdgeDistanceDimensionPosition
        {
            get => model.EdgeDistanceDimensionPosition;
            set => SetProperty(model.EdgeDistanceDimensionPosition, value, model, (m, v) => m.EdgeDistanceDimensionPosition = v);
        }

        public DimensionLineOptionsVm DistanceFromEdgeLineOptions { get; }
        public DrawingPresentationOptionsVm DistanceFromEdgePresentationOptions { get; }


        public HoledRectangleSandblastInfoDimensionOptions CopyPropertiesToModel(HoledRectangleSandblastInfoDimensionOptions model)
        {
            throw new NotSupportedException($"{nameof(HoledRectangleSandblastInfoDimensionOptionsVm)} does not support {nameof(CopyPropertiesToModel)} Method");
        }

        public HoledRectangleSandblastInfoDimensionOptions GetModel()
        {
            return model;
        }

        public void SetModel(HoledRectangleSandblastInfoDimensionOptions model)
        {
            if (this.model != model) this.model = model;
            SandblastBodyDimensionOptions.SetModel(model.SandblastBodyDimensionOptions);
            DistanceFromEdgeLineOptions.SetModel(model.DistanceFromEdgeLineOptions);
            DistanceFromEdgePresentationOptions.SetModel(model.DistanceFromEdgePresentationOptions);
            OnPropertyChanged("");
        }
    }

}

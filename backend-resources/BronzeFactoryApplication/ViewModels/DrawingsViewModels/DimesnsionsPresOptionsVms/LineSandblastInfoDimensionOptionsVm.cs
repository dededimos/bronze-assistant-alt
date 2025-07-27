using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using MirrorsLib.DrawingElements.SandblastDrawingOptions;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels.DimesnsionsPresOptionsVms
{
    public partial class LineSandblastInfoDimensionOptionsVm : BaseViewModel, IEditorViewModel<LineSandblastInfoDimensionOptions>
    {
        public LineSandblastInfoDimensionOptionsVm(Func<RectangleDimensionsPresentationOptionsVm> rectangleDimOptionsVmFactory,
            Func<DimensionLineOptionsVm> lineOptionsVmFactory,
            Func<DrawingPresentationOptionsVm> presOptionsVmFactory)
        {
            SandblastBodyDimensionOptions = rectangleDimOptionsVmFactory.Invoke();
            DistanceFromEdgeLineOptions = lineOptionsVmFactory.Invoke();
            DistanceFromEdgePresentationOptions = presOptionsVmFactory.Invoke();
            SetModel(model);
        }

        private LineSandblastInfoDimensionOptions model = LineSandblastInfoDimensionOptions.Defaults(false);


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

        public bool ShowDistanceFromTop
        {
            get => model.ShowDistanceFromTop;
            set => SetProperty(model.ShowDistanceFromTop, value, model, (m, v) => m.ShowDistanceFromTop = v);
        }

        public bool ShowDistanceFromBottom
        {
            get => model.ShowDistanceFromBottom;
            set => SetProperty(model.ShowDistanceFromBottom, value, model, (m, v) => m.ShowDistanceFromBottom = v);
        }

        public bool ShowDistanceFromLeft
        {
            get => model.ShowDistanceFromLeft;
            set => SetProperty(model.ShowDistanceFromLeft, value, model, (m, v) => m.ShowDistanceFromLeft = v);
        }

        public bool ShowDistanceFromRight
        {
            get => model.ShowDistanceFromRight;
            set => SetProperty(model.ShowDistanceFromRight, value, model, (m, v) => m.ShowDistanceFromRight = v);
        }

        public DimensionLineOptionsVm DistanceFromEdgeLineOptions { get; }
        public DrawingPresentationOptionsVm DistanceFromEdgePresentationOptions { get; }



        public LineSandblastInfoDimensionOptions CopyPropertiesToModel(LineSandblastInfoDimensionOptions model)
        {
            throw new NotImplementedException();
        }

        public LineSandblastInfoDimensionOptions GetModel()
        {
            return model;
        }

        public void SetModel(LineSandblastInfoDimensionOptions model)
        {
            if (this.model != model) this.model = model;
            SandblastBodyDimensionOptions.SetModel(model.SandblastBodyDimensionOptions);
            DistanceFromEdgeLineOptions.SetModel(model.DistanceFromEdgeLineOptions);
            DistanceFromEdgePresentationOptions.SetModel(model.DistanceFromEdgePresentationOptions);
            OnPropertyChanged("");
        }
    }


}

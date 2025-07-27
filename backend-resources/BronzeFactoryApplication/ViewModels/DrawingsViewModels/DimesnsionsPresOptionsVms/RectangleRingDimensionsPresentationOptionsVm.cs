using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using MirrorsLib.DrawingElements;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels.DimesnsionsPresOptionsVms
{
    public partial class RectangleRingDimensionsPresentationOptionsVm : ShapeDimensionsPresentationOptionsVm, IEditorViewModel<RectangleRingDimensionsPresentationOptions>
    {
        public RectangleRingDimensionsPresentationOptionsVm(Func<DimensionLineOptionsVm> lineOptionsVmFactory,
                                                            Func<DrawingPresentationOptionsVm> presOptionsVmFactory)
            : base(MirrorDrawOptions.GetDefaultDimensionOptions<RectangleRingDimensionsPresentationOptions>(false),
                 lineOptionsVmFactory, presOptionsVmFactory)
        {
            ThicknessLineOptions = lineOptionsVmFactory.Invoke();
            ThicknessPresentationOptions = presOptionsVmFactory.Invoke();
            InnerLengthLineOptions = lineOptionsVmFactory.Invoke();
            InnerLengthPresentationOptions = presOptionsVmFactory.Invoke();
            InnerHeightLineOptions = lineOptionsVmFactory.Invoke();
            InnerHeightPresentationOptions = presOptionsVmFactory.Invoke();
            SetModel(RectangleRingOptions);
        }

        private RectangleRingDimensionsPresentationOptions RectangleRingOptions { get => (RectangleRingDimensionsPresentationOptions)model; }

        public bool ShowThickness
        {
            get => RectangleRingOptions.ShowThickness;
            set => SetProperty(RectangleRingOptions.ShowThickness, value, RectangleRingOptions, (m, v) => m.ShowThickness = v);
        }

        public RectangleRingThicknessDimensionPosition THicknessPosition
        {
            get => RectangleRingOptions.ThicknessPosition;
            set => SetProperty(RectangleRingOptions.ThicknessPosition, value, RectangleRingOptions, (m, v) => m.ThicknessPosition = v);
        }

        public double ThicknessMarginFromShape
        {
            get => RectangleRingOptions.ThicknessMarginFromShape;
            set => SetProperty(RectangleRingOptions.ThicknessMarginFromShape, value, RectangleRingOptions, (m, v) => m.ThicknessMarginFromShape = v);
        }

        public DimensionLineOptionsVm ThicknessLineOptions { get; }
        public DrawingPresentationOptionsVm ThicknessPresentationOptions { get; }

        public bool ShowInnerLength
        {
            get => RectangleRingOptions.ShowInnerLength;
            set => SetProperty(RectangleRingOptions.ShowInnerLength, value, RectangleRingOptions, (m, v) => m.ShowInnerLength = v);
        }

        public RectangleLengthDimensionPosition InnerLengthPosition
        {
            get => RectangleRingOptions.InnerLengthPosition;
            set => SetProperty(RectangleRingOptions.InnerLengthPosition, value, RectangleRingOptions, (m, v) => m.InnerLengthPosition = v);
        }

        public DimensionLineOptionsVm InnerLengthLineOptions { get; }
        public DrawingPresentationOptionsVm InnerLengthPresentationOptions { get; }

        public double InnerLengthMarginFromShape
        {
            get => RectangleRingOptions.InnerLengthMarginFromShape;
            set => SetProperty(RectangleRingOptions.InnerLengthMarginFromShape, value, RectangleRingOptions, (m, v) => m.InnerLengthMarginFromShape = v);
        }

        public bool ShowInnerHeight
        {
            get => RectangleRingOptions.ShowInnerHeight;
            set => SetProperty(RectangleRingOptions.ShowInnerHeight, value, RectangleRingOptions, (m, v) => m.ShowInnerHeight = v);
        }

        public RectangleHeightDimensionPosition InnerHeightPosition
        {
            get => RectangleRingOptions.InnerHeightPosition;
            set => SetProperty(RectangleRingOptions.InnerHeightPosition, value, RectangleRingOptions, (m, v) => m.InnerHeightPosition = v);
        }

        public DimensionLineOptionsVm InnerHeightLineOptions { get; }
        public DrawingPresentationOptionsVm InnerHeightPresentationOptions { get; }

        public double InnerHeightMarginFromShape
        {
            get => RectangleRingOptions.InnerHeightMarginFromShape;
            set => SetProperty(RectangleRingOptions.InnerHeightMarginFromShape, value, RectangleRingOptions, (m, v) => m.InnerHeightMarginFromShape = v);
        }

        public RectangleRingDimensionsPresentationOptions CopyPropertiesToModel(RectangleRingDimensionsPresentationOptions model)
        {
            throw new NotSupportedException($"{nameof(RectangleRingDimensionsPresentationOptionsVm)} does not support {nameof(CopyPropertiesToModel)} Method");
        }

        public RectangleRingDimensionsPresentationOptions GetModel()
        {
            return RectangleRingOptions;
        }

        public void SetModel(RectangleRingDimensionsPresentationOptions model)
        {
            SetBaseModel(model);
            ThicknessLineOptions.SetModel(model.ThicknessLineOptions);
            ThicknessPresentationOptions.SetModel(model.ThicknessPresentationOptions);
            InnerLengthLineOptions.SetModel(model.InnerLengthLineOptions);
            InnerLengthPresentationOptions.SetModel(model.InnerLengthPresentationOptions);
            InnerHeightLineOptions.SetModel(model.InnerHeightLineOptions);
            InnerHeightPresentationOptions.SetModel(model.InnerHeightPresentationOptions);
            OnPropertyChanged("");
        }
    }

}

using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using MirrorsLib.DrawingElements;

namespace BronzeFactoryApplication.ViewModels.DrawingsViewModels.DimesnsionsPresOptionsVms
{
    public partial class RectangleDimensionsPresentationOptionsVm : ShapeDimensionsPresentationOptionsVm, IEditorViewModel<RectangleDimensionsPresentationOptions>
    {
        public RectangleDimensionsPresentationOptionsVm(Func<DimensionLineOptionsVm> dimensionLinesOptionsVmFactory,
            Func<DrawingPresentationOptionsVm> presOptionsVmFactory) : base(MirrorDrawOptions.GetDefaultDimensionOptions<RectangleDimensionsPresentationOptions>(false), dimensionLinesOptionsVmFactory, presOptionsVmFactory)
        {
            TopLeftRadiusLineOptions = dimensionLinesOptionsVmFactory.Invoke();
            TopRightRadiusLineOptions = dimensionLinesOptionsVmFactory.Invoke();
            BottomLeftRadiusLineOptions = dimensionLinesOptionsVmFactory.Invoke();
            BottomRightRadiusLineOptions = dimensionLinesOptionsVmFactory.Invoke();

            TopLeftRadiusPresentationOptions = presOptionsVmFactory.Invoke();
            TopRightRadiusPresentationOptions = presOptionsVmFactory.Invoke();
            BottomLeftRadiusPresentationOptions = presOptionsVmFactory.Invoke();
            BottomRightRadiusPresentationOptions = presOptionsVmFactory.Invoke();
            SetModel(RectangleOptions);
        }

        private RectangleDimensionsPresentationOptions RectangleOptions { get => (RectangleDimensionsPresentationOptions)model; }

        public RectangleRadiusDimensionShowOption RadiusOptionWhenAllZero
        {
            get => RectangleOptions.RadiusOptionWhenAllZero;
            set => SetProperty(RectangleOptions.RadiusOptionWhenAllZero, value, RectangleOptions, (m, v) => m.RadiusOptionWhenAllZero = v);
        }

        public RectangleRadiusDimensionShowOption RadiusOptionWhenTotalRadius
        {
            get => RectangleOptions.RadiusOptionWhenTotalRadius;
            set => SetProperty(RectangleOptions.RadiusOptionWhenTotalRadius, value, RectangleOptions, (m, v) => m.RadiusOptionWhenTotalRadius = v);
        }

        public double TopLeftRadiusMarginFromShape
        {
            get => RectangleOptions.TopLeftRadiusMarginFromShape;
            set => SetProperty(RectangleOptions.TopLeftRadiusMarginFromShape, value, RectangleOptions, (m, v) => m.TopLeftRadiusMarginFromShape = v);
        }

        public double TopRightRadiusMarginFromShape
        {
            get => RectangleOptions.TopRightRadiusMarginFromShape;
            set => SetProperty(RectangleOptions.TopRightRadiusMarginFromShape, value, RectangleOptions, (m, v) => m.TopRightRadiusMarginFromShape = v);
        }

        public double BottomLeftRadiusMarginFromShape
        {
            get => RectangleOptions.BottomLeftRadiusMarginFromShape;
            set => SetProperty(RectangleOptions.BottomLeftRadiusMarginFromShape, value, RectangleOptions, (m, v) => m.BottomLeftRadiusMarginFromShape = v);
        }

        public double BottomRightRadiusMarginFromShape
        {
            get => RectangleOptions.BottomRightRadiusMarginFromShape;
            set => SetProperty(RectangleOptions.BottomRightRadiusMarginFromShape, value, RectangleOptions, (m, v) => m.BottomRightRadiusMarginFromShape = v);
        }

        public DimensionLineOptionsVm TopLeftRadiusLineOptions { get; }
        public DimensionLineOptionsVm TopRightRadiusLineOptions { get; }
        public DimensionLineOptionsVm BottomLeftRadiusLineOptions { get; }
        public DimensionLineOptionsVm BottomRightRadiusLineOptions { get; }

        public DrawingPresentationOptionsVm TopLeftRadiusPresentationOptions { get; }
        public DrawingPresentationOptionsVm TopRightRadiusPresentationOptions { get; }
        public DrawingPresentationOptionsVm BottomLeftRadiusPresentationOptions { get; }
        public DrawingPresentationOptionsVm BottomRightRadiusPresentationOptions { get; }



        public RectangleDimensionsPresentationOptions CopyPropertiesToModel(RectangleDimensionsPresentationOptions model)
        {
            throw new NotSupportedException($"{nameof(RectangleDimensionsPresentationOptionsVm)} does not support {nameof(CopyPropertiesToModel)} Method");
        }

        public RectangleDimensionsPresentationOptions GetModel()
        {
            return RectangleOptions;
        }

        public void SetModel(RectangleDimensionsPresentationOptions model)
        {
            SetBaseModel(model);
            TopLeftRadiusLineOptions.SetModel(model.TopLeftRadiusLineOptions);
            TopRightRadiusLineOptions.SetModel(model.TopRightRadiusLineOptions);
            BottomLeftRadiusLineOptions.SetModel(model.BottomLeftRadiusLineOptions);
            BottomRightRadiusLineOptions.SetModel(model.BottomRightRadiusLineOptions);

            TopLeftRadiusPresentationOptions.SetModel(model.TopLeftRadiusPresentationOptions);
            TopRightRadiusPresentationOptions.SetModel(model.TopRightRadiusPresentationOptions);
            BottomLeftRadiusPresentationOptions.SetModel(model.BottomLeftRadiusPresentationOptions);
            BottomRightRadiusPresentationOptions.SetModel(model.BottomRightRadiusPresentationOptions);
            OnPropertyChanged("");
        }
    }


}

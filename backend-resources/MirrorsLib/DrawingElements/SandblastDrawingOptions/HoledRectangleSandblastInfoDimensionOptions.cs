using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using DrawingLibrary.Models.PresentationOptions;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;

namespace MirrorsLib.DrawingElements.SandblastDrawingOptions
{
    public class HoledRectangleSandblastInfoDimensionOptions : IDeepClonable<HoledRectangleSandblastInfoDimensionOptions>
    {
        public RectangleRingDimensionsPresentationOptions SandblastBodyDimensionOptions { get; set; } = new();
        public RectangleEdgeDistanceDimensionAnchorPoint EdgeDistanceDimensionPosition { get; set; } = RectangleEdgeDistanceDimensionAnchorPoint.TopMiddle;
        public DimensionLineOptions DistanceFromEdgeLineOptions { get; set; } = new();
        public DrawingPresentationOptions DistanceFromEdgePresentationOptions { get; set; } = new();

        public static HoledRectangleSandblastInfoDimensionOptions Defaults(bool isSketch)
        {
            var defaults = new HoledRectangleSandblastInfoDimensionOptions()
            {
                SandblastBodyDimensionOptions = MirrorDrawOptions.GetDefaultDimensionOptions<RectangleRingDimensionsPresentationOptions>(isSketch),
                DistanceFromEdgeLineOptions = DimensionLineOptions.DefaultTwoArrowLineOptions(),
                DistanceFromEdgePresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferSmallerXAnchorline),
            };
            var dimensionDrawingOption = isSketch ? DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption : DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption;
            switch (dimensionDrawingOption)
            {
                case DimensionDrawingOption.DoNotAllowDimensionDraws:
                case DimensionDrawingOption.AllowOnlyMainDrawBasicDimensionsDraws:
                case DimensionDrawingOption.AllowOnlyShapeDimensions:
                    defaults.EdgeDistanceDimensionPosition = RectangleEdgeDistanceDimensionAnchorPoint.None;
                    break;
                case DimensionDrawingOption.AllowAllDimensionDraws:
                case DimensionDrawingOption.AllowOnlyDistancesDimensions:
                    defaults.EdgeDistanceDimensionPosition = RectangleEdgeDistanceDimensionAnchorPoint.TopMiddle;
                    break;
                default:
                    throw new EnumValueNotSupportedException(dimensionDrawingOption);
            }
            return defaults;
        }

        public HoledRectangleSandblastInfoDimensionOptions GetDeepClone()
        {
            var clone = (HoledRectangleSandblastInfoDimensionOptions)this.MemberwiseClone();
            clone.SandblastBodyDimensionOptions = this.SandblastBodyDimensionOptions.GetDeepClone();
            clone.DistanceFromEdgeLineOptions = this.DistanceFromEdgeLineOptions.GetDeepClone();
            clone.DistanceFromEdgePresentationOptions = this.DistanceFromEdgePresentationOptions.GetDeepClone();
            return clone;
        }
    }

}

using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using DrawingLibrary.Models.PresentationOptions;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;

namespace MirrorsLib.DrawingElements.SupportsDrawingOptions
{
    public class MirrorBackFrameSupportDimensionOptions : IDeepClonable<MirrorBackFrameSupportDimensionOptions>
    {
        public RectangleRingDimensionsPresentationOptions BodyDimensionOptions { get; set; } = new();
        public RectangleEdgeDistanceDimensionAnchorPoint EdgeDistanceAnchorPoint { get; set; }
        public bool ShowEdgeDistance { get; set; }
        public DimensionLineOptions EdgeDistanceLineOptions { get; set; } = new();
        public DrawingPresentationOptions EdgeDistancePresentationOptions { get; set; } = new();

        public static MirrorBackFrameSupportDimensionOptions Defaults(bool isSketch)
        {
            var dimensionDrawingOption = isSketch ? DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption : DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption;

            var defaults = new MirrorBackFrameSupportDimensionOptions()
            {
                BodyDimensionOptions = MirrorDrawOptions.GetDefaultDimensionOptions<RectangleRingDimensionsPresentationOptions>(isSketch),
                EdgeDistanceAnchorPoint = RectangleEdgeDistanceDimensionAnchorPoint.LeftMiddle,
                EdgeDistanceLineOptions = DimensionLineOptions.DefaultTwoArrowLineOptions(),
                EdgeDistancePresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch),
            };
            switch (dimensionDrawingOption)
            {
                case DimensionDrawingOption.DoNotAllowDimensionDraws:
                case DimensionDrawingOption.AllowOnlyMainDrawBasicDimensionsDraws:
                case DimensionDrawingOption.AllowOnlyShapeDimensions:
                    defaults.BodyDimensionOptions.ShowDimensions = false;
                    defaults.ShowEdgeDistance = false;
                    break;
                case DimensionDrawingOption.AllowAllDimensionDraws:
                case DimensionDrawingOption.AllowOnlyDistancesDimensions:
                    defaults.ShowEdgeDistance = true;
                    break;
                default:
                    throw new EnumValueNotSupportedException(dimensionDrawingOption);
            }
            return defaults;
        }

        public MirrorBackFrameSupportDimensionOptions GetDeepClone()
        {
            var clone = (MirrorBackFrameSupportDimensionOptions)this.MemberwiseClone();
            clone.BodyDimensionOptions = this.BodyDimensionOptions.GetDeepClone();
            clone.EdgeDistanceLineOptions = this.EdgeDistanceLineOptions.GetDeepClone();
            clone.EdgeDistancePresentationOptions = this.EdgeDistancePresentationOptions.GetDeepClone();
            return clone;
        }
    }
}

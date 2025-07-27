using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using DrawingLibrary.Models.PresentationOptions;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;

namespace MirrorsLib.DrawingElements.SandblastDrawingOptions
{
    public class CircularSandblastInfoDimensionOptions : IDeepClonable<CircularSandblastInfoDimensionOptions>
    {
        public CircleRingDimensionsPresentationOptions SandblastBodyDimensionOptions { get; set; } = new();
        public bool ShowDistanceFromEdge { get; set; }
        public CircularSandblastEdgeDimensionPosition EdgeDistanceDimensionPosition { get; set; }
        public DimensionLineOptions DistanceFromEdgeLineOptions { get; set; } = new();
        public DrawingPresentationOptions DistanceFromEdgePresentationOptions { get; set; } = new();
        public static CircularSandblastInfoDimensionOptions Defaults(bool isSketch)
        {
            var bodyDimOptions = MirrorDrawOptions.GetDefaultDimensionOptions<CircleRingDimensionsPresentationOptions>(isSketch);
            bodyDimOptions.ThicknessPosition = RectangleRingThicknessDimensionPosition.BottomMiddle;
            var defaults = new CircularSandblastInfoDimensionOptions()
            {
                SandblastBodyDimensionOptions = bodyDimOptions,
                ShowDistanceFromEdge = true,
                EdgeDistanceDimensionPosition = CircularSandblastEdgeDimensionPosition.Top,
                DistanceFromEdgeLineOptions = DimensionLineOptions.DefaultTwoArrowLineOptions(),
                DistanceFromEdgePresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferSmallerXAnchorline),
            };
            var dimensionDrawingOption = isSketch ? DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption : DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption;
            switch (dimensionDrawingOption)
            {
                case DimensionDrawingOption.DoNotAllowDimensionDraws:
                case DimensionDrawingOption.AllowOnlyMainDrawBasicDimensionsDraws:
                case DimensionDrawingOption.AllowOnlyShapeDimensions:
                    defaults.SandblastBodyDimensionOptions.ShowDimensions = false;
                    break;
                case DimensionDrawingOption.AllowAllDimensionDraws:
                case DimensionDrawingOption.AllowOnlyDistancesDimensions:
                    defaults.SandblastBodyDimensionOptions.ShowDimensions = true;
                    defaults.ShowDistanceFromEdge = true;
                    defaults.SandblastBodyDimensionOptions.ShowDiameter = true;
                    defaults.SandblastBodyDimensionOptions.ShowRadius = false;
                    break;
                default:
                    throw new EnumValueNotSupportedException(dimensionDrawingOption);
            }
            return defaults;
        }

        public CircularSandblastInfoDimensionOptions GetDeepClone()
        {
            var clone = (CircularSandblastInfoDimensionOptions)this.MemberwiseClone();
            clone.SandblastBodyDimensionOptions = this.SandblastBodyDimensionOptions.GetDeepClone();
            clone.DistanceFromEdgeLineOptions = this.DistanceFromEdgeLineOptions.GetDeepClone();
            clone.DistanceFromEdgePresentationOptions = this.DistanceFromEdgePresentationOptions.GetDeepClone();
            return clone;
        }
    }

}

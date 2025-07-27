using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using DrawingLibrary.Models.PresentationOptions;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;

namespace MirrorsLib.DrawingElements.SandblastDrawingOptions
{
    public class TwoLineSandblastInfoDimensionOptions : IDeepClonable<TwoLineSandblastInfoDimensionOptions>
    {
        /// <summary>
        /// The Dimension Presentation Options of the Sandblast body
        /// </summary>
        public RectangleDimensionsPresentationOptions SandblastBodyDimensionOptions { get; set; } = new();
        /// <summary>
        /// The Position of the Dimension measuring the Vertical Distance of the Sandblast from the Edge of the Mirror (ex. Distance from Top, Distance from Bottom)
        /// </summary>
        public LineSandblastVerticalDistancePosition VerticalDistanceFromEdgePosition { get; set; } = LineSandblastVerticalDistancePosition.Middle;
        /// <summary>
        /// The Position of the Dimension measuring the Horizontal Distance of the Sandblast from the Edge of the Mirror (ex. Distance from Left, Distance from Right)
        /// </summary>
        public LineSandblastHorizontalDistancePosition HorizontalDistanceFromEdgePosition { get; set; } = LineSandblastHorizontalDistancePosition.Middle;
        public bool AtLeastOneEdgeDitanceVisible { get => ShowHorizontalDistance || ShowVerticalDistance; }
        public bool ShowHorizontalDistance { get; set; }
        public bool ShowVerticalDistance { get; set; }
        /// <summary>
        /// The Line Options of the Various Dimensions for the Distance from the mirror's edge
        /// </summary>
        public DimensionLineOptions DistanceFromEdgeLineOptions { get; set; } = new();
        /// <summary>
        /// The Presentation Options of all the Distance Lines
        /// </summary>
        public DrawingPresentationOptions DistanceFromEdgePresentationOptions { get; set; } = new();
        public TwoLineSandblastInfoDimensionOptions GetDeepClone()
        {
            var clone = (TwoLineSandblastInfoDimensionOptions)this.MemberwiseClone();
            clone.SandblastBodyDimensionOptions = this.SandblastBodyDimensionOptions.GetDeepClone();
            clone.DistanceFromEdgeLineOptions = this.DistanceFromEdgeLineOptions.GetDeepClone();
            clone.DistanceFromEdgePresentationOptions = this.DistanceFromEdgePresentationOptions.GetDeepClone();
            return clone;
        }
        public static TwoLineSandblastInfoDimensionOptions Defaults(bool isSketch)
        {
            var bodyOptions = MirrorDrawOptions.GetDefaultDimensionOptions<RectangleDimensionsPresentationOptions>(isSketch);
            bodyOptions.LengthPosition = RectangleLengthDimensionPosition.Top;
            bodyOptions.LengthPresentationOptions = bodyOptions.LengthPresentationOptions.WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferSmallerYAnchorline);

            var defaults = new TwoLineSandblastInfoDimensionOptions()
            {
                SandblastBodyDimensionOptions = bodyOptions,
                VerticalDistanceFromEdgePosition = LineSandblastVerticalDistancePosition.Middle,
                HorizontalDistanceFromEdgePosition = LineSandblastHorizontalDistancePosition.Middle,
                DistanceFromEdgeLineOptions = DimensionLineOptions.DefaultTwoArrowLineOptions(),
                DistanceFromEdgePresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferSmallerXSmallerYAnchorline),
            };
            var dimensionDrawingOption = isSketch ? DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption : DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption;
            switch (dimensionDrawingOption)
            {
                case DimensionDrawingOption.DoNotAllowDimensionDraws:
                case DimensionDrawingOption.AllowOnlyMainDrawBasicDimensionsDraws:
                case DimensionDrawingOption.AllowOnlyShapeDimensions:
                    defaults.ShowHorizontalDistance = false;
                    defaults.ShowVerticalDistance = false;
                    break;
                case DimensionDrawingOption.AllowAllDimensionDraws:
                case DimensionDrawingOption.AllowOnlyDistancesDimensions:
                    defaults.ShowHorizontalDistance = true;
                    defaults.ShowVerticalDistance = true;
                    break;
                default:
                    throw new EnumValueNotSupportedException(dimensionDrawingOption);
            }
            return defaults;
        }
    }

}

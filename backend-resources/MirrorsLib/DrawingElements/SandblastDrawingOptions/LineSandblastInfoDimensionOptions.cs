using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using DrawingLibrary.Models.PresentationOptions;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;

namespace MirrorsLib.DrawingElements.SandblastDrawingOptions
{
    public class LineSandblastInfoDimensionOptions : IDeepClonable<LineSandblastInfoDimensionOptions>
    {
        
        /// <summary>
        /// The Dimension Presentation Options of the Sandblast body
        /// </summary>
        public RectangleDimensionsPresentationOptions SandblastBodyDimensionOptions { get; set; } = new();
        /// <summary>
        /// The Position of the Dimension measuring the Vertical Distance of the Sandblast from the Edge of the Mirror (ex. Distance from Top, Distance from Bottom)
        /// </summary>
        public LineSandblastVerticalDistancePosition VerticalDistanceFromEdgePosition { get; set; }
        /// <summary>
        /// The Position of the Dimension measuring the Horizontal Distance of the Sandblast from the Edge of the Mirror (ex. Distance from Left, Distance from Right)
        /// </summary>
        public LineSandblastHorizontalDistancePosition HorizontalDistanceFromEdgePosition { get; set; }

        public bool AtLeastOneDistanceFromEdgeVisible { get => ShowDistanceFromTop || ShowDistanceFromRight || ShowDistanceFromLeft || ShowDistanceFromBottom; }

        /// <summary>
        /// Weather to Show the Distance of the Sandblast from the Top Edge of the Mirror
        /// </summary>
        public bool ShowDistanceFromTop { get; set; }
        /// <summary>
        /// Weather to Show the Distance of the Sandblast from the Bottom Edge of the Mirror
        /// </summary>
        public bool ShowDistanceFromBottom { get; set; }
        /// <summary>
        /// Weather to Show the Distance of the Sandblast from the Left Edge of the Mirror
        /// </summary>
        public bool ShowDistanceFromLeft { get; set; }
        /// <summary>
        /// Weather to Show the Distance of the Sandblast from the Right Edge of the Mirror
        /// </summary>
        public bool ShowDistanceFromRight { get; set; }
        /// <summary>
        /// The Line Options of the Various Dimensions for the Distance from the mirror's edge
        /// </summary>
        public DimensionLineOptions DistanceFromEdgeLineOptions { get; set; } = new();
        /// <summary>
        /// The Presentation Options of all the Distance Lines
        /// </summary>
        public DrawingPresentationOptions DistanceFromEdgePresentationOptions { get; set; } = new();

        public static LineSandblastInfoDimensionOptions Defaults(bool isSketch)
        {
            var dimensionDrawingOption = isSketch ? DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption : DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption;

            var defaults = new LineSandblastInfoDimensionOptions()
            {
                SandblastBodyDimensionOptions = MirrorDrawOptions.GetDefaultDimensionOptions<RectangleDimensionsPresentationOptions>(isSketch),
                VerticalDistanceFromEdgePosition = LineSandblastVerticalDistancePosition.Middle,
                HorizontalDistanceFromEdgePosition = LineSandblastHorizontalDistancePosition.Middle,
                DistanceFromEdgeLineOptions = DimensionLineOptions.DefaultTwoArrowLineOptions(),
                DistanceFromEdgePresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferSmallerXSmallerYAnchorline),
            };
            switch (dimensionDrawingOption)
            {
                case DimensionDrawingOption.DoNotAllowDimensionDraws:
                case DimensionDrawingOption.AllowOnlyMainDrawBasicDimensionsDraws:
                case DimensionDrawingOption.AllowOnlyShapeDimensions:
                    defaults.ShowDistanceFromTop = false;
                    defaults.ShowDistanceFromBottom = false;
                    defaults.ShowDistanceFromLeft = false;
                    defaults.ShowDistanceFromRight = false;
                    break;
                case DimensionDrawingOption.AllowAllDimensionDraws:
                case DimensionDrawingOption.AllowOnlyDistancesDimensions:
                    defaults.ShowDistanceFromTop = true;
                    defaults.ShowDistanceFromBottom = false;
                    defaults.ShowDistanceFromLeft = true;
                    defaults.ShowDistanceFromRight = false;
                    break;
                default:
                    throw new EnumValueNotSupportedException(dimensionDrawingOption);
            }
            return defaults;
        }

        public LineSandblastInfoDimensionOptions GetDeepClone()
        {
            var clone = (LineSandblastInfoDimensionOptions)this.MemberwiseClone();
            clone.SandblastBodyDimensionOptions = this.SandblastBodyDimensionOptions.GetDeepClone();
            clone.DistanceFromEdgeLineOptions = this.DistanceFromEdgeLineOptions.GetDeepClone();
            clone.DistanceFromEdgePresentationOptions = this.DistanceFromEdgePresentationOptions.GetDeepClone();
            return clone;
        }
    }

}

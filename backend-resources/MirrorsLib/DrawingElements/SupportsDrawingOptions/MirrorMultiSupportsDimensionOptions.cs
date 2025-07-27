using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using DrawingLibrary.Models.PresentationOptions;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;
using MirrorsLib.DrawingElements.SandblastDrawingOptions;
using MirrorsLib.MirrorElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsLib.DrawingElements.SupportsDrawingOptions
{
    public class MirrorMultiSupportsDimensionOptions : IDeepClonable<MirrorMultiSupportsDimensionOptions>
    {
        public RectangleDimensionsPresentationOptions SupportBodyDimensionOptions { get; set; } = new();
        /// <summary>
        /// The Point from which the dimension line starts to measure the horizontal distance
        /// </summary>
        public EdgeLineVerticalDistanceDimensionAnchor VerticalDistanceFromParentAnchor { get; set; }
        /// <summary>
        /// The Point from which the dimension line starts to measure the vertical distance
        /// </summary>
        public EdgeLineHorizontalDistanceDimensionAnchor HorizontalDistanceFromParentAnchor { get; set; }

        public bool ShowDistancesFromParent { get; set; }

        public DimensionLineOptions DistanceFromParentLineOptions { get; set; } = new();
        public DrawingPresentationOptions DistanceFromParentPresentationOptions { get; set; } = new();

        public static MirrorMultiSupportsDimensionOptions Defaults(bool isSketch)
        {
            var dimensionDrawingOption = isSketch ? DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption : DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption;

            var defaults = new MirrorMultiSupportsDimensionOptions()
            {
                SupportBodyDimensionOptions = MirrorDrawOptions.GetDefaultDimensionOptions<RectangleDimensionsPresentationOptions>(isSketch),
                VerticalDistanceFromParentAnchor = EdgeLineVerticalDistanceDimensionAnchor.Middle,
                HorizontalDistanceFromParentAnchor = EdgeLineHorizontalDistanceDimensionAnchor.Middle,
                DistanceFromParentLineOptions = DimensionLineOptions.DefaultTwoArrowLineOptions(),
                DistanceFromParentPresentationOptions = DrawingPresentationOptions.DefaultDimensionOptions(isSketch).WithTextAnchorLineOption(AnchorLinePreferenceOption.PreferSmallerXSmallerYAnchorline),
            };
            switch (dimensionDrawingOption)
            {
                case DimensionDrawingOption.DoNotAllowDimensionDraws:
                case DimensionDrawingOption.AllowOnlyMainDrawBasicDimensionsDraws:
                case DimensionDrawingOption.AllowOnlyShapeDimensions:
                    defaults.ShowDistancesFromParent = false;
                    break;
                case DimensionDrawingOption.AllowAllDimensionDraws:
                case DimensionDrawingOption.AllowOnlyDistancesDimensions:
                    defaults.ShowDistancesFromParent = true;
                    break;
                default:
                    throw new EnumValueNotSupportedException(dimensionDrawingOption);
            }
            return defaults;
        }

        public MirrorMultiSupportsDimensionOptions GetDeepClone()
        {
            var clone = (MirrorMultiSupportsDimensionOptions)this.MemberwiseClone();
            clone.SupportBodyDimensionOptions = this.SupportBodyDimensionOptions.GetDeepClone();
            clone.DistanceFromParentLineOptions = this.DistanceFromParentLineOptions.GetDeepClone();
            clone.DistanceFromParentPresentationOptions = this.DistanceFromParentPresentationOptions.GetDeepClone();
            return clone;
        }
    }
}

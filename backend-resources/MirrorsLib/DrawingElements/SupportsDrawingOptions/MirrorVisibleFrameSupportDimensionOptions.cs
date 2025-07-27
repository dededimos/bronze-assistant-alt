using CommonHelpers.Exceptions;
using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using DrawingLibrary.Models.PresentationOptions;
using DrawingLibrary.Models.PresentationOptions.ShapeSpecific;

namespace MirrorsLib.DrawingElements.SupportsDrawingOptions
{
    public class MirrorVisibleFrameSupportDimensionOptions : IDeepClonable<MirrorVisibleFrameSupportDimensionOptions>
    {
        /// <summary>
        /// The Dimension Options for the Main Body of the Frame
        /// </summary>
        public ShapeDimensionsPresentationOptions BodyDimensionOptions { get; set; } = new();
        /// <summary>
        /// The Dimension Options for the extra Body of the Frame if there is one
        /// </summary>
        public ShapeDimensionsPresentationOptions ExtraBodyDimensionOptions { get; set; } = new();
        /// <summary>
        /// The Point from which the dimension line starts to measure the horizontal distance
        /// </summary>
        
        public static MirrorVisibleFrameSupportDimensionOptions DefaultsRectangleRingFrame(bool isSketch)
        {
            var dimensionDrawingOption = isSketch ? DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption : DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption;
            var bodyOptions = MirrorDrawOptions.GetDefaultDimensionOptions<RectangleRingDimensionsPresentationOptions>(isSketch, true);
            var extraBodyOptions = MirrorDrawOptions.GetDefaultDimensionOptions<RectangleRingDimensionsPresentationOptions>(isSketch);
            extraBodyOptions.ShowDimensions = false;

            switch (dimensionDrawingOption)
            {
                case DimensionDrawingOption.DoNotAllowDimensionDraws:
                case DimensionDrawingOption.AllowOnlyMainDrawBasicDimensionsDraws:
                case DimensionDrawingOption.AllowOnlyShapeDimensions:
                    bodyOptions.ShowThickness = false;
                    break;
                case DimensionDrawingOption.AllowAllDimensionDraws:
                case DimensionDrawingOption.AllowOnlyDistancesDimensions:
                    bodyOptions.ShowThickness = true;
                    break;
                default:
                    throw new EnumValueNotSupportedException(dimensionDrawingOption);
            }

            MirrorVisibleFrameSupportDimensionOptions defaults = new()
            {
                BodyDimensionOptions =bodyOptions,
                ExtraBodyDimensionOptions = extraBodyOptions,
            };

            return defaults;
        }
        public static MirrorVisibleFrameSupportDimensionOptions DefaultsCircleRingFrame(bool isSketch)
        {
            var dimensionDrawingOption = isSketch ? DrawingPresentationOptionsGlobal.SketchDrawDimensionDrawingOption : DrawingPresentationOptionsGlobal.NormalDrawDimensionDrawingOption;
            var bodyOptions = MirrorDrawOptions.GetDefaultDimensionOptions<CircleRingDimensionsPresentationOptions>(isSketch, true);
            var extraBodyOptions = MirrorDrawOptions.GetDefaultDimensionOptions<CircleRingDimensionsPresentationOptions>(isSketch);
            extraBodyOptions.ShowDimensions = false;

            switch (dimensionDrawingOption)
            {
                case DimensionDrawingOption.DoNotAllowDimensionDraws:
                case DimensionDrawingOption.AllowOnlyMainDrawBasicDimensionsDraws:
                case DimensionDrawingOption.AllowOnlyShapeDimensions:
                    bodyOptions.ShowThickness = false;
                    break;
                case DimensionDrawingOption.AllowAllDimensionDraws:
                case DimensionDrawingOption.AllowOnlyDistancesDimensions:
                    bodyOptions.ShowThickness = true;
                    break;
                default:
                    throw new EnumValueNotSupportedException(dimensionDrawingOption);
            }

            MirrorVisibleFrameSupportDimensionOptions defaults = new()
            {
                BodyDimensionOptions = bodyOptions,
                ExtraBodyDimensionOptions = extraBodyOptions,
            };

            return defaults;
        }

        public MirrorVisibleFrameSupportDimensionOptions GetDeepClone()
        {
            var clone = (MirrorVisibleFrameSupportDimensionOptions)this.MemberwiseClone();
            clone.BodyDimensionOptions = BodyDimensionOptions.GetDeepClone();
            clone.ExtraBodyDimensionOptions = ExtraBodyDimensionOptions.GetDeepClone();
            return clone;
        }
    }
}

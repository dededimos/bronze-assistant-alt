using CommonInterfacesBronze;
using DrawingLibrary.Enums;

namespace DrawingLibrary.Models.PresentationOptions
{
    public class DimensionLineOptions : IDeepClonable<DimensionLineOptions>
    {
        /// <summary>
        /// The Thickness of the Arrows in the dimension Line
        /// </summary>
        public double ArrowThickness { get; set; } = 5;
        /// <summary>
        /// The Length of the Arrows in the dimension Line
        /// </summary>
        public double ArrowLength { get; set; } = 10;
        /// <summary>
        /// Weather to Include an arrow at the Start
        /// </summary>
        public bool IncludeStartArrow { get; set; } = true;
        /// <summary>
        /// Weather to Include an arrow at the End
        /// </summary>
        public bool IncludeEndArrow { get; set; } = true;
        /// <summary>
        /// The unit text to add at the end of the dimension Value
        /// </summary>
        public string DimensionUnitString { get; set; } = " mm";
        /// <summary>
        /// The Prefix in a Dimensions Value , for Example "Φ" for diameters
        /// </summary>
        public string DimensionTextPrefix { get; set; } = string.Empty;

        /// <summary>
        /// The Distance of the Text from the Dimension
        /// </summary>
        public double TextMarginFromDimension { get; set; } = 10;
        /// <summary>
        /// The Length of the Dimension Line when it points somewhere rather than showing distance between two points (as in Radius , Diameter e.t.c.)
        /// </summary>
        public double OneEndLineLength { get; set; } = 30;
        /// <summary>
        /// The Angle with X Axis for a OneEndLine . Transformation Applies after setting Start/End Points and rotates line around Start
        /// </summary>
        public double StartRotationAngle { get; set; } = 0;
        public int DimensionValueRoundingDecimals { get; set; } = 0;
        /// <summary>
        /// Weather the Dimensions is Two lines that show the measurement of a small thickness
        /// </summary>
        public bool IsTwoLinesDimension { get; set; }
        /// <summary>
        /// The Threshold multiplier for the Length of the Dimension Line to be considered a Two Lines Dimension
        /// <para>ex. 4 means the dimension will become a two lined dimension if the Length is less than 4 Times the Arrows Length</para>
        /// </summary>
        public double TwoLinesDimensionArrowLengthThresholdMultiplier { get; set; } = 4;
        /// <summary>
        /// If false the Text will be placed after the End Arrow Line when the Dimension is seperated in two lines
        /// </summary>
        public bool CenterTextOnTwoLineDimension { get; set; } = true;

        public DimensionLineOptions GetDeepClone()
        {
            return (DimensionLineOptions)MemberwiseClone();
        }

        public DimensionLineOptions WithTextMarginFromDimension(double margin)
        {
            TextMarginFromDimension = margin;
            return this;
        }

        public DimensionLineOptions WithStartRotationAngle(double angleRadians)
        {
            StartRotationAngle = angleRadians;
            return this;
        }
        public DimensionLineOptions WithDimensionTextPrefix(string prefix)
        {
            DimensionTextPrefix = prefix;
            return this;
        }
        public DimensionLineOptions WithNonCenteredTextOnTwoLineDimension()
        {
            CenterTextOnTwoLineDimension = false;
            return this;
        }

        /// <summary>
        /// Creates a TwoArrow Dimension Line Options with the Default Values
        /// </summary>
        /// <param name="inverseTextMargin">Weather to calculate the Text Margin as Negative to Move the Dimension to the Negative X,Y dpending on Orientation</param>
        /// <returns></returns>
        public static DimensionLineOptions DefaultTwoArrowLineOptions()
        {
            return new()
            {
                ArrowThickness = DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.ArrowThickness,
                ArrowLength = DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.ArrowLength,
                IncludeStartArrow = true,
                IncludeEndArrow = true,
                DimensionUnitString = DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.MILIMETERSUFFIX,
                DimensionTextPrefix = string.Empty,
                TextMarginFromDimension = DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.TextMarginFromDimensionLine,
                DimensionValueRoundingDecimals = DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.DimensionValueRoundingDecimals,
                OneEndLineLength = DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.OneEndLineLength,
                TwoLinesDimensionArrowLengthThresholdMultiplier = DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.TwoLinesDimensionArrowLengthThresholdMultiplier,
            };
        }
        public static DimensionLineOptions DefaultDiameterLineOptions() => SingleArrowLineOptions(DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.DIAMETERPREFIX);
        public static DimensionLineOptions DefaultRadiusLineOptions() => SingleArrowLineOptions(DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.RADIUSPREFIX);
        /// <summary>
        /// Creates a Single Arrow Dimension Line Options with the Default Values
        /// </summary>
        /// <param name="textPrefix"></param>
        /// <param name="inverseTextMargin">Weather to calculate the Text Margin as Negative to Move the Dimension to the Negative X,Y dpending on Orientation</param>
        /// <returns></returns>
        public static DimensionLineOptions SingleArrowLineOptions(string textPrefix = "")
        {
            return new()
            {
                ArrowThickness = DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.ArrowThickness,
                ArrowLength = DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.ArrowLength,
                IncludeStartArrow = true,
                IncludeEndArrow = false,
                DimensionUnitString = " mm",
                DimensionTextPrefix = textPrefix,
                TextMarginFromDimension = DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.TextMarginFromDimensionLine,
                OneEndLineLength = DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.OneEndLineLength,
                DimensionValueRoundingDecimals = DrawingPresentationOptionsGlobal.DimensionLineOptionsGlobal.DimensionValueRoundingDecimals,
            };
        }
    }
}

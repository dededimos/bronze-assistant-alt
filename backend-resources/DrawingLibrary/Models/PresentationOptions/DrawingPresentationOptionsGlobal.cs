
using DrawingLibrary.Enums;

namespace DrawingLibrary.Models.PresentationOptions
{
    public static class DrawingPresentationOptionsGlobal
    {
        private static bool isDarkTheme;
        /// <summary>
        /// Weather a Dark Theme is being Used
        /// </summary>
        public static bool IsDarkTheme
        {
            get
            {
                if (isAlwaysDarkTheme) return true;
                if (isAlwaysLightTheme) return false;
                return isDarkTheme;
            }
            set => isDarkTheme = value;
        }
        private static bool isAlwaysDarkTheme = false;
        private static bool isAlwaysLightTheme = false;
        /// <summary>
        /// Overrides and returns Global Options as in the Light Theme
        /// </summary>
        public static void OverrideThemeToAlwaysLight()
        {
            isAlwaysLightTheme = true;
            isAlwaysDarkTheme = false;
        }
        /// <summary>
        /// Overrides and returns Global Options as in the Dark Theme
        /// </summary>
        public static void OverrideThemeToAlwaysDark()
        {
            isAlwaysDarkTheme = true;
            isAlwaysLightTheme = false;
        }
        /// <summary>
        /// Stops all overrides and returns global options to the theme set by the user
        /// </summary>
        public static void StopThemeOverrides()
        {
            isAlwaysDarkTheme = false;
            isAlwaysLightTheme = false;
        }

        /// <summary>
        /// The Global Height of Text
        /// </summary>
        public static double TextHeight { get; set; } = 12;

        public static DimensionDrawingOption NormalDrawDimensionDrawingOption { get; set; } = DimensionDrawingOption.AllowOnlyMainDrawBasicDimensionsDraws;
        public static DimensionDrawingOption SketchDrawDimensionDrawingOption { get; set; } = DimensionDrawingOption.AllowAllDimensionDraws;

        public static DrawBrush StrokeSketchDark { get; set; } = DrawBrushes.White;
        public static DrawBrush StrokeSketchLight { get; set; } = DrawBrushes.Black;
        public static DrawBrush StrokeSketch { get => IsDarkTheme ? StrokeSketchDark : StrokeSketchLight; }
        public static double StrokeThicknessSketch { get; set; } = 1;
        public static List<double> StrokeDashArraySketch { get; set; } = [];

        public static class DimensionLineOptionsGlobal
        {
            public const string MILIMETERSUFFIX = " mm";
            public const string DIAMETERPREFIX = "Φ";
            public const string RADIUSPREFIX = "R";
            public const string ELLIPSERADIUSPREFIX = "Re";
            public const string CIRCLERADIUSPREFIX = "Rc";
            public const string CIRCLEDIAMETERPREFIX = "Φc";
            public const string CHORDLENGTHPREFIX = "Cut";

            public static double ArrowThickness { get; set; } = 5;
            public static double ArrowLength { get; set; } = 10;
            /// <summary>
            /// The Distance of the Text from the Dimension
            /// </summary>
            public static double TextMarginFromDimensionLine { get; set; } = 5;
            /// <summary>
            /// The Length of the Dimension Line when it points somewhere rather than showing distance between two points (as in Radius , Diameter e.t.c.)
            /// </summary>
            public static double OneEndLineLength { get; set; } = 15;
            /// <summary>
            /// The Number of DecimalPoints in Dimension Value
            /// </summary>
            public static int DimensionValueRoundingDecimals { get; set; } = 0;
            /// <summary>
            /// The Threshold multiplier for the Length of the Dimension Line to be considered a Two Lines Dimension
            /// <para>ex. 4 means the dimension will become a two lines dimension if the Length is less than 4 Times the Arrows Length</para>
            /// </summary>
            public static double TwoLinesDimensionArrowLengthThresholdMultiplier { get; set; } = 4;
        }
        public static class DimensionPresentationOptionsGlobal
        {
            private static double textHeight = double.NaN;
            public static bool IsTextHeightEqualToGlobal { get => double.IsNaN(textHeight); }
            public static double TextHeight
            {
                get => double.IsNaN(textHeight) ? DrawingPresentationOptionsGlobal.TextHeight : textHeight;
                set
                {
                    textHeight = value;
                }
            }

            public static DrawBrush DarkFill { get; set; } = DrawBrushes.White;
            public static DrawBrush LightFill { get; set; } = DrawBrushes.Black;
            public static DrawBrush Fill { get => IsDarkTheme ? DarkFill : LightFill; }

            public static DrawBrush DarkStroke { get; set; } = DrawBrushes.White;
            public static DrawBrush LightStroke { get; set; } = DrawBrushes.Black;
            public static DrawBrush Stroke { get => IsDarkTheme ? DarkStroke : LightStroke; }

            public static double StrokeThickness { get; set; } = 1;
            public static List<double> StrokeDashArray { get; set; } = [4, 4];
            public static List<double> StrokeDashArrayHelpLines { get; set; } = [8, 12, 2, 12];

            private static double opacity = 1;
            public static double Opacity
            {
                get => opacity;
                set => opacity = Math.Max(0, Math.Min(1, value));
            }
            public static bool UseShadow { get; set; } = false;

            public static double DimensionMarginFromShape { get; set; } = 20;
        }
    }
}

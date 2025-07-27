using CommonInterfacesBronze;
using DrawingLibrary.Enums;
using DrawingLibrary.Interfaces;
using DrawingLibrary.Models.ConcreteGraphics;
using System;
using System.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DrawingLibrary.Models.PresentationOptions
{
    public class DrawingPresentationOptions : IDeepClonable<DrawingPresentationOptions>
    {
        public DrawBrush Fill { get; set; } = new();
        public DrawBrush Stroke { get; set; } = new();
        /// <summary>
        /// The Pattern Type to use as fill
        /// </summary>
        public FillPatternType FillPattern { get; set; } = FillPatternType.NoPattern;
        public double StrokeThickness { get; set; } = 1;
        public List<double> StrokeDashArray { get; set; } = [];
        public double Opacity { get; set; } = 1;
        /// <summary>
        /// The Desired pixels height for the Drawing's Text
        /// </summary>
        public double TextHeight { get; set; } = 12;
        public bool UseShadow { get; set; } = false;

        /// <summary>
        /// Determines which anchorline to choose from between two parallel anchor lines , when calculating the anchorLine for the Text
        /// </summary>
        public AnchorLinePreferenceOption TextAnchorLineOption { get; set; } = AnchorLinePreferenceOption.PreferGreaterXGreaterYAnchorline;

        public DrawingPresentationOptions WithFill(DrawBrush fill)
        {
            Fill = fill;
            return this;
        }
        public DrawingPresentationOptions WithStroke(DrawBrush stroke)
        {
            Stroke = stroke;
            return this;
        }
        public DrawingPresentationOptions WithTextAnchorLineOption(AnchorLinePreferenceOption option)
        {
            TextAnchorLineOption = option;
            return this;
        }
        public DrawingPresentationOptions WithFillPattern(FillPatternType fillPattern)
        {
            FillPattern = fillPattern;
            return this;
        }
        public DrawingPresentationOptions WithStrokeDashArray(List<double> dashArray)
        {
            this.StrokeDashArray = dashArray;
            return this;
        }

        public virtual DrawingPresentationOptions GetDeepClone()
        {
            var clone = (DrawingPresentationOptions)MemberwiseClone();
            clone.StrokeDashArray = new(StrokeDashArray);
            clone.Fill = Fill.GetDeepClone();
            clone.Stroke = Stroke.GetDeepClone();
            return clone;
        }

        /// <summary>
        /// Presentation Options for a sketch without Fill
        /// </summary>
        /// <returns></returns>
        public static DrawingPresentationOptions DefaultSketchOptions()
        {
            return new()
            {
                TextHeight = DrawingPresentationOptionsGlobal.TextHeight,
                Fill = DrawBrush.Empty,
                Opacity = 1,
                Stroke = DrawingPresentationOptionsGlobal.StrokeSketch,
                StrokeThickness = DrawingPresentationOptionsGlobal.StrokeThicknessSketch,
                StrokeDashArray = DrawingPresentationOptionsGlobal.StrokeDashArraySketch,
                UseShadow = false
            };
        }
        
        /// <summary>
        /// Presentation Options for a Dimension
        /// </summary>
        /// <returns></returns>
        public static DrawingPresentationOptions DefaultDimensionOptions()
        {
            return new()
            {
                Fill = DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.Fill,
                Opacity = DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.Opacity,
                Stroke = DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.Stroke,
                StrokeThickness = DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeThickness,
                StrokeDashArray = DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeDashArray,
                UseShadow = DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.UseShadow,
                TextHeight = DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.TextHeight
            };
        }
        /// <summary>
        /// Presentation Options for a Dimension on a sketch Draw
        /// </summary>
        /// <returns></returns>
        public static DrawingPresentationOptions DefaultSketchDimensionOptions()
        {
            return new()
            {
                TextHeight = DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.TextHeight,
                Fill = DrawingPresentationOptionsGlobal.StrokeSketch,
                Opacity = DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.Opacity,
                Stroke = DrawingPresentationOptionsGlobal.StrokeSketch,
                StrokeThickness = DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeThickness,
                StrokeDashArray = DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeDashArray,
                UseShadow = false
            };
        }
        /// <summary>
        /// Returns the Default Presentation Options for a Dimension Line
        /// </summary>
        /// <param name="isSketch"></param>
        /// <returns></returns>
        public static DrawingPresentationOptions DefaultDimensionOptions(bool isSketch)
        {
            return isSketch ? DefaultSketchDimensionOptions() : DefaultDimensionOptions();
        }

        /// <summary>
        /// Returns the Default Presentation Options for a Help Line
        /// </summary>
        /// <param name="isSketch"></param>
        /// <returns></returns>
        public static DrawingPresentationOptions DefaultHelpLineOptions(bool isSketch)
        {
            return isSketch ? DefaultSketchHelpLineOptions() : DefaultHelpLineOptions();
        }
        public static DrawingPresentationOptions DefaultHelpLineOptions()
        {
            return DefaultDimensionOptions().WithStrokeDashArray(DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeDashArrayHelpLines);
        }
        public static DrawingPresentationOptions DefaultSketchHelpLineOptions()
        {
            return DefaultSketchDimensionOptions().WithStrokeDashArray(DrawingPresentationOptionsGlobal.DimensionPresentationOptionsGlobal.StrokeDashArrayHelpLines);
        }
    }
}

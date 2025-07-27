using CommonHelpers.Comparers;
using CommonHelpers.Exceptions;
using DocumentFormat.OpenXml.Wordprocessing;
using DrawingLibrary.Enums;
using DrawingLibrary.Interfaces;
using ShapesLibrary.Services;
using ShapesLibrary.ShapeInfoModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace BronzeFactoryApplication.Helpers.Converters
{
    public class DrawingToGeometryTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IDrawing drawing) return GetDrawingTextGeometry(drawing.TextAnchorLine,drawing.DrawingText,drawing.PresentationOptions.TextAnchorLineOption,drawing.PresentationOptions.TextHeight);
            else return Geometry.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{this.GetType().Name} does not Support two-Way Binding");
        }

        /// <summary>
        /// Transforms the <see cref="IDrawing.DrawingText"/> into a <see cref="Geometry"/> object
        /// the returned geometry is anchored acoording to <see cref="IDrawing.TextAnchorLine"/> of the Drawing
        /// </summary>
        /// <param name="drawing">The Drawing from which to extract the Drawing text</param>
        /// <returns>The Geometry of the Text</returns>
        public static Geometry GetDrawingTextGeometry(LineInfo? textAnchorLine,string? text,AnchorLinePreferenceOption anchorOption,double textHeight = 50,bool centerTextToAnchorLine = true)
        {
            if (textAnchorLine is null || string.IsNullOrEmpty(text) || textHeight <= 0) return Geometry.Empty;

            //Create the Formatted Text , insert an em value that defines APPROXIMATELY THE TEXT HEIGHT 
            FormattedText formattedText = new(
                    text,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("TimesNewRoman"),
                    textHeight, Brushes.Red, 96)
            {
                TextAlignment = System.Windows.TextAlignment.Left
            };
            
            PointXY anchorLineMidPoint = textAnchorLine.GetLineMidPoint();

            //BuildGeometry always places the Y of the point at the Upper baseLine of the Text
            //Meaning the Text is placed below the Point. So when it rotates it always goes to the bottom of the Point or to the Left of the Point
            //So we need to offset by its height whenever our text needs to be in the opposite direction
            //ONLY Text with GreaterY Or SmallerX does not need offset all the Rest need the text to be placed atopTheAnchorLine (always placed horizontaly and then rotated)
            double textYOffset = 0;
            switch (anchorOption)
            {
                case AnchorLinePreferenceOption.PreferGreaterXAnchorline: //Dimension Line should be on the Left
                case AnchorLinePreferenceOption.PreferSmallerYAnchorline: //Dimension Line should be on Bottom of Text
                case AnchorLinePreferenceOption.PreferGreaterXSmallerYAnchorline: //Dimension Line should be on Left OR on Bottom of Text
                    textYOffset = -formattedText.Height;
                    break;
                case AnchorLinePreferenceOption.PreferSmallerXAnchorline: //Dimension Line should be on Right of Text
                case AnchorLinePreferenceOption.PreferGreaterYAnchorline: //Dimension Line should be on Top
                case AnchorLinePreferenceOption.PreferSmallerXGreaterYAnchorline: //Dimension Line should be on Right OR on Top of Text
                    textYOffset = 0;
                    break;
                    //In the below cases if the Anchor is Horizontal then discard the X and use the Y value for the Option
                case AnchorLinePreferenceOption.PreferSmallerXSmallerYAnchorline: //Dimension Line should be on Right OR on Bottom of Text
                    textYOffset = textAnchorLine.IsHorizontal ? -formattedText.Height : 0;
                    break;
                case AnchorLinePreferenceOption.PreferGreaterXGreaterYAnchorline: //Dimension Line should be on Left OR on Top of Text
                    textYOffset = textAnchorLine.IsHorizontal ? 0 : -formattedText.Height;
                    break;
                default:
                    throw new EnumValueNotSupportedException(anchorOption);
            }
            double adjustedPointX = 0;
            if (centerTextToAnchorLine)
            {
                adjustedPointX = anchorLineMidPoint.X - formattedText.Width / 2;
            }
            else
            {
                adjustedPointX = textAnchorLine.StartX;
            }
            Point adjustedPoint = new(adjustedPointX, anchorLineMidPoint.Y + textYOffset);//

            //Create a Text Geometry from the Formated Text , The Center Point of the Text is the Center of the Line and the Y is the Center of the Line minus the FontSize to center it
            Geometry textGeometry = formattedText.BuildGeometry(adjustedPoint);
                        
            //Transform the text angle according to the anchor angle
            //The text is always with 0 degrees we have to rotate it anti-clockwise by the dimensions angle with the axis X to follow it
            var angleRad = textAnchorLine.GetNonDirectionalAngleWithXAxis();
            if (Math.Abs(angleRad) <= DoubleSafeEqualityComparer.DefaultEpsilon ) return textGeometry;

            RotateTransform rotate = new(MathCalculations.VariousMath.RadiansToDegrees(angleRad), anchorLineMidPoint.X, anchorLineMidPoint.Y);
            TransformGroup group = new();
            group.Children.Add(rotate);
            textGeometry.Transform = group;
            return textGeometry;
        }
    }
}

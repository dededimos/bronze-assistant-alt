using SVGDrawingLibrary.Models.ConcreteShapes;
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
    public class DimensionLineDrawToAnchoredTextCoverter : IValueConverter
    {
        /// <summary>
        /// Creates the TextGeometries On a Collection of Dimensions
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<DimensionLineDraw> dims)
            {
                GeometryCollection col = new();
                foreach (var dim in dims)
                {
                    //Get the Anchor of the Text from the Dimension
                    LineDraw anchorLine = dim.GetTextAnchorMiddleLine();

                    //Create the Formatted Text
                    FormattedText text = new(
                        dim.RepresentedDimensionText,
                        CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("TimesNewRoman"),
                        50, Brushes.Red, 10)
                    {
                        TextAlignment = TextAlignment.Center,
                    };
                    //Create a Text Geometry from the Formated Text , The Center Point of the Text is the Center of the Line nad the Y is the Center of the Line minus Half the FontSize to center it
                    Geometry textGeometry = text.BuildGeometry(new Point(anchorLine.BoundingBoxCenterX, anchorLine.BoundingBoxCenterY-50));
                    //If the Dimension is Vertical Tranform it accordingly
                    if (dim.AngleWithAxisX is 90 or 45)
                    {
                        //The text is always with 0 degrees we have to rotate it anti-clockwise by the dimensions angle with the axis X to follow it
                        textGeometry.Transform = new RotateTransform(-(double)dim.AngleWithAxisX, anchorLine.BoundingBoxCenterX, anchorLine.BoundingBoxCenterY);
                    }
                    
                    col.Add(textGeometry);
                }
                return col;
            }

            return new GeometryCollection();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(DimensionLineDrawToAnchoredTextCoverter)} does not Support two-Way Binding");
        }
    }
}

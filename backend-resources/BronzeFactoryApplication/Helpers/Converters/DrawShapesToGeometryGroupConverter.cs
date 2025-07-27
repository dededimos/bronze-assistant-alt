using SVGDrawingLibrary.Models;
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
    public class DrawShapesToGeometryGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<DrawShape> shapes)
            {
                return new GeometryCollection(shapes.Select(s => Geometry.Parse(s.GetShapePathData())));
            }
            else
            {
                return new GeometryCollection();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"Two Way Binding is not Supported from {nameof(DrawShapesToGeometryGroupConverter)}");
        }
    }
}

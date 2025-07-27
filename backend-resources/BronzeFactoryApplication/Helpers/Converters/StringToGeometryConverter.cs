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
    public class StringToGeometryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not null && value is string s)
            {
                return ConvertToGeometry(s);
            }
            return null!;//Geometry.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"Two Way Binding is not Supported from Geometry to PathData string");
        }

        public static Geometry ConvertToGeometry(string pathData)
        {
            if (string.IsNullOrEmpty(pathData)) return Geometry.Empty;
            else return Geometry.Parse(pathData);
        }
    }
}

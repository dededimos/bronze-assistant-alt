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
    public class StringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not null and string s)
            {
               return ConvertToSolidColorBrush(s);
            }
            else
            {
                return Brushes.DarkRed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                return brush.Color.ToString();
            }
            else
            {
                return Brushes.Red.ToString();
            }
        }

        /// <summary>
        /// Converts a string into a Solid Color Brush . If not convertible returns Dark Red Solid Color Brush
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SolidColorBrush ConvertToSolidColorBrush(string value)
        {
            if (string.IsNullOrEmpty(value)) return Brushes.DarkRed;
            try
            {
                return new BrushConverter().ConvertFromString(value) as SolidColorBrush ?? Brushes.DarkRed;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error converting {value} to SolidColorBrush ,Solid Brush DarkRed has been selected automatically", value);
                return Brushes.DarkRed;
            }
        }
    }
}

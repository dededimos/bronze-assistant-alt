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
    public class SolidColorBrushToHexadecimalStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                return brush.Color.ToString();
            }
            else
            {
                return Brushes.Transparent.Color.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return new SolidColorBrush(Colors.Red);
            
            var colorString = value.ToString();
            Color color;
            try
            {
                color = (Color)ColorConverter.ConvertFromString(colorString);
            }
            catch (FormatException)
            {
                color = Colors.Red;
            }

            return new SolidColorBrush(color);
        }
    }
}

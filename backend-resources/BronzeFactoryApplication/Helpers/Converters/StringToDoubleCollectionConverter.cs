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
    public class StringToDoubleCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                if (s == string.Empty) return new DoubleCollection();
                var doubles = s.Split(',').Select(str => double.TryParse(str, out double res) ? res : throw new FormatException($"Invalid Format Conversion for string :'{s}' Failed - {nameof(StringToDoubleCollectionConverter)}"));
                return new DoubleCollection(doubles);
            }
            return new DoubleCollection();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DoubleCollection col)
            {
                return string.Join(',', col);
            }
            throw new NotSupportedException($"Only {nameof(DoubleCollection)} can be converted back to a string by a {nameof(StringToDoubleCollectionConverter)}");
        }
    }
}

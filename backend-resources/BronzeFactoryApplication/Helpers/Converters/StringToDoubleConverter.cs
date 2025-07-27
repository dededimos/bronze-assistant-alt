using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BronzeFactoryApplication.Helpers.Converters
{
    public class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value?.ToString(),NumberStyles.AllowDecimalPoint,CultureInfo.InvariantCulture,out double result))
            {
                return result;
            }
            else if (value is null)
            {
                return double.NaN;
            }
            throw new FormatException($"The Provided string was not on a Parsable Double Format , Conversion Failed {nameof(StringToDoubleConverter)}");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() ?? string.Empty;
        }
    }
}

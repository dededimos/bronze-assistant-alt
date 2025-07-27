using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BronzeFactoryApplication.Helpers.Converters
{
    public class StringToNullableDoubleValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convert from double? to string
            return value?.ToString() ?? string.Empty;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convert from string to double?
            if (string.IsNullOrEmpty(value as string))
                return null;

            if (double.TryParse(value as string, out double result))
                return result;

            throw new InvalidOperationException($"Converter can only ConvertBack strings that are parsable to a Nullable Double , Invalid input on,{nameof(StringToNullableDoubleValueConverter)}");
        }
    }
}

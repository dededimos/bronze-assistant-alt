using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BronzeFactoryApplication.Helpers.Converters
{
    public class ObjectEnumerableToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable enumerable) //if i do IEnumerable<object> value types are not considered ...
            {
                var list = enumerable.Cast<object>().ToList();
                if (list.Count == 0) return " - ";
                var separator = string.IsNullOrEmpty(parameter?.ToString()) ? " , " : parameter.ToString();
                return string.Join(separator, list.Select(o => o?.ToString()?.TryTranslateKeyWithoutError() ?? "null"));
            }
            return " - ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

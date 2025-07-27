using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BronzeFactoryApplication.Helpers.Converters
{
    public class RatioConverterDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double valueToCovert = (double)value;
            double ratio = double.Parse(parameter?.ToString() ?? "1", CultureInfo.InvariantCulture);
            return valueToCovert * ratio;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot ConvertBack a Ratio - This is not Supported");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BronzeFactoryApplication.Helpers.Converters
{
    public class VisibleWhenOneTrueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var item in values)
            {
                bool boolValue = item as bool? ?? false;
                if (boolValue is false) continue;
                else return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(VisibleWhenOneTrueConverter)} does not support Two-Way Binding");
        }
    }
}

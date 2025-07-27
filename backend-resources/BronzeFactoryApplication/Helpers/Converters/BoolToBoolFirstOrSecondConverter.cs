using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BronzeFactoryApplication.Helpers.Converters
{
    /// <summary>
    /// Evaluates all Values until it finds a true to Return it , otherwise Returns False
    /// </summary>
    public class BoolToBoolFirstOrSecondConverter : IMultiValueConverter
    {
        
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var item in values)
            {
                bool boolValue = item as bool? ?? false;
                if (boolValue is false) continue;
                else return true;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{this.GetType().Name} does not support Two-Way Binding");
        }
    }
}

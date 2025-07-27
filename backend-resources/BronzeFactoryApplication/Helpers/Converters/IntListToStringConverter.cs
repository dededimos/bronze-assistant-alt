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
    /// Converts an Enumerable of ints into strings separated by <see cref="Environment.NewLine"/>
    /// The Parameter defines a suffix on the end of each int (as a Unit)
    /// </summary>
    public class IntListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not null && value is IEnumerable<int> intList)
            {
                if (parameter is not null && parameter is string)
                {
                    var intListWithSuffix = intList.Select(i => $"{i}{parameter}");
                    return string.Join(Environment.NewLine, intListWithSuffix);
                }
                return string.Join(Environment.NewLine, intList);
            }
            else
            {
                return "UndefinedIntList";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot ConvertBack from string into List of Ints");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BronzeFactoryApplication.Helpers.Converters
{
    public class DateTimeToShortStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string dateString)
            {
                return DateTime.TryParse(dateString, out DateTime date) ? date.Date : DateTime.MinValue;
            }
            return DateTime.MinValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                return dateTime.Date.ToString("dd-MM-yyyy");
            }
            return string.Empty;
        }
    }
}

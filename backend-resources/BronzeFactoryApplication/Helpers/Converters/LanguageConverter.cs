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
    /// Convertsd a Key to a Translated Language Term depending on the Current Language Dictionary
    /// </summary>
    public class LanguageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not null)
            {
                var key = value.ToString();
                return App.Current.TryFindResource(key)?.ToString() ?? $"Key:({key}) not Found";
            }
            else
            {
                return "Undefined".TryTranslateKeyWithoutError();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot ConvertBack from Translated string to Key");
        }
    }
}

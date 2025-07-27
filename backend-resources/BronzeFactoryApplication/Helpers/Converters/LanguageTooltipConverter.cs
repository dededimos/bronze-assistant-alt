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
    /// For the Provided Value trys to Find the "{Value}Tooltip" key and return the translation
    /// If there is no such key , it returns the key translation or Not FOund Resource
    /// </summary>
    public class LanguageTooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not null)
            {
                var key = value.ToString();
                return App.Current.TryFindResource(key + "Tooltip")?.ToString() 
                    ?? App.Current.TryFindResource(key)?.ToString()
                    ?? $"Key:({key}) not Found";
            }
            else
            {
                return "Not Found Resource";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot ConvertBack from Translated string to Key");
        }
    }
}

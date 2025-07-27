using CommonInterfacesBronze;
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
    /// Converts a Localized String Value into the Current Language Value or the Default Value if one does not Exist
    /// <para>Minified with max 33 Charachters , except if a different parameter is defined</para>
    /// </summary>
    public class LocalizedStringMinifiedValueConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var locString = value as LocalizedString ?? new LocalizedString("Undefined Localized String");
            if (Application.Current is App app)
            {
                var selectedLanguage = app.SelectedLanguage;
                int maxCharacters = 33;
                if (parameter is not null)
                {
                    maxCharacters = int.TryParse(parameter.ToString(), out int result) ? result : 33;
                }
                
                return locString.GetMinifiedLocalizedValue(selectedLanguage, maxCharacters);
            }
            else
            {
                return "DesignTimeLocalizedValue";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Two Way Conversion from string to Localized String is not Supported");
        }
    }
    
    
}

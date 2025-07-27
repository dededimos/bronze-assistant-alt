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
    /// </summary>
    public class LocalizedStringValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var locString = value as LocalizedString ?? new LocalizedString("Undefined Localized String");
            if (Application.Current is App app)
            {
                var selectedLanguage = app.SelectedLanguage;
                return locString.GetLocalizedValue(selectedLanguage);
            }
            else return "DesignTimeLocalizedValue";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Two Way Conversion from string to Localized String is not Supported");
        }
    }
}

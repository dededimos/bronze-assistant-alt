using AccessoriesRepoMongoDB.Entities;
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
    /// Converts a List of TraitEntities into a single String with their Localized TraitValues
    /// </summary>
    public class TraitEntityListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var traitEntities = value as IEnumerable<TraitEntity> ?? new List<TraitEntity>();
            var currentLang = ((App)Application.Current).SelectedLanguage;
            string traitsString = string.Join(',', traitEntities.Select(e => e.Trait.GetLocalizedValue(currentLang)));
            
            // Return the Concatenated String whole if less than 53 chars else add '...' after the 50th char
            return (traitsString.Length <= 33 ? traitsString : string.Concat(traitsString.AsSpan(0,30),"..."));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

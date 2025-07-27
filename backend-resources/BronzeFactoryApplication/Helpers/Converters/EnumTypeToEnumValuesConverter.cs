using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BronzeFactoryApplication.Helpers.Converters
{
    public class EnumTypeToEnumValuesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Type type)
            {
                if (type.IsEnum)
                {
                    return Enum.GetValues(type);
                }
                else if(Nullable.GetUnderlyingType(type)?.IsEnum is true)
                {
                    return Enum.GetValues(Nullable.GetUnderlyingType(type)!);
                }
                
            }
            return new List<object>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

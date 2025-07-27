using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace BronzeFactoryApplication.Helpers.Converters
{
    public class ListOfDoubleToDoubleCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<double> doubles)
            {
                return new DoubleCollection(doubles);
            }
            else if (value is null) return new DoubleCollection();
            else throw new ArgumentException($"{nameof(ListOfDoubleToDoubleCollectionConverter)} can only convert from List<double> or null");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(ListOfDoubleToDoubleCollectionConverter)} does not Support two Way Binding");
        }
    }
}

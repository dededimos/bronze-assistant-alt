using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BronzeFactoryApplication.Helpers.Converters
{
    public class DrawImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CabinDrawNumber draw = value as CabinDrawNumber? ?? CabinDrawNumber.None;

            if (App.ImagesMap.CabinDrawImage.TryGetValue(draw, out string? path))
            {
                return path;
            }
            else
            {
                return App.ImagesMap.CabinDrawImage[CabinDrawNumber.None];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack from string Path is not Supported ,Please use One Way Binding");
        }
    }
}

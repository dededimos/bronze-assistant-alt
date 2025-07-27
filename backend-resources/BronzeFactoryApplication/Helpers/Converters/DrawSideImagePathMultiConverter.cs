using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace BronzeFactoryApplication.Helpers.Converters
{
    public class DrawSideImagePathMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values.Contains(null)) return null!;

            CabinDrawNumber draw = values[0] as CabinDrawNumber? ?? CabinDrawNumber.None;
            CabinSynthesisModel synthesisModel = values[1] as CabinSynthesisModel? ?? CabinSynthesisModel.Primary;

            if (App.ImagesMap.CabinDrawSideImage.TryGetValue((draw, synthesisModel), out string? path))
            {
                return new BitmapImage(new Uri(path,UriKind.Relative));
            }
            else
            {
                path = App.ImagesMap.CabinDrawSideImage[(draw,synthesisModel)];
                return new BitmapImage(new Uri(path, UriKind.Relative));
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

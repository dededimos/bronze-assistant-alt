using BarcodeLibrary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Type = System.Type;

namespace BronzeFactoryApplication.Helpers.Converters
{
    public class StringToBarcodeImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringBarcode = value as string ?? string.Empty;
            var bytes = BarcodeHelper.GetBarcodeBytes(stringBarcode);

            //Convert the Bytes into a BitmapImage Recognized by WPF
            using (var stream = new MemoryStream(bytes))
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // To make it cross-thread accessible
                return bitmapImage;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back from a barcode image to a string.");
        }
    }
}

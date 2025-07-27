using AzureBlobStorageLibrary;
using System.Globalization;
using System.Windows.Data;

namespace BronzeFactoryApplication.Helpers.Converters
{
    public class MirrorsBlobUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string partialUrl = value as string ?? "UndefinedURL";
            //Check if the url Exists in the Current Machine . If it does this is a filePath and does not need Changing
            if (File.Exists(partialUrl))
            {
                return partialUrl;
            }
            if (string.IsNullOrEmpty(partialUrl)) return "UndefinedURL";
            BlobPhotoSize photoSize = parameter as BlobPhotoSize? ?? BlobPhotoSize.Undefined;
            return App.BlobHelper.ConvertBlobPartialToFullUrl(partialUrl, BlobContainerOption.MirrorsBlobs, photoSize);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string fullUrl = value as string ?? string.Empty;
            if (File.Exists(fullUrl))
            {
                return fullUrl;
            }
            if (string.IsNullOrEmpty(fullUrl) || fullUrl == "UndefinedURL") return string.Empty;
            return App.BlobHelper.ConvertBlobFullUrlToPartial(fullUrl);

            //throw new NotSupportedException("Conversion From Abosulte to Partial URL is not Supported");
        }
    }
}

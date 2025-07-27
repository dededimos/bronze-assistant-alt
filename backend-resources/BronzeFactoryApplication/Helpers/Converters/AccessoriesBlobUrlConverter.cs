using AzureBlobStorageLibrary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using static AzureBlobStorageLibrary.BlobUrlHelper;

namespace BronzeFactoryApplication.Helpers.Converters
{
    public class AccessoriesBlobUrlConverter : IValueConverter
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
            return App.BlobHelper.ConvertBlobPartialToFullUrl(partialUrl, BlobContainerOption.AccessoriesBlobs,photoSize);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Conversion From Abosulte to Partial URL is not Supported");
        }
    }
}

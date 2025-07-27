using CommonHelpers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BathAccessoriesModelsLibrary.Services
{
    public class AccessoriesUrlHelper
    {
        private readonly AccessoriesUrlHelperOptions options;
        public static readonly (int width, int height) ImageThumbSize = (150, 150);
        public static readonly (int width, int height) ImageSmallSize = (320, 240);
        public static readonly (int width, int height) ImageMediumSize = (640, 480);
        public static readonly (int width, int height) ImageLargeSize = (800, 600);
        public static readonly (int width, int height) ImageFullSize = (1280, 720);

        public AccessoriesUrlHelper(IOptions<AccessoriesUrlHelperOptions> options)
        {
            this.options = options.Value;
        }

        /// <summary>
        /// Appends the Blob Container Path to the Files Relative Url
        /// </summary>
        /// <param name="url">The Relative Url</param>
        /// <returns></returns>
        public string AppendContainerPathToUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return url;
            return string.Concat(options.ContainerAddress, url);
        }

        public string RemoveContainerPathFromUrl(string url)
        {
            if (url.StartsWith(options.ContainerAddress))
            {
                return url.Remove(0, options.ContainerAddress.Length);
            }
            else
            {
                return url;
            }
        }

        public string GetThumbnail(string photoLink)
        {
            if (string.IsNullOrWhiteSpace(photoLink)) return string.Empty;
            return CommonVariousHelpers.AppendSuffixToFullPath(photoLink, options.ThumbnailSuffix);
        }
        public string GetSmallPhoto(string photoLink)
        {
            if (string.IsNullOrWhiteSpace(photoLink)) return string.Empty;
            return CommonVariousHelpers.AppendSuffixToFullPath(photoLink, options.SmallPhotoSuffix);
        }

        public string GetMediumPhoto(string photoLink)
        {
            if (string.IsNullOrWhiteSpace(photoLink)) return string.Empty;
            return CommonVariousHelpers.AppendSuffixToFullPath(photoLink, options.MediumPhotoSuffix);
        }
        public string GetLargePhoto(string photoLink)
        {
            if (string.IsNullOrWhiteSpace(photoLink)) return string.Empty;
            return CommonVariousHelpers.AppendSuffixToFullPath(photoLink, options.LargePhotoSuffix);
        }

        public string GetPhoto(string photoLink, PhotoSize size)
        {
            return size switch
            {
                PhotoSize.Thumbnail => GetThumbnail(photoLink),
                PhotoSize.Small => GetSmallPhoto(photoLink),
                PhotoSize.Medium => GetMediumPhoto(photoLink),
                PhotoSize.Large => GetLargePhoto(photoLink),
                PhotoSize.Full => photoLink,
                _ => throw new NotSupportedException($"The Selected Photo Size is not Supported : {size}"),
            };
        }
        /// <summary>
        /// Returns the specified Size of a photo of a Link or the Default Value if the link is empty or equal to the default value
        /// </summary>
        /// <param name="photoLink">The Link of the Photo</param>
        /// <param name="size">The Size of the Photo</param>
        /// <param name="defaultValue">The Default Value</param>
        /// <returns></returns>
        public string GetPhotoOrDefault(string photoLink, PhotoSize size,string defaultValue)
        {
            if (string.IsNullOrWhiteSpace(photoLink) || photoLink == defaultValue) 
            {
                return defaultValue;
            }
            else return GetPhoto(photoLink, size);
        }
    }

    public class AccessoriesUrlHelperOptions
    {
        /// <summary>
        /// The Suffix for the Thumbnail Photo
        /// </summary>
        public string ThumbnailSuffix { get; set; } = string.Empty;
        /// <summary>
        /// The Suffix for the Small Photo
        /// </summary>
        public string SmallPhotoSuffix { get; set; } = string.Empty;
        /// <summary>
        /// The Suffix for the Medium Photo
        /// </summary>
        public string MediumPhotoSuffix { get; set; } = string.Empty;
        /// <summary>
        /// The Suffix for the Large Photo
        /// </summary>
        public string LargePhotoSuffix { get; set; } = string.Empty;
        /// <summary>
        /// The Address of the Container Containing the Photos
        /// </summary>
        public string ContainerAddress { get; set; } = string.Empty;
    }

    public enum PhotoSize
    {
        /// <summary>
        /// 150x150 dpi
        /// </summary>
        Thumbnail,
        /// <summary>
        /// 320x240 dpi
        /// </summary>
        Small,
        /// <summary>
        /// 640x480 dpi
        /// </summary>
        Medium,
        /// <summary>
        /// 800x600 dpi
        /// </summary>
        Large,
        /// <summary>
        /// 1280x720 dpi
        /// </summary>
        Full
    }
}

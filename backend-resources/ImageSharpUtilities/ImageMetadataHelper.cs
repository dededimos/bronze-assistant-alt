using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSharpUtilities
{
    public static class ImageMetadataHelper
    {
        /// <summary>
        /// Retrieves the Image Height and Width from the provided image byte Data 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static (int width , int height) GetImageSizeFromData(byte[] data)
        {
            ImageInfo imageInfo = Image.Identify(data);
            return (imageInfo.Width, imageInfo.Height);
        }

    }
}

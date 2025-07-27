using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ImageSharpUtilities
{
    public static class ImagesHelpers
    {
        /// <summary>
        /// Returns information Regarding the Format of an Image
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<IImageFormat> DetectImageFormat(string filePath)
        {
            using Stream fileStream = File.OpenRead(filePath);
            return await Image.DetectFormatAsync(fileStream);
        }

        /// <summary>
        /// Returns a Memory Stream with the Resized Image
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public static async Task<MemoryStream> LoadAndResizeImage(string filePath, int newWidth, int newHeight)
        {
            using var image = await Image.LoadAsync(filePath);
            var resizedImage = ResizeImage(image, newWidth, newHeight);
            var ms = new MemoryStream();
            await resizedImage.SaveAsync(ms, new PngEncoder());
            return ms;
        }

        /// <summary>
        /// Loads an image to a Memory Stream
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public static async Task<MemoryStream> LoadImageAsync(string imagePath, MemoryStream stream, bool usePngEncoder = true)
        {
            using var image = await Image.LoadAsync(imagePath);
            await image.SaveAsync(stream, usePngEncoder ? new PngEncoder() : new JpegEncoder() { Quality = 100 });
            return stream;
        }


        /// <summary>
        /// Crops an Image (Rectangle Crop) by Removing the Outermost Pixels with the designated Color scheme(default is rgba : -250,250,250,250-) .
        /// Checks its pixel one by one to determine where the items Outer area is.
        /// </summary>
        /// <param name="image">The Image to Crop (if not in RGBA32 format use Image.CloneAs<Rgba32>())</param>
        /// <param name="red">The Red Threshold</param>
        /// <param name="green">The Green Threshold</param>
        /// <param name="blue">The Blue Threshold</param>
        /// <param name="alpha">The Alpha Threshold (opacity level) 255 means fully Visible , 0 means Transparent</param>
        /// <returns></returns>
        public static void CropWhiteOrTransparentBackground(Image<Rgba32> image, int red = 250, int green = 250, int blue = 250, int alpha = 250)
        {
            
            // Determine the crop rectangle by finding the bounding box of non-white and non-transparent pixels
            int minX = image.Width;
            int minY = image.Height;
            int maxX = 0;
            int maxY = 0;
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var pixel = image[x, y];
                    if (pixel.R < red || pixel.G < green || pixel.B < blue || pixel.A < alpha) // Adjust the threshold as needed
                    {
                        // Checks All the Pixels coordinates an Finds teh Rectangle where the Pixels stop being white or Transparent
                        // Starts Max from 0 so any found pixel is set as Max and then as it progresses finds the last max
                        // Starts Min from Full Picture so any found pixel is set as Min and then as it progresses finds the last min
                        minX = Math.Min(minX, x);
                        minY = Math.Min(minY, y);
                        maxX = Math.Max(maxX, x);
                        maxY = Math.Max(maxY, y);
                    }
                }
            }

            // Crop the image
            Rectangle cropRectangle = new(minX, minY, maxX - minX, maxY - minY);
            image.Mutate(x => x.Crop(cropRectangle));
        }

        /// <summary>
        /// Resizes an Image to the Selected new Width and Height
        /// If the final image will be stretched from the new Dimensions , it pads it until it reaches the correct dimensions
        /// </summary>
        /// <param name="image"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        private static Image ResizeImage(Image image, int newWidth, int newHeight)
        {
            ResizeOptions options = new()
            {
                Size = new Size(newWidth, newHeight),
                Mode = ResizeMode.Max
            };
            //Resize image keeping aspect Ratio
            image.Mutate(x => x.Resize(options));

            //Calculate Padding - if one of the dimensions is actually less as its aspect ratio was preserved
            int widthPadding = (newWidth - image.Width) / 2;
            int heightPadding = (newHeight - image.Height) / 2;

            //Pad the Image to the desired Size , fill with transparent pixels (leave space and put image in center)
            image.Mutate(x => x.Pad(widthPadding, heightPadding, new Rgba32(255, 255, 255, 0)));
            return image;
        }

        /// <summary>
        /// Resizes an Image to a Fixed Size . Any Needed pixels are added as Transparent
        /// If the Image is smaller only Transparent Pixels will be added without changing the actual image size (if selected so)
        /// If the Image is Bigger it will downsize to the highest Dimension and transparent pixels will be added to fill up the other one
        /// </summary>
        /// <param name="image">The Image to Resize</param>
        /// <param name="newImageWidth"></param>
        /// <param name="newImageHeight"></param>
        /// <param name="matchSizeWithTransparency">Weather the Resize should match the remaining size with Transparent Pixels . When image is eresized its aspect ratio is not changed .So Inevitably one of the Dimensions is not resized to the desired Size</param>
        public static void DownsizeImage(Image image, int newImageWidth, int newImageHeight, bool matchSizeWithTransparency)
        {
            image.Mutate(ctx =>
            {
                if (image.Width > newImageWidth || image.Height > newImageHeight)
                {
                    var resizeOptions = new ResizeOptions()
                    {
                        Size = new Size(newImageWidth, newImageHeight),
                        // maintains original aspect Ratio
                        Mode = ResizeMode.Max
                    };
                    // Resize the Image
                    ctx.Resize(resizeOptions);
                }

                // Match desired Size with Transparent Pixels
                if (matchSizeWithTransparency)
                {
                    // Calculate padding to center the Image to Add the Transparent pixels
                    int padWidth = (newImageWidth - ctx.GetCurrentSize().Width);
                    int padHeight = (newImageHeight - ctx.GetCurrentSize().Height);

                    // Pad with Transparent Pixels to reach the desired Size
                    ctx.BackgroundColor(new Rgba32(0, 0, 0, 0))
                    .Pad(padWidth, padHeight);
                }
            });
        }


    }
}

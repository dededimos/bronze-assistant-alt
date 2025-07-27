using CommonHelpers.HttpHelpers.FileModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobStorageLibrary
{
    public class BlobUrlHelper
    {
        /// <summary>
        /// The Name of the Blob container with Accessories
        /// </summary>
        private readonly string accessoriesContainerUrl = string.Empty;
        public string AccessoriesContainerUrl { get => accessoriesContainerUrl; }

        /// <summary>
        /// The Name of the Blob container with Cabins
        /// </summary>
        private readonly string cabinsContainerUrl = string.Empty;
        public string CabinsContainerUrl { get => cabinsContainerUrl; }
        /// <summary>
        /// The Name of the Blob container with Mirrors
        /// </summary>
        private readonly string mirrorsContainerUrl = string.Empty;
        public string MirrorsContainerUrl { get => mirrorsContainerUrl; }
        /// <summary>
        /// The Url of the whole Blob Server
        /// </summary>
        private readonly string mainBlobUrl = string.Empty;
        /// <summary>
        /// The Full Url of the Blob Container of Accessories
        /// </summary>
        private readonly string accessoriesAffixUrl = string.Empty;
        /// <summary>
        /// The Full Url of the Blob Container of Cabins
        /// </summary>
        private readonly string cabinsAffixUrl = string.Empty;
        /// <summary>
        /// The Full Url of the Blob Container of Mirrors
        /// </summary>
        private readonly string mirrorsAffixUrl = string.Empty;

        /// <summary>
        /// The Configuration Key to find the Name of the Accessories Container in the AppSettings.json
        /// </summary>
        private readonly string accessoriesContainerConfigName = "AccessoriesBlobStorageContainerName";
        /// <summary>
        /// The Configuration Key to find the Name of the Cabins Container in the AppSettings.json
        /// </summary>
        private readonly string cabinsContainerConfigName = "CabinsBlobStorageContainerName";
        /// <summary>
        /// The Configuration Key to find the Name of the Mirrors Container in the AppSettings.json
        /// </summary>
        private readonly string mirrorsContainerConfigName = "MirrorsBlobStorageContainerName";
        /// <summary>
        /// The Configuration Key to find the url of the whole blob server in the AppSettings.json
        /// </summary>
        private readonly string blobsMainUrlConfigName = "BlobsMainUrl";

        /// <summary>
        /// The Suffix Added to a Photo Url to represent the Thumbnail Version
        /// </summary>
        public static readonly string ThumbnailSuffix = "-Thumb";
        
        /// <summary>
        /// The Suffix Added to a Photo Url to represent the Small Version
        /// </summary>
        public static readonly string SmallPhotoSuffix = "-Small";
        /// <summary>
        /// The Suffix Added to a Photo Url to represent the Medium Version
        /// </summary>
        public static readonly string MediumPhotoSuffix = "-Medium";
        /// <summary>
        /// The Suffix Added to a Photo Url to represent the Large Version
        /// </summary>
        public static readonly string LargePhotoSuffix = "-Large";
        /// <summary>
        /// The Suffix Added to a File Url to represent the Pdf Sheet of an Accessory
        /// </summary>
        public static readonly string PdfFileSuffix = "-Sheet";

        //The Various Folder Names (should be put in the App settings json and intilized from there but cba ...)
        public static readonly string AccessoriesPhotosFolderName = "Accessories";
        public static readonly string SheetsFolderName = "Sheets";
        public static readonly string TraitClassesFolderName = "TraitClasses";
        public static readonly string FinishesFolderName = $"{TraitClassesFolderName}/Finishes";
        public static readonly string MaterialsFolderName = $"{TraitClassesFolderName}/Materials";
        public static readonly string SizesFolderName = $"{TraitClassesFolderName}/Sizes";
        public static readonly string ShapesFolderName = $"{TraitClassesFolderName}/Shapes";
        public static readonly string PrimaryTypesFolderName = $"{TraitClassesFolderName}/PrimaryTypes";
        public static readonly string SecondaryTypesFolderName = $"{TraitClassesFolderName}/SecondaryTypes";
        public static readonly string CategoriesFolderName = $"{TraitClassesFolderName}/Categories";
        public static readonly string SeriesFolderName = $"{TraitClassesFolderName}/Series";
        public static readonly string MountingTypesFolderName = $"{TraitClassesFolderName}/MountingTypes";
        public static readonly string DimensionsFolderName = $"{TraitClassesFolderName}/Dimensions";
        public static readonly string PricesFolderName = $"{TraitClassesFolderName}/Prices";

        private static readonly (int width, int height) ImageThumbSize = (150, 150);
        private static readonly (int width, int height) ImageSmallSize = (320, 240);
        private static readonly (int width, int height) ImageMediumSize = (640, 480);
        private static readonly (int width, int height) ImageLargeSize = (800, 600);
        private static readonly (int width, int height) ImageFullSize = (1280, 720);

        /// <summary>
        /// The Maximum Size that an Accessory Technical Sheet can Have
        /// </summary>
        public const long MaxUploadedFileSizeLimit = 5 * 1024 * 1024; //5MB

        public BlobUrlHelper(IConfiguration config)
        {
            accessoriesContainerUrl = config[accessoriesContainerConfigName] ?? throw new Exception($"Configuration Section Not Found -- {accessoriesContainerConfigName}");
            cabinsContainerUrl = config[cabinsContainerConfigName] ?? throw new Exception($"Configuration Section Not Found -- {cabinsContainerConfigName}");
            mirrorsContainerUrl = config[mirrorsContainerConfigName] ?? throw new Exception($"Configuration Section Not Found -- {mirrorsContainerConfigName}");
            mainBlobUrl = config[blobsMainUrlConfigName] ?? throw new Exception($"Configuration Section Not Found -- {blobsMainUrlConfigName}");

            accessoriesAffixUrl = $"{mainBlobUrl}{accessoriesContainerUrl}/";
            cabinsAffixUrl = $"{mainBlobUrl}{cabinsContainerUrl}/";
            mirrorsAffixUrl = $"{mainBlobUrl}{mirrorsContainerUrl}/";
        }

        /// <summary>
        /// Returns the Full Url of a file in the Accessories Blob 
        /// </summary>
        /// <param name="partialUrl">The Partial Url of the File (The RelativePath inside the blob , the Url that is Saved to the Database)</param>
        /// <param name="containerOption">The Container Option , Undefined if already contained in the Partial Url</param>
        /// <param name="size">The Size of the Photo of the Blob</param>
        /// <returns></returns>
        public string ConvertBlobPartialToFullUrl(string partialUrl,BlobContainerOption containerOption, BlobPhotoSize? size = null)
        {
            // Add a Suffix if needed (when for photos)
            string fullUrl = size switch
            {
                BlobPhotoSize.ThumbSizePhoto => AddSuffixToUrl(partialUrl, ThumbnailSuffix),
                BlobPhotoSize.SmallSizePhoto => AddSuffixToUrl(partialUrl, SmallPhotoSuffix),
                BlobPhotoSize.MediumSizePhoto => AddSuffixToUrl(partialUrl, MediumPhotoSuffix),
                BlobPhotoSize.LargeSizePhoto => AddSuffixToUrl(partialUrl, LargePhotoSuffix),
                _ => partialUrl,
            };

            string containerAffix = containerOption switch
            {
                BlobContainerOption.AccessoriesBlobs => accessoriesAffixUrl,
                BlobContainerOption.CabinsBlobs => cabinsAffixUrl,
                BlobContainerOption.MirrorsBlobs => mirrorsAffixUrl,
                BlobContainerOption.Undefined => mainBlobUrl,
                _ => throw new NotSupportedException($"The Specified {nameof(BlobContainerOption)}:'{containerOption}' is not Supported...")
            };

            return string.Concat(containerAffix, fullUrl);
        }

        /// <summary>
        /// Creates an Accessory File Relative Url without the FileExtension , by providing a FileName
        /// </summary>
        /// <param name="options">The Creation Options</param>
        /// <returns>The Full Url (includes Container name e.t.c) without the Extension of the File (e.x. without .png)</returns>
        public static string CreateBlobFileRelativeUrl(BlobUrlCreationOptions options)
        {
            // Add the Container Link at the Begining
            StringBuilder builder = new();

            // Append any Preceiding Folders
            foreach (var precedingFolder in options.PrecedingFolders)
            {
                if (!string.IsNullOrWhiteSpace(precedingFolder))
                {
                    // Add the Preceding Folders
                    builder.Append(precedingFolder).Append('/');
                }
            }

            // Append the FileName
            builder.Append(string.IsNullOrWhiteSpace(options.FileName) ? "????" : options.FileName);

            // Append Suffix to fileName
            if (!string.IsNullOrWhiteSpace(options.SuffixToFileName))
            {
                builder.Append('-').Append(options.SuffixToFileName);
            }

            if (options.MimeType is BronzeMimeType.PngImage or BronzeMimeType.JpgImage)
            {
                string itemVariationSuffix = options.PhotoSize switch
                {
                    BlobPhotoSize.ThumbSizePhoto => ThumbnailSuffix,
                    BlobPhotoSize.SmallSizePhoto => SmallPhotoSuffix,
                    BlobPhotoSize.MediumSizePhoto => MediumPhotoSuffix,
                    BlobPhotoSize.LargeSizePhoto => LargePhotoSuffix,
                    BlobPhotoSize.FullSizePhoto => string.Empty,
                    _ => string.Empty,
                };
                builder.Append(itemVariationSuffix);
            }
            else if (options.MimeType == BronzeMimeType.PdfFile)
            {
                builder.Append(PdfFileSuffix);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Returns the provided Url without its Blob Container Address (The Url Saved to the Database)
        /// </summary>
        /// <param name="absoluteUrl">The Full Url of a Blob File</param>
        /// <returns>The new Url without its Blob Container Prefix or Throws a Format Exception</returns>
        /// <exception cref="FormatException">When the Provided url is not at the correct format</exception>
        public string ConvertBlobFullUrlToPartial(string absoluteUrl)
        {
            string partialUrl = string.Empty;
            //Remove any suffixes
            partialUrl = absoluteUrl.Replace(ThumbnailSuffix, string.Empty);
            partialUrl = partialUrl.Replace(MediumPhotoSuffix, string.Empty);
            partialUrl = partialUrl.Replace(LargePhotoSuffix, string.Empty);
            partialUrl = partialUrl.Replace(SmallPhotoSuffix, string.Empty);

            if (partialUrl.Contains(accessoriesAffixUrl))
            {
                return partialUrl.Replace(accessoriesAffixUrl, "");
            }
            else if (partialUrl.Contains(mirrorsAffixUrl))
            {
                return partialUrl.Replace(mirrorsAffixUrl, "");
            }
            else if (partialUrl.Contains(cabinsAffixUrl))
            {
                return partialUrl.Replace(cabinsAffixUrl, "");
            }
            else
            {
                throw new FormatException($"Provided URI has an Invalid Format{Environment.NewLine}Does not contain Main Blob Url or Container Url : {mainBlobUrl}{Environment.NewLine}Provided Url : {absoluteUrl}");
            }
        }

        /// <summary>
        /// Returns the Size matching the Variation
        /// </summary>
        /// <param name="sizeVariation"></param>
        /// <returns></returns>
        public static (int width, int height) GetImageFixedSize(BlobPhotoSize sizeVariation)
        {
            return sizeVariation switch
            {
                BlobPhotoSize.ThumbSizePhoto => (ImageThumbSize.width, ImageThumbSize.height),
                BlobPhotoSize.SmallSizePhoto => (ImageSmallSize.width, ImageSmallSize.height),
                BlobPhotoSize.MediumSizePhoto => (ImageMediumSize.width, ImageMediumSize.height),
                BlobPhotoSize.LargeSizePhoto => (ImageLargeSize.width, ImageLargeSize.height),
                BlobPhotoSize.FullSizePhoto => (ImageFullSize.width, ImageFullSize.height),
                _ => (ImageFullSize.width, ImageFullSize.height),
            };
        }

        /// <summary>
        /// Returns the Name of a Sub Folder based on the Selected Enum Value
        /// </summary>
        /// <param name="subFolder">The Sub Folder</param>
        /// <returns></returns>
        public static string GetSubFolderName(AccessoriesBlobSubFolder subFolder)
        {
            return subFolder switch
            {
                AccessoriesBlobSubFolder.None => string.Empty,
                AccessoriesBlobSubFolder.AccessoriesPhotosFolder => AccessoriesPhotosFolderName,
                AccessoriesBlobSubFolder.SheetsFolder => SheetsFolderName,
                AccessoriesBlobSubFolder.FinishesFolder => FinishesFolderName,
                AccessoriesBlobSubFolder.MaterialsFolder => MaterialsFolderName,
                AccessoriesBlobSubFolder.SizesFolder => SizesFolderName,
                AccessoriesBlobSubFolder.ShapesTypesFolder => ShapesFolderName,
                AccessoriesBlobSubFolder.PrimaryTypesFolder => PrimaryTypesFolderName,
                AccessoriesBlobSubFolder.SecondaryTypesFolder => SecondaryTypesFolderName,
                AccessoriesBlobSubFolder.CategoriesFolder => CategoriesFolderName,
                AccessoriesBlobSubFolder.SeriesFolder => SeriesFolderName,
                AccessoriesBlobSubFolder.MountingTypesFolder => MountingTypesFolderName,
                AccessoriesBlobSubFolder.DimensionsFolder => DimensionsFolderName,
                AccessoriesBlobSubFolder.PricesFolder => PricesFolderName,
                AccessoriesBlobSubFolder.TraitClassesFolder => TraitClassesFolderName,
                _ => string.Empty,
            };
        }

        /// <summary>
        /// Adds a suffix to a url by removing the extension and then re appending it
        /// </summary>
        /// <param name="partialUrl">The partial url of an Image</param>
        /// <param name="suffix">The Suffix To the file Name without its Extension</param>
        /// <returns>The new url containing the suffix</returns>
        private static string AddSuffixToUrl(string partialUrl, string suffix)
        {
            //Remove the extension from the Url (.png)
            string urlWithoutExtension = partialUrl.Length > 4 ? partialUrl.Remove(partialUrl.Length - 4) : "";
            //Get only the extension (the last 4 letters ex '.png')
            string fileExtension = partialUrl.Length > 4 ? partialUrl[^4..] : "";

            //Build the new url containing the suffix
            StringBuilder builder = new();
            builder
                .Append(urlWithoutExtension)
                .Append(suffix)
                .Append(fileExtension);
            return builder.ToString();
        }

        /// <summary>
        /// Converts a Url of the Main photo into the Url of the Variation Requested
        /// </summary>
        /// <param name="mainImageUrl"></param>
        /// <param name="sizeVariation"></param>
        /// <returns></returns>
        public static string GetImageVariationUrl(string mainImageUrl, BlobPhotoSize sizeVariation)
        {
            // Add a Suffix if needed (when for photos)
            return sizeVariation switch
            {
                BlobPhotoSize.ThumbSizePhoto => AddSuffixToUrl(mainImageUrl, ThumbnailSuffix),
                BlobPhotoSize.SmallSizePhoto => AddSuffixToUrl(mainImageUrl, SmallPhotoSuffix),
                BlobPhotoSize.MediumSizePhoto => AddSuffixToUrl(mainImageUrl, MediumPhotoSuffix),
                BlobPhotoSize.LargeSizePhoto => AddSuffixToUrl(mainImageUrl, LargePhotoSuffix),
                _ => mainImageUrl,
            };
        }

        /// <summary>
        /// Returns the Path only without the fileName (ex. SomeFolder/SomeOtherFolder/filename.txt
        /// <para>Will return : SomeFolder/SomeOtherFolder</para>
        /// </summary>
        /// <param name="urlWithFileName">The url containing the fileName</param>
        /// <returns></returns>
        public static string GetBlobPathWithoutFileName(string urlWithFileName)
        {
            if (string.IsNullOrEmpty(urlWithFileName)) return string.Empty;
            return Path.GetDirectoryName(urlWithFileName) ?? string.Empty;
        }
    }

    /// <summary>
    /// The Options to Create a Url for an Uploaded Blob
    /// </summary>
    public class BlobUrlCreationOptions
    {
        /// <summary>
        /// The File Type
        /// </summary>
        public BronzeMimeType MimeType { get; set; }
        /// <summary>
        /// The Size Variation of the Photo if the Type is such
        /// </summary>
        public BlobPhotoSize PhotoSize { get; set; }
        /// <summary>
        /// Weather to Append an Extra Suffix in the FileName
        /// </summary>
        public string SuffixToFileName { get; set; }
        /// <summary>
        /// The Preceding Folders to the Blob File
        /// </summary>
        public string[] PrecedingFolders { get; set; }
        /// <summary>
        /// The File Name of the Blob File Without Its Extension
        /// </summary>
        public string FileName { get; set; }

        public BlobUrlCreationOptions(string fileName,
                                      BronzeMimeType mimeType,
                                      string suffixToFileName = "",
                                      BlobPhotoSize photoSize = BlobPhotoSize.Undefined,
                                      params string[] precedingFolders)
        {
            FileName = fileName;
            MimeType = mimeType;
            PrecedingFolders = precedingFolders;
            SuffixToFileName = suffixToFileName;
            PhotoSize = photoSize;
        }

    }
}

using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using CommonHelpers.HttpHelpers.FileModels;
using ImageSharpUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Formats.Png;
using CommonHelpers.Exceptions;

namespace AzureBlobStorageLibrary
{
    public class BlobStorageRepository : IBlobStorageRepository
    {
        private readonly ILogger<BlobStorageRepository> logger;
        private readonly BlobUrlHelper urlHelper;
        private readonly BlobContainerClient accessoriesBlobsContainer;
        private readonly BlobContainerClient cabinsBlobsContainer;
        private readonly BlobContainerClient mirrorsBlobsContainer;

        private readonly static IEnumerable<BlobPhotoSize> availablePhotoSizes = Enum.GetValues(typeof(BlobPhotoSize)).Cast<BlobPhotoSize>().Where(size => size != BlobPhotoSize.Undefined);
        private readonly static int numberOfAvailablePhotoSizes = availablePhotoSizes.Count();
        private readonly static double percentageOfEachPhotoSize = 1d / numberOfAvailablePhotoSizes * 100d;

        private readonly string storageConnectionStringConfigName = "BronzeBlobStorageConnectionString";
        private readonly string storageConnectionString;

        private readonly string accessoriesStorageContainerConfigName = "AccessoriesBlobStorageContainerName";
        private readonly string accessoriesStorageContainerName;

        private readonly string cabinsStorageContainerConfigName = "CabinsBlobStorageContainerName";
        private readonly string cabinsStorageContainerName;

        private readonly string mirrorsStorageContainerConfigName = "MirrorsBlobStorageContainerName";
        private readonly string mirrorsStorageContainerName;
        private readonly IEnumerable<string> acceptedMIMETypes = new List<string>() { "image/png", "image/jpg", "image/bmp", "application/pdf" };

        public BlobStorageRepository(IConfiguration config, ILogger<BlobStorageRepository> logger, BlobUrlHelper urlHelper)
        {
            storageConnectionString = config.GetConnectionString(storageConnectionStringConfigName)
                ?? throw new Exception($"-{storageConnectionStringConfigName}- was not Found in the AppSettings File");
            accessoriesStorageContainerName = config[accessoriesStorageContainerConfigName]
                ?? throw new Exception($"-{accessoriesStorageContainerConfigName}- was not Found in the AppSettings File");
            cabinsStorageContainerName = config[cabinsStorageContainerConfigName]
                ?? throw new Exception($"-{cabinsStorageContainerConfigName}- was not Found in the AppSettings File");
            mirrorsStorageContainerName = config[mirrorsStorageContainerConfigName]
                ?? throw new Exception($"-{mirrorsStorageContainerConfigName}- was not Found in the AppSettings File");

            accessoriesBlobsContainer = new(storageConnectionString, accessoriesStorageContainerName);
            cabinsBlobsContainer = new(storageConnectionString, cabinsStorageContainerName);
            mirrorsBlobsContainer = new(storageConnectionString, mirrorsStorageContainerName);

            this.logger = logger;
            this.urlHelper = urlHelper;
        }

        /// <summary>
        /// Uploads an Accessory Image to the Blob Storage along with all its Size Variations
        /// </summary>
        /// <param name="imageFilePath">The Path of the Image</param>
        /// <param name="fileName">The FileName of the Image</param>
        /// <param name="subFolder">The SubFolder to Put the Image to</param>
        /// <param name="cropPhotos">Weather to Crop the Photos White and Transparent Pixels</param>
        /// <returns>The Relative Path of the Full Image</returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> UploadAccessoryImagesToBlob(
            string imageFilePath,
            string fileName,
            AccessoriesBlobSubFolder subFolder,
            bool cropPhotos)
        {
            var photoSizes = Enum.GetValues(typeof(BlobPhotoSize))
                     .Cast<BlobPhotoSize>()
                     .Where(size => size != BlobPhotoSize.Undefined);
            string suffixToFileName = DateTime.Now.Ticks.ToString();
            string fullSizePhotoUrl = "";

            // Upload all Photo Sizes with the Same Ticks as Suffix
            foreach (var size in photoSizes)
            {
                string url = await UploadAccessoryImageToBlob(imageFilePath, size, subFolder, fileName, suffixToFileName, false, cropPhotos);
                if (size is BlobPhotoSize.FullSizePhoto) fullSizePhotoUrl = url;
            }
            if (string.IsNullOrWhiteSpace(fullSizePhotoUrl)) throw new Exception("Photos in all Sizes were Uploaded but the Operation Stopped Unexpectedly because a FullPhoto Url was not Returned...");
            return fullSizePhotoUrl;
        }

        /// <summary>
        /// Uploads an Accessory Image to the Blob Container for a certain Image Size Variation
        /// </summary>
        /// <param name="imageFilePathToDisk">The Image FilePath</param>
        /// <param name="photoSize">The Image Size Variation to be Created</param>
        /// <param name="folderToUploadTo">The Blob Folder where to Upload the Photo</param>
        /// <param name="fileName">The Main Code of the Accessory (this will be used in the Foldering structure of the Blob)</param>
        /// <param name="suffixToFileName">The Suffix to add to the end of the File Name (ex. Guid.ToString() or timeTicks.ToString())</param>
        /// <param name="matchSizeWithTransparency">Weather it should match the Size of the Image with Transparency when resizing the Image</param>
        /// <param name="cropPhoto">Weather to Crop the White and Transparent Pixels of the Image (Rectangle Crop)</param>
        /// <returns>The Relative Path of the Image Saved to the Blob</returns>
        public async Task<string> UploadAccessoryImageToBlob(
            string imageFilePathToDisk,
            BlobPhotoSize photoSize,
            AccessoriesBlobSubFolder folderToUploadTo,
            string fileName,
            string suffixToFileName,
            bool matchSizeWithTransparency,
            bool cropPhoto)
        {
            logger.LogInformation("Loading Image from file : {path}", imageFilePathToDisk);
            // Load the Image
            using (Image image = await Image.LoadAsync(imageFilePathToDisk))
            {
                // Set the Image as a Clone of Rgba32 if not already
                Image<Rgba32> imageRgba = image is Image<Rgba32> imgrgb ? imgrgb : image.CloneAs<Rgba32>();

                logger.LogInformation("Loaded Image To Memory... Width x Height : {width}x{height}", imageRgba.Width, imageRgba.Height);

                if (cropPhoto)
                {
                    // Crop
                    ImagesHelpers.CropWhiteOrTransparentBackground(imageRgba);
                    logger.LogInformation("Cropping Image ... New Dimensions : {width}x{height}", imageRgba.Width, imageRgba.Height);
                }


                //Resize
                var (width, height) = BlobUrlHelper.GetImageFixedSize(photoSize);
                ImagesHelpers.DownsizeImage(imageRgba, width, height, matchSizeWithTransparency);
                logger.LogInformation("Resizing Image ... New Dimensions : {width}x{height}", imageRgba.Width, imageRgba.Height);

                //Create Upload File
                using (MemoryStream stream = new())
                {
                    // Save the Image to a Memory Stream
                    await imageRgba.SaveAsync(stream, new PngEncoder());
                    // Dispose of either the cloned or original image (calling this dispose again as if cloned the using statement will only dispose the nonClone)


                    // Create a Buffer where the stream places the data
                    // OLD WAY : await stream.ReadAsync(buffer); stream.Seek(0, SeekOrigin.Begin); //not needed
                    var buffer = stream.ToArray();
                    double imageSizekb = Math.Round((buffer.Length / 1024d), 3);
                    logger.LogInformation("Image Size :{imageSizeMb}kb", imageSizekb);

                    // Get the SubFolder where the File should be Uploaded
                    string subFolder = BlobUrlHelper.GetSubFolderName(folderToUploadTo);
                    // Extra Folder for each Accessory Only
                    string extraSubFolderForAccessory =
                        folderToUploadTo is AccessoriesBlobSubFolder.AccessoriesPhotosFolder
                        ? fileName
                        : string.Empty;

                    BlobUrlCreationOptions urlOptions = new(
                        fileName,
                        BronzeMimeType.PngImage,
                        suffixToFileName, //The Ticks Of time or a Guid.ToString()
                        photoSize,
                        subFolder,
                        extraSubFolderForAccessory);
                    string relativeUrl = BlobUrlHelper.CreateBlobFileRelativeUrl(urlOptions);
                    var file = BronzeFile.CreateFile(buffer, BronzeMimeType.PngImage, urlHelper.AccessoriesContainerUrl, relativeUrl);

                    await UploadFileToBlobAsync(file, BlobContainerOption.AccessoriesBlobs);

                    imageRgba.Dispose();
                    return file.FileRelativePathWithExtension;
                }
            }
        }

        /// <summary>
        /// Uploads an Image to the specified Blob Container , win all its Variant Sizes
        /// </summary>
        /// <param name="containerOption">The Blob Container in which to save the Images</param>
        /// <param name="imageFilePath">The File Path to the Image</param>
        /// <param name="fileName">The fileName of the Image with which it will be saved to the Blob</param>
        /// <param name="matchSizeWithTransparency">Weather to Match any remaining size with Transparent pixels</param>
        /// <param name="cropPhotos">Weather to crop the Photo</param>
        /// <param name="precidingFolders">The Preciding folders before the FileName (without trailing '/')</param>
        /// <returns>The FilePath along with the extension of the FULL Sized Photo Only</returns>
        /// <exception cref="Exception">When something goes wrong</exception>
        public async Task<string> UploadImageWithAllSizesToBlobAsync(
            BlobContainerOption containerOption,
            string imageFilePath,
            string fileName,
            bool matchSizeWithTransparency,
            bool cropPhotos,
            IProgress<double>? progressPercentage = null,
            params string[] precidingFolders)
        {
            //Add a suffix to the Photo to uniquely identify it from the ticks of time
            string suffixToFileName = DateTime.Now.Ticks.ToString();

            //the variable where the full size Photo url will be saved for all the images
            string fullSizePhotoUrl = "";

            // Upload all Photo Sizes with the Same Ticks as Suffix
            double currentPercentage = 0;
            foreach (var size in availablePhotoSizes)
            {
                string url = await UploadImageToBlobAsync(containerOption, imageFilePath, size, fileName, suffixToFileName, matchSizeWithTransparency, cropPhotos, null,precidingFolders);
                if (size is BlobPhotoSize.FullSizePhoto) fullSizePhotoUrl = url;
                if (progressPercentage is not null)
                {
                    currentPercentage += percentageOfEachPhotoSize;
                    if (currentPercentage > 100) currentPercentage = 100;
                    progressPercentage?.Report(currentPercentage);
                }
            }
            if (string.IsNullOrWhiteSpace(fullSizePhotoUrl)) throw new Exception("Photos in all Sizes were Uploaded but the Operation Stopped Unexpectedly because a FullPhoto Url was not Returned...");
            return fullSizePhotoUrl;
        }

        /// <summary>
        /// Uploads an Image to one of the Blob Containers
        /// </summary>
        /// <param name="containerOption">The Blob Container to upload the Image to</param>
        /// <param name="imgFilePathToDisk">The Image File Path to Disk</param>
        /// <param name="photoSize">The Size of the Image to Upload</param>
        /// <param name="fileName">The FileName</param>
        /// <param name="suffixToFileName">Any Additional Suffix to the File Name (Usually Time.Ticks)</param>
        /// <param name="matchSizeWithTransparency">The Method resizes the Image, One of Height/Length cannot be resized if aspect ratio is kept.<para>This variable determines weather to fill the missing space with transparent pixels</para></param>
        /// <param name="cropPhoto">If True tries to Cut any White or Transparent Pixels from the image's bounding box , this happens before resizing</param>
        /// <param name="precidingFolders">The Folders of the Blob Container that precide the file name , No Trailing '/' are needed in the folders names</param>
        /// <returns>The Relative File Path along with the file extension of the saved item (meaning excludes the MainContainerUrl)</returns>
        /// <exception cref="EnumValueNotSupportedException">When the container option is not valid</exception>
        public async Task<string> UploadImageToBlobAsync(
            BlobContainerOption containerOption,
            string imgFilePathToDisk,
            BlobPhotoSize photoSize,
            string fileName,
            string suffixToFileName,
            bool matchSizeWithTransparency,
            bool cropPhoto,
            IProgress<double>? progressPercentage = null,
            params string[] precidingFolders)
        {
            logger.LogInformation("Loading Image from file : {path}", imgFilePathToDisk);
            using (Image image = await Image.LoadAsync(imgFilePathToDisk))
            {
                // Set the Image as a Clone of Rgba32 if not already
                Image<Rgba32> imageRgba = image is Image<Rgba32> imgrgb ? imgrgb : image.CloneAs<Rgba32>();

                logger.LogInformation("Loaded Image To Memory... Width x Height : {width}x{height}", imageRgba.Width, imageRgba.Height);
                progressPercentage?.Report(25);

                if (cropPhoto)
                {
                    // Crop
                    ImagesHelpers.CropWhiteOrTransparentBackground(imageRgba);
                    logger.LogInformation("Cropping Image ... New Dimensions : {width}x{height}", imageRgba.Width, imageRgba.Height);
                }


                //Resize
                var (width, height) = BlobUrlHelper.GetImageFixedSize(photoSize);
                ImagesHelpers.DownsizeImage(imageRgba, width, height, matchSizeWithTransparency);
                logger.LogInformation("Resizing Image ... New Dimensions : {width}x{height}", imageRgba.Width, imageRgba.Height);
                progressPercentage?.Report(50);

                //Create Upload File
                using (MemoryStream stream = new())
                {
                    // Save the Image to a Memory Stream
                    await imageRgba.SaveAsync(stream, new PngEncoder());
                    // Dispose of either the cloned or original image (calling this dispose again as if cloned the using statement will only dispose the nonClone)


                    // Create a Buffer where the stream places the data
                    // OLD WAY : await stream.ReadAsync(buffer); stream.Seek(0, SeekOrigin.Begin); //not needed
                    var buffer = stream.ToArray();
                    double imageSizekb = Math.Round((buffer.Length / 1024d), 3);
                    logger.LogInformation("Image Size :{imageSizeMb}kb", imageSizekb);
                    progressPercentage?.Report(75);

                    BlobUrlCreationOptions urlOptions = new(
                        fileName,
                        BronzeMimeType.PngImage,
                        suffixToFileName, //The Ticks Of time or a Guid.ToString()
                        photoSize,
                        precidingFolders);
                    string relativeUrl = BlobUrlHelper.CreateBlobFileRelativeUrl(urlOptions);
                    var containerUrl = containerOption switch
                    {
                        BlobContainerOption.AccessoriesBlobs => urlHelper.AccessoriesContainerUrl,
                        BlobContainerOption.CabinsBlobs => urlHelper.CabinsContainerUrl,
                        BlobContainerOption.MirrorsBlobs => urlHelper.MirrorsContainerUrl,
                        _ => throw new EnumValueNotSupportedException(containerOption),
                    };
                    var file = BronzeFile.CreateFile(buffer, BronzeMimeType.PngImage, containerUrl, relativeUrl);

                    await UploadFileToBlobAsync(file, containerOption);

                    imageRgba.Dispose();
                    progressPercentage?.Report(100);
                    return file.FileRelativePathWithExtension;
                }
            }
        }

        /// <summary>
        /// Uploads an Accessory Pdf Sheet to the Blob Container
        /// </summary>
        /// <param name="pdfFilePath">The Path to the Pdf File</param>
        /// <param name="fileName">The File Name</param>
        /// <returns>The Relative Path url of the Blob File inside the Blob COntainer (all the Path , except the Container Path)</returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> UploadAccessoryPdfSheetToBlob(string pdfFilePath, string fileName)
        {
            BlobUrlCreationOptions options = new(fileName,
                                                 BronzeMimeType.PdfFile,
                                                 DateTime.Now.Ticks.ToString(),
                                                 BlobPhotoSize.Undefined,
                                                 BlobUrlHelper.SheetsFolderName);

            string relativeUrl = BlobUrlHelper.CreateBlobFileRelativeUrl(options);

            // Read the File from the Disk
            using (FileStream fileStream = new(pdfFilePath, FileMode.Open, FileAccess.Read))
            {
                // Cannot directly add file to Memory  Stream , and FileStream does not have .ToArray Method
                // Put the File into a byte Array
                var buffer = new byte[fileStream.Length];
                int bytesRead = await fileStream.ReadAsync(buffer);
                if (bytesRead != buffer.Length)
                {
                    throw new Exception($"There was an Error while reading the File :{pdfFilePath}{Environment.NewLine}The Read Bytes : {bytesRead} where not equal to the Buffer Length :{buffer.Length}");
                }
                // Create the Bronze File 
                var file = BronzeFile.CreateFile(buffer, BronzeMimeType.PdfFile, urlHelper.AccessoriesContainerUrl, relativeUrl);
                // Upload it to Blob Storage
                string url = await UploadFileToBlobAsync(file, BlobContainerOption.AccessoriesBlobs);
                return url;
            }
        }

        /// <summary>
        /// Uploads a file to the Blob
        /// </summary>
        /// <param name="file"></param>
        /// <returns>The Relative Path of the File to the Blob , Containing its name as well (ex. Series/SomeImage.png)</returns>
        public async Task<string> UploadFileToBlobAsync(BronzeFile file, BlobContainerOption containerOption)
        {
            logger.LogInformation("Uploading File to Blob... , {fileName}", file.FileNameWithExtension);
            try
            {
                ThrowIfNotSupportedMIMEType(file);
                var container = SelectContainer(containerOption);

                // Create the certain uploaded Blob Client
                var blob = container.GetBlobClient(file.FileRelativePathWithExtension);

                // Pass the MIME Type so that it does not pass like an OctetStream
                BlobUploadOptions options = new() { HttpHeaders = new() { ContentType = file.MIMEType } };
                // Transform Data to Binary Data and Upload
                await blob.UploadAsync(new BinaryData(file.Data), options);

                logger.LogInformation("Upload Success , {file}", file.ShortDescription);
                logger.LogInformation("File Path : {filePath}", file.FileRelativePathWithExtension);
                return file.FileRelativePathWithExtension;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error While Uploading File to Blob... , {fileName}", file.FileNameWithExtension);
                throw;
            }
        }
        /// <summary>
        /// Deletes a Blob from the Blob Storage
        /// </summary>
        /// <param name="fileName">The Relative File Path to the Blob Container</param>
        /// <returns></returns>
        public async Task DeleteBlobAsync(string fileName, BlobContainerOption containerOption)
        {
            try
            {
                var container = SelectContainer(containerOption);
                var response = await container.DeleteBlobIfExistsAsync(fileName, DeleteSnapshotsOption.IncludeSnapshots)
                        ?? throw new Exception($"The Request Failed");

                if (response.Value is true)
                {
                    logger.LogInformation("Blob Deleted Successfully , {file}", fileName);
                }
                else
                {
                    string rawResponseStatus = response.GetRawResponse()?.Status.ToString() ?? "Raw Response was Undefined...";
                    throw new Exception($" Failed to Delete Blob , Received Response : {rawResponseStatus}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to Delete Blob");
                throw;
            }
        }
        /// <summary>
        /// Deletes all the Blobs under a certain Folder
        /// </summary>
        /// <param name="folderName">The Folder's name</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task DeleteBlobFolderAsync(string folderName, BlobContainerOption containerOption,IProgress<double>? progressPercentage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(folderName))
                {
                    throw new Exception("Provided folder Name was Null or Empty");
                }

                //Azure Blob storage uses the forward slash '/' to seperate folders and not the usual backslash '\'.
                //So any backlashes should be properly replaced as well as a forward slash should be in the end 
                folderName = folderName.Replace('\\', '/');
                if(folderName.EndsWith('/') is false)
                {
                    folderName += '/';
                }
                //Not working for some reason ...
                var container = SelectContainer(containerOption);

                List<BlobItem> blobs = new();
                await foreach (var blob in container.GetBlobsAsync(prefix:folderName))
                {
                    blobs.Add(blob);
                }
                
                if (!blobs.Any())
                {
                    throw new Exception($"No Blobs found for the Specified Folder {Environment.NewLine}: {folderName}");
                }
                else
                {
                    var count = blobs.Count;
                    double currentPercent = 0;
                    double percentIncrement = 1d / count * 100d;
                    foreach (var blob in blobs)
                    {
                        await DeleteBlobAsync(blob.Name, containerOption);
                        currentPercent += percentIncrement;
                        if (currentPercent > 100) currentPercent = 100;
                        progressPercentage?.Report(currentPercent);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Delete Blob Folder Operation Failed , Some of the Blobs might have already been Deleted...");
                throw;
            }
        }

        /// <summary>
        /// Gets the SAS uri to directly download a Blob
        /// </summary>
        /// <returns>The Absolute Uri to Download a certain Blob</returns>
        public async Task<string> GetBlobDownloadSasUriAsync(string fileName, BlobContainerOption containerOption)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    throw new Exception($"File Name was null or Empty...");
                }
                var container = SelectContainer(containerOption);
                var blob = container.GetBlobClient(fileName);
                if (await blob.ExistsAsync())
                {
                    BlobSasBuilder sasBuilder = new()
                    {
                        BlobContainerName = blob.BlobContainerName,   //Set the Container Name
                        BlobName = blob.Name,                         //Set the Blob Name
                        ContentDisposition = "attachment",            //Override Content Disposition to attachment (D/ls the blob directly)
                        ContentType = "application/octet-stream",     //Set the Content Type to octet Stream (D/ls the blob directly)
                        Resource = "b"                                //Sas Key is for a blob 'b' (otherwise for whole container 'c')
                    };
                    sasBuilder.SetPermissions(BlobSasPermissions.Read); //Sas Permission Key is only READ
                    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1); // Expire after 1h

                    //Generate the SAS URI , with which user can download blob
                    Uri sasUri = blob.GenerateSasUri(sasBuilder);
                    logger.LogInformation("Sas Uri for Download Successfully created for file {fileName}", fileName);
                    return sasUri.AbsoluteUri;
                }
                else
                {
                    throw new Exception("The Requested Blob for Download was not Found");
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed To retrieve SAS Uri to Download Blob with fileName {fileName}", fileName);
                throw;
            }
        }

        /// <summary>
        /// Returns the MetaData for all the Blobs stored in the Blob Container
        /// </summary>
        /// <returns></returns>
        public async IAsyncEnumerable<BronzeBlobItem> GetAllBlobsAsync(BlobContainerOption containerOption)
        {
            AsyncPageable<BlobItem> blobs;
            try
            {
                var container = SelectContainer(containerOption);
                blobs = container.GetBlobsAsync();
            }
            catch (Exception)
            {

                throw;
            }
            await foreach (var blob in blobs)
            {
                BronzeBlobItem item = new()
                {
                    ByteSize = blob.Properties.ContentLength ?? 0,
                    ContentType = blob.Properties.ContentType,
                    FileName = blob.Name,
                    LastModified = blob.Properties.LastModified ?? DateTimeOffset.Now
                };
                yield return item;
            }
        }

        /// <summary>
        /// Throws a Not supported MIME Type Exception , when the file trying to Upload is not of the Accepted Type
        /// </summary>
        /// <param name="file">The File to Upload</param>
        /// <exception cref="NotSupportedException"></exception>
        private void ThrowIfNotSupportedMIMEType(BronzeFile file)
        {
            if (!acceptedMIMETypes.Contains(file.MIMEType))
            {
                throw new NotSupportedException($"Not Supported File Type:{file.MIMEType} , Supported MIMETypes : {string.Join(" , ", acceptedMIMETypes)}");
            }
        }

        /// <summary>
        /// Returns the Container matching the option
        /// </summary>
        /// <param name="containerOption"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        private BlobContainerClient SelectContainer(BlobContainerOption containerOption)
        {
            return containerOption switch
            {
                BlobContainerOption.AccessoriesBlobs => accessoriesBlobsContainer,
                BlobContainerOption.CabinsBlobs => cabinsBlobsContainer,
                BlobContainerOption.MirrorsBlobs => mirrorsBlobsContainer,
                _ => throw new NotSupportedException($"Selected Container option is not Supported {containerOption}"),
            };
        }

    }

    public enum BlobContainerOption
    {
        Undefined = 0,
        AccessoriesBlobs = 1,
        CabinsBlobs = 2,
        MirrorsBlobs = 3,
    }
}

using CommonHelpers.HttpHelpers.FileModels;

namespace AzureBlobStorageLibrary
{
    public interface IBlobStorageRepository
    {
        /// <summary>
        /// Deletes a Blob from the Blob Storage
        /// </summary>
        /// <param name="fileName">The Relative File Path to the Blob Container</param>
        /// <returns></returns>
        Task DeleteBlobAsync(string fileName, BlobContainerOption containerOption);
        /// <summary>
        /// Deletes all the Blobs under a certain Folder
        /// </summary>
        /// <param name="folderName">The Folder's name</param>
        /// <param name="containerOption">The Container from which to delete the Folder From</param>
        /// <param name="progressPercentage">The Progress object reporting the percentage of completion</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        Task DeleteBlobFolderAsync(string folderName, BlobContainerOption containerOption,IProgress<double> progressPercentage);
        /// <summary>
        /// Returns the MetaData for all the Blobs stored in the Blob Container
        /// </summary>
        /// <returns></returns>
        IAsyncEnumerable<BronzeBlobItem> GetAllBlobsAsync(BlobContainerOption containerOption);
        /// <summary>
        /// Gets the SAS uri to directly download a Blob
        /// </summary>
        /// <returns>The Absolute Uri to Download a certain Blob</returns>
        Task<string> GetBlobDownloadSasUriAsync(string fileName, BlobContainerOption containerOption);
        /// <summary>
        /// Uploads a file to the Blob
        /// </summary>
        /// <param name="file"></param>
        /// <returns>The Relative Path of the File to the Blob , Containing its name as well (ex. Series/SomeImage.png)</returns>
        Task<string> UploadFileToBlobAsync(BronzeFile file, BlobContainerOption containerOption);

        /// <summary>
        /// Uploads an Accessory Image to the Blob Storage along with all its Size Variations
        /// </summary>
        /// <param name="imageFilePath">The Path of the Image</param>
        /// <param name="fileName">The FileName of the Image</param>
        /// <param name="subFolder">The SubFolder to Put the Image to</param>
        /// <param name="cropPhotos">Weather to Crop the Photos White and Transparent Pixels</param>
        /// <returns>The Relative Path of the Full Image</returns>
        Task<string> UploadAccessoryImagesToBlob(string imageFilePath,
                                                 string fileName,
                                                 AccessoriesBlobSubFolder subFolder,
                                                 bool cropPhotos);
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
        Task<string> UploadAccessoryImageToBlob(string imageFilePathToDisk,
                                                BlobPhotoSize photoSize,
                                                AccessoriesBlobSubFolder folderToUploadTo,
                                                string fileName,
                                                string suffixToFileName,
                                                bool matchSizeWithTransparency,
                                                bool cropPhoto);
        /// <summary>
        /// Uploads an Accessory Pdf Sheet to the Blob Container
        /// </summary>
        /// <param name="pdfFilePath">The Path to the Pdf File</param>
        /// <param name="fileName">The File Name</param>
        /// <returns>The Relative Path url of the Blob File inside the Blob COntainer (all the Path , except the Container Path)</returns>
        /// <exception cref="Exception"></exception>
        Task<string> UploadAccessoryPdfSheetToBlob(string pdfFilePath, string fileName);
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
        /// <param name="progressPercentage">The Progress object Sending the Percentage of Completion</param>
        /// <param name="precidingFolders">The Folders of the Blob Container that precide the file name , No Trailing '/' are needed in the folders names</param>
        /// <returns>The Relative File Path along with the file extension of the saved item (meaning excludes the MainContainerUrl)</returns>
        Task<string> UploadImageToBlobAsync(
            BlobContainerOption containerOption,
            string imgFilePathToDisk,
            BlobPhotoSize photoSize,
            string fileName,
            string suffixToFileName,
            bool matchSizeWithTransparency,
            bool cropPhoto,
            IProgress<double>? progressPercentage = null,
            params string[] precidingFolders);
        /// <summary>
        /// Uploads an Image to the specified Blob Container , win all its Variant Sizes
        /// </summary>
        /// <param name="containerOption">The Blob Container in which to save the Images</param>
        /// <param name="imageFilePath">The File Path to the Image</param>
        /// <param name="fileName">The fileName of the Image with which it will be saved to the Blob</param>
        /// <param name="matchSizeWithTransparency">Weather to Match any remaining size with Transparent pixels</param>
        /// <param name="cropPhotos">Weather to crop the Photo</param>
        /// <param name="progressPercentage">The Progress object Sending the Percentage of Completion</param>
        /// <param name="precidingFolders">The Preciding folders before the FileName (without trailing '/')</param>
        /// <returns>The FilePath along with the extension of the FULL Sized Photo Only</returns>
        /// <exception cref="Exception">When something goes wrong</exception>
        Task<string> UploadImageWithAllSizesToBlobAsync(
            BlobContainerOption containerOption,
            string imageFilePath,
            string fileName,
            bool matchSizeWithTransparency,
            bool cropPhotos,
            IProgress<double>? progressPercentage = null,
            params string[] precidingFolders);
    }
}
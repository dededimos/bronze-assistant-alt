using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelpers.HttpHelpers.FileModels
{
    public class BronzeFile
    {
        private byte[]? _data;
        /// <summary>
        /// The File Data into a byte Array
        /// </summary>
        public byte[] Data { get => _data is not null ? _data : Array.Empty<byte>(); set => _data = value; }

        /// <summary>
        /// The Url of the Container that the file resides in
        /// </summary>
        public string MainURL { get; set; } = "";

        /// <summary>
        /// The FilePath along with its extension without the Main Container URL (ex. /accessories/0/test.png )
        /// </summary>
        public string FileRelativePathWithExtension { get => $"{FileNameWithPrecedingFolders}.{FileExtension}"; }
        /// <summary>
        /// The Full Url of the File in the Container (ex. BlobContainerAddress/accessories/Series/loft.png)
        /// </summary>
        public string FullUrl { get => $"{MainURL}{FileRelativePathWithExtension}"; }

        /// <summary>
        /// The FileName and any preceding folders without the Extension and without the Main Url 
        /// (ex. /accessories/Loft/test )
        /// </summary>
        public string FileNameWithPrecedingFolders { get; set; } = "";

        /// <summary>
        /// Only the File Name along with its Extension
        /// </summary>
        public string FileNameWithExtension { get => FileRelativePathWithExtension[..FileRelativePathWithExtension.LastIndexOf('/')]; }

        /// <summary>
        /// The File Extension , ex. jpg/png/bmp e.t.c.
        /// </summary>
        public string FileExtension { get => GetFileExtensionString();}

        /// <summary>
        /// The MIME Type of the File , ex: image/png , image/jpg , application/pdf e.t.c.
        /// </summary>
        public string MIMEType { get; set; } = "";

        /// <summary>
        /// The FileType along with its size as a string
        /// </summary>
        public string ShortDescription { get => GetShortDescription(); }

        private string GetShortDescription()
        {
            var sizeInKb = $" -- Size: {Data.Length / 1024:0}kb";

            return MIMEType switch
            {
                "image/png" or "image/jpg" or "image/bmp" => $"Image File {sizeInKb}",
                "application/pdf" => $"PDF File {sizeInKb}",
                "text/plain" => $"Text File {sizeInKb}",
                "application/vnd.ms-excel" => $"Excel(old) File {sizeInKb}",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => $"ExcelX File {sizeInKb}",
                "text/csv" => $"CSV File {sizeInKb}",
                "application/json" => $"JSON File {sizeInKb}",
                _ => $"Unrecognized File Type {sizeInKb}",
            };
        }

        /// <summary>
        /// The Size in Bytes
        /// </summary>
        public int Size { get; set; }

        public BronzeFile(int size)
        {
            this.Size = size;
            this.Data = new byte[Size];
        }

        public BronzeFile(byte[] data)
        {
            this.Size = data.Length;
            this.Data = data;
        }

        /// <summary>
        /// Needs it otherwise cannot be Deserilized
        /// </summary>
        public BronzeFile()
        {

        }

        private string GetFileExtensionString()
        {
            return MIMEType switch
            {
                "image/png" => "png",
                "image/jpg" => "jpg",
                "image/bmp" => "bmp",
                "application/pdf" => "pdf",
                "text/plain" => "txt",
                "application/vnd.ms-excel" => "xls",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => "xlsx",
                "text/csv" => "csv",
                "application/json" => "json",
                _ => "bin",
            };
        }

        public static BronzeFile CreateFile(byte[] data , BronzeMimeType mimeType,string blobContainerUrl,string fileNamePathWithoutExtension)
        {
            return new(data)
            {
                MIMEType = mimeType.GetEnumDescription(),
                MainURL = blobContainerUrl,
                FileNameWithPrecedingFolders = fileNamePathWithoutExtension,
            };
        }
    }

    public enum BronzeMimeType
    {
        [Description("application/octet-stream")]
        OctetStream = 0,

        [Description("image/png")]
        PngImage = 1,

        [Description("image/jpeg")]
        JpgImage = 2,

        [Description("image/bmp")]
        BmpImage = 3,

        [Description("application/pdf")]
        PdfFile = 4,

        [Description("text/plain")]
        TextFile = 5,

        [Description("application/vnd.ms-excel")]
        ExcelDocument = 6,

        [Description("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        ExcelDocumentX = 7,

        [Description("text/csv")]
        CsvFile = 8,

        [Description("application/json")]
        JsonFile = 9,
    }

}

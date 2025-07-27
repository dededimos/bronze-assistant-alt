namespace AzureBlobStorageLibrary
{
    public class BronzeBlobItem
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public DateTimeOffset LastModified { get; set; }
        public long ByteSize { get; set; }
    }
}

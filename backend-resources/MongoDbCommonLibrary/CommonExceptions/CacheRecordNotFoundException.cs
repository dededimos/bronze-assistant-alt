namespace MongoDbCommonLibrary.CommonExceptions
{
    /// <summary>
    /// An Exception when a Cache record is not found
    /// </summary>
    public class CacheRecordNotFoundException : Exception
    {
        public CacheRecordNotFoundException() : base() { }

        public CacheRecordNotFoundException(string message) : base(message) { }

        public CacheRecordNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }

}

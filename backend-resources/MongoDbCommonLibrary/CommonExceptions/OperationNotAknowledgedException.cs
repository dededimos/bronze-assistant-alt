namespace MongoDbCommonLibrary.CommonExceptions
{
    /// <summary>
    /// An Exception when the server did not aknowledge the Operation (Connection Issue or some other reason)
    /// </summary>
    public class OperationNotAknowledgedException : Exception
    {
        public OperationNotAknowledgedException() : base() { }

        public OperationNotAknowledgedException(string message) : base(message) { }

        public OperationNotAknowledgedException(string message, Exception innerException) : base(message, innerException) { }
    }
}

namespace CommonInterfacesBronze
{
    public interface IWithMetadata
    {
        public Dictionary<string, string> Metadata { get; }

        void AddMetadata(string key, string metadata);
        void RemoveMetadata(string key);
        string? GetMetadata(string key);
    }
}

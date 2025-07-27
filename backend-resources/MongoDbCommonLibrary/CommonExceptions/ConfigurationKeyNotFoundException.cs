namespace MongoDbCommonLibrary.CommonExceptions
{
    public class ConfigurationKeyNotFoundException(string key) : Exception($"Key : {key} , not found in Configuration File"){}

}

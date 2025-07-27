using System.Text.Json.Serialization;

namespace GalaxyStockHelper
{
    public class MetadataInfo
    {
        [JsonPropertyName("HasNext")]
        public bool HasNext { get; set; }
    }

}

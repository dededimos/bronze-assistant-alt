using System.Text.Json.Serialization;

namespace GalaxyStockHelper
{
    public class GalaxyStockResponse
    {
        [JsonPropertyName("Items")]
        public List<GalaxyItemStockInfo> Items { get; set; } = new List<GalaxyItemStockInfo>();

        [JsonPropertyName("Metadata")]
        public List<MetadataInfo> Metadata { get; set; } = new List<MetadataInfo>();
    }

}

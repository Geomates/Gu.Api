using System.Text.Json.Serialization;

namespace Gu.PaftaBulucu.Data.Models
{
    public class SheetEntry
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lng")]
        public double Lng { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("scale")]
        public int Scale { get; set; }
    }
}
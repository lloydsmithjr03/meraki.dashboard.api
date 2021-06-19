using System.Text.Json.Serialization;

namespace Meraki.Dashboard.Api.Models
{
    public class TimespanQueryModel
    {
        [JsonPropertyName("t0")] 
        public string T0 { get; set; }
        [JsonPropertyName("t1")] 
        public string T1 { get; set; }
        [JsonPropertyName("timespan")] 
        public int? Timespan { get; set; }
    }
}
using System.Text.Json.Serialization;

namespace Meraki.Dashboard.Api.Organizations.Models
{
    public class ApiRequestOverview
    {
        [JsonPropertyName("responseCodeCounts")]
        public ResponseCodeCounts ResponseCodeCounts { get; set; }
    }

    public class ResponseCodeCounts
    {
        [JsonPropertyName("200")]
        public int? _200 { get; set; }
        [JsonPropertyName("400")]
        public int? _400 { get; set; }
        [JsonPropertyName("403")]
        public int? _403 { get; set; }
        [JsonPropertyName("404")]
        public int? _404 { get; set; }
        [JsonPropertyName("429")]
        public int? _429 { get; set; }
        [JsonPropertyName("500")]
        public int? _500 { get; set; }
        [JsonPropertyName("502")]
        public int? _502 { get; set; }
        [JsonPropertyName("503")]
        public int? _503 { get; set; }
        [JsonPropertyName("504")]
        public int? _504 { get; set; }
    }
    
}
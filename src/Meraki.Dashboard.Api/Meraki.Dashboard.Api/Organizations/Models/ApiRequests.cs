using System;
using System.Text.Json.Serialization;

namespace Meraki.Dashboard.Api.Organizations.Models
{
    public class ApiRequests
    {
        [JsonPropertyName("adminId")]
        public string AdminId { get; set; }
        [JsonPropertyName("method")]
        public string Method { get; set; }
        [JsonPropertyName("host")]
        public string Host { get; set; }
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("queryString")]
        public string QueryString { get; set; }
        [JsonPropertyName("userAgent")]
        public string UserAgent { get; set; }
        [JsonPropertyName("ts")]
        public DateTime Ts { get; set; }
        [JsonPropertyName("responseCode")]
        public int ResponseCode { get; set; }
        [JsonPropertyName("sourceIp")]
        public string SourceIp { get; set; }
    }
}
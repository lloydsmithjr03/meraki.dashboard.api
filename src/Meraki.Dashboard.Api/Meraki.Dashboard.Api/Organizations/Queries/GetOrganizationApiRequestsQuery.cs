using System.Text.Json.Serialization;
using Meraki.Dashboard.Api.Helpers;
using Meraki.Dashboard.Api.Models;

namespace Meraki.Dashboard.Api.Organizations.Queries
{
    public class GetOrganizationApiRequestsQuery: PaginatedQueryModel
    {
        [JsonPropertyName("adminId")]
        public string AdminId { get; set; }
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("method")]
        public string Method { get; set; }
        [JsonPropertyName("responseCode")]
        public int? ResponseCode { get; set; }
        [JsonPropertyName("sourceIp")]
        public string SourceIp { get; set; }
    }
}
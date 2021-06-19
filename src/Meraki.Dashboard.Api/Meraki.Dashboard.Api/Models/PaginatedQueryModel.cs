using System.Text.Json.Serialization;

namespace Meraki.Dashboard.Api.Models
{
    public class PaginatedQueryModel: TimespanQueryModel
    {
        [JsonPropertyName("perPage")] 
        public int? PerPage { get; set; }
        [JsonPropertyName("startingAfter")] 
        public string StartingAfter { get; set; }
        [JsonPropertyName("endingBefore")] 
        public string EndingBefore { get; set; }
    }
}
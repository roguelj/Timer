#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models
{
    public class Project : IKeyedEntity
    {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("companyId")]
        public int CompanyId { get; set; }

        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("subStatus")]
        public string SubStatus { get; set; }

        [JsonProperty("tagIds")]
        public int[] TagIds { get; set; }

        [JsonProperty("updateId")]
        public int? UpdateId { get; set; }

        [JsonProperty("timeBudgetId")]
        public int? TimeBudgetId { get; set; }
    }
}

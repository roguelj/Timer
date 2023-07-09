#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3
{
    public class Budget
    {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("projectId")]
        public int ProjectId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("capacityUsed")]
        public int CapacityUsed { get; set; }

        [JsonProperty("capacity")]
        public int Capacity { get; set; }

        [JsonProperty("isRepeating")]
        public bool IsRepeating { get; set; }

        [JsonProperty("repeatPeriod")]
        public int RepeatPeriod { get; set; }

        [JsonProperty("repeatUnit")]
        public string RepeatUnit { get; set; }

        [JsonProperty("sequenceNumber")]
        public int SequenceNumber { get; set; }

        [JsonProperty("startDateTime")]
        public DateTime StartDateTime { get; set; }

        [JsonProperty("endDateTime")]
        public object EndDateTime { get; set; }

        [JsonProperty("currencyCode")]
        public object CurrencyCode { get; set; }

        [JsonProperty("timelogType")]
        public string TimelogType { get; set; }

        [JsonProperty("expenseType")]
        public object ExpenseType { get; set; }

        [JsonProperty("defaultRate")]
        public object DefaultRate { get; set; }

        [JsonProperty("notificationIds")]
        public object[] NotificationIds { get; set; }

        [JsonProperty("notifications")]
        public object[] Notifications { get; set; }

        [JsonProperty("createdByUserId")]
        public int CreatedByUserId { get; set; }

        [JsonProperty("createdBy")]
        public int CreatedBy { get; set; }

        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedByUserId")]
        public int UpdatedByUserId { get; set; }

        [JsonProperty("updatedBy")]
        public int UpdatedBy { get; set; }

        [JsonProperty("dateUpdated")]
        public DateTime DateUpdated { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

}

#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models
{
    public class TimeLog :IKeyedEntity
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int Id { get; set; }

        public string Name { get => this.Description; }

        [JsonProperty("dateCreated", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DateCreated { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("timeLogged", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? TimeLogged { get; set; }

        [JsonProperty("billable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Billable { get; set; }

        [JsonProperty("deleted", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Deleted { get; set; }

        [JsonProperty("dateDeleted", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DateDeleted { get; set; }

        [JsonProperty("hasStartTime")]
        public bool HasStartTime { get; set; }

        [JsonProperty("dateEdited", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DateEdited { get; set; }

        [JsonProperty("deskTicketId", NullValueHandling = NullValueHandling.Ignore)]
        public int? DeskTicketId { get; set; }

        [JsonProperty("invoiceNumber", NullValueHandling = NullValueHandling.Ignore)]
        public string InvoiceNumber { get; set; }

        [JsonProperty("userId", NullValueHandling = NullValueHandling.Ignore)]
        public int? UserId { get; set; }

        [JsonProperty("taskId", NullValueHandling = NullValueHandling.Ignore)]
        public int? TaskId { get; set; }

        [JsonProperty("projectId", NullValueHandling = NullValueHandling.Ignore)]
        public int? ProjectId { get; set; }

        [JsonProperty("loggedByUserId", NullValueHandling = NullValueHandling.Ignore)]
        public int? LoggedByUserId { get; set; }

        [JsonProperty("deletedByUserId", NullValueHandling = NullValueHandling.Ignore)]
        public int? DeletedByUserId { get; set; }

        [JsonProperty("editedByUserId", NullValueHandling = NullValueHandling.Ignore)]
        public int? EditedByUserId { get; set; }

        [JsonProperty("taskIdPreMove", NullValueHandling = NullValueHandling.Ignore)]
        public int? TaskIdPreMove { get; set; }

        [JsonProperty("projectBillingInvoiceId", NullValueHandling = NullValueHandling.Ignore)]
        public int? ProjectBillingInvoiceId { get; set; }

        [JsonProperty("tagIds", NullValueHandling = NullValueHandling.Ignore)]
        public List<int> TagIds { get; set; }


        [JsonProperty("date", NullValueHandling = NullValueHandling.Ignore)]
        public string Date { get; set; }

        [JsonProperty("time", NullValueHandling = NullValueHandling.Ignore)]
        public string Time { get; set; }

        [JsonProperty("minutes", NullValueHandling = NullValueHandling.Ignore)]
        public int? Minutes { get; set; }

        [JsonProperty("hours", NullValueHandling = NullValueHandling.Ignore)]
        public int? Hours { get; set; }

        [JsonProperty("isBillable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsBillable { get; set; }

    }
}

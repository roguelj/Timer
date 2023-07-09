#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3
{
    public class TimeLog
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; }

        [JsonProperty("minutes")]
        public int Minutes { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("timeLogged")]
        public DateTime TimeLogged { get; set; }

        [JsonProperty("billable")]
        public bool Billable { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        [JsonProperty("dateDeleted")]
        public object DateDeleted { get; set; }

        [JsonProperty("hasStartTime")]
        public bool HasStartTime { get; set; }

        [JsonProperty("dateEdited")]
        public DateTime? DateEdited { get; set; }

        [JsonProperty("deskTicketId")]
        public object DeskTicketId { get; set; }

        [JsonProperty("invoiceNumber")]
        public string InvoiceNumber { get; set; }

        [JsonProperty("userId")]
        public int? UserId { get; set; }

        [JsonProperty("taskId")]
        public int? TaskId { get; set; }

        [JsonProperty("projectId")]
        public int? ProjectId { get; set; }

        [JsonProperty("loggedByUserId")]
        public int? LoggedByUserId { get; set; }

        [JsonProperty("deletedByUserId")]
        public object DeletedByUserId { get; set; }

        [JsonProperty("editedByUserId")]
        public int? EditedByUserId { get; set; }

        [JsonProperty("taskIdPreMove")]
        public int? TaskIdPreMove { get; set; }

        [JsonProperty("projectBillingInvoiceId")]
        public object ProjectBillingInvoiceId { get; set; }

        [JsonProperty("tagIds")]
        public int[] TagIds { get; set; }

    }
}

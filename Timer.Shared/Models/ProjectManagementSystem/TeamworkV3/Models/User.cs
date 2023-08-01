# nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models
{
    public class User
    {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("companyId")]
        public int CompanyId { get; set; }

        [JsonProperty("company")]
        public Company Company { get; set; }

        [JsonProperty("isAdmin")]
        public bool IsAdmin { get; set; }

        [JsonProperty("isClientUser")]
        public bool IsClientUser { get; set; }

        [JsonProperty("isServiceAccount")]
        public bool IsServiceAccount { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        [JsonProperty("avatarUrl")]
        public string AvatarUrl { get; set; }

        [JsonProperty("lengthOfDay")]
        public int LengthOfDay { get; set; }

        [JsonProperty("workingHoursId")]
        public int WorkingHoursId { get; set; }

        [JsonProperty("workingHour")]
        public SubType WorkingHour { get; set; }

        [JsonProperty("canAddProjects")]
        public bool CanAddProjects { get; set; }

        [JsonIgnore]
        public string Name
        {
            get => $"{FirstName} {LastName}";
        }

        [JsonIgnore]
        public string Colour { get; set; }
    }
}

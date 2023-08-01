#nullable disable

using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models
{

    public class ProjectTask
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("projectId")]
        public int ProjectId { get; set; }

        public ProjectTask() { }

        public ProjectTask(int id, string name, int projectId)
        {
            this.Id = id;
            this.Name = name;
            this.ProjectId = projectId;
        }

    }

}

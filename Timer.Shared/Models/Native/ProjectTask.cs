using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Timer.Shared.Models.Native
{
    public record ProjectTask
    {

        [JsonProperty("id")]
        public string? Id { get; set; }


        [JsonProperty("name")]
        public string? Name { get; set; }


        [JsonProperty("projectId")]
        public string? ProjectId { get; set; }


        [JsonProperty("taskListId")]
        public string? TaskListId { get; set; }


        [JsonProperty("taskListName")]
        public string? TaskListName { get; set; }


        [SetsRequiredMembers]
        public ProjectTask(string? id, string? name, string? projectId)
        {
            this.Id = id;
            this.Name = name;
            this.ProjectId = projectId;
        }


        [SetsRequiredMembers]
        public ProjectTask(int id, string? name, int projectId)
        {
            this.Id = id.ToString();
            this.Name = name;
            this.ProjectId = projectId.ToString();
        }
    }

}

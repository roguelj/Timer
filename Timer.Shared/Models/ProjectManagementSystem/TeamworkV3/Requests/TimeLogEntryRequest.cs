using Newtonsoft.Json;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Requests
{

    public class TimeLogEntryRequest
    {

        [JsonProperty("tags")]
        public List<Tag>? Tags { get; set; }

        [JsonProperty("timelog")]
        public TimeLogInput Timelog { get; set; }

        [JsonProperty("timelogOptions")]
        public TimeLogOptions? TimeLogOptions { get; set; }

        public TimeLogEntryRequest(DateTime startDateTime, DateTime endDateTime, string projectID, string? taskId, List<int>? tagIds, bool isBillable, string description)
        {

            this.Timelog = new TimeLogInput
            {
                Minutes = (endDateTime - startDateTime).Minutes,
                Hours = (endDateTime - startDateTime).Hours,
                ProjectId = int.Parse(projectID),
                TaskId = taskId is not null ? int.Parse(taskId) : (int?)null,
                TagIds = tagIds,
                Date = startDateTime.ToString("yyyy-MM-dd"),
                Time = startDateTime.ToString("HH:mm:ss"),
                HasStartTime = true,
                IsBillable = isBillable,
                Description = description
            };


        }

    }

}

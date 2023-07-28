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

        public TimeLogEntryRequest(DateTime startDateTime, DateTime endDateTime, int projectID, int? taskId, List<int>? tagIds, bool isBillable, string description)
        {

            Timelog = new TimeLogInput
            {
                Minutes = (endDateTime - startDateTime).Minutes,
                Hours = (endDateTime - startDateTime).Hours,
                ProjectId = projectID,
                TaskId = taskId,
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

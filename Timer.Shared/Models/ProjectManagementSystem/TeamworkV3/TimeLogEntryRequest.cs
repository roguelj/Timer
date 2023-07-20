using Newtonsoft.Json;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3
{

    public class TimeLogEntryRequest
    {

        [JsonProperty("tags")]
        public List<Tag>? Tags { get; set; }

        [JsonProperty("timelog")]
        public TimeLog Timelog { get; set; }

        [JsonProperty("timelogOptions")]
        public TimeLogOptions? TimeLogOptions { get; set; }

        public TimeLogEntryRequest(DateTime startDateTime, DateTime endDateTime, int projectID, int? taskId, List<int>? tagIds, bool isBillable, string description) 
        {

            this.Timelog = new TimeLog
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

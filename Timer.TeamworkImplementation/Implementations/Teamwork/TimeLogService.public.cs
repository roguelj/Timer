using Microsoft.Extensions.Logging;
using System.Text;
using Timer.Base.Resources;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV1;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;
using Timer.Base.Interfaces;
using Timer.TeamworkImplementation.Extensions;

namespace Timer.Shared.Services.Implementations.Teamwork
{
    public partial class TimeLogService : ITimeLogService
    {

        public async Task<DateTimeOffset?> GetEndTimeOfLastTimeLogEntryAsync(CancellationToken cancellationToken)
        {

            if (await this.Me(cancellationToken) is Person currentUser)
            {

                // get the last entry
                var entry = (await this.MyLastTimeEntry(currentUser.Id, cancellationToken));

                // get the datetime of the last entry, or the current datetime if there is none
                var lastTimeEntry = (entry?.TimeLogged ?? this.SystemClock.UtcNow);

                // get the duration of the last entry
                var durationMinutes = entry?.Minutes ?? 0;

                // adjust entry datetime to local
                lastTimeEntry = TimeZoneInfo.ConvertTimeFromUtc(lastTimeEntry.DateTime, TimeZoneInfo.Local);

                // return the end of the last time entry to the caller
                return lastTimeEntry.AddMinutes(durationMinutes);

            }
            else
            {
                this.Logger.LogError(LogMessages.UnknownUser);
                return null;

            }

        }


        public async Task<bool> LogTime(DateTime startDateTime, DateTime endDateTime, int projectId, int? taskId, List<int> tagIds, bool isBillable, string description, CancellationToken cancellationToken)
        {
            // create the client and add the auth
            var client = this.HttpClientFactory.CreateClient();
            var auth = this.IsBasicAuth() ? "Basic" : "Bearer";
            var atoken = this.AccessToken();
            var token = this.IsBasicAuth() ? Convert.ToBase64String(Encoding.ASCII.GetBytes($"{atoken}:")) : atoken;
            client.DefaultRequestHeaders.Add("Authorization", $"{auth} {token}");

            // create the request object
            var timeLogEntryRequest = new Models.ProjectManagementSystem.TeamworkV3.Requests.TimeLogEntryRequest(startDateTime, endDateTime, projectId, taskId, tagIds, isBillable, description);

            // determine the endpoint to hit
            var endpoint = taskId.HasValue ? $"{this.V3EndpointUrlBase}/tasks/{taskId}/time.json" : $"{this.V3EndpointUrlBase}/projects/{projectId}/time.json";

            // post the request
            var response = await client.PostAsJsonAsync(endpoint, timeLogEntryRequest, cancellationToken);
            await this.LogResponseContent(response, cancellationToken);

            // process response
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                this.Logger.LogError(LogMessages.IsSuccessStatusCodeFailure, response.StatusCode, "");
                this.Logger.LogError(LogMessages.LogTimeFailure);
                return false;
            }

        }


        public async Task<List<Timer.Base.Models.Project>?> Projects(CancellationToken cancellationToken)
        {
            return await this.GetAndPageProjects("projects.json", null, cancellationToken);
        }


        public async Task<List<Timer.Base.Models.Project>?> Projects(string searchCriteria, CancellationToken cancellationToken)
        {
            return await this.GetAndPageProjects("projects.json", $"searchTerm={searchCriteria}", cancellationToken);
        }

        public async Task<List<Timer.Base.Models.Project>?> Projects(bool starredOnly, CancellationToken cancellationToken)
        {
            return await this.GetAndPageProjects("projects.json", $"onlyStarredProjects={starredOnly}", cancellationToken);
        }

        public async Task<List<Timer.Base.Models.Project>?> RecentProjects(CancellationToken cancellationToken)
        {

            var myUserId = (await this.Me(cancellationToken)).Id;

            var recent = await this.GetOrSetRecentActivity(myUserId, cancellationToken);
            var recentItems = recent.SelectMany(sm => sm.Items);
            var itemLookup = recent.SelectMany(sm => sm.Included.Projects);

            return recentItems
                .GroupBy(gb => gb.ProjectId)
                .OrderByDescending(ob => ob.Sum(s => s.Minutes))
                .Select(s => new Base.Models.Project(s.Key!.Value, itemLookup.FirstOrDefault(f => f.Key == s.Key).Value.Name))
                .ToList();

        }


        public async Task<List<Timer.Base.Models.Tag>?> RecentTags(CancellationToken cancellationToken)
        {

            var myUserId = (await this.Me(cancellationToken)).Id;

            var recent = await this.GetOrSetRecentActivity(myUserId, cancellationToken);
            var recentItems = recent.SelectMany(sm => sm.Items);
            var itemLookup = recent.SelectMany(sm => sm.Included.Tags);


            // get a distinct list of tags
            var tags = recentItems
                        .Where(s => s.TagIds is not null && s.TagIds.Count != 0)
                        .SelectMany(sm => sm.TagIds)
                        .Distinct()
                        .ToList();


            // project to new List<int> and return to user
            return tags
                    .Select(s =>
                    {
                        var tag = itemLookup.FirstOrDefault(f => f.Key == s).Value;
                        return new Base.Models.Tag(s, tag.ProjectId, tag.Project.ToModelSubType(), tag.Name, tag.Colour);
                    })
                    .ToList();
        }


        public async Task<List<Timer.Base.Models.ProjectTask>?> RecentTasks(CancellationToken cancellationToken)
        {

            var myUserId = (await this.Me(cancellationToken)).Id;

            var recent = await this.GetOrSetRecentActivity(myUserId, cancellationToken);
            var recentItems = recent.SelectMany(sm => sm.Items);
            var taskLookup = recent.SelectMany(sm => sm.Included.Tasks);
            var taskListLookup = recent.SelectMany(sm => sm.Included.TaskLists);

            // create the projector to create a new ProjectTask from the IGrouping
            var projector = (IGrouping<int?, TimeLog> input) =>
            {
                var taskId = input.Key!.Value;

                var task = taskLookup.FirstOrDefault(f => f.Key == input.Key).Value;
                var taskList = taskListLookup.FirstOrDefault(f => f.Key == task.TaskListId);


                return new Base.Models.ProjectTask(taskId, task.Name, input.First().ProjectId!.Value, task.TaskListId, taskList.Value.Name);

            };

            return recentItems
                    .Where(w=> w.TaskId.HasValue).ToList()              // materialise the IEnumerable so that we don't get any NullReferenceException
                    .GroupBy(gb => gb.TaskId)
                    .OrderByDescending(ob => ob.Sum(s => s.Minutes))    // most recent first
                    .Select(projector)
                    .ToList();

        }


        public async Task<List<Timer.Base.Models.Tag>?> Tags(CancellationToken cancellationToken)
        {
            return await this.GetAndPageTags("tags.json", null, cancellationToken);
        }


        public async Task<List<Timer.Base.Models.Tag>?> Tags(string searchCriteria, CancellationToken cancellationToken)
        {
            return await this.GetAndPageTags("tag.json", $"searchTerm={searchCriteria}", cancellationToken);
        }


        public async Task<List<Timer.Base.Models.ProjectTask>?> Tasks(CancellationToken cancellationToken)
        {
            return await this.GetAndPageTasks("tasks.json", null, cancellationToken);
        }

        public async Task<List<Timer.Base.Models.ProjectTask>?> Tasks(int projectId, CancellationToken cancellationToken)
        {
            return await this.GetAndPageTasks("tasks.json", $"projectIds={projectId}", cancellationToken);
        }

        public async Task<List<Timer.Base.Models.ProjectTask>?> Tasks(string searchCriteria, CancellationToken cancellationToken)
        {
            return await this.GetAndPageTasks("tasks.json", $"searchTerm={searchCriteria}", cancellationToken);
        }

        public async Task<List<Timer.Base.Models.ProjectTask>?> MyTasks(int projectId, CancellationToken cancellationToken)
        {
            var myUserId = (await this.Me(cancellationToken)).Id;
            return await this.GetAndPageTasks($"projects/{projectId}/tasks.json", $"responsiblePartyIds={myUserId}", cancellationToken);
        }
    }

}

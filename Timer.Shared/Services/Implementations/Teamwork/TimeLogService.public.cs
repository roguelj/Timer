using System.Text;
using Timer.Shared.Models;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV1;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;
using Timer.Shared.Resources;
using Timer.Shared.Services.Interfaces;
using LogRes = Timer.Shared.Resources.LogMessages;

namespace Timer.Shared.Services.Implementations.Teamwork
{
    internal partial class TimeLogService : ITimeLogService
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
                this.Logger.Error(LogRes.UnknownUser);
                return null;

            }

        }


        public async Task<bool> LogTime(DateTime startDateTime, DateTime endDateTime, int projectId, int? taskId, List<int> tagIds, bool isBillable, string description, CancellationToken cancellationToken)
        {
            // create the client and add the auth
            var client = HttpClientFactory.CreateClient();
            var auth = IsBasicAuth() ? "Basic" : "Bearer";
            var atoken = await AccessToken();
            var token = IsBasicAuth() ? Convert.ToBase64String(Encoding.ASCII.GetBytes($"{atoken}:")) : atoken;
            client.DefaultRequestHeaders.Add("Authorization", $"{auth} {token}");

            // create the request object
            var timeLogEntryRequest = new Models.ProjectManagementSystem.TeamworkV3.Requests.TimeLogEntryRequest(startDateTime, endDateTime, projectId, taskId, tagIds, isBillable, description);

            // determine the endpoint to hit
            var endpoint = taskId.HasValue ? $"{V3EndpointUrlBase}/tasks/{taskId}/time.json" : $"{V3EndpointUrlBase}/projects/{projectId}/time.json";

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
                this.Logger.Error(LogMessages.IsSuccessStatusCodeFailure, response.StatusCode, "");
                this.Logger.Error(LogMessages.LogTimeFailure);
                return false;
            }

        }


        public async Task<List<KeyedEntity>?> Projects(CancellationToken cancellationToken)
        {
            return await this.GetAndPageV3Endpoint<Project, ProjectResponse<Project>>("projects.json", null, cancellationToken);
        }


        public async Task<List<KeyedEntity>?> Projects(string searchCriteria, CancellationToken cancellationToken)
        {
            return await this.GetAndPageV3Endpoint<Project, ProjectResponse<Project>>("projects.json", $"searchTerm={searchCriteria}", cancellationToken);
        }


        public async Task<List<KeyedEntity>?> RecentProjects(CancellationToken cancellationToken)
        {

            var myUserId = (await this.Me(cancellationToken)).Id;

            var recent = await this.GetOrSetRecentActivity(myUserId, cancellationToken);
            var recentItems = recent.SelectMany(sm => sm.Items);
            var itemLookup = recent.SelectMany(sm => sm.Included.Projects);

            return recentItems
                .GroupBy(gb => gb.ProjectId)
                .OrderByDescending(ob => ob.Sum(s => s.Minutes))
                .Select(s => new KeyedEntity(s.Key!.Value, itemLookup.FirstOrDefault(f => f.Key == s.Key).Value.Name))
                .ToList();

        }


        public async Task<List<KeyedEntity>?> RecentTags(CancellationToken cancellationToken)
        {

            var myUserId = (await this.Me(cancellationToken)).Id;

            var recent = await this.GetOrSetRecentActivity(myUserId, cancellationToken);
            var recentItems = recent.SelectMany(sm => sm.Items);
            var itemLookup = recent.SelectMany(sm => sm.Included.Tags);


            // get a distinct list of tags
            var tags = recentItems
                        .Where(s => s.TagIds is not null && s.TagIds.Any())
                        .SelectMany(sm => sm.TagIds)
                        .Distinct()
                        .ToList();


            // project to new List<KeyedEntity> and return to user
            return tags
                    .Select(s =>
                    {
                        var tag = itemLookup.FirstOrDefault(f => f.Key == s).Value;
                        return new KeyedEntity(s, tag.Name, tag.Colour);
                    })
                    .ToList();
        }


        public async Task<List<KeyedEntity>?> RecentTasks(CancellationToken cancellationToken)
        {

            var myUserId = (await this.Me(cancellationToken)).Id;

            var recent = await this.GetOrSetRecentActivity(myUserId, cancellationToken);
            var recentItems = recent.SelectMany(sm => sm.Items);
            var itemLookup = recent.SelectMany(sm => sm.Included.Tasks);

            return recentItems
                .Where(w=> w.TaskId.HasValue).ToList()
                .GroupBy(gb => gb.TaskId)
                .OrderByDescending(ob => ob.Sum(s => s.Minutes))
                .Select(s => new KeyedEntity(s.Key!.Value, itemLookup.FirstOrDefault(f => f.Key == s.Key).Value.Name))
                .ToList();

        }


        public async Task<List<KeyedEntity>?> Tags(CancellationToken cancellationToken)
        {
            return await this.GetAndPageV3Endpoint<Tag, TagResponse<Tag>>("tags.json", null, cancellationToken);
        }


        public async Task<List<KeyedEntity>?> Tags(string searchCriteria, CancellationToken cancellationToken)
        {
            return await this.GetAndPageV3Endpoint<Tag, TagResponse<Tag>>("tag.json", $"searchTerm={searchCriteria}", cancellationToken);
        }


        public async Task<List<KeyedEntity>?> Tasks(CancellationToken cancellationToken)
        {
            return await this.GetAndPageV3Endpoint<Models.ProjectManagementSystem.TeamworkV3.Models.Task, TaskResponse<Models.ProjectManagementSystem.TeamworkV3.Models.Task>>("tasks.json", null, cancellationToken);
        }


        public async Task<List<KeyedEntity>?> Tasks(string searchCriteria, CancellationToken cancellationToken)
        {
            return await this.GetAndPageV3Endpoint<Models.ProjectManagementSystem.TeamworkV3.Models.Task, TaskResponse<Models.ProjectManagementSystem.TeamworkV3.Models.Task>>("tasks.json", $"searchTerm={searchCriteria}", cancellationToken);
        }

    }

}

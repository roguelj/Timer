using DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer.Shared.Extensions;
using Timer.Shared.Models;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV1;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3;
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
            var timeLogEntryRequest = new TimeLogEntryRequest(startDateTime, endDateTime, projectId, taskId, tagIds, isBillable, description);

            // determine the endpoint to hit
            var endpoint = taskId.HasValue ? $"{V3EndpointUrlBase}/tasks/{taskId}/time.json" : $"{V3EndpointUrlBase}/projects/{projectId}/time.json";

            // post the request
            var response = await client.PostAsJsonAsync(endpoint, timeLogEntryRequest, cancellationToken);


#if DEBUG
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            Logger.Verbose(responseContent);
#endif

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                Logger.Error(LogMessages.IsSuccessStatusCodeFailure, response.StatusCode, "");
                Logger.Error(LogMessages.LogTimeFailure);
                return false;
            }

        }

        public async Task<List<KeyedEntity>?> Projects(CancellationToken cancellationToken)
        {
            return await this.GetAndPage<Project, ProjectResponse<Project>>("projects.json", cancellationToken);
        }

        public Task<List<KeyedEntity>?> Projects(string searchCriteria, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<KeyedEntity>?> RecentProjects(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<KeyedEntity>?> RecentTags(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<KeyedEntity>?> RecentTasks(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<KeyedEntity>?> Tags(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<KeyedEntity>?> Tags(string searchCriteria, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<KeyedEntity>?> Tasks(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<KeyedEntity>?> Tasks(string searchCriteria, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text;
using Timer.Shared.Application;
using Timer.Shared.Extensions;
using Timer.Shared.Models;
using Timer.Shared.Models.Options;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV1;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3;
using Timer.Shared.Services.Interfaces;
using Timer.Shared.ViewModels;
using LogMessages = Timer.Shared.Resources.LogMessages;


namespace Timer.Shared.Services.Implementations.tmp
{
    internal class TeamworkTimeLogService1
    {





        // ---------------------------------
        // V3 method implementations






        private async Task<List<KeyedEntity>?> MyRecentProjects(int myUserId, CancellationToken cancellationToken)
        {

            var client = HttpClientFactory.CreateClient();
            var endPoint = await RequestMyRecentActivityAsync(myUserId);
            var response = await client.SendAsync(endPoint, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var teResponse = await response.Content.ReadAsAsync<TimeLogResponse>();
                var timeLogs = teResponse.TimeLogs;

                // return a new list of KeyedEntity, by grouping the time log responses on the project id,
                // sorting by total time (sum) descending, and projecting to a new List<KeyedEntity>
                return timeLogs
                        .GroupBy(gb => gb.ProjectId)
                        .OrderByDescending(ob => ob.Sum(s => s.Minutes))
                        .Select(s => new KeyedEntity(s.Key!.Value, teResponse.Included.Projects.FirstOrDefault(f => f.Key == s.Key).Value.Name))
                        .ToList();


            }
            else
            {

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Logger.Error(responseContent);

                return null;

            }

        }

        private async Task<List<KeyedEntity>?> MyRecentTasks(int myUserId, CancellationToken cancellationToken)
        {

            var client = HttpClientFactory.CreateClient();
            var endPoint = await RequestMyRecentActivityAsync(myUserId);
            var response = await client.SendAsync(endPoint, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var teResponse = await response.Content.ReadAsAsync<TimeLogResponse>();
                var timeLogs = teResponse.TimeLogs;

                // return a new list of KeyedEntity, by grouping the time log responses on the task id,
                // sorting by total time (sum) descending, and projecting to a new List<KeyedEntity>
                return timeLogs
                        .Where(s => s.TaskId.HasValue)
                        .ToList()
                        .GroupBy(gb => (gb.TaskId, gb.ProjectId))
                        .OrderByDescending(ob => ob.Sum(s => s.Minutes))
                        .Select(s => new KeyedEntity(s.Key!.TaskId.Value, teResponse.Included.Tasks.FirstOrDefault(f => f.Key == s.Key.TaskId.Value).Value.Name, s.Key.ProjectId.Value))
                        .ToList();
            }
            else
            {

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Logger.Error(responseContent);

                return null;

            }

        }

        private async Task<List<KeyedEntity>?> MyRecentTags(int myUserId, CancellationToken cancellationToken)
        {

            var client = HttpClientFactory.CreateClient();
            var endPoint = await RequestMyRecentActivityAsync(myUserId);
            var response = await client.SendAsync(endPoint, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var teResponse = await response.Content.ReadAsAsync<TimeLogResponse>();
                var timeLogs = teResponse.TimeLogs;

                // get a distinct list of tags
                var tags = timeLogs
                            .Where(s => s.TagIds is not null && s.TagIds.Any())
                            .SelectMany(sm => sm.TagIds)
                            .Distinct()
                            .ToList();


                // project to new List<KeyedEntity> and return to user
                return tags
                        .Select(s =>
                        {
                            var tag = teResponse.Included.Tags.FirstOrDefault(f => f.Key == s).Value;
                            return new KeyedEntity(s, tag.Name, tag.Color);
                        })
                        .ToList();
            }
            else
            {

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Logger.Error(responseContent);

                return null;

            }

        }



    }

}
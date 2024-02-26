using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using System.Xml.Linq;
using Timer.Shared.Application;
using Timer.Shared.Extensions;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Responses;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Responses.ResponseMeta;
using Timer.Shared.Resources;

namespace Timer.Shared.Services.Implementations.Teamwork
{
    internal partial class TimeLogService
    {

        private async Task<TimeLog?> MyLastTimeEntry(int myUserId, CancellationToken cancellationToken)
        {

            var client = this.HttpClientFactory.CreateClient();

            // build the query parameter string
            List<string> queryParameters = new List<string>
            {
                $"assignedToUserIds={myUserId}",
                "pageSize=500",
                "orderBy=date",
                "orderMode=desc"
            };

            var request = new HttpRequestMessage(HttpMethod.Get, $"{this.V3EndpointUrlBase}/time.json?{string.Join("&", queryParameters)}");
            request.AddAuthenticationHeader(this.IsBasicAuth(), this.AccessToken());

            var response = await client.SendAsync(request, cancellationToken);
            await this.LogResponseContent(response, cancellationToken);

            // process the response
            if (response.IsSuccessStatusCode)
            {
                var teResponse = await response.Content.ReadAsAsync<TimeLogResponse>();
                return teResponse.Items.OrderByDescending(o => o.TimeLogged.Value.AddMinutes(o.Minutes ?? 0)).FirstOrDefault();
            }
            else
            {

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                this.Logger.Error(responseContent);

                return null;

            }

        }


        private async Task<List<Project>> GetAndPageProjects(string path, string? parameters, CancellationToken cancellationToken) 
        {

            var client = this.HttpClientFactory.CreateClient();
            var result = new List<Project>();
            var shouldExit = false;
            var page = 1;

            const int pageSize = 100;

            do
            {

                var parameterList = new List<string>
                {
                    $"page={page}",
                    $"pageSize={pageSize}",
                    "fields[projects]=id,name",
                    parameters ?? string.Empty
                };

                var parameterString = string.Join('&', parameterList);


                // create the HttpRequestMessage
                var request = new HttpRequestMessage(HttpMethod.Get, $"{this.V3EndpointUrlBase}/{path}?{parameterString}");
                request.AddAuthenticationHeader(this.IsBasicAuth(), this.AccessToken());


                // perform the request, get the response
                var httpResponse = await client.SendAsync(request, cancellationToken);


                // process the response
                if (httpResponse.IsSuccessStatusCode)
                {

                    var response = await httpResponse.Content.ReadAsAsync<ProjectResponse>();
                    result.AddRange(response.Items);

                    if (response.Meta.Page.HasMore)
                    {
                        page++;
                    }
                    else
                    {
                        shouldExit = true;
                    }

                }
                else
                {
                    shouldExit = true;
                    this.Logger.Error(LogMessages.IsSuccessStatusCodeFailure, httpResponse.StatusCode, "GetAndPage");
                }

            } while (!shouldExit);

            return result;
           
        }


        private async Task<List<ProjectTask>> GetAndPageTasks(string path, string? parameters, CancellationToken cancellationToken)
        {

            var client = this.HttpClientFactory.CreateClient();
            var result = new List<ProjectTask>();
            var taskLists = new List<TaskList>();

            var shouldExit = false;
            var page = 1;

            const int pageSize = 100;

            do
            {
                var parameterList = new List<string>
                {
                    $"page={page}",
                    $"pageSize={pageSize}",
                    "include=tasklists",
                    "fields[tasklists]=id,projectId,name",
                    "fields[tasks]=id,name,taskListId",
                    parameters ?? string.Empty
                };

                var parameterString = string.Join('&', parameterList);

                // create the HttpRequestMessage
                var request = new HttpRequestMessage(HttpMethod.Get, $"{this.V3EndpointUrlBase}/{path}?{parameterString}");
                request.AddAuthenticationHeader(this.IsBasicAuth(), this.AccessToken());


                // perform the request, get the response
                var httpResponse = await client.SendAsync(request, cancellationToken);


                // process the response
                if (httpResponse.IsSuccessStatusCode)
                {

                    var response = await httpResponse.Content.ReadAsAsync<TaskResponse>();
                    result.AddRange(response.Items);
                    taskLists.AddRange(response.Included.TaskLists.Select(s => s.Value));

                    if (response.Meta.Page.HasMore)
                    {
                        page++;
                    }
                    else
                    {
                        shouldExit = true;
                    }

                }
                else
                {
                    shouldExit = true;
                    this.Logger.Error(LogMessages.IsSuccessStatusCodeFailure, httpResponse.StatusCode, "GetAndPage");
                }

            } while (!shouldExit);


            // inject the projectId and task list name into each task, using the data from the included items
            foreach(var projectTask in result)
            {
                var taskList = taskLists.First(f => f.Id.Equals(projectTask.TaskListId));
                projectTask.ProjectId = taskList.ProjectId;
                projectTask.TaskListName = taskList.Name;
            }

            return result;

        }


        private async Task<List<Tag>> GetAndPageTags(string path, string? parameters, CancellationToken cancellationToken)
        {

            var client = this.HttpClientFactory.CreateClient();
            var result = new List<Tag>();
            var shouldExit = false;
            var page = 1;

            const int pageSize = 100;

            do
            {

                // create the additional parameters string, if any
                var additionalParameters = parameters is null ? string.Empty : $"&{parameters}";


                // create the HttpRequestMessage
                var request = new HttpRequestMessage(HttpMethod.Get, $"{this.V3EndpointUrlBase}/{path}?page={page}&pageSize={pageSize}{additionalParameters}");
                request.AddAuthenticationHeader(this.IsBasicAuth(), this.AccessToken());


                // perform the request, get the response
                var httpResponse = await client.SendAsync(request, cancellationToken);
 

                // process the response
                if (httpResponse.IsSuccessStatusCode)
                {

                    var response = await httpResponse.Content.ReadAsAsync<TagResponse>();
                    result.AddRange(response.Items);

                    if (response.Meta.Page.HasMore)
                    {
                        page++;
                    }
                    else
                    {
                        shouldExit = true;
                    }

                }
                else
                {
                    shouldExit = true;
                    this.Logger.Error(LogMessages.IsSuccessStatusCodeFailure, httpResponse.StatusCode, "GetAndPage");
                }

            } while (!shouldExit);

            return result;

        }


        private async Task<List<TimeLogResponse>> GetAndPageV3TimeLogResponse(string path, string? parameters, CancellationToken cancellationToken) 
        {

            var client = this.HttpClientFactory.CreateClient();
            var result = new List<TimeLogResponse>();
            var shouldExit = false;
            var page = 1;

            const int pageSize = 100;

            do
            {

                // create the additional parameters string, if any
                var additionalParameters = parameters is null ? string.Empty : $"&{parameters}";


                // create the HttpRequestMessage
                var request = new HttpRequestMessage(HttpMethod.Get, $"{this.V3EndpointUrlBase}/{path}?page={page}&pageSize={pageSize}{additionalParameters}");
                request.AddAuthenticationHeader(this.IsBasicAuth(), this.AccessToken());


                // perform the request, get the response
                var httpResponse = await client.SendAsync(request, cancellationToken);
                await this.LogResponseContent(httpResponse, cancellationToken);


                // process the response
                if (httpResponse.IsSuccessStatusCode)
                {

                    var response = await httpResponse.Content.ReadAsAsync<TimeLogResponse>();
                    result.Add(response);

                    if (response.Meta.Page.HasMore)
                    {
                        page++;
                    }
                    else
                    {
                        shouldExit = true;
                    }

                }
                else
                {
                    shouldExit = true;
                    this.Logger.Error(LogMessages.IsSuccessStatusCodeFailure, httpResponse.StatusCode, "GetAndPage");
                }

            } while (!shouldExit);

            return result;

        }


        private async Task<List<TimeLogResponse>> GetOrSetRecentActivity(int myUserId, CancellationToken cancellationToken)
        {

            // cache this response for a short amount of time
            // given that it hits the API 3 times for each 'Log Time' request, it's probably sensible

            if (!this.MemoryCache.TryGetValue(CacheKeyConstants.ITIMELOG_SERVICE_TEAMWORK_RECENT_ACTIVITY_KEY, out List<TimeLogResponse>? cacheValue))
            {

                var daysToConsiderRecent = Math.Max(0, this.Options.Value.DaysToConsiderRecent ?? 14);
                var startDate = this.SystemClock.UtcNow.AddDays(-daysToConsiderRecent);
                var endDate = this.SystemClock.UtcNow;

                // build the query parameter string
                List<string> queryParameters = new List<string>
                {
                    $"assignedToUserIds={myUserId}",
                    "pageSize=500",
                    "orderBy=date",
                    "orderMode=desc",
                    $"startDate={startDate:yyyy-MM-dd}",
                    $"endDate={endDate:yyyy-MM-dd}",
                    "include=projects,tasks,tags,tasks.tasklists",
                    "fields[tasklists]=id,projectId,name"
                };

                cacheValue = await this.GetAndPageV3TimeLogResponse("time.json", string.Join("&", queryParameters), cancellationToken);
                this.MemoryCache.Set(CacheKeyConstants.ITIMELOG_SERVICE_TEAMWORK_RECENT_ACTIVITY_KEY, cacheValue, RecentActivityMemoryCacheEntryOptions);
            }

            return cacheValue;

        }

    }

}

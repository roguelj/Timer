using Microsoft.Extensions.Caching.Memory;
using Timer.Shared.Application;
using Timer.Shared.Extensions;
using Timer.Shared.Models;
using Timer.Shared.Models.Options;
using Timer.Shared.Models.ProjectManagementSystem;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV1;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Requests;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Responses;
using Timer.Shared.Resources;

namespace Timer.Shared.Services.Implementations.Teamwork
{
    internal partial class TimeLogService
    {


        private async Task<TimeLog?> MyLastTimeEntry(int myUserId, CancellationToken cancellationToken)
        {

            var client = HttpClientFactory.CreateClient();

            // build the query parameter string
            List<string> queryParameters = new List<string>
            {
                $"assignedToUserIds={myUserId}",
                "pageSize=500",
                "orderBy=date",
                "orderMode=desc"
            };

            var request = new HttpRequestMessage(HttpMethod.Get, $"{V3EndpointUrlBase}/time.json?{string.Join("&", queryParameters)}");
            request.AddAuthenticationHeader(this.IsBasicAuth(), await this.AccessToken());

            var response = await client.SendAsync(request, cancellationToken);
            await this.LogResponseContent(response, cancellationToken);

            // process the response
            if (response.IsSuccessStatusCode)
            {
                var teResponse = await response.Content.ReadAsAsync<TimeLogResponse<TimeLog>>();
                return teResponse.Items.OrderByDescending(o => o.TimeLogged.Value.AddMinutes(o.Minutes ?? 0)).FirstOrDefault();
            }
            else
            {

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                this.Logger.Error(responseContent);

                return null;

            }

        }


        private async Task<List<KeyedEntity>> GetAndPageV3Endpoint<TEntity, TEntityResponse>(string path, string? parameters, CancellationToken cancellationToken) where TEntityResponse : IKeyedEntityResponse<TEntity> where TEntity : IKeyedEntity
        {

            var client = HttpClientFactory.CreateClient();
            var result = new List<KeyedEntity>();
            var shouldExit = false;
            var page = 1;

            const int pageSize = 100;

            do
            {

                // create the additional parameters string, if any
                var additionalParameters = parameters is null ? string.Empty : $"&{parameters}";


                // create the HttpRequestMessage
                var request = new HttpRequestMessage(HttpMethod.Get, $"{V3EndpointUrlBase}/{path}?page={page}&pageSize={pageSize}{additionalParameters}");
                request.AddAuthenticationHeader(this.IsBasicAuth(), await this.AccessToken());


                // perform the request, get the response
                var httpResponse = await client.SendAsync(request, cancellationToken);
                var content = await this.LogResponseContent(httpResponse, cancellationToken);


                // process the response
                if (httpResponse.IsSuccessStatusCode)
                {

                    var response = await httpResponse.Content.ReadAsAsync<TEntityResponse>();
                    result.AddRange(response.Items.Select(s => s.ToKeyedEntity()));

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

        private async Task<List<TimeLogResponse<TimeLog>>> GetAndPageV3TimeLogResponse(string path, string? parameters, CancellationToken cancellationToken) 
        {

            var client = HttpClientFactory.CreateClient();
            var result = new List<TimeLogResponse<TimeLog>>();
            var shouldExit = false;
            var page = 1;

            const int pageSize = 100;

            do
            {

                // create the additional parameters string, if any
                var additionalParameters = parameters is null ? string.Empty : $"&{parameters}";


                // create the HttpRequestMessage
                var request = new HttpRequestMessage(HttpMethod.Get, $"{V3EndpointUrlBase}/{path}?page={page}&pageSize={pageSize}{additionalParameters}");
                request.AddAuthenticationHeader(this.IsBasicAuth(), await this.AccessToken());


                // perform the request, get the response
                var httpResponse = await client.SendAsync(request, cancellationToken);
                await this.LogResponseContent(httpResponse, cancellationToken);


                // process the response
                if (httpResponse.IsSuccessStatusCode)
                {

                    var response = await httpResponse.Content.ReadAsAsync<TimeLogResponse<TimeLog>>();
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


        private async Task<List<TimeLogResponse<TimeLog>>> GetOrSetRecentActivity(int myUserId, CancellationToken cancellationToken)
        {

            // cache this response for a short amount of time
            // given that it hits the API 3 times for each 'Log Time' request, it's probably sensible

            if (!this.MemoryCache.TryGetValue(CacheKeyConstants.ITIMELOG_SERVICE_TEAMWORK_RECENT_ACTIVITY_KEY, out List<TimeLogResponse<TimeLog>>? cacheValue))
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
                    "include=projects,tasks,tags"
                };

                var responses = await this.GetAndPageV3TimeLogResponse("time.json", string.Join("&", queryParameters), cancellationToken);
                cacheValue = responses;
                this.MemoryCache.Set(CacheKeyConstants.ITIMELOG_SERVICE_TEAMWORK_RECENT_ACTIVITY_KEY, cacheValue);
            }

            return cacheValue;

        }

    }

}

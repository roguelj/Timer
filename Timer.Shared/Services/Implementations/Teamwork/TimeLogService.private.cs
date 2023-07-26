using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer.Shared.Application;
using Timer.Shared.Extensions;
using Timer.Shared.Models;
using Timer.Shared.Models.Options;
using Timer.Shared.Models.ProjectManagementSystem;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV1;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3;
using Timer.Shared.Resources;

namespace Timer.Shared.Services.Implementations.Teamwork
{
    internal partial class TimeLogService
    {

        private async Task<Person?> Me(CancellationToken cancellationToken)
        {

            if (!MemoryCache.TryGetValue(CacheKeyConstants.ITIMELOG_SERVICE_TEAMWORK_ME_KEY, out Person? cacheValue))
            {

                var client = HttpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Get, $"{V1EndpointUrlBase}/me.json");
                request.AddAuthenticationHeader(IsBasicAuth(), await AccessToken());

                var response = await client.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode && await response.Content.ReadAsStringAsync(cancellationToken) is string responseContent)
                {

                    // log
                    Logger.Verbose(responseContent);


                    // deserialise the response
                    var userDetailResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<UserDetailResponse>(responseContent);
                    if (userDetailResponse != null)
                    {
                        cacheValue = userDetailResponse.Person;
                        MemoryCache.Set(CacheKeyConstants.ITIMELOG_SERVICE_TEAMWORK_ME_KEY, cacheValue, MeMemoryCacheEntryOptions);
                    }

                }
                else if (!response.IsSuccessStatusCode)
                {
                    Logger.Error(LogMessages.IsSuccessStatusCodeFailure, response.StatusCode, "Me");
                }
                else
                {
                    Logger.Error(LogMessages.ResponseReadFailure, "Me");
                }

            }

            return cacheValue;

        }


        private async Task<string> AccessToken()
        {

            if (Options.Value is null)
            {
                throw new ArgumentNullException("options value is null");
            }
            else
            {
                return Options.Value.ApiKey;
            }

        }

        private bool IsBasicAuth()
        {
            return Options.Value is TeamworkOptions twOptions && twOptions.AuthType.Equals("basic", StringComparison.InvariantCultureIgnoreCase);
        }


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

            if (response.IsSuccessStatusCode)
            {
                var teResponse = await response.Content.ReadAsAsync<TimeLogResponse>();
                return teResponse.TimeLogs.OrderByDescending(o => o.TimeLogged.Value.AddMinutes(o.Minutes ?? 0)).FirstOrDefault();
            }
            else
            {

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Logger.Error(responseContent);

                return null;

            }

        }


        // request generators
        private async Task<HttpRequestMessage> RequestTasksAsync()
        {

            // build the query parameter string
            List<string> queryParameters = new List<string>
            {
                // add other parameters
                $"includeCompletedTasks=false",
                $"pageSize={250}"
            };


            // construct the full endpoint url and request
            var fullQuery = $"{V1EndpointUrlBase}/tasks.json?{string.Join("&", queryParameters)}";
            var request = new HttpRequestMessage(HttpMethod.Get, fullQuery);
            request.AddAuthenticationHeader(this.IsBasicAuth(), await this.AccessToken());

            return request;

        }


        private async Task<HttpRequestMessage> BuildV3GetRequest(string uri, int page, int pageSize, string additionalParameters = "")
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{V3EndpointUrlBase}/{uri}?page={page}&pageSize={pageSize}&{additionalParameters}".TrimEnd('&'));
            request.AddAuthenticationHeader(this.IsBasicAuth(), await this.AccessToken());
            return request;
        }


        private async Task<List<KeyedEntity>> GetAndPage<TEntity, TEntityResponse>(string uriPathFragment, CancellationToken cancellationToken) where TEntityResponse : IKeyedEntityResponse<TEntity> where TEntity : IKeyedEntity
        {

            var client = HttpClientFactory.CreateClient();
            var result = new List<KeyedEntity>();
            var shouldExit = false;
            var page = 1;

            const int pageSize = 100;

            HttpRequestMessage? request = default;

            do
            {

                request = await this.BuildV3GetRequest(uriPathFragment, page, pageSize);
                var httpResponse = await client.SendAsync(request, cancellationToken);

                if (httpResponse.IsSuccessStatusCode)
                {

                    var response = await httpResponse.Content.ReadAsAsync<TEntityResponse>();
                    result.AddRange(response.Items.Select(s => s.ToKeyedEntity()));

                    if (response.Meta.Page.HasMore)
                    {
                        page += pageSize;
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
    }

}

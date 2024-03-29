﻿using Microsoft.Extensions.Caching.Memory;
using Timer.Shared.Application;
using Timer.Shared.Extensions;
using Timer.Shared.Models.Options;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV1;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Requests;
using Timer.Shared.Resources;

namespace Timer.Shared.Services.Implementations.Teamwork
{
    internal partial class TimeLogService
    {

        private async Task<Person> Me(CancellationToken cancellationToken)
        {

            if (!this.MemoryCache.TryGetValue(CacheKeyConstants.ITIMELOG_SERVICE_TEAMWORK_ME_KEY, out Person? cacheValue))
            {

                var client = this.HttpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Get, $"{this.V1EndpointUrlBase}/me.json");
                request.AddAuthenticationHeader(this.IsBasicAuth(), this.AccessToken());

                var response = await client.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode && await response.Content.ReadAsStringAsync(cancellationToken) is string responseContent)
                {

                    await this.LogResponseContent(response, cancellationToken);

                    // deserialise the response
                    var userDetailResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<UserDetailResponse>(responseContent);
                    if (userDetailResponse != null)
                    {
                        cacheValue = userDetailResponse.Person;
                        this.MemoryCache.Set(CacheKeyConstants.ITIMELOG_SERVICE_TEAMWORK_ME_KEY, cacheValue, MeMemoryCacheEntryOptions);
                    }

                }
                else if (!response.IsSuccessStatusCode)
                {
                    this.Logger.Error(LogMessages.IsSuccessStatusCodeFailure, response.StatusCode, "Me");
                }
                else
                {
                    this.Logger.Error(LogMessages.ResponseReadFailure, "Me");
                }

            }

            return cacheValue;

        }

        private string AccessToken()
        {

            if (this.Options.Value is null)
            {
                throw new ArgumentNullException("options value is null");
            }
            else
            {
                return this.Options.Value.ApiKey;
            }

        }

        private bool IsBasicAuth()
        {
            return this.Options.Value is TeamworkOptions twOptions && twOptions.AuthType.Equals("basic", StringComparison.InvariantCultureIgnoreCase);
        }

    }
}

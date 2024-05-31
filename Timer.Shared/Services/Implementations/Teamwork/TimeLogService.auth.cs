using Microsoft.Extensions.Caching.Memory;
using Timer.Shared.Application;
using Timer.Shared.Constants;
using Timer.Shared.Extensions;
using Timer.Shared.Models.Options;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV1;

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

                    this.Logger.Information(LogMessage.HTTP_STATUS_CODE, response.StatusCode);

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
                    this.Logger.Error(LogMessage.HTTP_STATUS_CODE, response.StatusCode);
                }
                else
                {
                    this.Logger.Error(LogMessage.RESPONSE_READ_FAILURE);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer.Shared.Extensions;

namespace Timer.Shared.Services.Implementations
{
    public partial class TeamworkTimeLogService
    {


        private async Task<HttpRequestMessage> RequestMyRecentActivityAsync(int myUserId)
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

            var request = new HttpRequestMessage(HttpMethod.Get, $"{V3EndpointUrlBase}/time.json?{string.Join("&", queryParameters)}");
            request.AddAuthenticationHeader(this.IsBasicAuth(), await this.AccessToken());

            return request;

        }





    }
}

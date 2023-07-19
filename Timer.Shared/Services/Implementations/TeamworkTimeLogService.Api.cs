using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text;
using Timer.Shared.Application;
using Timer.Shared.Extensions;
using Timer.Shared.Models;
using Timer.Shared.Models.ProjectManagementSystem;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV1;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3;
using Timer.Shared.Services.Interfaces;
using Timer.Shared.ViewModels;
using LogMessages = Timer.Shared.Resources.LogMessages;


namespace Timer.Shared.Services.Implementations
{
    internal partial class TeamworkTimeLogService
    {

        // injected services
        private ILogger Logger { get; }
        private IHttpClientFactory HttpClientFactory { get; }
        private IOptions<TeamworkOptions> Options { get; }
        private IMemoryCache MemoryCache { get; }
        private ISystemClock SystemClock { get; }


        // endpoint properties
        private string V1EndpointUrlBase { get => $"{this.Options.Value.TeamworkEndPointUrlBase}"; }
        private string V3EndpointUrlBase { get => $"{this.Options.Value.TeamworkEndPointUrlBase}/projects/api/v3"; }


        private MemoryCacheEntryOptions MeMemoryCacheEntryOptions { get; } = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(8));


        // constructor
        public TeamworkTimeLogService(ILogger logger, IHttpClientFactory httpClientFactory, IOptions<TeamworkOptions> options, IMemoryCache memoryCache, ISystemClock systemClock)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.MemoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            this.SystemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        }

        public async Task<string> AccessToken()
        {

            if(this.Options.Value is null)
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



        // ---------------------------------
        // HttpRequestMessage factories

  
        private async Task<HttpRequestMessage> RequestMeAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{V1EndpointUrlBase}/me.json");
            request.AddAuthenticationHeader(this.IsBasicAuth(), await this.AccessToken());
            return request;
        }

        private async Task<HttpRequestMessage> RequestTimeEntriesAsync(ApiQueryParameters apiQueryParameters, int page, int pageSize)
        {

            // build the query parameter string
            List<string> queryParameters = new List<string>();


            // add selected users and projects
            if (apiQueryParameters.SelectedUsers.Any()) queryParameters.Add($"assignedToUserIds={string.Join(",", apiQueryParameters.SelectedUsers.Select(s => s.Id))}");
            if (apiQueryParameters.SelectedProjects.Any()) queryParameters.Add($"projectIds={string.Join(",", apiQueryParameters.SelectedProjects.Select(s => s.Id))}");


            // add selected tags
            if (apiQueryParameters.SelectedProjectTags.Count > 0) queryParameters.Add($"projectTagIds={string.Join(",", apiQueryParameters.SelectedProjectTags.Select(s => s.Id))}");
            if (apiQueryParameters.SelectedTaskTags.Count > 0) queryParameters.Add($"taskTagIds={string.Join(",", apiQueryParameters.SelectedTaskTags.Select(s => s.Id))}");
            if (apiQueryParameters.SelectedTimeTags.Count > 0) queryParameters.Add($"tagIds={string.Join(",", apiQueryParameters.SelectedTimeTags.Select(s => s.Id))}");


            // add other parameters
            queryParameters.Add($"startDate={apiQueryParameters.From:yyyy-MM-dd}");
            queryParameters.Add($"endDate={apiQueryParameters.To:yyyy-MM-dd}");
            queryParameters.Add($"pageSize={pageSize}");
            queryParameters.Add($"page={page}");
            queryParameters.Add($"include=tags,tasks,users,projects,tasks.tasklists&fields[tasklists]=name,id&fields[tasks]=id,name,taskListId");


            var request = new HttpRequestMessage(HttpMethod.Get, $"{V3EndpointUrlBase}/time.json?{string.Join("&", queryParameters)}");
            request.AddAuthenticationHeader(this.IsBasicAuth(), await this.AccessToken());

            return request;

        }

        private async Task<HttpRequestMessage> RequestMyLastTimeEntryAsync(int myUserId)
        {

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

            return request;

        }

        private async Task<HttpRequestMessage> RequestMyRecentActivityAsync(int myUserId)
        {

            var daysToConsiderRecent = Math.Max(0,this.Options.Value.DaysToConsiderRecent ?? 14);
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

        private async Task<HttpRequestMessage> RequestTasksAsync(ApiQueryParameters apiQueryParameters, bool includeTasksWithNoDueDate)
        {

            // build the query parameter string
            List<string> queryParameters = new List<string>();

            // add selected users and projects
            if (apiQueryParameters.SelectedUsers.Any()) queryParameters.Add($"assignedToUserIds={string.Join(",", apiQueryParameters.SelectedUsers.Select(s => s.Id))}");
            if (apiQueryParameters.SelectedProjects.Any()) queryParameters.Add($"projectIds={string.Join(",", apiQueryParameters.SelectedProjects.Select(s => s.Id))}");


            // add selected tags
            if (apiQueryParameters.SelectedProjectTags.Count > 0) queryParameters.Add($"projectTagIds={string.Join(",", apiQueryParameters.SelectedProjectTags.Select(s => s.Id))}");
            if (apiQueryParameters.SelectedTaskTags.Count > 0) queryParameters.Add($"tag-ids={string.Join(",", apiQueryParameters.SelectedTaskTags.Select(s => s.Id))}");
            if (apiQueryParameters.SelectedTimeTags.Count > 0) queryParameters.Add($"tagIds={string.Join(",", apiQueryParameters.SelectedTimeTags.Select(s => s.Id))}");


            // add other parameters
            queryParameters.Add($"includeTasksWithoutDueDates={includeTasksWithNoDueDate.ToString().ToLowerInvariant()}");
            queryParameters.Add($"includeCompletedTasks=false");
            queryParameters.Add($"pageSize={250}");


            // construct the full endpoint url and request
            var fullQuery = $"{V1EndpointUrlBase}/tasks.json?{string.Join("&", queryParameters)}";
            var request = new HttpRequestMessage(HttpMethod.Get, fullQuery);
            request.AddAuthenticationHeader(this.IsBasicAuth(), await this.AccessToken());

            return request;

        }

        private async Task<HttpRequestMessage> RequestPeopleAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{V3EndpointUrlBase}/people.json");
            request.AddAuthenticationHeader(this.IsBasicAuth(), await this.AccessToken());
            return request;
        }

        private async Task<HttpRequestMessage> RequestStarredProjectsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{V3EndpointUrlBase}/projects/starred.json?include=projectUpdates,tags,projectBudgets");
            request.AddAuthenticationHeader(this.IsBasicAuth(), await this.AccessToken());
            return request;
        }

        private async Task<HttpRequestMessage> RequestTagsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{V3EndpointUrlBase}/tags.json?pageSize=250");
            request.AddAuthenticationHeader(this.IsBasicAuth(), await this.AccessToken());
            return request;
        }


        // ---------------------------------
        // V1 method implementations
        private async Task<TasksResponse?> Tasks(string token, ApiQueryParameters apiQueryParameters, bool includeTasksWithNoDueDate, CancellationToken cancellationToken)
        {

            var client = this.HttpClientFactory.CreateClient();
            var response = await client.SendAsync(await RequestTasksAsync(apiQueryParameters, includeTasksWithNoDueDate), cancellationToken);

            if (response.IsSuccessStatusCode)
            {

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                this.Logger.Verbose(responseContent);

                return Newtonsoft.Json.JsonConvert.DeserializeObject<TasksResponse>(responseContent);

            }
            else
            {
                return null;
            }
        }

        private async Task<Person?> Me(CancellationToken cancellationToken)
        {

            if (!this.MemoryCache.TryGetValue(CacheKeyConstants.ITIMELOG_SERVICE_TEAMWORK_ME_KEY, out Person? cacheValue))
            {

                var client = this.HttpClientFactory.CreateClient();
                var response = await client.SendAsync(await RequestMeAsync(), cancellationToken);

                if (response.IsSuccessStatusCode && await response.Content.ReadAsStringAsync(cancellationToken) is string responseContent)
                {

                    // log
                    this.Logger.Verbose(responseContent);


                    // deserialise the response
                    var userDetailResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<UserDetailResponse>(responseContent);
                    if (userDetailResponse != null)
                    {
                        cacheValue = userDetailResponse.Person;
                        this.MemoryCache.Set(CacheKeyConstants.ITIMELOG_SERVICE_TEAMWORK_ME_KEY, cacheValue, this.MeMemoryCacheEntryOptions);
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


        // ---------------------------------
        // V3 method implementations
        private async Task<Token?> ObtainToken(string temporaryToken, CancellationToken cancellationToken)
        {

            var options = this.Options.Value;
            var client = this.HttpClientFactory.CreateClient();


            // build the request
            var tokenRequest = new Models.ProjectManagementSystem.TeamworkV3.TokenRequest
            {
                Code = temporaryToken,
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret,
                RedirectUri = options.RedirectUri
            };


            // make the call and interpret the response
            var response = await client.PostAsJsonAsync(options.TokenRequestUrl, tokenRequest, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<Models.ProjectManagementSystem.TeamworkV3.Token>();
            }
            else
            {
                return null;
            }

        }

        private async Task<List<TimeLog>> TimeEntries(string token, ApiQueryParameters apiQueryParameters, CancellationToken cancellationToken)
        {

            int page = 1;
            int pageSize = 500;
            bool finished = false;
            var timeLogs = new List<TimeLog>();

            while (!finished)
            {

                var client = this.HttpClientFactory.CreateClient();
                var endPoint = await RequestTimeEntriesAsync(apiQueryParameters, page, pageSize);
                var response = await client.SendAsync(endPoint, cancellationToken);

                this.Logger.Verbose(endPoint.RequestUri!.AbsoluteUri);

                if (response.IsSuccessStatusCode)
                {
                    var teResponse = await response.Content.ReadAsAsync<TimeLogResponse>();
                    timeLogs.AddRange(teResponse.TimeLogs);

                    if (!teResponse.Meta.Page.HasMore)
                    {
                        finished = true;
                    }
                    page += 1; 

                }
                else
                {

                    var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    this.Logger.Error(responseContent);
                    finished = true;
                }
            }

            return timeLogs;

        }

        private async Task<UserResponse?> Users(string token, CancellationToken cancellationToken)
        {

            var client = this.HttpClientFactory.CreateClient();
            var response = await client.SendAsync(await RequestPeopleAsync(), cancellationToken);

            if (response.IsSuccessStatusCode)
            {

#if DEBUG
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                this.Logger.Verbose(responseContent);
#endif

                return await response.Content.ReadAsAsync<Models.ProjectManagementSystem.TeamworkV3.UserResponse>();

            }
            else
            {
                return null;
            }
        }

        async Task<List<KeyedEntity>?> ITimeLogService.AllProjectsAsync(CancellationToken cancellationToken)
        {

            // TODO: paginate

            var client = this.HttpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{V3EndpointUrlBase}/projects.json?pageSize=250");
            request.AddAuthenticationHeader(this.IsBasicAuth(), await this.AccessToken());

            var response = await client.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {

#if DEBUG
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                this.Logger.Verbose(responseContent);
#endif

                var projectsResponse = await response.Content.ReadAsAsync<ProjectResponse>();

                return projectsResponse.Projects.Select(s => s.ToKeyedEntity()).ToList();

            }
            else
            {
                return null;
            }

        }

        private async Task<ProjectResponse?> StarredProjects(string token, CancellationToken cancellationToken)
        {

            var client = this.HttpClientFactory.CreateClient();
            var response = await client.SendAsync(await RequestStarredProjectsAsync(), cancellationToken);

            if (response.IsSuccessStatusCode)
            {

#if DEBUG
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                this.Logger.Verbose(responseContent);
#endif

                return await response.Content.ReadAsAsync<Models.ProjectManagementSystem.TeamworkV3.ProjectResponse>();

            }
            else
            {
                return null;
            }

        }

        private async Task<TagResponse?> Tags(string token, CancellationToken cancellationToken)
        {

            var client = this.HttpClientFactory.CreateClient();
            var response = await client.SendAsync(await this.RequestTagsAsync(), cancellationToken);

            if (response.IsSuccessStatusCode)
            {

#if DEBUG
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                this.Logger.Verbose(responseContent);
#endif

                return await response.Content.ReadAsAsync<Models.ProjectManagementSystem.TeamworkV3.TagResponse>();

            }
            else
            {
                return null;
            }

        }

        private async Task<TimeLog?> MyLastTimeEntry(int myUserId, CancellationToken cancellationToken)
        {

            var client = this.HttpClientFactory.CreateClient();
            var endPoint = await RequestMyLastTimeEntryAsync( myUserId);
            var response = await client.SendAsync(endPoint, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var teResponse = await response.Content.ReadAsAsync<TimeLogResponse>();
                return teResponse.TimeLogs.OrderByDescending(o => o.TimeLogged.Value.AddMinutes(o.Minutes ?? 0)).FirstOrDefault();  
            }
            else
            {

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                this.Logger.Error(responseContent);

                return null;

            }

        }

        private async Task<List<KeyedEntity>?> MyRecentProjects(int myUserId, CancellationToken cancellationToken)
        {

            var client = this.HttpClientFactory.CreateClient();
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
                        .OrderByDescending(ob=> ob.Sum(s=> s.Minutes))
                        .Select(s => new KeyedEntity(s.Key!.Value, teResponse.Included.Projects.FirstOrDefault(f => f.Key == s.Key).Value.Name))
                        .ToList(); 


            }
            else
            {

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                this.Logger.Error(responseContent);

                return null;

            }

        }

        private async Task<List<KeyedEntity>?> MyRecentTasks(int myUserId, CancellationToken cancellationToken)
        {

            var client = this.HttpClientFactory.CreateClient();
            var endPoint = await RequestMyRecentActivityAsync(myUserId);
            var response = await client.SendAsync(endPoint, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var teResponse = await response.Content.ReadAsAsync<TimeLogResponse>();
                var timeLogs = teResponse.TimeLogs;

                // return a new list of KeyedEntity, by grouping the time log responses on the task id,
                // sorting by total time (sum) descending, and projecting to a new List<KeyedEntity>
                return timeLogs
                        .Where(s=> s.TaskId.HasValue)
                        .ToList()
                        .GroupBy(gb => (gb.TaskId, gb.ProjectId))
                        .OrderByDescending(ob => ob.Sum(s => s.Minutes))
                        .Select(s => new KeyedEntity(s.Key!.TaskId.Value, teResponse.Included.Tasks.FirstOrDefault(f => f.Key == s.Key.TaskId.Value).Value.Name, s.Key.ProjectId.Value))
                        .ToList();
            }
            else
            {

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                this.Logger.Error(responseContent);

                return null;

            }

        }

        private async Task<List<KeyedEntity>?> MyRecentTags(int myUserId, CancellationToken cancellationToken)
        {

            var client = this.HttpClientFactory.CreateClient();
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
                this.Logger.Error(responseContent);

                return null;

            }

        }

        public async Task<bool> CreateTimeEntry(DateTime startDateTime, DateTime endDateTime, int projectId, int? taskId, List<int>? tags, CancellationToken cancellationToken)
        {

            // create the client and add the auth
            var client = this.HttpClientFactory.CreateClient();
            var auth = this.IsBasicAuth() ? "Basic" : "Bearer";
            var atoken = await this.AccessToken();
            var token = this.IsBasicAuth() ? Convert.ToBase64String(Encoding.ASCII.GetBytes($"{atoken}:")) : atoken;
            client.DefaultRequestHeaders.Add("Authorization", $"{auth} {token}");

            // create the request object
            var timeLogEntryRequest = new TimeLogEntryRequest(startDateTime, endDateTime, projectId, taskId, tags);

            // determine the endpoint to hit
            var endpoint = taskId.HasValue ? $"{V3EndpointUrlBase}/tasks/{taskId}/time.json" : $"{V3EndpointUrlBase}/projects/{projectId}/time.json";

            // post the request
            var response = await client.PostAsJsonAsync(endpoint, timeLogEntryRequest, cancellationToken);


#if DEBUG
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            this.Logger.Verbose(responseContent);
#endif

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

    }

}
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Security.Policy;
using System.Text;
using Timer.Shared.Extensions;
using Timer.Shared.Models.ProjectManagementSystem;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV1;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV3;
using Timer.Shared.ViewModels;


namespace Timer.Shared.Services.Implementations
{
    internal partial class TeamworkTimeLogService
    {

        // injected services
        private ILogger Logger { get; }
        private IHttpClientFactory HttpClientFactory { get; }
        private IOptions<TeamworkOptions> Options { get; }
        private IMemoryCache MemoryCache { get; }


        // endpoint properties
        private string V1EndpointUrlBase { get => $"{this.Options.Value.TeamworkEndPointUrlBase}"; }
        private string V3EndpointUrlBase { get => $"{this.Options.Value.TeamworkEndPointUrlBase}/projects/api/v3"; }


        // constructor
        public TeamworkTimeLogService(ILogger logger, IHttpClientFactory httpClientFactory, IOptions<TeamworkOptions> options, IMemoryCache memoryCache)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.MemoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
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
            return true; // TODO: fix this
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
                "pageSize=1",
                "orderBy=date",
                "orderMode=desc"
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

        private async Task<HttpRequestMessage> RequestProjectsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{V3EndpointUrlBase}/projects.json?pageSize=250");
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

        private async Task<HttpRequestMessage> RequestInsertTimeEntryForProjectAsync(int projectId)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{V3EndpointUrlBase}/projects/{projectId}/time.json");
            request.AddAuthenticationHeader(this.IsBasicAuth(), await this.AccessToken());
            return request;
        }

        private async Task<HttpRequestMessage> RequestInsertTimeEntryForTaskAsync(int taskId)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{V3EndpointUrlBase}/tasks/{taskId}/time.json");
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

#if DEBUG
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                this.Logger.Verbose(responseContent);
#endif

                return Newtonsoft.Json.JsonConvert.DeserializeObject<TasksResponse>(responseContent);

            }
            else
            {
                return null;
            }
        }

        private async Task<Person?> Me(CancellationToken cancellationToken)
        {

            var cacheKey = "itimelogservice:teamwork:me"; // TODO: eliminate magic string

            if (!this.MemoryCache.TryGetValue(cacheKey, out Person? cacheValue))
            {

                var client = this.HttpClientFactory.CreateClient();
                var response = await client.SendAsync(await RequestMeAsync(), cancellationToken);

                if (response.IsSuccessStatusCode)
                {

#if DEBUG
                    var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    this.Logger.Verbose(responseContent);
#endif

                    cacheValue = Newtonsoft.Json.JsonConvert.DeserializeObject<UserDetailResponse>(responseContent).Person;

                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(8));

                    this.MemoryCache.Set(cacheKey, cacheValue, cacheEntryOptions);

                }
                else
                {
                    cacheValue = null;
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

        private async Task<ProjectResponse?> Projects(string token, CancellationToken cancellationToken)
        {

            var client = this.HttpClientFactory.CreateClient();
            var response = await client.SendAsync(await RequestProjectsAsync(), cancellationToken);

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
                return teResponse.TimeLogs.OrderByDescending(o => o.TimeLogged).FirstOrDefault();  
            }
            else
            {

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                this.Logger.Error(responseContent);

                return null;

            }

        }



        //        public async Task CreateTimeEntryForProject(string token, int minutes, DateTime start, int projectId, CancellationToken cancellationToken)
        //        {

        //            var client = this.HttpClientFactory.CreateClient();
        //            var response = await client.PostAsJsonAsync(RequestInsertTimeEntryForProject(token, projectId), cancellationToken);

        //            if (response.IsSuccessStatusCode)
        //            {



        //#if DEBUG
        //                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        //                this.Logger.Verbose(responseContent);
        //#endif

        //                return await response.Content.ReadAsAsync<Models.ProjectManagementSystem.TeamworkV3.TagResponse>();

        //            }
        //            else
        //            {
        //                return null;
        //            }


        //        }

    }

}
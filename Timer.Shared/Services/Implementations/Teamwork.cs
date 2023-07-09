using Microsoft.Extensions.Options;
using Serilog;
using Timer.Shared.Models.ProjectManagementSystem;
using Timer.Shared.Models.ProjectManagementSystem.TeamworkV1;
using Timer.Shared.Services.Interfaces;
using Timer.Shared.ViewModels;


namespace Timer.Shared.Services.Implementations
{
    internal class Teamwork : ITimeLogService
    {

        // injected services
        private ILogger Logger { get; }
        private IHttpClientFactory HttpClientFactory { get; }
        private IOptions<TeamworkOptions> Options { get; }


        // endpoint properties
        private string V1EndpointUrlBase { get => $"{this.Options.Value.TeamworkEndPointUrlBase}"; }
        private string V3EndpointUrlBase { get => $"{this.Options.Value.TeamworkEndPointUrlBase}/projects/api/v3"; }


        // constructor
        public Teamwork(ILogger logger, IHttpClientFactory httpClientFactory, IOptions<TeamworkOptions> options)
        {
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
        }


        // ---------------------------------
        // HttpRequestMessage factories
        private HttpRequestMessage RequestTimeEntries(string token, ApiQueryParameters apiQueryParameters, int page, int pageSize)
        {

            // build the query parameter string
            List<string> queryParameters = new List<string>();


            // add selected users and projects
            if (apiQueryParameters.SelectedUserList().Any()) queryParameters.Add($"assignedToUserIds={string.Join(",", apiQueryParameters.SelectedUserList().Select(s => s.Id))}");
            if (apiQueryParameters.SelectedProjectList().Any()) queryParameters.Add($"projectIds={string.Join(",", apiQueryParameters.SelectedProjectList().Select(s => s.Id))}");


            // add selected tags
            if (apiQueryParameters.SelectedProjectTagList().Count > 0) queryParameters.Add($"projectTagIds={string.Join(",", apiQueryParameters.SelectedProjectTagList().Select(s => s.Id))}");
            if (apiQueryParameters.SelectedTaskTagList().Count > 0) queryParameters.Add($"taskTagIds={string.Join(",", apiQueryParameters.SelectedTaskTagList().Select(s => s.Id))}");
            if (apiQueryParameters.SelectedTimeTagList().Count > 0) queryParameters.Add($"tagIds={string.Join(",", apiQueryParameters.SelectedTimeTagList().Select(s => s.Id))}");


            // add other parameters
            queryParameters.Add($"startDate={apiQueryParameters.From:yyyy-MM-dd}");
            queryParameters.Add($"endDate={apiQueryParameters.To:yyyy-MM-dd}");
            queryParameters.Add($"pageSize={pageSize}");
            queryParameters.Add($"page={page}");
            queryParameters.Add($"include=tags,tasks,users,projects,tasks.tasklists&fields[tasklists]=name,id&fields[tasks]=id,name,taskListId");


            var request = new HttpRequestMessage(HttpMethod.Get, $"{V3EndpointUrlBase}/time.json?{string.Join("&", queryParameters)}");
            request.Headers.Add("Authorization", $"Bearer {token}");

            return request;

        }

        private HttpRequestMessage RequestTasks(string token, ApiQueryParameters apiQueryParameters, bool includeTasksWithNoDueDate)
        {

            // build the query parameter string
            List<string> queryParameters = new List<string>();

            // add selected users and projects
            if (apiQueryParameters.SelectedUserList().Any()) queryParameters.Add($"assignedToUserIds={string.Join(",", apiQueryParameters.SelectedUserList().Select(s => s.Id))}");
            if (apiQueryParameters.SelectedProjectList().Any()) queryParameters.Add($"projectIds={string.Join(",", apiQueryParameters.SelectedProjectList().Select(s => s.Id))}");


            // add selected tags
            if (apiQueryParameters.SelectedProjectTagList().Count > 0) queryParameters.Add($"projectTagIds={string.Join(",", apiQueryParameters.SelectedProjectTagList().Select(s => s.Id))}");
            if (apiQueryParameters.SelectedTaskTagList().Count > 0) queryParameters.Add($"tag-ids={string.Join(",", apiQueryParameters.SelectedTaskTagList().Select(s => s.Id))}");
            if (apiQueryParameters.SelectedTimeTagList().Count > 0) queryParameters.Add($"tagIds={string.Join(",", apiQueryParameters.SelectedTimeTagList().Select(s => s.Id))}");


            // add other parameters
            queryParameters.Add($"includeTasksWithoutDueDates={includeTasksWithNoDueDate.ToString().ToLowerInvariant()}");
            queryParameters.Add($"includeCompletedTasks=false");
            queryParameters.Add($"pageSize={250}");


            // construct the full endpoint url and request
            var fullQuery = $"{V1EndpointUrlBase}/tasks.json?{string.Join("&", queryParameters)}";
            var request = new HttpRequestMessage(HttpMethod.Get, fullQuery);
            request.Headers.Add("Authorization", $"Bearer {token}");

            return request;

        }

        private HttpRequestMessage RequestPeople(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{V3EndpointUrlBase}/people.json");
            request.Headers.Add("Authorization", $"Bearer {token}");
            return request;
        }

        private HttpRequestMessage RequestProjects(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{V3EndpointUrlBase}/projects.json?pageSize=250");
            request.Headers.Add("Authorization", $"Bearer {token}");
            return request;
        }

        private HttpRequestMessage RequestStarredProjects(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{V3EndpointUrlBase}/projects/starred.json?include=projectUpdates,tags,projectBudgets");
            request.Headers.Add("Authorization", $"Bearer {token}");
            return request;
        }

        private HttpRequestMessage RequestTags(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{V3EndpointUrlBase}/tags.json?pageSize=250");
            request.Headers.Add("Authorization", $"Bearer {token}");
            return request;
        }


        // ---------------------------------
        // V1 method implementations
        public async Task<TasksResponse?> Tasks(string token, ApiQueryParameters apiQueryParameters, bool includeTasksWithNoDueDate, CancellationToken cancellationToken)
        {

            var client = this.HttpClientFactory.CreateClient();
            var response = await client.SendAsync(RequestTasks(token, apiQueryParameters, includeTasksWithNoDueDate), cancellationToken);

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



        // ---------------------------------
        // V3 method implementations
        public async Task<Models.ProjectManagementSystem.TeamworkV3.Token?> ObtainToken(string temporaryToken, CancellationToken cancellationToken)
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

        public async Task<List<Models.ProjectManagementSystem.TeamworkV3.TimeLog>> TimeEntries(string token, ApiQueryParameters apiQueryParameters, CancellationToken cancellationToken)
        {

            int page = 1;
            int pageSize = 500;
            bool finished = false;
            var timeLogs = new List<Models.ProjectManagementSystem.TeamworkV3.TimeLog>();

            while (!finished)
            {

                var client = this.HttpClientFactory.CreateClient();
                var endPoint = RequestTimeEntries(token, apiQueryParameters, page, pageSize);
                var response = await client.SendAsync(endPoint, cancellationToken);

                this.Logger.Verbose(endPoint.RequestUri!.AbsoluteUri);

                if (response.IsSuccessStatusCode)
                {
                    var teResponse = await response.Content.ReadAsAsync<Models.ProjectManagementSystem.TeamworkV3.TimeLogResponse>();
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

        public async Task<Models.ProjectManagementSystem.TeamworkV3.UserResponse?> Users(string token, CancellationToken cancellationToken)
        {

            var client = this.HttpClientFactory.CreateClient();
            var response = await client.SendAsync(RequestPeople(token), cancellationToken);

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

        public async Task<Models.ProjectManagementSystem.TeamworkV3.ProjectResponse?> Projects(string token, CancellationToken cancellationToken)
        {

            var client = this.HttpClientFactory.CreateClient();
            var response = await client.SendAsync(RequestProjects(token), cancellationToken);

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

        public async Task<Models.ProjectManagementSystem.TeamworkV3.ProjectResponse?> StarredProjects(string token, CancellationToken cancellationToken)
        {

            var client = this.HttpClientFactory.CreateClient();
            var response = await client.SendAsync(RequestStarredProjects(token), cancellationToken);

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

        public async Task<Models.ProjectManagementSystem.TeamworkV3.TagResponse?> Tags(string token, CancellationToken cancellationToken)
        {

            var client = this.HttpClientFactory.CreateClient();
            var response = await client.SendAsync(RequestTags(token), cancellationToken);

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


    }

}
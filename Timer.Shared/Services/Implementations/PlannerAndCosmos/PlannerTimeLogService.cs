//using Microsoft.Azure.Cosmos;
//using Microsoft.Graph;
//using Microsoft.Graph.Models;
//using Timer.Shared.Models.Cosmos;
//using Timer.Shared.Models.Identity;
//using Timer.Shared.Services.Implementations.Auth;
//using Timer.Shared.Services.Interfaces;
//using Project = Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models.Project;
//using ProjectTask = Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models.ProjectTask;
//using Tag = Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models.Tag;
//using User = Timer.Shared.Models.Identity.User;

using Microsoft.Azure.Cosmos;
using Microsoft.Graph.Models;
using Timer.Shared.Models.Native;
using Timer.Shared.Services.Implementations.Auth;
using Timer.Shared.Services.Interfaces;

namespace Timer.Shared.Services.Implementations.PlannerAndCosmos
{

    /// <summary>
    /// ITimeLogService implementation backed by the Microsoft Planner API (via Microsoft Graph).
    /// Mapping: Planner Plan -> Project, Planner Task -> ProjectTask, Plan Category labels -> Tags.
    /// Time log entries are persisted to an Azure Cosmos DB container.
    /// </summary>
    public class PlannerTimeLogService : ITimeLogService
    {

        private const string DatabaseId = "TimerDb";
        private const string ContainerId = "TimeLogEntries";

        private readonly CosmosClient _cosmosClient;

        private AuthService AuthService { get; }

        public PlannerTimeLogService(CosmosClient cosmosClient, AuthService authService)
        {
            this._cosmosClient = cosmosClient;
            this.AuthService    = authService;
        }


        public async Task<List<Project>?> Projects(CancellationToken cancellationToken)
        {
            var plans = await this.AuthService.GraphClient.Me.Planner.Plans.GetAsync(cancellationToken: cancellationToken);
            return plans?.Value?.Select(MapPlan).ToList();
        }

        public async Task<List<Project>?> Projects(string searchCriteria, CancellationToken cancellationToken)
        {
            var projects = await this.Projects(cancellationToken);
            return projects?.Where(p => p.Name?.Contains(searchCriteria, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
        }

        public async Task<List<Project>?> Projects(bool starredOnly, CancellationToken cancellationToken)
        {
            // Planner has no concept of starred plans; return all.
            return await this.Projects(cancellationToken);
        }

        public async Task<List<Project>?> RecentProjects(CancellationToken cancellationToken)
        {
            // Planner does not expose recent activity on plans; return all.
            return await this.Projects(cancellationToken);
        }

        public async Task<List<ProjectTask>?> Tasks(CancellationToken cancellationToken)
        {
            var tasks = await this.AuthService.GraphClient.Me.Planner.Tasks.GetAsync(cancellationToken: cancellationToken);
            return tasks?.Value?.Select(MapTask).ToList();
        }

        public async Task<List<ProjectTask>?> Tasks(int projectId, CancellationToken cancellationToken)
        {
            var tasks = await this.AuthService.GraphClient.Planner.Plans[projectId.ToString()].Tasks.GetAsync(cancellationToken: cancellationToken);
            return tasks?.Value?.Select(MapTask).ToList();
        }

        public async Task<List<ProjectTask>?> MyTasks(string projectId, CancellationToken cancellationToken)
        {
            var me = await this.AuthService.GraphClient.Me.GetAsync(cancellationToken: cancellationToken);
            var tasks = await this.AuthService.GraphClient.Planner.Plans[projectId.ToString()].Tasks.GetAsync(cancellationToken: cancellationToken);
            return tasks?.Value?
                .Where(t => t.Assignments?.AdditionalData?.ContainsKey(me?.Id ?? string.Empty) ?? false)
                .Select(MapTask)
                .ToList();
        }

        public async Task<List<ProjectTask>?> Tasks(string searchCriteria, CancellationToken cancellationToken)
        {
            var tasks = await this.Tasks(cancellationToken);
            return tasks?.Where(t => t.Name?.Contains(searchCriteria, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
        }

        public async Task<List<ProjectTask>?> RecentTasks(CancellationToken cancellationToken)
        {
            var tasks = await this.AuthService.GraphClient.Me.Planner.Tasks.GetAsync(cancellationToken: cancellationToken);
            return tasks?.Value?
                .OrderByDescending(t => t.CreatedDateTime)
                .Take(100)
                .Select(MapTask)
                .ToList();
        }

        public async Task<List<Tag>?> Tags(CancellationToken cancellationToken)
        {
            // Planner "tags" are the category label descriptions defined per-plan.
            var plans = await this.AuthService.GraphClient.Me.Planner.Plans.GetAsync(cancellationToken: cancellationToken);
            var tags = new List<Tag>();

            foreach (var plan in plans?.Value ?? [])
            {
                var details = await this.AuthService.GraphClient.Planner.Plans[plan.Id].Details.GetAsync(cancellationToken: cancellationToken);
                var labels = details?.CategoryDescriptions?.AdditionalData?
                    .Where(kvp => kvp.Value is string s && !string.IsNullOrWhiteSpace(s))
                    .Select(kvp => new Tag(kvp.Key, kvp.Value.ToString(), plan.Id));

                if (labels is not null) tags.AddRange(labels);
            }

            return tags.DistinctBy(t => t.Name).ToList();
        }

        public async Task<List<Tag>?> Tags(string searchCriteria, CancellationToken cancellationToken)
        {
            var tags = await this.Tags(cancellationToken);
            return tags?.Where(t => t.Name?.Contains(searchCriteria, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
        }

        public async Task<List<Tag>?> RecentTags(CancellationToken cancellationToken)
        {
            // Planner does not track tag usage recency; return all.
            return await this.Tags(cancellationToken);
        }

        public async Task<DateTimeOffset?> GetEndTimeOfLastTimeLogEntryAsync(CancellationToken cancellationToken)
        {
            var container = await this.GetContainerAsync(cancellationToken);
            var userId = await this.GetUserIdAsync(cancellationToken);

            var query = new QueryDefinition("SELECT TOP 1 c.endDateTime FROM c WHERE c.userId = @userId ORDER BY c.endDateTime DESC")
                .WithParameter("@userId", userId);

            using var iterator = container.GetItemQueryIterator<TimeLogEntryDocument>(
                query,
                requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(userId) });

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(cancellationToken);
                var entry = response.FirstOrDefault();
                if (entry is not null) return entry.EndDateTime;
            }

            return null;
        }

        public async Task<bool> LogTime(DateTime startDateTime,
                            DateTime endDateTime,
                            Project project,
                            ProjectTask? projectTask,
                            List<int> tagIds,
                            bool isBillable,
                            string description,
                            CancellationToken cancellationToken)
        {
            var container = await this.GetContainerAsync(cancellationToken);
            var userId = await this.GetUserIdAsync(cancellationToken);

            var entry = new TimeLogEntryDocument
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
                Span = endDateTime - startDateTime,
                DurationInSeconds = (int)(endDateTime - startDateTime).TotalSeconds,
                Project = project,
                Task = projectTask,
                TagIds = tagIds,
                IsBillable = isBillable,
                Description = description,
                User = this.AuthService.LoggedInUser ?? throw new InvalidOperationException("Unable to resolve the current user.")
            };

            try
            {
                var response = await container.CreateItemAsync(entry, new PartitionKey(userId), cancellationToken: cancellationToken);
                return response.StatusCode == System.Net.HttpStatusCode.Created;
            }
            catch (Exception ex)
            {
                return false;
            }


        }

        private async Task<Container> GetContainerAsync(CancellationToken cancellationToken)
        {
            var database = (await this._cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId, cancellationToken: cancellationToken)).Database;
            var container = (await database.CreateContainerIfNotExistsAsync(ContainerId, "/userId", cancellationToken: cancellationToken)).Container;
            return container;
        }

        private async Task<string> GetUserIdAsync(CancellationToken cancellationToken)
        {
            var me = this.AuthService.LoggedInUser;
            return me?.ObjectId ?? throw new InvalidOperationException("Unable to resolve the current user id.");
        }

        private static Project MapPlan(PlannerPlan plan) => new(plan.Id, plan.Title);

        private static ProjectTask MapTask(PlannerTask task) => new(task.Id, task.Title, task.PlanId);

    }

}
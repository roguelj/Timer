using Microsoft.Azure.Cosmos;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Timer.Shared.Services.Implementations.Auth;
using Timer.Shared.Services.Interfaces;
using Project = Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models.Project;
using ProjectTask = Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models.ProjectTask;
using Tag = Timer.Shared.Models.ProjectManagementSystem.TeamworkV3.Models.Tag;

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

        public async Task<List<ProjectTask>?> MyTasks(int projectId, CancellationToken cancellationToken)
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
                .Take(10)
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
                    .Select(kvp => new Tag { Name = kvp.Value!.ToString() });

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

        public async Task<bool> LogTime(DateTime startDateTime, DateTime endDateTime, int projectId, int? taskId, List<int> tagIds, bool isBillable, string description, CancellationToken cancellationToken)
        {
            var container = await this.GetContainerAsync(cancellationToken);
            var userId = await this.GetUserIdAsync(cancellationToken);

            var entry = new TimeLogEntryDocument
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
                ProjectId = projectId,
                TaskId = taskId,
                TagIds = tagIds,
                IsBillable = isBillable,
                Description = description
            };

            var response = await container.CreateItemAsync(entry, new PartitionKey(userId), cancellationToken: cancellationToken);
            return response.StatusCode == System.Net.HttpStatusCode.Created;
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

        private static Project MapPlan(PlannerPlan plan) => new()
        {
            Id = plan.Id?.GetHashCode() ?? 0, // Planner uses string ids; hashed to fit the int contract
            Name = plan.Title
        };

        private static ProjectTask MapTask(PlannerTask task) => new()
        {
            Id = task.Id?.GetHashCode() ?? 0,
            ProjectId = task.PlanId?.GetHashCode() ?? 0,
            Name = task.Title
        };

        private sealed class TimeLogEntryDocument
        {
            [Newtonsoft.Json.JsonProperty("id")]
            public string Id { get; set; } = string.Empty;

            [Newtonsoft.Json.JsonProperty("userId")]
            public string UserId { get; set; } = string.Empty;

            [Newtonsoft.Json.JsonProperty("startDateTime")]
            public DateTimeOffset StartDateTime { get; set; }

            [Newtonsoft.Json.JsonProperty("endDateTime")]
            public DateTimeOffset EndDateTime { get; set; }

            [Newtonsoft.Json.JsonProperty("projectId")]
            public int ProjectId { get; set; }

            [Newtonsoft.Json.JsonProperty("taskId")]
            public int? TaskId { get; set; }

            [Newtonsoft.Json.JsonProperty("tagIds")]
            public List<int> TagIds { get; set; } = [];

            [Newtonsoft.Json.JsonProperty("isBillable")]
            public bool IsBillable { get; set; }

            [Newtonsoft.Json.JsonProperty("description")]
            public string Description { get; set; } = string.Empty;
        }

    }

}
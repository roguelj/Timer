using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Timer.Base.Interfaces;
using Timer.Base.Models;
using Timer.SqlImplementation.Db.Context;
using Timer.SqlImplementation.Db.Extensions;

/*
 * tags are not scoped to user 
 * projects should be scoped to user
 * tasks should be scoped to user
 */

namespace Timer.SqlImplementation
{
    public class SqlTimerStore : ITimeLogService
    {

        private SqlTimeContext SqlTimeContext { get; }
        private ILogger<SqlTimerStore> Logger { get; }


        public SqlTimerStore(ILogger<SqlTimerStore> logger, SqlTimeContext context)
        {
            this.Logger = logger;
            this.SqlTimeContext = context;
        }


        public async Task<DateTimeOffset?> GetEndTimeOfLastTimeLogEntryAsync(CancellationToken cancellationToken)
        {

            var userId = this.GetCurrentUserId();

            return await this.SqlTimeContext
                                .TimeLogEntries
                                .Where(w => w.UserId == userId)
                                .OrderByDescending(o => o.EndDateTime)
                                .Select(s => s.EndDateTime)
                                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<bool> LogTime(DateTime startDateTime, DateTime endDateTime, int projectId, int? taskId, List<int> tagIds, bool isBillable, string description, CancellationToken cancellationToken)
        {

            var newTimeLogEntry = new Db.Models.TimeLogEntry()
            {
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
                ProjectId = projectId,
                TaskId = taskId,
              //  TagIds = tagIds,
                IsBillable = isBillable,
                Description = description,
                UserId = this.GetCurrentUserId()
            };

            try
            {
                this.SqlTimeContext.TimeLogEntries.Add(newTimeLogEntry);
                return this.SqlTimeContext.SaveChangesAsync(cancellationToken).ContinueWith(t => t.Result > 0, cancellationToken);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Error logging time entry");
                return Task.FromResult(false);
            }

        }

        public async Task<List<ProjectTask>?> MyTasks(int projectId, CancellationToken cancellationToken)
        {
            
            var userId = this.GetCurrentUserId();

            return await this.SqlTimeContext
                .ProjectTasks
                .Include(i => i.ProjectTaskUsers.Where(w => w.UserId == userId));
         


            //return await this.SqlTimeContext
            //                 .Projects
            //                 .Include(i => i.ProjectTasks)
            //                 .Where(w => w.Id == projectId)
            //                 .SelectMany(s => s.ProjectTasks)

        }

        public async Task<List<Project>?> Projects(CancellationToken cancellationToken)
        {

            return await this.SqlTimeContext
                                .Projects
                                .Select(s => new Project(s.Id, s.Name))
                                .ToListAsync(cancellationToken);
            
        }

        public async Task<List<Project>?> Projects(string searchCriteria, CancellationToken cancellationToken)
        {
            return await this.SqlTimeContext
                                .Projects
                                .Where(w => w.Name.Contains(searchCriteria, StringComparison.OrdinalIgnoreCase))    
                                .Select(s => new Project(s.Id, s.Name))
                                .ToListAsync(cancellationToken);
        }

        public async Task<List<Project>?> Projects(bool starredOnly, CancellationToken cancellationToken)
        {
            return await this.SqlTimeContext
                                .Projects
                                .Where(w => w.IsStarred == true)
                                .Select(s => new Project(s.Id, s.Name))
                                .ToListAsync(cancellationToken);
        }

        public Task<List<Project>?> RecentProjects(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<Tag>?> RecentTags(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProjectTask>?> RecentTasks(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Tag>?> Tags(CancellationToken cancellationToken)
        {

            return (await this.SqlTimeContext
                                .Tags
                                .ToListAsync(cancellationToken))
                                .Select(s => s.ToModelTag())
                                .ToList();
                                  
        }

        public async Task<List<Tag>?> Tags(string searchCriteria, CancellationToken cancellationToken)
        {

            return (await this.SqlTimeContext
                        .Tags
                        .Where(w => w.Name.Contains(searchCriteria, StringComparison.OrdinalIgnoreCase))
                        .ToListAsync(cancellationToken))
                        .Select(s => s.ToModelTag())
                        .ToList();
        }

        public Task<List<ProjectTask>?> Tasks(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProjectTask>?> Tasks(int projectId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProjectTask>?> Tasks(string searchCriteria, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        // private methods
        private int GetCurrentUserId() => throw new NotImplementedException();


    }
}

using Microsoft.EntityFrameworkCore;

namespace Timer.SqlImplementation.Db.Context
{
    public class SqlTimeContext : DbContext
    {

        public SqlTimeContext(DbContextOptions<SqlTimeContext> options) : base(options)
        {
        }

        public DbSet<Models.Project> Projects { get; set; } 

        public DbSet<Models.Tag> Tags { get; set; } 

        public DbSet<Models.TaskList> TaskLists { get; set; }

        public DbSet<Models.ProjectTask> ProjectTasks { get; set; }

        public DbSet<Models.SubType> SubTypes { get; set; }

        public DbSet<Models.TimeLogEntry> TimeLogEntries { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
        }

    }

}

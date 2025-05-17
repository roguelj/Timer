namespace Timer.Base.Models
{
    public record ProjectTask
    {
        public ProjectTask(int id, string name, int projectId, int taskListId, string taskListName)
        {
            this.Id = id;
            this.Name = name;
            this.ProjectId = projectId;
            this.TaskListId = taskListId;
            this.TaskListName = taskListName;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int ProjectId { get; set; }

        public int TaskListId { get; set; }

        public string TaskListName { get; set; }

    }
}

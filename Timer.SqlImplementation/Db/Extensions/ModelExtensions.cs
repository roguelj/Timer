using Timer.SqlImplementation.Db.Models;

namespace Timer.SqlImplementation.Db.Extensions
{

    using ProjectDto = Timer.Base.Models.Project;
    using ProjectTaskDto = Timer.Base.Models.ProjectTask;
    using SubTypeDto = Timer.Base.Models.SubType;
    using TagDto = Timer.Base.Models.Tag;
    using TaskListDto = Timer.Base.Models.TaskList;

    public static class ModelExtensions
    {

        // Project
        public static ProjectDto ToModelProject(this Project project) => new(project.Id, project.Name);

       // public static IEnumerable<ProjectDto> ToModelProjects(this Iq<Project> projects) => projects.Sele(ToModelProject);


        // ProjectTask
        public static ProjectTaskDto ToModelProjectTask(this ProjectTask projectTask) => new(projectTask.Id, projectTask.Name, projectTask.ProjectId, projectTask.TaskListId, projectTask.TaskListName);

        public static IEnumerable<ProjectTaskDto> ToModelProjectTasks(this IQueryable<ProjectTask> projectTasks) => projectTasks.Select(ToModelProjectTask);


        // Tag
        public static TagDto ToModelTag(this Tag tag) => new(tag.Id, tag.ProjectId, tag.Project.ToModelSubType(), tag.Name, tag.Colour);

        public static IEnumerable<TagDto> ToModelTags(this IEnumerable<Tag> tags) => tags.Select(ToModelTag);


        // TaskList
        public static TaskListDto ToModelTasklist(this TaskList taskList) => new(taskList.Id, taskList.ProjectId, taskList.Name);

        public static IEnumerable<TaskListDto> ToModelTasklists(this IEnumerable<TaskList> taskLists) => taskLists.Select(ToModelTasklist);


        // SubType
        public static SubTypeDto ToModelSubType(this SubType subType) => new(subType.Id, subType.Type);
    }
}

namespace Timer.SqlImplementation.Db.Models
{
    public class SubType
    {
        public required int Id { get; set; }
        public required string Type { get; set; } = null!;
        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();

    }

}

namespace Timer.SqlImplementation.Db.Models
{
    public class Tag
    {
        public required int Id { get; set; }
        public required int ProjectId { get; set; }
        public required string Name { get; set; }
        public required string Colour { get; set; }
        public virtual SubType? Project { get; set; } = null!;
    }
}

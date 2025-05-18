namespace Timer.SqlImplementation.Db.Models
{
    public class TimeLogEntry
    {
        public int Id { get; set; }

        public required DateTime StartDateTime { get; set; }

        public required DateTime EndDateTime { get; set; }

        public required int ProjectId { get; set; }

        public int? TaskId { get; set; } 

        public required string Description { get; set; }

        public bool IsBillable { get; set; }

        public required int UserId { get; set; }

    }

}

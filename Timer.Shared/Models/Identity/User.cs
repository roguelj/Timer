namespace Timer.Shared.Models.Identity
{
    public class User
    {

        public required int Id { get; set; }

        public required string Name { get; set; }

        public required string UserName { get; set; }

        public required string TenantId { get; set; }

        public required string ObjectId { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Timer.Shared.Models.Identity
{
    internal class User
    {

        public required int Id { get; set; }

        public required string Name { get; set; }

        public required string UserName { get; set; }

        public required string TenantId { get; set; }

        public required string ObjectId { get; set; }

    }
}

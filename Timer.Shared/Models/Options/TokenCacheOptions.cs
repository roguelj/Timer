namespace Timer.Shared.Models.Options
{
    public class TokenCacheOptions
    {
        public required string FileName { get; set; }

        public required byte[] Entropy { get; set; }

    }
}

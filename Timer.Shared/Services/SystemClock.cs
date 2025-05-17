

using Timer.Base.Interfaces;

namespace Timer.Shared.Services.Implementations
{
    public class SystemClock : ISystemClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}

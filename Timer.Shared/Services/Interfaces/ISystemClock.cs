namespace Timer.Shared.Services.Interfaces
{

    public interface ISystemClock
    {
        DateTimeOffset UtcNow { get; }

        DateTimeOffset Now { get; }

    }

}

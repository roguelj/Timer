namespace Timer.Base.Interfaces
{
    public interface ISystemClock
    {
        DateTimeOffset UtcNow { get; }
    }

}

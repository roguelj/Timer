namespace Timer.Shared.Models.Options
{
    public class UserInterfaceOptions
    {

        public bool AlwaysOnTop { get; set; }

        public TimeOnly? TimeOfFirstTask { get; set; }

        public TimeOnly? BreakStart { get; set; }

        public TimeOnly? BreakEnd { get; set; }

        public string? Theme { get; set; }

    }
}

namespace Timer.Shared.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime MonthStart(this DateTime Input)
        {
            return new DateTime(Input.Year, Input.Month, 1);
        }

        public static DateTime MonthEnd(this DateTime Input)
        {
            return new DateTime(Input.Year, Input.Month, 1).AddMonths(1).AddDays(-1);
        }
    }
}

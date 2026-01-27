namespace ProfilesService.Application.Common
{
    public static class DateOnlyExtensions
    {
        public static DateTime ToUtcDateTime(this DateOnly date)
        {
            return date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        }
    }
}

namespace TuitionManagementSystem.Web.Features.Schedule;

public static class DateTimeUtc
{
    public static DateTime ToUtcAssumingLocal(DateTime dt) =>
        dt.Kind switch
        {
            DateTimeKind.Utc => dt,
            DateTimeKind.Local => dt.ToUniversalTime(),
            _ => DateTime.SpecifyKind(dt, DateTimeKind.Local).ToUniversalTime()
        };

    public static DateTime? ToUtcAssumingLocal(DateTime? dt) =>
        dt.HasValue ? ToUtcAssumingLocal(dt.Value) : null;

    public static List<DateTime> ToUtcAssumingLocal(IEnumerable<DateTime> values) =>
        values.Select(ToUtcAssumingLocal).ToList();
}

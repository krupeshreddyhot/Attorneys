namespace Attorneys.API.Common;

internal static class DateTimeHelper
{
    /// <summary>
    /// PostgreSQL timestamptz via Npgsql requires UTC. JSON date strings arrive as Kind=Unspecified.
    /// </summary>
    public static DateTime? ToUtcDate(DateTime? value) =>
        value.HasValue ? DateTime.SpecifyKind(value.Value.Date, DateTimeKind.Utc) : null;
}

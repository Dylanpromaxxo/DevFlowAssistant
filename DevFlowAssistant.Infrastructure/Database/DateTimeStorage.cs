using System.Globalization;

namespace DevFlowAssistant.Infrastructure.Database;

public static class DateTimeStorage
{
    public static string ToStorage(DateTime value)
    {
        return value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);
    }

    public static DateTime FromStorage(string value)
    {
        if (DateTime.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out var parsed))
        {
            return parsed.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(parsed, DateTimeKind.Utc)
                : parsed.ToUniversalTime();
        }

        if (DateTime.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out parsed))
        {
            return parsed;
        }

        if (DateTime.TryParse(
                value,
                CultureInfo.CurrentCulture,
                DateTimeStyles.AssumeLocal,
                out parsed))
        {
            return parsed.ToUniversalTime();
        }

        return DateTime.UtcNow;
    }

    public static string? ToStorageNullable(DateTime? value)
    {
        return value.HasValue ? ToStorage(value.Value) : null;
    }

    public static DateTime? FromStorageNullable(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : FromStorage(value);
    }
}

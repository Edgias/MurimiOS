namespace EdgIQ.MurimiOS.Domain.Common;

public enum AmbiguousLocalTimeResolution
{
    Earlier = 1,
    Later = 2
}

public readonly record struct BusinessTime
{
    public BusinessTime(DateTimeOffset occurredAt, DateTimeOffset recordedAt)
    {
        OccurredAtUtc = occurredAt.ToUniversalTime();
        RecordedAtUtc = recordedAt.ToUniversalTime();
    }

    public DateTimeOffset OccurredAtUtc { get; }
    public DateTimeOffset RecordedAtUtc { get; }
    public TimeSpan RecordingDelay => RecordedAtUtc - OccurredAtUtc;
}

public static class RegionalTime
{
    public static DateTimeOffset ToUtc(
        DateTime localTime,
        string timeZoneId,
        AmbiguousLocalTimeResolution ambiguousResolution = AmbiguousLocalTimeResolution.Earlier)
    {
        TimeZoneInfo zone = ResolveTimeZone(timeZoneId);
        DateTime unspecified = DateTime.SpecifyKind(localTime, DateTimeKind.Unspecified);
        if (zone.IsInvalidTime(unspecified))
            throw new ArgumentException("The local time does not exist in the configured timezone.", nameof(localTime));

        if (zone.IsAmbiguousTime(unspecified))
        {
            DateTimeOffset[] candidates = zone.GetAmbiguousTimeOffsets(unspecified)
                .Select(offset => new DateTimeOffset(unspecified, offset).ToUniversalTime())
                .OrderBy(value => value)
                .ToArray();
            return ambiguousResolution == AmbiguousLocalTimeResolution.Earlier ? candidates[0] : candidates[^1];
        }

        return TimeZoneInfo.ConvertTimeToUtc(unspecified, zone);
    }

    public static DateTimeOffset ToLocal(DateTimeOffset utcTime, string timeZoneId) =>
        TimeZoneInfo.ConvertTime(utcTime, ResolveTimeZone(timeZoneId));

    public static TimeZoneInfo ResolveTimeZone(string timeZoneId)
    {
        string normalized = timeZoneId?.Trim() ?? string.Empty;
        if (normalized.Length is 0 or > 100) throw new ArgumentException("A timezone identifier is required.", nameof(timeZoneId));
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(normalized);
        }
        catch (TimeZoneNotFoundException exception)
        {
            throw new ArgumentException($"Unknown timezone '{normalized}'.", nameof(timeZoneId), exception);
        }
        catch (InvalidTimeZoneException exception)
        {
            throw new ArgumentException($"Invalid timezone '{normalized}'.", nameof(timeZoneId), exception);
        }
    }
}

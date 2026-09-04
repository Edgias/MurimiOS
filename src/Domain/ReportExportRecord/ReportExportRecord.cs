using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.ReportExportRecord;

public sealed class ReportExportRecord : TenantEntity
{
    private ReportExportRecord()
    {
    }

    public ReportExportRecord(
        Guid id,
        Guid cropZoneId,
        string reportType,
        string format,
        string sha256,
        DateTimeOffset generatedAt,
        string? requestedBy) : base(id)
    {
        if (cropZoneId == Guid.Empty)
        {
            throw new ArgumentException("A crop zone is required.", nameof(cropZoneId));
        }

        CropZoneId = cropZoneId;
        ReportType = RequireText(reportType, nameof(reportType), 120);
        Format = RequireText(format, nameof(format), 20).ToLowerInvariant();
        Sha256 = RequireSha256(sha256);
        GeneratedAt = generatedAt;
        RequestedBy = OptionalText(requestedBy, 200);
    }

    public Guid CropZoneId { get; private set; }

    public string ReportType { get; private set; } = string.Empty;

    public string Format { get; private set; } = string.Empty;

    public string Sha256 { get; private set; } = string.Empty;

    public DateTimeOffset GeneratedAt { get; private set; }

    public string? RequestedBy { get; private set; }

    private static string RequireText(string value, string parameter, int maximumLength)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximumLength)
        {
            throw new ArgumentException($"{parameter} must contain between 1 and {maximumLength} characters.", parameter);
        }

        return result;
    }

    private static string RequireSha256(string value)
    {
        string result = value?.Trim().ToLowerInvariant() ?? string.Empty;
        if (result.Length != 64 || result.Any(character => !Uri.IsHexDigit(character)))
        {
            throw new ArgumentException("A hexadecimal SHA-256 digest is required.", nameof(value));
        }

        return result;
    }

    private static string? OptionalText(string? value, int maximumLength)
    {
        string? result = value?.Trim();
        if (result?.Length > maximumLength)
        {
            throw new ArgumentException($"The requester cannot exceed {maximumLength} characters.", nameof(value));
        }

        return string.IsNullOrEmpty(result) ? null : result;
    }
}

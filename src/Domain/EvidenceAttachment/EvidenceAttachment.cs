using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.EvidenceAttachment;

public sealed class EvidenceAttachment : TenantEntity
{
    private EvidenceAttachment()
    {
    }

    public EvidenceAttachment(
        Guid id,
        Guid cropZoneId,
        string fileName,
        string contentType,
        long byteSize,
        string sha256,
        string objectKey,
        DateTimeOffset capturedAt,
        double? latitude,
        double? longitude,
        double? accuracyMetres) : base(id)
    {
        if (cropZoneId == Guid.Empty)
        {
            throw new ArgumentException("A crop zone is required.", nameof(cropZoneId));
        }

        if (byteSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(byteSize), "Evidence must contain at least one byte.");
        }

        ValidateLocation(latitude, longitude, accuracyMetres);
        CropZoneId = cropZoneId;
        FileName = RequireText(fileName, nameof(fileName), 255);
        ContentType = RequireText(contentType, nameof(contentType), 120).ToLowerInvariant();
        ByteSize = byteSize;
        Sha256 = RequireSha256(sha256);
        ObjectKey = RequireText(objectKey, nameof(objectKey), 500);
        CapturedAt = capturedAt;
        Latitude = latitude;
        Longitude = longitude;
        AccuracyMetres = accuracyMetres;
    }

    public Guid CropZoneId { get; private set; }

    public string FileName { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public long ByteSize { get; private set; }

    public string Sha256 { get; private set; } = string.Empty;

    public string ObjectKey { get; private set; } = string.Empty;

    public DateTimeOffset CapturedAt { get; private set; }

    public double? Latitude { get; private set; }

    public double? Longitude { get; private set; }

    public double? AccuracyMetres { get; private set; }

    internal static void ValidateLocation(double? latitude, double? longitude, double? accuracyMetres)
    {
        if (latitude.HasValue != longitude.HasValue)
        {
            throw new ArgumentException("Latitude and longitude must be supplied together.");
        }

        if (latitude is < -90 or > 90 || longitude is < -180 or > 180)
        {
            throw new ArgumentOutOfRangeException(nameof(latitude), "The GPS coordinate is outside WGS84 bounds.");
        }

        if (accuracyMetres < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(accuracyMetres), "GPS accuracy cannot be negative.");
        }
    }

    internal static string RequireSha256(string value)
    {
        string result = value?.Trim().ToLowerInvariant() ?? string.Empty;
        if (result.Length != 64 || result.Any(character => !Uri.IsHexDigit(character)))
        {
            throw new ArgumentException("A lowercase or uppercase hexadecimal SHA-256 digest is required.", nameof(value));
        }

        return result;
    }

    private static string RequireText(string value, string parameter, int maximumLength)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximumLength)
        {
            throw new ArgumentException($"{parameter} must contain between 1 and {maximumLength} characters.", parameter);
        }

        return result;
    }
}

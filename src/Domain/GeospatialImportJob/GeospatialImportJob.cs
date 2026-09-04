using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Domain.GeospatialImportJob;

public sealed class GeospatialImportJob : TenantEntity
{
    private GeospatialImportJob() { }

    public GeospatialImportJob(Guid id, Guid farmId, GeospatialImportFormat format, string fileName,
        string contentType, long byteSize, string sha256, string objectKey, string parserVersion) : base(id)
    {
        if (farmId == Guid.Empty) throw new ArgumentException("A farm is required.", nameof(farmId));
        if (!Enum.IsDefined(format)) throw new ArgumentOutOfRangeException(nameof(format));
        if (byteSize is <= 0 or > 52_428_800) throw new ArgumentOutOfRangeException(nameof(byteSize));
        FarmId = farmId;
        Format = format;
        FileName = Require(fileName, nameof(fileName), 255);
        ContentType = Require(contentType, nameof(contentType), 120).ToLowerInvariant();
        ByteSize = byteSize;
        Sha256 = RequireSha256(sha256);
        ObjectKey = Require(objectKey, nameof(objectKey), 500);
        ParserVersion = Require(parserVersion, nameof(parserVersion), 40);
        Status = GeospatialImportStatus.Uploaded;
    }

    public Guid FarmId { get; private set; }
    public GeospatialImportFormat Format { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long ByteSize { get; private set; }
    public string Sha256 { get; private set; } = string.Empty;
    public string ObjectKey { get; private set; } = string.Empty;
    public string ParserVersion { get; private set; } = string.Empty;
    public GeospatialImportStatus Status { get; private set; }
    public int FeatureCount { get; private set; }
    public int EligibleFeatureCount { get; private set; }
    public int WarningCount { get; private set; }
    public int ErrorCount { get; private set; }
    public string? FailureCode { get; private set; }
    public string? FailureDetail { get; private set; }
    public DateTimeOffset? ValidatedAt { get; private set; }
    public DateTimeOffset? AppliedAt { get; private set; }
    public string? AppliedBy { get; private set; }

    public void CompleteValidation(int featureCount, int eligibleFeatureCount, int warningCount, int errorCount,
        DateTimeOffset validatedAt)
    {
        if (Status != GeospatialImportStatus.Uploaded) throw new InvalidOperationException("Only an uploaded import can be validated.");
        if (featureCount < 0 || eligibleFeatureCount < 0 || eligibleFeatureCount > featureCount || warningCount < 0 || errorCount < 0)
            throw new ArgumentOutOfRangeException(nameof(featureCount));
        FeatureCount = featureCount;
        EligibleFeatureCount = eligibleFeatureCount;
        WarningCount = warningCount;
        ErrorCount = errorCount;
        ValidatedAt = validatedAt;
        Status = featureCount > 0 && errorCount == 0 ? GeospatialImportStatus.Validated : GeospatialImportStatus.Rejected;
    }

    public void Fail(string code, string detail, DateTimeOffset failedAt)
    {
        if (Status != GeospatialImportStatus.Uploaded) throw new InvalidOperationException("Only an uploaded import can fail parsing.");
        FailureCode = Require(code, nameof(code), 80).ToLowerInvariant();
        FailureDetail = Require(detail, nameof(detail), 2_000);
        ValidatedAt = failedAt;
        Status = GeospatialImportStatus.Failed;
    }

    public void MarkApplied(string appliedBy, DateTimeOffset appliedAt)
    {
        if (Status == GeospatialImportStatus.Applied) return;
        if (Status != GeospatialImportStatus.Validated) throw new InvalidOperationException("Only a validated import can be applied.");
        AppliedBy = Require(appliedBy, nameof(appliedBy), 240);
        AppliedAt = appliedAt;
        Status = GeospatialImportStatus.Applied;
    }

    private static string Require(string? value, string parameter, int maximum)
    {
        string normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length is 0 || normalized.Length > maximum)
            throw new ArgumentException($"{parameter} must contain between 1 and {maximum} characters.", parameter);
        return normalized;
    }

    private static string RequireSha256(string? value)
    {
        string normalized = value?.Trim().ToLowerInvariant() ?? string.Empty;
        if (normalized.Length != 64 || normalized.Any(character => !Uri.IsHexDigit(character)))
            throw new ArgumentException("A hexadecimal SHA-256 digest is required.", nameof(value));
        return normalized;
    }
}

using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.MediaUploadSession;
using EvidenceAttachment = global::EdgIQ.MurimiOS.Domain.EvidenceAttachment.EvidenceAttachment;

public enum MediaUploadStatus
{
    Pending = 1,
    Completed = 2,
    Expired = 3
}

public sealed class MediaUploadSession : TenantEntity
{
    private MediaUploadSession()
    {
    }

    public MediaUploadSession(
        Guid id,
        Guid evidenceId,
        Guid cropZoneId,
        string fileName,
        string contentType,
        long expectedBytes,
        string expectedSha256,
        DateTimeOffset capturedAt,
        double? latitude,
        double? longitude,
        double? accuracyMetres,
        DateTimeOffset expiresAt) : base(id)
    {
        if (evidenceId == Guid.Empty || cropZoneId == Guid.Empty)
        {
            throw new ArgumentException("Evidence and crop-zone identifiers are required.");
        }

        if (expectedBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(expectedBytes));
        }

        EvidenceAttachment.ValidateLocation(latitude, longitude, accuracyMetres);
        EvidenceId = evidenceId;
        CropZoneId = cropZoneId;
        FileName = RequireText(fileName, nameof(fileName), 255);
        ContentType = RequireText(contentType, nameof(contentType), 120).ToLowerInvariant();
        ExpectedBytes = expectedBytes;
        ExpectedSha256 = EvidenceAttachment.RequireSha256(expectedSha256);
        CapturedAt = capturedAt;
        Latitude = latitude;
        Longitude = longitude;
        AccuracyMetres = accuracyMetres;
        ExpiresAt = expiresAt;
        Status = MediaUploadStatus.Pending;
    }

    public Guid EvidenceId { get; private set; }

    public Guid CropZoneId { get; private set; }

    public string FileName { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public long ExpectedBytes { get; private set; }

    public string ExpectedSha256 { get; private set; } = string.Empty;

    public long ReceivedBytes { get; private set; }

    public DateTimeOffset CapturedAt { get; private set; }

    public double? Latitude { get; private set; }

    public double? Longitude { get; private set; }

    public double? AccuracyMetres { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }

    public MediaUploadStatus Status { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public void Advance(long expectedOffset, long bytesWritten)
    {
        if (Status != MediaUploadStatus.Pending)
        {
            throw new InvalidOperationException("Only a pending upload can receive bytes.");
        }

        if (expectedOffset != ReceivedBytes)
        {
            throw new InvalidOperationException($"Upload offset mismatch. Expected {ReceivedBytes}.");
        }

        if (bytesWritten <= 0 || ReceivedBytes + bytesWritten > ExpectedBytes)
        {
            throw new ArgumentOutOfRangeException(nameof(bytesWritten));
        }

        ReceivedBytes += bytesWritten;
    }

    public void Complete(DateTimeOffset completedAt)
    {
        if (Status == MediaUploadStatus.Completed)
        {
            return;
        }

        if (ReceivedBytes != ExpectedBytes)
        {
            throw new InvalidOperationException("The upload is incomplete.");
        }

        Status = MediaUploadStatus.Completed;
        CompletedAt = completedAt;
    }

    public void ReconcileStoredBytes(long storedBytes)
    {
        if (Status != MediaUploadStatus.Pending || storedBytes < 0 || storedBytes > ExpectedBytes)
        {
            throw new InvalidOperationException("The stored upload length cannot be reconciled.");
        }

        ReceivedBytes = storedBytes;
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

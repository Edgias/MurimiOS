using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.IngestionJob;

public sealed class IngestionRecord : TenantEntity
{
    private IngestionRecord() { }
    public IngestionRecord(Guid id, Guid jobId, Guid feedId, Guid farmId, string externalRecordId,
        int schemaVersion, DateTimeOffset observedAt, DateTimeOffset receivedAt, DateTimeOffset? sourceModifiedAt,
        string contentSha256, string payloadJson, IngestionQuality quality) : base(id)
    {
        if (jobId == Guid.Empty || feedId == Guid.Empty || farmId == Guid.Empty || schemaVersion <= 0 || !Enum.IsDefined(quality)) throw new ArgumentException("Job, feed, farm, and a positive schema version are required.");
        JobId = jobId; FeedId = feedId; FarmId = farmId; ExternalRecordId = Text(externalRecordId, nameof(externalRecordId), 240);
        SchemaVersion = schemaVersion; ObservedAt = observedAt.ToUniversalTime(); ReceivedAt = receivedAt.ToUniversalTime(); SourceModifiedAt = sourceModifiedAt?.ToUniversalTime();
        ContentSha256 = Sha256(contentSha256); PayloadJson = Text(payloadJson, nameof(payloadJson), 1_000_000); Quality = quality; Status = IngestionRecordStatus.Received;
    }
    public Guid JobId { get; private set; }
    public Guid FeedId { get; private set; }
    public Guid FarmId { get; private set; }
    public string ExternalRecordId { get; private set; } = string.Empty;
    public int SchemaVersion { get; private set; }
    public DateTimeOffset ObservedAt { get; private set; }
    public DateTimeOffset ReceivedAt { get; private set; }
    public DateTimeOffset? SourceModifiedAt { get; private set; }
    public string ContentSha256 { get; private set; } = string.Empty;
    public string PayloadJson { get; private set; } = "{}";
    public IngestionQuality Quality { get; private set; }
    public IngestionRecordStatus Status { get; private set; }
    public DateTimeOffset? ProcessedAt { get; private set; }
    public string? RejectionCode { get; private set; }
    public string? RejectionReason { get; private set; }
    public void MarkProcessed(DateTimeOffset at) { if (Status != IngestionRecordStatus.Received) throw new InvalidOperationException("Only a received record can be processed."); Status = IngestionRecordStatus.Processed; ProcessedAt = at.ToUniversalTime(); }
    public void Reject(DateTimeOffset at, string code, string reason) { if (Status != IngestionRecordStatus.Received) throw new InvalidOperationException("Only a received record can be rejected."); Status = IngestionRecordStatus.Rejected; ProcessedAt = at.ToUniversalTime(); RejectionCode = Text(code, nameof(code), 80); RejectionReason = Text(reason, nameof(reason), 2_000); }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string Sha256(string? value) { string result = value?.Trim().ToLowerInvariant() ?? string.Empty; if (result.Length != 64 || result.Any(x => !Uri.IsHexDigit(x))) throw new ArgumentException("A hexadecimal SHA-256 digest is required.", nameof(value)); return result; }
}

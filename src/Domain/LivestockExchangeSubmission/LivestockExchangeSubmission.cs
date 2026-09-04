using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission;
using LivestockRegulatoryExchangeType = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryExchangeType;

/// <summary>An immutable source snapshot plus the mutable state of its external delivery.</summary>
public sealed class LivestockExchangeSubmission : TenantEntity
{
    private LivestockExchangeSubmission() { }

    public LivestockExchangeSubmission(Guid id, Guid profileId, Guid farmId, Guid subjectId,
        Guid sourceRecordId, LivestockRegulatoryExchangeType exchangeType, string providerCode,
        string schemaVersion, string mediaType, string payloadJson, string payloadSha256,
        DateTimeOffset createdAt, string createdBy, DateTimeOffset? reportingDueAt) : base(id)
    {
        if (profileId == Guid.Empty || farmId == Guid.Empty || subjectId == Guid.Empty || sourceRecordId == Guid.Empty)
            throw new ArgumentException("Profile, farm, subject, and source record identifiers are required.");
        if (exchangeType is not (LivestockRegulatoryExchangeType.BreedRegistration
            or LivestockRegulatoryExchangeType.MilkRecording))
            throw new ArgumentException("Only breed-registration and milk-recording exchanges use this submission type.", nameof(exchangeType));

        ProfileId = profileId;
        FarmId = farmId;
        SubjectId = subjectId;
        SourceRecordId = sourceRecordId;
        ExchangeType = exchangeType;
        ProviderCode = Text(providerCode, nameof(providerCode), 80).ToLowerInvariant();
        SchemaVersion = Text(schemaVersion, nameof(schemaVersion), 40);
        MediaType = Text(mediaType, nameof(mediaType), 120).ToLowerInvariant();
        PayloadJson = Json(payloadJson);
        PayloadSha256 = Hash(payloadSha256);
        CreatedAt = createdAt.ToUniversalTime();
        CreatedBy = Text(createdBy, nameof(createdBy), 240);
        ReportingDueAt = reportingDueAt?.ToUniversalTime();
        Status = LivestockExchangeSubmissionStatus.Ready;
    }

    public Guid ProfileId { get; private set; }
    public Guid FarmId { get; private set; }
    public Guid SubjectId { get; private set; }
    public Guid SourceRecordId { get; private set; }
    public LivestockRegulatoryExchangeType ExchangeType { get; private set; }
    public string ProviderCode { get; private set; } = string.Empty;
    public string SchemaVersion { get; private set; } = string.Empty;
    public string MediaType { get; private set; } = string.Empty;
    public string PayloadJson { get; private set; } = "{}";
    public string PayloadSha256 { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTimeOffset? ReportingDueAt { get; private set; }
    public LivestockExchangeSubmissionStatus Status { get; private set; }
    public DateTimeOffset? FirstSubmittedAt { get; private set; }
    public DateTimeOffset? DecidedAt { get; private set; }
    public string? ExternalReference { get; private set; }
    public string? RejectionCode { get; private set; }
    public string? RejectionMessage { get; private set; }

    public void MarkSubmitted(DateTimeOffset at, string externalReference)
    {
        EnsureOpen();
        FirstSubmittedAt ??= at.ToUniversalTime();
        ExternalReference = Text(externalReference, nameof(externalReference), 160);
        Status = LivestockExchangeSubmissionStatus.Submitted;
    }

    public void Accept(DateTimeOffset at, string externalReference)
    {
        EnsureOpen();
        FirstSubmittedAt ??= at.ToUniversalTime();
        DecidedAt = at.ToUniversalTime();
        ExternalReference = Text(externalReference, nameof(externalReference), 160);
        RejectionCode = null;
        RejectionMessage = null;
        Status = LivestockExchangeSubmissionStatus.Accepted;
    }

    public void Reject(DateTimeOffset at, string code, string message)
    {
        EnsureOpen();
        FirstSubmittedAt ??= at.ToUniversalTime();
        DecidedAt = at.ToUniversalTime();
        RejectionCode = Text(code, nameof(code), 80);
        RejectionMessage = Text(message, nameof(message), 2_000);
        Status = LivestockExchangeSubmissionStatus.Rejected;
    }

    private void EnsureOpen()
    {
        if (Status is LivestockExchangeSubmissionStatus.Accepted or LivestockExchangeSubmissionStatus.Rejected)
            throw new InvalidOperationException("A final external submission cannot be changed.");
    }

    private static string Text(string? value, string parameter, int maximum)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximum)
            throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter);
        return result;
    }

    private static string Json(string? value)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 or > 128_000) throw new ArgumentException("Payload JSON is required and cannot exceed 128000 characters.");
        using JsonDocument _ = JsonDocument.Parse(result);
        return result;
    }

    private static string Hash(string? value)
    {
        string result = value?.Trim().ToLowerInvariant() ?? string.Empty;
        if (result.Length != 64 || result.Any(x => !Uri.IsHexDigit(x)))
            throw new ArgumentException("A SHA-256 payload hash is required.", nameof(value));
        return result;
    }
}

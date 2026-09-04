using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument;

public sealed class LivestockStatutoryMovementDocument : TenantEntity
{
    private LivestockStatutoryMovementDocument() { }

    public LivestockStatutoryMovementDocument(Guid id, Guid movementId, Guid subjectId, Guid farmId,
        string jurisdiction, string providerCode, string schemaVersion, string documentNumber,
        string snapshotJson, string payloadSha256, DateTimeOffset createdAt, string createdBy,
        DateTimeOffset reportingDueAt, DateTimeOffset registerDueAt, Guid? regulatoryProfileId = null,
        string documentMediaType = "application/json", string? renderedDocument = null) : base(id)
    {
        if (movementId == Guid.Empty || subjectId == Guid.Empty || farmId == Guid.Empty)
            throw new ArgumentException("Movement, subject, and farm identifiers are required.");
        if (reportingDueAt == default || registerDueAt == default)
            throw new ArgumentException("Statutory deadlines are required.");
        MovementId = movementId;
        SubjectId = subjectId;
        FarmId = farmId;
        Jurisdiction = Text(jurisdiction, nameof(jurisdiction), 20).ToUpperInvariant();
        ProviderCode = Text(providerCode, nameof(providerCode), 80).ToLowerInvariant();
        SchemaVersion = Text(schemaVersion, nameof(schemaVersion), 40);
        DocumentNumber = Text(documentNumber, nameof(documentNumber), 120);
        SnapshotJson = Json(snapshotJson, nameof(snapshotJson));
        PayloadSha256 = Hash(payloadSha256);
        CreatedAt = createdAt.ToUniversalTime();
        CreatedBy = Text(createdBy, nameof(createdBy), 240);
        ReportingDueAt = reportingDueAt.ToUniversalTime();
        RegisterDueAt = registerDueAt.ToUniversalTime();
        if (regulatoryProfileId == Guid.Empty) throw new ArgumentException("An optional regulatory profile id must be non-empty.", nameof(regulatoryProfileId));
        RegulatoryProfileId = regulatoryProfileId;
        DocumentMediaType = Text(documentMediaType, nameof(documentMediaType), 120).ToLowerInvariant();
        RenderedDocument = string.IsNullOrWhiteSpace(renderedDocument) ? snapshotJson.Trim() : renderedDocument.Trim();
        if (RenderedDocument.Length > 128_000) throw new ArgumentException("The rendered document cannot exceed 128000 characters.", nameof(renderedDocument));
        Status = StatutoryMovementDocumentStatus.Ready;
    }

    public Guid MovementId { get; private set; }
    public Guid SubjectId { get; private set; }
    public Guid FarmId { get; private set; }
    public string Jurisdiction { get; private set; } = string.Empty;
    public string ProviderCode { get; private set; } = string.Empty;
    public string SchemaVersion { get; private set; } = string.Empty;
    public string DocumentNumber { get; private set; } = string.Empty;
    public string SnapshotJson { get; private set; } = "{}";
    public string PayloadSha256 { get; private set; } = string.Empty;
    public StatutoryMovementDocumentStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTimeOffset ReportingDueAt { get; private set; }
    public DateTimeOffset RegisterDueAt { get; private set; }
    public Guid? RegulatoryProfileId { get; private set; }
    public string DocumentMediaType { get; private set; } = "application/json";
    public string RenderedDocument { get; private set; } = "{}";
    public DateTimeOffset? SubmittedAt { get; private set; }
    public DateTimeOffset? DecidedAt { get; private set; }
    public string? ExternalReference { get; private set; }
    public string? RejectionCode { get; private set; }
    public string? RejectionMessage { get; private set; }

    public void Accept(DateTimeOffset submittedAt, DateTimeOffset decidedAt, string externalReference)
    {
        EnsureReady();
        if (decidedAt < submittedAt) throw new ArgumentException("Decision cannot precede submission.");
        SubmittedAt = submittedAt.ToUniversalTime();
        DecidedAt = decidedAt.ToUniversalTime();
        ExternalReference = Text(externalReference, nameof(externalReference), 160);
        Status = StatutoryMovementDocumentStatus.Accepted;
    }

    public void Reject(DateTimeOffset submittedAt, DateTimeOffset decidedAt, string code, string message)
    {
        EnsureReady();
        if (decidedAt < submittedAt) throw new ArgumentException("Decision cannot precede submission.");
        SubmittedAt = submittedAt.ToUniversalTime();
        DecidedAt = decidedAt.ToUniversalTime();
        RejectionCode = Text(code, nameof(code), 80);
        RejectionMessage = Text(message, nameof(message), 2_000);
        Status = StatutoryMovementDocumentStatus.Rejected;
    }

    private void EnsureReady()
    {
        if (Status != StatutoryMovementDocumentStatus.Ready)
            throw new InvalidOperationException("Only a ready statutory document can be submitted.");
    }

    private static string Text(string? value, string parameter, int max)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > max) throw new ArgumentException($"{parameter} is required and cannot exceed {max} characters.", parameter);
        return result;
    }
    private static string Json(string? value, string parameter)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 or > 64_000) throw new ArgumentException($"{parameter} is required and cannot exceed 64000 characters.", parameter);
        return result;
    }
    private static string Hash(string? value)
    {
        string result = value?.Trim().ToLowerInvariant() ?? string.Empty;
        if (result.Length != 64 || result.Any(x => !Uri.IsHexDigit(x))) throw new ArgumentException("A SHA-256 payload hash is required.", nameof(value));
        return result;
    }
}

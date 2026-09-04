using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.BusinessDocumentExchange;
using ExchangeStatus = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.ExchangeStatus;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;

public sealed class BusinessDocumentExchange : TenantEntity
{
    private BusinessDocumentExchange() { }
    public BusinessDocumentExchange(Guid id, Guid connectionId, Guid farmId, BusinessDocumentType documentType,
        ExchangeDirection direction, string externalDocumentId, int schemaVersion, string mappingVersion,
        string payloadJson, string payloadSha256, DateTimeOffset occurredAt, DateTimeOffset receivedAt) : base(id)
    {
        if (connectionId == Guid.Empty || farmId == Guid.Empty || schemaVersion <= 0 || !Enum.IsDefined(documentType) || !Enum.IsDefined(direction) || occurredAt > receivedAt.AddMinutes(5)) throw new ArgumentException("A valid business document exchange is required.");
        ConnectionId = connectionId; FarmId = farmId; DocumentType = documentType; Direction = direction;
        ExternalDocumentId = EquipmentAsset.Text(externalDocumentId, 240); SchemaVersion = schemaVersion;
        MappingVersion = EquipmentAsset.Text(mappingVersion, 60); PayloadJson = EquipmentAsset.Text(payloadJson, 1_000_000);
        PayloadSha256 = EquipmentAsset.Sha256(payloadSha256); OccurredAt = occurredAt.ToUniversalTime(); ReceivedAt = receivedAt.ToUniversalTime(); Status = ExchangeStatus.Received;
    }
    public Guid ConnectionId { get; private set; }
    public Guid FarmId { get; private set; }
    public BusinessDocumentType DocumentType { get; private set; }
    public ExchangeDirection Direction { get; private set; }
    public string ExternalDocumentId { get; private set; } = string.Empty;
    public int SchemaVersion { get; private set; }
    public string MappingVersion { get; private set; } = string.Empty;
    public string PayloadJson { get; private set; } = "{}";
    public string PayloadSha256 { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; private set; }
    public DateTimeOffset ReceivedAt { get; private set; }
    public ExchangeStatus Status { get; private set; }
    public DateTimeOffset? ProcessedAt { get; private set; }
    public Guid? AppliedEntityId { get; private set; }
    public string? RejectionReason { get; private set; }
    public void Validate() { if (Status != ExchangeStatus.Received) throw new InvalidOperationException("Only received documents can be validated."); Status = ExchangeStatus.Validated; }
    public void Complete(DateTimeOffset at, Guid? appliedEntityId) { if (Status != ExchangeStatus.Validated || appliedEntityId == Guid.Empty) throw new InvalidOperationException("Only validated documents can complete with an optional non-empty entity reference."); Status = Direction == ExchangeDirection.Inbound ? ExchangeStatus.Applied : ExchangeStatus.Delivered; ProcessedAt = at.ToUniversalTime(); AppliedEntityId = appliedEntityId; }
    public void Reject(DateTimeOffset at, string reason) { if (Status is ExchangeStatus.Applied or ExchangeStatus.Delivered or ExchangeStatus.Rejected) throw new InvalidOperationException("The document is already terminal."); Status = ExchangeStatus.Rejected; ProcessedAt = at.ToUniversalTime(); RejectionReason = EquipmentAsset.Text(reason, 2_000); }
}

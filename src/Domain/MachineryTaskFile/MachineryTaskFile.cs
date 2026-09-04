using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.MachineryTaskFile;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;

public sealed class MachineryTaskFile : TenantEntity
{
    private MachineryTaskFile() { }
    public MachineryTaskFile(Guid id, Guid farmId, Guid equipmentId, Guid? workOrderId, Guid? fieldId,
        MachineryTaskDirection direction, MachineryTaskFormat format, string fileName, string objectKey,
        long byteSize, string sha256, int schemaVersion, string mappingVersion, string? externalTaskId,
        DateTimeOffset createdAt) : base(id)
    {
        if (farmId == Guid.Empty || equipmentId == Guid.Empty || workOrderId == Guid.Empty || fieldId == Guid.Empty || byteSize <= 0 || schemaVersion <= 0 || !Enum.IsDefined(direction) || !Enum.IsDefined(format)) throw new ArgumentException("A valid task file is required.");
        FarmId = farmId; EquipmentId = equipmentId; WorkOrderId = workOrderId; FieldId = fieldId; Direction = direction; Format = format;
        FileName = EquipmentAsset.Text(fileName, 255); ObjectKey = EquipmentAsset.Text(objectKey, 500); ByteSize = byteSize; Sha256 = EquipmentAsset.Sha256(sha256);
        SchemaVersion = schemaVersion; MappingVersion = EquipmentAsset.Text(mappingVersion, 60); ExternalTaskId = EquipmentAsset.Optional(externalTaskId, 240);
        CreatedAt = createdAt.ToUniversalTime(); Status = ExchangeStatus.Received;
    }
    public Guid FarmId { get; private set; }
    public Guid EquipmentId { get; private set; }
    public Guid? WorkOrderId { get; private set; }
    public Guid? FieldId { get; private set; }
    public MachineryTaskDirection Direction { get; private set; }
    public MachineryTaskFormat Format { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string ObjectKey { get; private set; } = string.Empty;
    public long ByteSize { get; private set; }
    public string Sha256 { get; private set; } = string.Empty;
    public int SchemaVersion { get; private set; }
    public string MappingVersion { get; private set; } = string.Empty;
    public string? ExternalTaskId { get; private set; }
    public ExchangeStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? ProcessedAt { get; private set; }
    public string? RejectionReason { get; private set; }
    public void Validate() { if (Status != ExchangeStatus.Received) throw new InvalidOperationException("Only received task files can be validated."); Status = ExchangeStatus.Validated; }
    public void Complete(DateTimeOffset at) { if (Status != ExchangeStatus.Validated) throw new InvalidOperationException("Only validated task files can be completed."); Status = Direction == MachineryTaskDirection.Import ? ExchangeStatus.Applied : ExchangeStatus.Delivered; ProcessedAt = at.ToUniversalTime(); }
    public void Reject(DateTimeOffset at, string reason) { if (Status is ExchangeStatus.Applied or ExchangeStatus.Delivered or ExchangeStatus.Rejected) throw new InvalidOperationException("The task file is already terminal."); Status = ExchangeStatus.Rejected; ProcessedAt = at.ToUniversalTime(); RejectionReason = EquipmentAsset.Text(reason, 2_000); }
}

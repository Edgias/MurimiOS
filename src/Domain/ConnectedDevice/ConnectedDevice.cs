using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.ConnectedDevice;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;

public sealed class ConnectedDevice : TenantEntity
{
    private ConnectedDevice() { }
    public ConnectedDevice(Guid id, Guid farmId, string deviceKey, string name, ConnectedDeviceKind kind,
        ConnectedDeviceProtocol protocol, Guid? connectionId, Guid? equipmentId, Guid? fieldId, Guid? holdingId,
        string? firmwareVersion, DateTimeOffset provisionedAt) : base(id)
    {
        if (farmId == Guid.Empty || connectionId == Guid.Empty || equipmentId == Guid.Empty || fieldId == Guid.Empty || holdingId == Guid.Empty)
            throw new ArgumentException("A farm is required and optional references must be non-empty.");
        if (!Enum.IsDefined(kind) || !Enum.IsDefined(protocol)) throw new ArgumentOutOfRangeException(nameof(kind));
        if (new[] { equipmentId, fieldId, holdingId }.Count(x => x.HasValue) > 1) throw new ArgumentException("A device can bind to only one equipment, field, or holding target.");
        FarmId = farmId; DeviceKey = EquipmentAsset.Text(deviceKey, 160); Name = EquipmentAsset.Text(name, 200);
        Kind = kind; Protocol = protocol; ConnectionId = connectionId; EquipmentId = equipmentId; FieldId = fieldId; HoldingId = holdingId;
        FirmwareVersion = EquipmentAsset.Optional(firmwareVersion, 80); ProvisionedAt = provisionedAt.ToUniversalTime(); Status = ConnectedDeviceStatus.Provisioned;
    }
    public Guid FarmId { get; private set; }
    public string DeviceKey { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public ConnectedDeviceKind Kind { get; private set; }
    public ConnectedDeviceProtocol Protocol { get; private set; }
    public Guid? ConnectionId { get; private set; }
    public Guid? EquipmentId { get; private set; }
    public Guid? FieldId { get; private set; }
    public Guid? HoldingId { get; private set; }
    public string? FirmwareVersion { get; private set; }
    public ConnectedDeviceStatus Status { get; private set; }
    public DateTimeOffset ProvisionedAt { get; private set; }
    public DateTimeOffset? ActivatedAt { get; private set; }
    public DateTimeOffset? LastSeenAt { get; private set; }
    public DateTimeOffset? RetiredAt { get; private set; }
    public void Activate(DateTimeOffset at) { if (Status is not (ConnectedDeviceStatus.Provisioned or ConnectedDeviceStatus.Suspended)) throw new InvalidOperationException("Only provisioned or suspended devices can activate."); Status = ConnectedDeviceStatus.Active; ActivatedAt ??= at.ToUniversalTime(); }
    public void Suspend() { if (Status != ConnectedDeviceStatus.Active) throw new InvalidOperationException("Only an active device can be suspended."); Status = ConnectedDeviceStatus.Suspended; }
    public void Retire(DateTimeOffset at) { if (Status == ConnectedDeviceStatus.Retired) throw new InvalidOperationException("The device is already retired."); Status = ConnectedDeviceStatus.Retired; RetiredAt = at.ToUniversalTime(); }
    public void RecordSeen(DateTimeOffset at, string? firmwareVersion) { if (Status != ConnectedDeviceStatus.Active) throw new InvalidOperationException("Only active devices can report observations."); DateTimeOffset observed = at.ToUniversalTime(); if (LastSeenAt.HasValue && observed < LastSeenAt) return; LastSeenAt = observed; FirmwareVersion = EquipmentAsset.Optional(firmwareVersion, 80) ?? FirmwareVersion; }
}

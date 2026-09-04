using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.MachineryTelemetryRecord;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;

public sealed class MachineryTelemetryRecord : TenantEntity
{
    private MachineryTelemetryRecord() { }
    public MachineryTelemetryRecord(Guid id, Guid farmId, Guid equipmentId, Guid? deviceId, DateTimeOffset observedAt,
        DateTimeOffset receivedAt, decimal? engineHours, decimal? speedKph, decimal? fuelLevelPercent,
        string state, double? latitude, double? longitude, string externalRecordId, string payloadSha256,
        Guid? ingestionRecordId) : base(id)
    {
        if (farmId == Guid.Empty || equipmentId == Guid.Empty || deviceId == Guid.Empty || ingestionRecordId == Guid.Empty) throw new ArgumentException("Farm and equipment are required; optional references must be non-empty.");
        if (observedAt > receivedAt.AddMinutes(5) || engineHours < 0 || speedKph < 0 || fuelLevelPercent is < 0 or > 100 || latitude is < -90 or > 90 || longitude is < -180 or > 180 || latitude.HasValue != longitude.HasValue) throw new ArgumentException("Telemetry values are invalid.");
        FarmId = farmId; EquipmentId = equipmentId; DeviceId = deviceId; ObservedAt = observedAt.ToUniversalTime(); ReceivedAt = receivedAt.ToUniversalTime();
        EngineHours = engineHours; SpeedKph = speedKph; FuelLevelPercent = fuelLevelPercent; State = EquipmentAsset.Text(state, 80);
        Latitude = latitude; Longitude = longitude; ExternalRecordId = EquipmentAsset.Text(externalRecordId, 240);
        PayloadSha256 = EquipmentAsset.Sha256(payloadSha256); IngestionRecordId = ingestionRecordId;
    }
    public Guid FarmId { get; private set; }
    public Guid EquipmentId { get; private set; }
    public Guid? DeviceId { get; private set; }
    public DateTimeOffset ObservedAt { get; private set; }
    public DateTimeOffset ReceivedAt { get; private set; }
    public decimal? EngineHours { get; private set; }
    public decimal? SpeedKph { get; private set; }
    public decimal? FuelLevelPercent { get; private set; }
    public string State { get; private set; } = string.Empty;
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public string ExternalRecordId { get; private set; } = string.Empty;
    public string PayloadSha256 { get; private set; } = string.Empty;
    public Guid? IngestionRecordId { get; private set; }
}

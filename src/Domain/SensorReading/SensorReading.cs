using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.SensorReading;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;

public sealed class SensorReading : TenantEntity
{
    private SensorReading() { }
    public SensorReading(Guid id, Guid farmId, Guid deviceId, Guid channelId, string metricCode, decimal value,
        string unit, DateTimeOffset observedAt, DateTimeOffset receivedAt, SensorValueQuality quality,
        string externalReadingId, string payloadSha256, Guid? ingestionRecordId, double? latitude, double? longitude) : base(id)
    {
        if (farmId == Guid.Empty || deviceId == Guid.Empty || channelId == Guid.Empty || ingestionRecordId == Guid.Empty || !Enum.IsDefined(quality)) throw new ArgumentException("Farm, device, channel, and quality are required.");
        if (observedAt > receivedAt.AddMinutes(5) || latitude is < -90 or > 90 || longitude is < -180 or > 180 || latitude.HasValue != longitude.HasValue) throw new ArgumentException("Reading timestamps or location are invalid.");
        FarmId = farmId; DeviceId = deviceId; ChannelId = channelId; MetricCode = EquipmentAsset.Text(metricCode, 120).ToLowerInvariant();
        Value = value; Unit = EquipmentAsset.Text(unit, 40); ObservedAt = observedAt.ToUniversalTime(); ReceivedAt = receivedAt.ToUniversalTime();
        Quality = quality; ExternalReadingId = EquipmentAsset.Text(externalReadingId, 240); PayloadSha256 = EquipmentAsset.Sha256(payloadSha256);
        IngestionRecordId = ingestionRecordId; Latitude = latitude; Longitude = longitude;
    }
    public Guid FarmId { get; private set; }
    public Guid DeviceId { get; private set; }
    public Guid ChannelId { get; private set; }
    public string MetricCode { get; private set; } = string.Empty;
    public decimal Value { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public DateTimeOffset ObservedAt { get; private set; }
    public DateTimeOffset ReceivedAt { get; private set; }
    public SensorValueQuality Quality { get; private set; }
    public string ExternalReadingId { get; private set; } = string.Empty;
    public string PayloadSha256 { get; private set; } = string.Empty;
    public Guid? IngestionRecordId { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
}

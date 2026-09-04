using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.ConnectedDevice;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;

public sealed class SensorChannel : TenantEntity
{
    private SensorChannel() { }
    public SensorChannel(Guid id, Guid deviceId, string channelKey, string metricCode, string unit,
        decimal? minimumValidValue, decimal? maximumValidValue, int samplingIntervalSeconds) : base(id)
    {
        if (deviceId == Guid.Empty || samplingIntervalSeconds is < 1 or > 2_592_000 || maximumValidValue < minimumValidValue)
            throw new ArgumentException("A valid device, range, and sampling interval are required.");
        DeviceId = deviceId; ChannelKey = EquipmentAsset.Text(channelKey, 120); MetricCode = EquipmentAsset.Text(metricCode, 120).ToLowerInvariant();
        Unit = EquipmentAsset.Text(unit, 40); MinimumValidValue = minimumValidValue; MaximumValidValue = maximumValidValue;
        SamplingIntervalSeconds = samplingIntervalSeconds; IsActive = true;
    }
    public Guid DeviceId { get; private set; }
    public string ChannelKey { get; private set; } = string.Empty;
    public string MetricCode { get; private set; } = string.Empty;
    public string Unit { get; private set; } = string.Empty;
    public decimal? MinimumValidValue { get; private set; }
    public decimal? MaximumValidValue { get; private set; }
    public int SamplingIntervalSeconds { get; private set; }
    public bool IsActive { get; private set; }
    public void Disable() { if (!IsActive) throw new InvalidOperationException("The channel is already disabled."); IsActive = false; }
    public bool IsWithinRange(decimal value) => (!MinimumValidValue.HasValue || value >= MinimumValidValue) && (!MaximumValidValue.HasValue || value <= MaximumValidValue);
}

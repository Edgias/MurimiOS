using EdgIQ.MurimiOS.Application.Common.Abstractions;
using System.Globalization;
using System.Text.Json;

namespace EdgIQ.MurimiOS.Infrastructure.Integrations;
using MachineryTelemetryValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.MachineryTelemetryValue;
using ProviderAdapterException = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderAdapterException;
using ProviderRecord = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderRecord;
using SensorChannelValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.SensorChannelValue;
using SensorReadingValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.SensorReadingValue;

public static class MqttPayloadDecoder
{
    public static ProviderRecord DecodeSensor(string topic, string payloadJson, string feedConfigurationJson,
        DateTimeOffset receivedAt, bool thingsStack)
    {
        JsonElement payload = ProviderJson.Object(payloadJson, "MQTT payload");
        JsonElement config = ProviderJson.Object(feedConfigurationJson, "feed configuration");
        JsonElement measurements = payload;
        string? deviceKey;
        string? firmware = null;
        DateTimeOffset observedAt;
        string externalId;
        double? latitude = null;
        double? longitude = null;
        if (thingsStack || payload.TryGetProperty("uplink_message", out _))
        {
            JsonElement identifiers = RequiredObject(payload, "end_device_ids");
            JsonElement uplink = RequiredObject(payload, "uplink_message");
            deviceKey = identifiers.String("device_id") ?? identifiers.String("dev_eui");
            measurements = uplink.TryGetProperty("normalized_payload", out JsonElement normalized) && normalized.ValueKind == JsonValueKind.Object
                ? normalized : RequiredObject(uplink, "decoded_payload");
            observedAt = ParseTime(uplink.String("received_at") ?? payload.String("received_at"), receivedAt);
            string frame = uplink.TryGetProperty("f_cnt", out JsonElement frameValue) ? frameValue.ToString() : "0";
            externalId = $"{deviceKey}:{frame}";
            if (uplink.TryGetProperty("version_ids", out JsonElement versions) && versions.ValueKind == JsonValueKind.Object)
                firmware = versions.String("firmware_version");
            if (uplink.TryGetProperty("locations", out JsonElement locations) && locations.ValueKind == JsonValueKind.Object)
            {
                JsonElement location = locations.EnumerateObject().Select(x => x.Value).FirstOrDefault(x => x.ValueKind == JsonValueKind.Object);
                latitude = Double(location, "latitude"); longitude = Double(location, "longitude");
            }
        }
        else
        {
            deviceKey = payload.String(config.String("deviceKeyProperty") ?? "deviceKey");
            observedAt = ParseTime(payload.String(config.String("observedAtProperty") ?? "observedAt"), receivedAt);
            externalId = payload.String(config.String("externalIdProperty") ?? "id") ?? $"{topic}:{observedAt:O}";
            firmware = payload.String(config.String("firmwareProperty") ?? "firmwareVersion");
            string measurementsProperty = config.String("measurementsProperty") ?? "readings";
            if (payload.TryGetProperty(measurementsProperty, out JsonElement configuredMeasurements)) measurements = configuredMeasurements;
            latitude = Double(payload, "latitude"); longitude = Double(payload, "longitude");
        }
        if (string.IsNullOrWhiteSpace(deviceKey)) throw new ProviderAdapterException("mqtt_device_missing", "The MQTT payload does not identify a device.", false);
        if (measurements.ValueKind != JsonValueKind.Object) throw new ProviderAdapterException("mqtt_measurements_invalid", "MQTT measurements must be a JSON object.", false);
        JsonElement units = config.TryGetProperty("units", out JsonElement configuredUnits) && configuredUnits.ValueKind == JsonValueKind.Object
            ? configuredUnits : default;
        string defaultUnit = config.String("defaultUnit") ?? "1";
        var channels = new List<SensorChannelValue>();
        foreach (JsonProperty property in measurements.EnumerateObject())
        {
            if (!property.Value.TryGetDecimal(out decimal value)) continue;
            string unit = units.ValueKind == JsonValueKind.Object && units.TryGetProperty(property.Name, out JsonElement unitValue)
                && unitValue.ValueKind == JsonValueKind.String ? unitValue.GetString()! : defaultUnit;
            channels.Add(new SensorChannelValue(property.Name, value, unit, $"{externalId}:{property.Name}", latitude, longitude));
        }
        if (channels.Count == 0) throw new ProviderAdapterException("mqtt_measurements_empty", "The MQTT payload contains no numeric channel values.", false);
        return new ProviderRecord(externalId, observedAt, null, payloadJson, new SensorReadingValue(deviceKey, firmware, channels));
    }

    public static ProviderRecord DecodeMachinery(string topic, string payloadJson, string feedConfigurationJson,
        DateTimeOffset receivedAt)
    {
        JsonElement payload = ProviderJson.Object(payloadJson, "MQTT machinery payload");
        JsonElement config = ProviderJson.Object(feedConfigurationJson, "feed configuration");
        string? equipmentCode = payload.String(config.String("equipmentCodeProperty") ?? "equipmentCode");
        if (string.IsNullOrWhiteSpace(equipmentCode)) throw new ProviderAdapterException("mqtt_equipment_missing", "The MQTT payload does not identify equipment.", false);
        DateTimeOffset observedAt = ParseTime(payload.String(config.String("observedAtProperty") ?? "observedAt"), receivedAt);
        string externalId = payload.String(config.String("externalIdProperty") ?? "id") ?? $"{topic}:{observedAt:O}";
        var value = new MachineryTelemetryValue(equipmentCode, payload.String("deviceKey"), payload.Decimal("engineHours"),
            payload.Decimal("speedKph"), payload.Decimal("fuelLevelPercent"), payload.String("state") ?? "unknown",
            Double(payload, "latitude"), Double(payload, "longitude"));
        return new ProviderRecord(externalId, observedAt, null, payloadJson, value);
    }

    private static JsonElement RequiredObject(JsonElement value, string name) => value.TryGetProperty(name, out JsonElement result)
        && result.ValueKind == JsonValueKind.Object ? result : throw new ProviderAdapterException("mqtt_payload_invalid", $"MQTT payload requires object '{name}'.", false);
    private static DateTimeOffset ParseTime(string? value, DateTimeOffset fallback) => DateTimeOffset.TryParse(value,
        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset parsed) ? parsed.ToUniversalTime() : fallback;
    private static double? Double(JsonElement value, string name) => value.ValueKind == JsonValueKind.Object
        && value.TryGetProperty(name, out JsonElement property) && property.TryGetDouble(out double result) ? result : null;
}

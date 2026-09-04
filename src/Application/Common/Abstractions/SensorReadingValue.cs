using System.Text.Json;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed record SensorReadingValue(string DeviceKey, string? FirmwareVersion,
    IReadOnlyList<SensorChannelValue> Channels) : ProviderNormalizedRecord;

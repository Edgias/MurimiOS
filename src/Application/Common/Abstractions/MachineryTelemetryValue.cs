using System.Text.Json;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed record MachineryTelemetryValue(string EquipmentCode, string? DeviceKey, decimal? EngineHours,
    decimal? SpeedKph, decimal? FuelLevelPercent, string State, double? Latitude, double? Longitude)
    : ProviderNormalizedRecord;

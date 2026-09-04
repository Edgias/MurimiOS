using System.Text.Json;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed record SensorChannelValue(string ChannelKey, decimal Value, string Unit, string ExternalReadingId,
    double? Latitude = null, double? Longitude = null);

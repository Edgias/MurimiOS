using System.Text.Json;
using EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;
using SatelliteFieldStatistic = global::EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic.SatelliteFieldStatistic;

using VegetationIndex = global::EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic.VegetationIndex;

public sealed record SatelliteStatisticValue(
    VegetationIndex Index,
    string? CustomIndex,
    decimal Mean,
    decimal Minimum,
    decimal Maximum,
    decimal StandardDeviation,
    decimal ValidPixelPercent,
    decimal CloudPercent) : ProviderNormalizedRecord;

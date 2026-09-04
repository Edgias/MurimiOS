using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WeatherLocation;
using EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.IngestSatellite;
using VegetationIndex = global::EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic.VegetationIndex;
using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;
public sealed record SatelliteStatisticItem(Guid? Id, string ProviderAcquisitionId, VegetationIndex Index,
    string? CustomIndex, DateTimeOffset AcquiredAt, decimal Mean, decimal Minimum, decimal Maximum,
    decimal StandardDeviation, decimal ValidPixelPercent, decimal CloudPercent, InsightQuality Quality);

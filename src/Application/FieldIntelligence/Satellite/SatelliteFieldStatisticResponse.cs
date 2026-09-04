using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WeatherLocation;
using EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.Satellite;
using WeatherLocation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherLocation;

using SatelliteFieldStatistic = global::EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic.SatelliteFieldStatistic;
using VegetationIndex = global::EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic.VegetationIndex;
using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;
public sealed record SatelliteFieldStatisticResponse(Guid Id, Guid FarmId, Guid FieldId,
    Guid BoundaryVersionId, Guid FeedId, Guid IngestionRecordId, string ProviderAcquisitionId,
    VegetationIndex Index, string? CustomIndex, DateTimeOffset AcquiredAt, DateTimeOffset ProcessedAt,
    decimal Mean, decimal Minimum, decimal Maximum, decimal StandardDeviation,
    decimal ValidPixelPercent, decimal CloudPercent, InsightQuality Quality);

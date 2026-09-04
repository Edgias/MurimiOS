using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WeatherLocation;
using EdgIQ.MurimiOS.Domain.FieldMapLayer;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.MapLayers;
using WeatherLocation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherLocation;

using FieldMapLayer = global::EdgIQ.MurimiOS.Domain.FieldMapLayer.FieldMapLayer;
using MapLayerKind = global::EdgIQ.MurimiOS.Domain.FieldMapLayer.MapLayerKind;
using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;
public sealed record FieldMapLayerResponse(Guid Id, Guid FarmId, Guid FieldId, Guid BoundaryVersionId,
    MapLayerKind Kind, string Name, string SourceType, Guid SourceId, DateTimeOffset SourceObservedAt,
    DateTimeOffset GeneratedAt, DateTimeOffset ValidFrom, DateTimeOffset? ValidTo,
    JsonElement GeoJson, JsonElement Style, InsightQuality Quality);

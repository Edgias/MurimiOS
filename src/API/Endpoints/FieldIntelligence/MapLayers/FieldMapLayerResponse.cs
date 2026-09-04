using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WeatherLocation;
using EdgIQ.MurimiOS.Domain.FieldMapLayer;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.MapLayers;
using MapLayerKind = global::EdgIQ.MurimiOS.Domain.FieldMapLayer.MapLayerKind;
using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;
public sealed record FieldMapLayerResponse(Guid Id, Guid FarmId, Guid FieldId, Guid BoundaryVersionId,
    MapLayerKind Kind, string Name, string SourceType, Guid SourceId, DateTimeOffset SourceObservedAt,
    DateTimeOffset GeneratedAt, DateTimeOffset ValidFrom, DateTimeOffset? ValidTo,
    JsonElement GeoJson, JsonElement Style, InsightQuality Quality);

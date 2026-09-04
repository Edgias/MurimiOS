using System.Text.Json;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed record ProviderSpatialTarget(
    Guid? FieldId,
    Guid? BoundaryVersionId,
    decimal Latitude,
    decimal Longitude,
    string? BoundaryGeoJson);

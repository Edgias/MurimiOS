using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Production.Shared;

public sealed record CropZoneResponse(
    Guid Id,
    Guid SeasonId,
    Guid FieldId,
    Guid BoundaryVersionId,
    string Name,
    string Crop,
    string? Variety,
    decimal AreaHa);

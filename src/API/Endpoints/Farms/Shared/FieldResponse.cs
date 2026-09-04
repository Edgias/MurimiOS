using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Farms.Shared;

public sealed record FieldResponse(
    Guid Id,
    Guid FarmId,
    string Name,
    Guid BoundaryId,
    decimal AreaHa,
    DateOnly ValidFrom);

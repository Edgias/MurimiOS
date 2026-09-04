using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Farms.PostFarmsFarmIdGuidFields;
using CoordinateRequest = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.Shared.CoordinateRequest;

public sealed record CreateFieldRequest(
    Guid? Id,
    Guid? BoundaryId,
    string Name,
    DateOnly ValidFrom,
    IReadOnlyList<CoordinateRequest> Exterior,
    string? Source);

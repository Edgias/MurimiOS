using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Production.PostApiSeasonsSeasonIdGuidCropZones;
using CoordinateRequest = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.Shared.CoordinateRequest;

public sealed record CreateCropZoneRequest(
    Guid? Id,
    Guid FieldId,
    Guid BoundaryVersionId,
    string Name,
    string Crop,
    string? Variety,
    IReadOnlyList<CoordinateRequest>? Exterior);

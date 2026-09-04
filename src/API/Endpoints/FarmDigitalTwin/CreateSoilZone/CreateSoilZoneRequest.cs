using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CreateSoilZone;
using CoordinateRequest = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.Shared.CoordinateRequest;
public sealed record CreateSoilZoneRequest(Guid? Id, Guid? BoundaryId, Guid FieldId, Guid FieldBoundaryVersionId,
    string Code, string Name, string? Classification, string? ManagementGuidance,
    IReadOnlyList<CoordinateRequest> Exterior, DateOnly ValidFrom, string? Source);

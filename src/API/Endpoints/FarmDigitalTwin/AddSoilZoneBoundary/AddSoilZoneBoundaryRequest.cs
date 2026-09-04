using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.AddSoilZoneBoundary;
using CoordinateRequest = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.Shared.CoordinateRequest;
public sealed record AddSoilZoneBoundaryRequest(Guid? Id, Guid FieldBoundaryVersionId,
    IReadOnlyList<CoordinateRequest> Exterior, DateOnly ValidFrom, string? Source);

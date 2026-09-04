using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.CreateScouting;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;
using ScoutingCategory = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingCategory;
public sealed record CreateScoutingObservationRequest(Guid? Id, Guid? BoundaryVersionId, Guid? CropZoneId,
    ScoutingCategory Category, InsightSeverity Severity, string? IssueCode, string Title, string Notes,
    DateTimeOffset ObservedAt, decimal? Latitude, decimal? Longitude, Guid? EvidenceId, Guid? SourceCommandId);

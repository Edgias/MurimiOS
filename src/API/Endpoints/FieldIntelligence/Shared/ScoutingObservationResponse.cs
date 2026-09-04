using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Shared;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;
using ScoutingCategory = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingCategory;
public sealed record ScoutingObservationResponse(Guid Id, Guid FarmId, Guid FieldId,
    Guid BoundaryVersionId, Guid? CropZoneId, ScoutingCategory Category, InsightSeverity Severity,
    string? IssueCode, string Title, string Notes, DateTimeOffset ObservedAt, DateTimeOffset RecordedAt,
    string RecordedBy, double? Latitude, double? Longitude, Guid? EvidenceId, Guid? SourceCommandId);

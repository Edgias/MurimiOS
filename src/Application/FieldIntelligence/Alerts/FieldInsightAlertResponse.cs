using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;
using EdgIQ.MurimiOS.Domain.FieldInsightAlert;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.Alerts;
using ScoutingObservation = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingObservation;

using FieldInsightAlert = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.FieldInsightAlert;
using InsightAlertStatus = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.InsightAlertStatus;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;
public sealed record FieldInsightAlertResponse(Guid Id, Guid FarmId, Guid FieldId,
    Guid BoundaryVersionId, string AlertKey, string SourceType, Guid SourceId, InsightSeverity Severity,
    string Title, string Message, DateTimeOffset DetectedAt, DateTimeOffset ObservedAt,
    DateTimeOffset? ExpiresAt, InsightAlertStatus Status, DateTimeOffset? AcknowledgedAt,
    string? AcknowledgedBy, DateTimeOffset? ResolvedAt, string? ResolvedBy, string? Resolution,
    DateTimeOffset? DismissedAt, string? DismissedBy, string? DismissalReason,
    Guid? FollowUpWorkOrderId, uint Version);

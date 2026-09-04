using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;
using EdgIQ.MurimiOS.Domain.FieldInsightAlert;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.AcknowledgeAlert;
using ScoutingObservation = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingObservation;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using FieldInsightAlert = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.FieldInsightAlert;
using InsightAlertStatus = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.InsightAlertStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
public sealed record FieldInsightAlertResponse(Guid Id, Guid FarmId, Guid FieldId,
    Guid BoundaryVersionId, string AlertKey, string SourceType, Guid SourceId, InsightSeverity Severity,
    string Title, string Message, DateTimeOffset DetectedAt, DateTimeOffset ObservedAt,
    DateTimeOffset? ExpiresAt, InsightAlertStatus Status, DateTimeOffset? AcknowledgedAt,
    string? AcknowledgedBy, DateTimeOffset? ResolvedAt, string? ResolvedBy, string? Resolution,
    DateTimeOffset? DismissedAt, string? DismissedBy, string? DismissalReason,
    Guid? FollowUpWorkOrderId, uint Version);

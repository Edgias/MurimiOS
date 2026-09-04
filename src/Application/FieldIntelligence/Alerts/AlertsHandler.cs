using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;
using EdgIQ.MurimiOS.Domain.FieldInsightAlert;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.Alerts;
using ScoutingObservation = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingObservation;

using FieldInsightAlert = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.FieldInsightAlert;
using InsightAlertStatus = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.InsightAlertStatus;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;

public sealed class AlertsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<AlertsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(AlertsQuery message, CancellationToken cancellationToken) => AlertsAsync(message.farmId, message.fieldId, message.status, message.minimumSeverity, message.limit, db, scope, cancellationToken);

    private static async Task<UseCaseResult> AlertsAsync(Guid? farmId, Guid? fieldId, InsightAlertStatus? status,
        InsightSeverity? minimumSeverity, int? limit, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken)
    {
        ResolvedOrganisationScope resolved = await scope.ResolveAsync(cancellationToken);
        if (farmId is { } requested && !resolved.AllowsFarm(requested)) return UseCaseResults.NotFound();
        IQueryable<FieldInsightAlert> query = db.FieldInsightAlerts.AsNoTracking();
        if (farmId is { } farm) query = query.Where(x => x.FarmId == farm);
        else if (!resolved.IsUnrestricted) query = query.Where(x => resolved.FarmIds!.Contains(x.FarmId));
        if (fieldId is { } field) query = query.Where(x => x.FieldId == field);
        if (status is { } state) query = query.Where(x => x.Status == state);
        if (minimumSeverity is { } severity) query = query.Where(x => x.Severity >= severity);
        FieldInsightAlert[] rows = await query.OrderByDescending(x => x.Status == InsightAlertStatus.Open)
            .ThenByDescending(x => x.Severity).ThenByDescending(x => x.DetectedAt)
            .Take(Math.Clamp(limit ?? 250, 1, 2_000)).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows.Select(ToAlert).ToArray());
    }

    private static FieldInsightAlertResponse ToAlert(FieldInsightAlert x) => new(x.Id, x.FarmId, x.FieldId,
        x.BoundaryVersionId, x.AlertKey, x.SourceType, x.SourceId, x.Severity, x.Title, x.Message,
        x.DetectedAt, x.ObservedAt, x.ExpiresAt, x.Status, x.AcknowledgedAt, x.AcknowledgedBy,
        x.ResolvedAt, x.ResolvedBy, x.Resolution, x.DismissedAt, x.DismissedBy, x.DismissalReason, x.FollowUpWorkOrderId, x.Version);

}

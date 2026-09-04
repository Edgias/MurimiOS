using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.FieldInsightAlert;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.DismissAlert;
using ScoutingObservation = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingObservation;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using FieldInsightAlert = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.FieldInsightAlert;
using InsightAlertStatus = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.InsightAlertStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class DismissAlertHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<DismissAlertCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(DismissAlertCommand message, CancellationToken cancellationToken) => DismissAlertAsync(message.alertId, message.request, db, scope, tenant, actor, clock, context, cancellationToken);
    private static Task<UseCaseResult> DismissAlertAsync(Guid alertId, AlertDecisionRequest request, IPlatformDbContext db,
        IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock,
        UseCaseContext context, CancellationToken cancellationToken) =>        ChangeAlertAsync(alertId, "dismiss", request.Reason, db, scope, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> ChangeAlertAsync(Guid alertId, string command, string? reason,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        FieldInsightAlert? alert = await db.FieldInsightAlerts.SingleOrDefaultAsync(x => x.Id == alertId, cancellationToken);
        if (alert is null || !await scope.CanAccessFarmAsync(alert.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        DateTimeOffset now = clock.GetUtcNow();
        try
        {
            if (command == "acknowledge") alert.Acknowledge(now, actor.Subject);
            else if (command == "resolve") alert.Resolve(now, actor.Subject, reason!);
            else alert.Dismiss(now, actor.Subject, reason!);
        }
        catch (ArgumentException ex) { return Bad("alert_decision_invalid", ex.Message); }
        catch (InvalidOperationException ex) { return Conflict("alert_transition_invalid", ex.Message); }
        string action = command == "acknowledge" ? "InsightAlertAcknowledged" : command == "resolve" ? "InsightAlertResolved" : "InsightAlertDismissed";
        AddEvent(db, tenant, actor, nameof(FieldInsightAlert), alert.Id, alert.FarmId, action,
            $"FieldIntelligence.{action}", new { alert.Id, alert.FieldId, alert.Status, Reason = reason }, now, context);
        try { await db.SaveChangesAsync(cancellationToken); }
        catch (DbUpdateException ex) { return Conflict("alert_decision_conflict", DbMessage(ex)); }
        return UseCaseResults.Ok(ToAlert(alert));
    }

    private static FieldInsightAlertResponse ToAlert(FieldInsightAlert x) => new(x.Id, x.FarmId, x.FieldId,
        x.BoundaryVersionId, x.AlertKey, x.SourceType, x.SourceId, x.Severity, x.Title, x.Message,
        x.DetectedAt, x.ObservedAt, x.ExpiresAt, x.Status, x.AcknowledgedAt, x.AcknowledgedBy,
        x.ResolvedAt, x.ResolvedBy, x.Resolution, x.DismissedAt, x.DismissedBy, x.DismissalReason, x.FollowUpWorkOrderId, x.Version);


    private static void AddEvent(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string entityType, Guid entityId, Guid? farmId, string action, string eventType, object payload,
        DateTimeOffset now, UseCaseContext context)
    {
        string json = JsonSerializer.Serialize(payload);
        string syncOperation = action switch
        {
            "InsightAlertAcknowledged" => "AlertAcknowledged",
            "InsightAlertDismissed" => "AlertDismissed",
            "AlertWorkOrderCreated" => "AlertWorkCreated",
            _ => action
        };
        if (farmId is { } farm) db.SyncChanges.Add(new SyncChange(tenant.TenantId, farm, entityType, entityId, syncOperation, json, now));
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, entityType, entityId, farmId, actor.ScopeOrganisationNodeId, json,
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now));
    }

    private static string DbMessage(DbUpdateException ex) => ex.InnerException?.Message ?? ex.Message;

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

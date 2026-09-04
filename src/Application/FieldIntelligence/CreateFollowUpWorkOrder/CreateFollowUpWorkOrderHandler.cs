using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.FieldInsightAlert;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.CreateFollowUpWorkOrder;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using FieldInsightAlert = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.FieldInsightAlert;
using InsightAlertStatus = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.InsightAlertStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using WorkOrderPriority = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderPriority;
using WorkOrderStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatus;

public sealed class CreateFollowUpWorkOrderHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<CreateFollowUpWorkOrderCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateFollowUpWorkOrderCommand message, CancellationToken cancellationToken) => CreateFollowUpWorkOrderAsync(message.alertId, message.request, db, scope, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> CreateFollowUpWorkOrderAsync(Guid alertId,
        CreateAlertWorkOrderRequest request, IPlatformDbContext db, IOrganisationScopeAccess scope,
        ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context,
        CancellationToken cancellationToken)
    {
        FieldInsightAlert? alert = await db.FieldInsightAlerts.SingleOrDefaultAsync(x => x.Id == alertId, cancellationToken);
        if (alert is null || !await scope.CanAccessFarmAsync(alert.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        if (alert.Status is InsightAlertStatus.Resolved or InsightAlertStatus.Dismissed) return Conflict("alert_closed", "A closed alert cannot create follow-up work.");
        try
        {
            DateTimeOffset now = clock.GetUtcNow(); Guid id = NonEmptyOrNew(request.Id);
            string number = string.IsNullOrWhiteSpace(request.Number) ? $"WO-{now:yyyyMMdd}-{id.ToString("N")[..8].ToUpperInvariant()}" : request.Number;
            var order = new WorkOrder(id, alert.FarmId, number, request.Title, request.WorkType,
                request.Priority, request.ScheduledStart, request.ScheduledEnd, request.Instructions,
                actor.Subject, now, alert.FieldId, alert.BoundaryVersionId);
            if (request.PlanImmediately) order.Plan(now);
            db.WorkOrders.Add(order); alert.LinkWorkOrder(id);
            AddEvent(db, tenant, actor, nameof(FieldInsightAlert), alert.Id, alert.FarmId,
                "AlertWorkOrderCreated", "FieldIntelligence.AlertWorkOrderCreated", new { alert.Id, WorkOrderId = id, order.Number, order.Status }, now, context);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/operations/work-orders/{id}", new AlertWorkOrderResponse(alert.Id, id, order.Number, order.Status));
        }
        catch (ArgumentException ex) { return Bad("alert_work_order_invalid", ex.Message); }
        catch (InvalidOperationException ex) { return Conflict("alert_work_order_conflict", ex.Message); }
        catch (DbUpdateException ex) { return Conflict("alert_work_order_conflict", DbMessage(ex)); }
    }


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

    private static Guid NonEmptyOrNew(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static string DbMessage(DbUpdateException ex) => ex.InnerException?.Message ?? ex.Message;

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

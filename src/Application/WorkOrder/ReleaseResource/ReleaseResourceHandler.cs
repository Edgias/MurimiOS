using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.WorkOrder.ReleaseResource;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;

using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using WorkOrderResource = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderResource;
using WorkResourceStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResourceStatus;
using WorkResourceType = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResourceType;

public sealed class ReleaseResourceHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<ReleaseResourceCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ReleaseResourceCommand message, CancellationToken cancellationToken) => ReleaseResourceAsync(message.workOrderId, message.resourceId, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> ReleaseResourceAsync(Guid workOrderId, Guid resourceId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock,
        UseCaseContext context, CancellationToken cancellationToken)
    {
        WorkOrder? order = await FindAccessibleAsync(workOrderId, db, scope, cancellationToken);
        if (order is null) return UseCaseResults.NotFound();
        WorkOrderResource? resource = await db.WorkOrderResources.SingleOrDefaultAsync(x => x.Id == resourceId && x.WorkOrderId == order.Id, cancellationToken);
        if (resource is null) return UseCaseResults.NotFound();
        try { resource.Release(); }
        catch (InvalidOperationException ex) { return Conflict("resource_not_releasable", ex.Message); }
        DateTimeOffset now = clock.GetUtcNow(); AddRecords(db, tenant, actor, order, "ResourceReleased", "WorkOrder.ResourceReleased", now, context, new { resourceId });
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Ok(ToResource(resource));
    }


    private static async Task<WorkOrder?> FindAccessibleAsync(Guid id, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        WorkOrder? order = await db.WorkOrders.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return order is not null && await scope.CanAccessFarmAsync(order.FarmId, cancellationToken) ? order : null;
    }

    private static WorkOrderResourceResponse ToResource(WorkOrderResource x) => new(x.Id, x.ResourceType, x.ResourceName, x.ResourceReference,
        x.AgriculturalInputId, x.StockLotId, x.Quantity, x.Unit, x.EnteredQuantity, x.EnteredUnit, x.UnitCost, x.Currency, x.PlannedCost, x.Status);

    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        WorkOrder order, string action, string eventType, DateTimeOffset now, UseCaseContext context, object? additional = null)
    {
        object payload = new { order.Id, order.FarmId, order.Number, order.Status, order.ScheduledStart, order.ScheduledEnd,
            order.FieldId, order.CropZoneId, order.LivestockSubjectId, order.CropPlanId, order.PlannedOperationId, Additional = additional };
        string json = JsonSerializer.Serialize(payload);
        db.SyncChanges.Add(new SyncChange(tenant.TenantId, order.FarmId, nameof(WorkOrder), order.Id, action, json, now));
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, nameof(WorkOrder), order.Id, order.FarmId, actor.ScopeOrganisationNodeId, json,
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now));
    }

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

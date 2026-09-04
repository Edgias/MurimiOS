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
namespace EdgIQ.MurimiOS.Application.WorkOrder.AddResource;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using MeasurementUnits = global::EdgIQ.MurimiOS.Domain.Common.MeasurementUnits;

using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using WorkOrderResource = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderResource;
using WorkOrderStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatus;
using WorkResourceStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResourceStatus;
using WorkResourceType = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResourceType;

public sealed class AddResourceHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<AddResourceCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(AddResourceCommand message, CancellationToken cancellationToken) => AddResourceAsync(message.workOrderId, message.request, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> AddResourceAsync(Guid workOrderId, AddWorkOrderResourceRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        WorkOrder? order = await FindAccessibleAsync(workOrderId, db, scope, cancellationToken);
        if (order is null) return UseCaseResults.NotFound();
        if (order.Status is not (WorkOrderStatus.Draft or WorkOrderStatus.Planned or WorkOrderStatus.Assigned))
            return Conflict("resources_locked", "Resources can only be reserved before work starts.");
        if (request.AgriculturalInputId is { } inputId && !await db.AgriculturalInputs.AnyAsync(x => x.Id == inputId && x.FarmId == order.FarmId, cancellationToken))
            return Bad("input_scope_invalid", "The agricultural input must belong to the work-order farm.");
        if (request.StockLotId is { } lotId)
        {
            var lot = await db.InputStockLots.AsNoTracking().Where(x => x.Id == lotId)
                .Select(x => new { x.InputId, x.QuantityOnHand, x.ExpiresOn }).SingleOrDefaultAsync(cancellationToken);
            if (lot is null || lot.InputId != request.AgriculturalInputId) return Bad("stock_lot_invalid", "The stock lot must belong to the selected agricultural input.");
            decimal reserved = await db.WorkOrderResources.Where(x => x.StockLotId == lotId && x.Status == WorkResourceStatus.Reserved).SumAsync(x => (decimal?)x.Quantity, cancellationToken) ?? 0m;
            decimal canonical;
            try { canonical = EdgIQ.MurimiOS.Domain.Common.MeasurementUnits.Capture(request.Quantity, request.Unit, request.Unit).CanonicalValue; }
            catch (ArgumentException ex) { return Bad("resource_invalid", ex.Message); }
            if (reserved + canonical > lot.QuantityOnHand) return Conflict("stock_over_reserved", "The requested quantity exceeds unreserved stock on hand.");
            if (lot.ExpiresOn is { } expiry && expiry < DateOnly.FromDateTime(order.ScheduledStart.UtcDateTime)) return Conflict("stock_expired", "The stock lot expires before the scheduled work.");
        }
        if (request.ResourceType == WorkResourceType.Machinery && !string.IsNullOrWhiteSpace(request.ResourceReference)
            && await HasResourceConflictAsync(order, request.ResourceReference.Trim(), db, cancellationToken))
            return Conflict("resource_schedule_conflict", "The machinery is reserved for another active work order during this schedule.");
        try
        {
            var resource = new WorkOrderResource(NonEmptyOrNew(request.Id), order.Id, request.ResourceType,
                request.ResourceName, request.Quantity, request.Unit, request.UnitCostPerCanonicalUnit, request.Currency,
                request.AgriculturalInputId, request.StockLotId, request.ResourceReference);
            db.WorkOrderResources.Add(resource);
            DateTimeOffset now = clock.GetUtcNow();
            AddRecords(db, tenant, actor, order, "ResourceReserved", "WorkOrder.ResourceReserved", now, context,
                new { resource.Id, resource.ResourceType, resource.ResourceName, resource.Quantity, resource.Unit, resource.ResourceReference });
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/operations/work-orders/{order.Id}/resources/{resource.Id}", ToResource(resource));
        }
        catch (ArgumentException ex) { return Bad("resource_invalid", ex.Message); }
        catch (DbUpdateException ex) { return Conflict("resource_conflict", DatabaseMessage(ex)); }
    }


    private static async Task<WorkOrder?> FindAccessibleAsync(Guid id, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        WorkOrder? order = await db.WorkOrders.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return order is not null && await scope.CanAccessFarmAsync(order.FarmId, cancellationToken) ? order : null;
    }

    private static Task<bool> HasResourceConflictAsync(WorkOrder order, string reference, IPlatformDbContext db, CancellationToken cancellationToken) =>
        db.WorkOrders.AnyAsync(other => other.Id != order.Id && other.FarmId == order.FarmId
            && other.Status != WorkOrderStatus.Cancelled && other.Status != WorkOrderStatus.Superseded
            && other.ScheduledStart < order.ScheduledEnd && other.ScheduledEnd > order.ScheduledStart
            && db.WorkOrderResources.Any(r => r.WorkOrderId == other.Id && r.ResourceType == WorkResourceType.Machinery
                && r.ResourceReference == reference && r.Status == WorkResourceStatus.Reserved), cancellationToken);

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

    private static Guid NonEmptyOrNew(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static string DatabaseMessage(DbUpdateException ex) => ex.InnerException?.Message ?? ex.Message;

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

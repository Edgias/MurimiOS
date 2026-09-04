using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.WorkOrder.List;

using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using WorkOrderPriority = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderPriority;
using WorkOrderStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatus;
using WorkResourceStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResourceStatus;

public sealed class ListHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListQuery message, CancellationToken cancellationToken) => ListAsync(message.farmId, message.status, message.assigneeId, message.from, message.to, message.limit, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListAsync(Guid? farmId, WorkOrderStatus? status, Guid? assigneeId,
        DateTimeOffset? from, DateTimeOffset? to, int? limit, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        int pageSize = Math.Clamp(limit ?? 100, 1, 500);
        ResolvedOrganisationScope resolved = await scope.ResolveAsync(cancellationToken);
        if (farmId is { } requestedFarm && !resolved.AllowsFarm(requestedFarm)) return UseCaseResults.NotFound();
        IQueryable<WorkOrder> query = db.WorkOrders.AsNoTracking();
        if (farmId is { } id) query = query.Where(x => x.FarmId == id);
        else if (!resolved.IsUnrestricted) query = query.Where(x => resolved.FarmIds!.Contains(x.FarmId));
        if (status is { } state) query = query.Where(x => x.Status == state);
        if (from is { } starts) query = query.Where(x => x.ScheduledEnd > starts.ToUniversalTime());
        if (to is { } ends) query = query.Where(x => x.ScheduledStart < ends.ToUniversalTime());
        if (assigneeId is { } member)
            query = query.Where(order => db.WorkOrderAssignments.Any(a => a.WorkOrderId == order.Id && a.MembershipId == member && a.UnassignedAt == null));
        WorkOrder[] orders = await query.OrderBy(x => x.ScheduledStart).ThenByDescending(x => x.Priority).Take(pageSize).ToArrayAsync(cancellationToken);
        Guid[] ids = orders.Select(x => x.Id).ToArray();
        var metrics = await db.WorkOrderAssignments.AsNoTracking().Where(x => ids.Contains(x.WorkOrderId) && x.UnassignedAt == null)
            .GroupBy(x => x.WorkOrderId).Select(x => new { Id = x.Key, Count = x.Count() }).ToDictionaryAsync(x => x.Id, x => x.Count, cancellationToken);
        var costs = await db.WorkOrderResources.AsNoTracking().Where(x => ids.Contains(x.WorkOrderId) && x.Status != WorkResourceStatus.Released)
            .GroupBy(x => x.WorkOrderId).Select(x => new { Id = x.Key, Cost = x.Sum(r => r.Quantity * r.UnitCost) }).ToDictionaryAsync(x => x.Id, x => x.Cost, cancellationToken);
        return UseCaseResults.Ok(orders.Select(x => ToSummary(x, metrics.GetValueOrDefault(x.Id), costs.GetValueOrDefault(x.Id))).ToArray());
    }

    private static WorkOrderSummaryResponse ToSummary(WorkOrder x, int assignees, decimal plannedCost) => new(x.Id, x.FarmId, x.Number,
        x.Title, x.WorkType, x.Priority, x.Status, x.ScheduledStart, x.ScheduledEnd, x.FieldId, x.BoundaryVersionId,
        x.CropZoneId, x.LivestockSubjectId, x.CropPlanId, x.PlannedOperationId, x.PreviousWorkOrderId,
        x.SupersededByWorkOrderId, assignees, decimal.Round(plannedCost, 2, MidpointRounding.AwayFromZero), x.Version);

}

using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.WorkOrder.Conflicts;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using WorkOrderPriority = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderPriority;
using WorkOrderStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatus;
using WorkResourceStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResourceStatus;

public sealed class ConflictsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ConflictsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ConflictsQuery message, CancellationToken cancellationToken) => ConflictsAsync(message.farmId, message.from, message.to, message.membershipId, message.resourceReference, message.excludeWorkOrderId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ConflictsAsync(Guid farmId, DateTimeOffset from, DateTimeOffset to,
        Guid? membershipId, string? resourceReference, Guid? excludeWorkOrderId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessFarmAsync(farmId, cancellationToken)) return UseCaseResults.NotFound();
        if (to <= from) return Bad("conflict_window_invalid", "Conflict window end must be later than start.");
        IQueryable<WorkOrder> query = db.WorkOrders.AsNoTracking().Where(x => x.FarmId == farmId
            && x.Status != WorkOrderStatus.Cancelled && x.Status != WorkOrderStatus.Superseded
            && x.ScheduledStart < to && x.ScheduledEnd > from);
        if (excludeWorkOrderId is { } excluded) query = query.Where(x => x.Id != excluded);
        if (membershipId is { } member) query = query.Where(x => db.WorkOrderAssignments.Any(a => a.WorkOrderId == x.Id && a.MembershipId == member && a.UnassignedAt == null));
        if (!string.IsNullOrWhiteSpace(resourceReference))
        {
            string reference = resourceReference.Trim();
            query = query.Where(x => db.WorkOrderResources.Any(r => r.WorkOrderId == x.Id && r.ResourceReference == reference && r.Status == WorkResourceStatus.Reserved));
        }
        WorkOrder[] conflicts = await query.OrderBy(x => x.ScheduledStart).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(conflicts.Select(x => ToSummary(x, 0, 0)).ToArray());
    }

    private static WorkOrderSummaryResponse ToSummary(WorkOrder x, int assignees, decimal plannedCost) => new(x.Id, x.FarmId, x.Number,
        x.Title, x.WorkType, x.Priority, x.Status, x.ScheduledStart, x.ScheduledEnd, x.FieldId, x.BoundaryVersionId,
        x.CropZoneId, x.LivestockSubjectId, x.CropPlanId, x.PlannedOperationId, x.PreviousWorkOrderId,
        x.SupersededByWorkOrderId, assignees, decimal.Round(plannedCost, 2, MidpointRounding.AwayFromZero), x.Version);

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

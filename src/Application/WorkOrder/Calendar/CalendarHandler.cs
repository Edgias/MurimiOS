using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using EdgIQ.MurimiOS.Domain.CropPlan;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.WorkOrder.Calendar;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;

using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using WorkOrderStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatus;

public sealed class CalendarHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<CalendarQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CalendarQuery message, CancellationToken cancellationToken) => CalendarAsync(message.farmId, message.from, message.to, message.assigneeId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> CalendarAsync(Guid farmId, DateTimeOffset from, DateTimeOffset to,
        Guid? assigneeId, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessFarmAsync(farmId, cancellationToken)) return UseCaseResults.NotFound();
        if (to <= from || to - from > TimeSpan.FromDays(370)) return Bad("calendar_window_invalid", "Calendar end must be after start and the window cannot exceed 370 days.");
        string timeZone = await db.Farms.Where(x => x.Id == farmId).Select(x => x.TimeZone).SingleOrDefaultAsync(cancellationToken) ?? "UTC";
        IQueryable<WorkOrder> query = db.WorkOrders.AsNoTracking().Where(x => x.FarmId == farmId && x.ScheduledEnd > from && x.ScheduledStart < to);
        if (assigneeId is { } member) query = query.Where(x => db.WorkOrderAssignments.Any(a => a.WorkOrderId == x.Id && a.MembershipId == member && a.UnassignedAt == null));
        WorkOrder[] orders = await query.OrderBy(x => x.ScheduledStart).ToArrayAsync(cancellationToken);
        var items = orders.Select(x => new OperationalCalendarItem("WorkOrder", x.Id, x.Number, x.Title, x.WorkType,
            x.Status.ToString(), x.ScheduledStart, x.ScheduledEnd, x.FieldId, x.CropZoneId, x.LivestockSubjectId, x.CropPlanId, x.PlannedOperationId, false)).ToList();
        if (assigneeId is null)
        {
            Guid[] materialized = await db.WorkOrders.Where(x => x.FarmId == farmId && x.PlannedOperationId != null && x.Status != WorkOrderStatus.Superseded).Select(x => x.PlannedOperationId!.Value).ToArrayAsync(cancellationToken);
            DateOnly fromDate = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(from, ResolveTimeZone(timeZone)).DateTime);
            DateOnly toDate = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(to, ResolveTimeZone(timeZone)).DateTime);
            var planned = await (from operation in db.PlannedCropOperations.AsNoTracking()
                join plan in db.CropPlans.AsNoTracking() on operation.PlanId equals plan.Id
                join allocation in db.CropPlanAllocations.AsNoTracking() on operation.AllocationId equals allocation.Id
                where plan.FarmId == farmId && plan.Status == CropPlanStatus.Approved
                    && operation.PlannedEnd >= fromDate && operation.PlannedStart <= toDate && !materialized.Contains(operation.Id)
                select new { Operation = operation, Plan = plan, Allocation = allocation }).ToArrayAsync(cancellationToken);
            items.AddRange(planned.Select(x => new OperationalCalendarItem("CropPlan", x.Operation.Id, null,
                $"{x.Operation.OperationType} - {x.Allocation.Crop}", x.Operation.OperationType, "ApprovedPlan",
                LocalToUtc(x.Operation.PlannedStart, timeZone), LocalToUtc(x.Operation.PlannedEnd.AddDays(1), timeZone),
                x.Allocation.FieldId, x.Allocation.CropZoneId, null, x.Plan.Id, x.Operation.Id, true)));
        }
        return UseCaseResults.Ok(new OperationalCalendarResponse(farmId, timeZone, from, to, items.OrderBy(x => x.Start).ToArray()));
    }

    private static DateTimeOffset LocalToUtc(DateOnly date, string timeZone)
    {
        DateTime local = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified);
        return new DateTimeOffset(TimeZoneInfo.ConvertTimeToUtc(local, ResolveTimeZone(timeZone)), TimeSpan.Zero);
    }

    private static TimeZoneInfo ResolveTimeZone(string id) { try { return TimeZoneInfo.FindSystemTimeZoneById(id); } catch (TimeZoneNotFoundException) { return TimeZoneInfo.Utc; } catch (InvalidTimeZoneException) { return TimeZoneInfo.Utc; } }

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

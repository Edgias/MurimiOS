using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockRationPlan;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockProduction.ChangeRation;
using LivestockRationLine = global::EdgIQ.MurimiOS.Domain.LivestockRationPlan.LivestockRationLine;
using LivestockRationPlan = global::EdgIQ.MurimiOS.Domain.LivestockRationPlan.LivestockRationPlan;
using RationPlanStatus = global::EdgIQ.MurimiOS.Domain.LivestockRationPlan.RationPlanStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class ChangeRationHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, TimeProvider clock) : IRequestHandler<ChangeRationCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeRationCommand message, CancellationToken cancellationToken) => ChangeRationAsync(message.planId, message.command, db, scope, tenant, clock, cancellationToken);

    private static async Task<UseCaseResult> ChangeRationAsync(Guid planId, string command, IPlatformDbContext db,
        IOrganisationScopeAccess scope, ITenantContext tenant, TimeProvider clock, CancellationToken cancellationToken)
    {
        LivestockRationPlan? plan = await db.LivestockRationPlans.SingleOrDefaultAsync(x => x.Id == planId, cancellationToken);
        if (plan is null || !await scope.CanAccessFarmAsync(plan.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        DateTimeOffset now = clock.GetUtcNow();
        if (command == "activate" && await db.LivestockRationPlans.AnyAsync(x => x.SubjectId == plan.SubjectId
            && x.Status == RationPlanStatus.Active && x.Id != plan.Id, cancellationToken))
            return UseCaseResults.Conflict(new { code = "active_ration_exists", message = "Retire the current active ration first." });
        try { if (command == "activate") plan.Activate(now); else plan.Retire(now); }
        catch (InvalidOperationException exception) { return UseCaseResults.Conflict(new { code = "ration_lifecycle_invalid", message = exception.Message }); }
        AddEvents(db, tenant, plan.FarmId, "LivestockRationPlan", plan.Id, "Updated", new { plan.Id, plan.Status }, now);
        await db.SaveChangesAsync(cancellationToken);
        LivestockRationLine[] lines = await db.LivestockRationLines.AsNoTracking().Where(x => x.PlanId == plan.Id).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(RationResponse(plan, lines));
    }


    private static void AddEvents(IPlatformDbContext db, ITenantContext tenant, Guid farmId, string entityType,
        Guid entityId, string operation, object value, DateTimeOffset occurredAt)
    {
        string json = JsonSerializer.Serialize(value);
        db.SyncChanges.Add(new SyncChange(tenant.TenantId, farmId, entityType, entityId, operation, json, occurredAt));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, $"{entityType}.{operation}", json, occurredAt));
    }

    private static object RationResponse(LivestockRationPlan x, IEnumerable<LivestockRationLine> lines) => new
    {
        x.Id, x.FarmId, x.SubjectId, x.Name, x.EffectiveFrom, x.EffectiveTo, x.TargetHeadCount, x.Status,
        x.ActivatedAt, x.RetiredAt, x.Notes,
        TotalKgPerHeadPerDay = lines.Sum(line => line.QuantityKgPerHeadPerDay),
        DryMatterKgPerHeadPerDay = lines.Sum(line => line.QuantityKgPerHeadPerDay * line.DryMatterPercent / 100m),
        Lines = lines.Select(line => new { line.Id, line.FeedBatchId, line.FeedName, line.QuantityKgPerHeadPerDay,
            line.EnteredQuantity, line.EnteredUnit, line.DryMatterPercent }).ToArray()
    };

}

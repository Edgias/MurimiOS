using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockGrazingPlan;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockProduction.ChangeGrazing;
using LivestockGrazingPlan = global::EdgIQ.MurimiOS.Domain.LivestockGrazingPlan.LivestockGrazingPlan;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class ChangeGrazingHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, TimeProvider clock) : IRequestHandler<ChangeGrazingCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeGrazingCommand message, CancellationToken cancellationToken) => ChangeGrazingAsync(message.farmId, message.planId, message.command, db, scope, tenant, clock, cancellationToken);

    private static async Task<UseCaseResult> ChangeGrazingAsync(Guid farmId, Guid planId, string command,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, TimeProvider clock,
        CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessFarmAsync(farmId, cancellationToken)) return UseCaseResults.NotFound();
        LivestockGrazingPlan? plan = await db.LivestockGrazingPlans.SingleOrDefaultAsync(x => x.Id == planId && x.FarmId == farmId, cancellationToken);
        if (plan is null) return UseCaseResults.NotFound();
        DateTimeOffset now = clock.GetUtcNow();
        try { if (command == "activate") plan.Activate(); else if (command == "complete") plan.Complete(now); else plan.Cancel(now); }
        catch (InvalidOperationException exception) { return UseCaseResults.Conflict(new { code = "grazing_lifecycle_invalid", message = exception.Message }); }
        AddEvents(db, tenant, farmId, "LivestockGrazingPlan", plan.Id, "Updated", GrazingResponse(plan), now);
        await db.SaveChangesAsync(cancellationToken); return UseCaseResults.Ok(GrazingResponse(plan));
    }


    private static void AddEvents(IPlatformDbContext db, ITenantContext tenant, Guid farmId, string entityType,
        Guid entityId, string operation, object value, DateTimeOffset occurredAt)
    {
        string json = JsonSerializer.Serialize(value);
        db.SyncChanges.Add(new SyncChange(tenant.TenantId, farmId, entityType, entityId, operation, json, occurredAt));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, $"{entityType}.{operation}", json, occurredAt));
    }

    private static object GrazingResponse(LivestockGrazingPlan x) => new { x.Id, x.FarmId, x.GroupSubjectId, x.HoldingId,
        x.StartsAt, x.EndsAt, x.TargetHeadCount, x.RestUntil, x.AvailableDryMatterKg,
        DryMatterKgPerHead = x.AvailableDryMatterKg / x.TargetHeadCount,
        PlannedDays = decimal.Round((decimal)(x.EndsAt - x.StartsAt).TotalDays, 2), x.Status, x.ClosedAt, x.Notes };

}

using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.CostCentre;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Finance.CloseCostCentre;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using CostCentre = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentre;
using CostCentreType = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentreType;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class CloseCostCentreHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<CloseCostCentreCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CloseCostCentreCommand message, CancellationToken cancellationToken) => CloseCostCentreAsync(message.costCentreId, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> CloseCostCentreAsync(Guid costCentreId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock,
        UseCaseContext context, CancellationToken cancellationToken)
    {
        CostCentre? centre = await db.CostCentres.SingleOrDefaultAsync(x => x.Id == costCentreId, cancellationToken);
        if (centre is null || !await scope.CanAccessFarmAsync(centre.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        DateTimeOffset now = clock.GetUtcNow();
        try { centre.Close(now, actor.Subject); }
        catch (InvalidOperationException ex) { return Conflict("cost_centre_closed", ex.Message); }
        AddRecords(db, tenant, actor, "CostCentreClosed", "Finance.CostCentreClosed", nameof(CostCentre), centre.Id,
            centre.FarmId, new { centre.Id, centre.ClosedAt, centre.ClosedBy }, now, context);
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Ok(ToCostCentre(centre));
    }


    private static CostCentreResponse ToCostCentre(CostCentre x) => new(x.Id, x.FarmId, x.Code, x.Name,
        x.Type, x.TargetId, x.Currency, x.IsActive, x.ClosedAt, x.ClosedBy);


    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string action, string eventType, string entityType, Guid entityId, Guid farmId, object payload,
        DateTimeOffset now, UseCaseContext context)
    {
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, entityType, entityId, farmId, actor.ScopeOrganisationNodeId, JsonSerializer.Serialize(payload),
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        string syncOperation = action.Contains("Created", StringComparison.Ordinal)
            || action.Contains("Posted", StringComparison.Ordinal)
            || action.Contains("Recorded", StringComparison.Ordinal)
            || action.Contains("Imported", StringComparison.Ordinal) ? "Created" : "Updated";
        db.SyncChanges.Add(new SyncChange(tenant.TenantId, farmId, entityType, entityId, syncOperation,
            JsonSerializer.Serialize(payload), now));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType,
            JsonSerializer.Serialize(payload), now));
    }

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

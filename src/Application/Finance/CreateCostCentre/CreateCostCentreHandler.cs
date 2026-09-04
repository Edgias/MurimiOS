using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.CostCentre;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Finance.CreateCostCentre;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using CostCentre = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentre;
using CostCentreType = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentreType;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class CreateCostCentreHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<CreateCostCentreCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateCostCentreCommand message, CancellationToken cancellationToken) => CreateCostCentreAsync(message.farmId, message.request, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> CreateCostCentreAsync(Guid farmId, CreateCostCentreRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        if (!await FarmAllowedAsync(farmId, db, scope, cancellationToken)) return UseCaseResults.NotFound();
        string? targetError = await ValidateCostCentreTargetAsync(farmId, request.Type, request.TargetId, db, cancellationToken);
        if (targetError is not null) return Bad("cost_centre_target_invalid", targetError);
        Guid id = NewId(request.Id); DateTimeOffset now = clock.GetUtcNow();
        try
        {
            var centre = new CostCentre(id, farmId, request.Code, request.Name, request.Type, request.TargetId, request.Currency);
            db.CostCentres.Add(centre);
            AddRecords(db, tenant, actor, "CostCentreCreated", "Finance.CostCentreCreated", nameof(CostCentre), id,
                farmId, ToCostCentre(centre), now, context);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/finance/cost-centres/{id}", ToCostCentre(centre));
        }
        catch (ArgumentException ex) { return Bad("cost_centre_invalid", ex.Message); }
        catch (DbUpdateException) { return Conflict("cost_centre_exists", "The cost-centre code already exists for this farm."); }
    }


    private static async Task<bool> FarmAllowedAsync(Guid farmId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken) =>
        await scope.CanAccessFarmAsync(farmId, cancellationToken)
        && await db.Farms.AnyAsync(x => x.Id == farmId, cancellationToken);


    private static async Task<string?> ValidateCostCentreTargetAsync(Guid farmId, CostCentreType type,
        Guid? targetId, IPlatformDbContext db, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(type)) return "Cost-centre type is invalid.";
        if (targetId == Guid.Empty) return "A supplied target id must be non-empty.";
        return type switch
        {
            CostCentreType.Farm when targetId != farmId => "A farm cost centre must target its own farm.",
            CostCentreType.Farm => null,
            CostCentreType.Field when targetId is null || !await db.FarmFields.AnyAsync(x => x.Id == targetId && x.FarmId == farmId, cancellationToken)
                => "The target field does not belong to this farm.",
            CostCentreType.CropZone when targetId is null || !await (from zone in db.CropZones
                join season in db.GrowingSeasons on zone.SeasonId equals season.Id
                where zone.Id == targetId && season.FarmId == farmId select zone.Id).AnyAsync(cancellationToken)
                => "The target crop zone does not belong to this farm.",
            CostCentreType.LivestockSubject when targetId is null || !await db.LivestockSubjects.AnyAsync(x => x.Id == targetId && x.FarmId == farmId, cancellationToken)
                => "The target livestock subject does not belong to this farm.",
            CostCentreType.Overhead when targetId is not null => "An overhead cost centre cannot have a target.",
            _ => null
        };
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


    private static Guid NewId(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

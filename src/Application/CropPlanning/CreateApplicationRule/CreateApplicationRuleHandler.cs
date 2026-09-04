using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.AgriculturalInput;
using EdgIQ.MurimiOS.Domain.CropPlan;
using EdgIQ.MurimiOS.Domain.CropProductApplicationRule;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropPlanning.CreateApplicationRule;
using AgriculturalInput = global::EdgIQ.MurimiOS.Domain.AgriculturalInput.AgriculturalInput;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropProductApplicationRule = global::EdgIQ.MurimiOS.Domain.CropProductApplicationRule.CropProductApplicationRule;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class CreateApplicationRuleHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<CreateApplicationRuleCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateApplicationRuleCommand message, CancellationToken cancellationToken) => CreateApplicationRuleAsync(message.inputId, message.request, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> CreateApplicationRuleAsync(Guid inputId, CreateProductApplicationRuleRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        AgriculturalInput? input = await db.AgriculturalInputs.SingleOrDefaultAsync(x => x.Id == inputId, cancellationToken);
        if (input is null || !await scope.CanAccessFarmAsync(input.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        string country = await db.Farms.Where(x => x.Id == input.FarmId).Select(x => x.CountryCode).SingleAsync(cancellationToken);
        string jurisdiction = request.Jurisdiction?.Trim().ToUpperInvariant() ?? string.Empty;
        if (!jurisdiction.Equals(country, StringComparison.OrdinalIgnoreCase))
            return Bad("jurisdiction_invalid", "The initial planning rule catalogue must match the farm jurisdiction.");
        bool overlaps = await db.CropProductApplicationRules.AnyAsync(x => x.InputId == inputId
            && x.Crop.ToLower() == request.Crop.Trim().ToLower() && x.Jurisdiction == jurisdiction
            && (x.EffectiveTo == null || x.EffectiveTo >= request.EffectiveFrom)
            && (request.EffectiveTo == null || x.EffectiveFrom <= request.EffectiveTo), cancellationToken);
        if (overlaps) return Conflict("application_rule_overlap", "An overlapping product authorization already exists.");
        Guid id = NonEmptyOrNew(request.Id); DateTimeOffset now = clock.GetUtcNow();
        try
        {
            var rule = new CropProductApplicationRule(id, inputId, request.Crop, jurisdiction,
                request.MaximumRatePerHa, request.RateUnit, request.ReEntryHours, request.PreHarvestIntervalDays,
                request.EffectiveFrom, request.EffectiveTo, request.AuthorizationReference);
            db.CropProductApplicationRules.Add(rule);
            AddChange(db, tenant.TenantId, input.FarmId, "ProductApplicationRule", id, "Created", ToRule(rule), now);
            AddAudit(db, tenant, actor, "ProductRuleCreated", null, input.FarmId,
                new { rule.Id, rule.InputId, rule.Crop, rule.Jurisdiction, rule.MaximumRatePerHa, rule.RateUnit }, now, context, id);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/crop-planning/inputs/{inputId}/application-rules/{id}", ToRule(rule));
        }
        catch (ArgumentException ex) { return Bad("application_rule_invalid", ex.Message); }
    }

    private static ProductApplicationRuleResponse ToRule(CropProductApplicationRule x) => new(x.Id, x.InputId,
        x.Crop, x.Jurisdiction, x.MaximumRatePerHa, x.RateUnit, x.EnteredMaximumRatePerHa, x.EnteredRateUnit,
        x.ReEntryHours, x.PreHarvestIntervalDays, x.EffectiveFrom, x.EffectiveTo, x.AuthorizationReference);

    private static void AddChange(IPlatformDbContext db, Guid tenantId, Guid farmId, string entityType,
        Guid entityId, string operation, object payload, DateTimeOffset now) =>
        db.SyncChanges.Add(new SyncChange(tenantId, farmId, entityType, entityId, operation, JsonSerializer.Serialize(payload), now));

    private static void AddAudit(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string action, CropPlan? plan, Guid farmId, object changes, DateTimeOffset now, UseCaseContext context, Guid? entityId = null) =>
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, plan is null ? nameof(CropProductApplicationRule) : nameof(CropPlan), entityId ?? plan!.Id,
            farmId, actor.ScopeOrganisationNodeId, JsonSerializer.Serialize(changes),
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));

    private static Guid NonEmptyOrNew(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

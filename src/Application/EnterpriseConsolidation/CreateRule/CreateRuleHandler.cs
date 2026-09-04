using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule;
using EdgIQ.MurimiOS.Domain.OrganisationNode;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.EnterpriseConsolidation.CreateRule;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using IntercompanyEliminationRule = global::EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule.IntercompanyEliminationRule;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class CreateRuleHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<CreateRuleCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateRuleCommand message, CancellationToken cancellationToken) => CreateRuleAsync(message.request, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> CreateRuleAsync(CreateEliminationRuleRequest request, IPlatformDbContext db,
        IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock,
        UseCaseContext context, CancellationToken ct)
    {
        ResolvedOrganisationScope resolved = await scope.ResolveAsync(ct);
        if (!resolved.AllowsNode(request.ConsolidationOrganisationNodeId)) return UseCaseResults.NotFound();
        if (!await CompaniesWithinScopeAsync(request.ConsolidationOrganisationNodeId,
            [request.SellerCompanyId, request.BuyerCompanyId], db, ct))
            return Bad("elimination_company_scope_invalid", "Both legal companies must be descendants of the consolidation organisation.");
        try
        {
            DateTimeOffset now = clock.GetUtcNow();
            var rule = new IntercompanyEliminationRule(Id(request.Id), request.ConsolidationOrganisationNodeId,
                request.SellerCompanyId, request.BuyerCompanyId, request.RuleKey, request.RuleVersion, request.Name,
                request.Currency, request.BalancingTolerance, request.EffectiveFrom, request.EffectiveTo, actor.Subject);
            db.IntercompanyEliminationRules.Add(rule); AddRecords(db, tenant, actor, "EliminationRuleCreated",
                "Enterprise.EliminationRuleCreated", nameof(IntercompanyEliminationRule), rule.Id,
                rule.ConsolidationOrganisationNodeId, rule, now, context);
            await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/enterprise/elimination-rules/{rule.Id}", rule);
        }
        catch (ArgumentException ex) { return Bad("elimination_rule_invalid", ex.Message); }
        catch (DbUpdateException) { return Conflict("elimination_rule_conflict", "This elimination rule version already exists."); }
    }


    private static async Task<bool> CompaniesWithinScopeAsync(Guid rootId, Guid[] companyIds,
        IPlatformDbContext db, CancellationToken ct)
    {
        OrganisationNode? root = await db.OrganisationNodes.AsNoTracking().SingleOrDefaultAsync(x => x.Id == rootId, ct);
        if (root is null) return false;
        var companies = await (from company in db.LegalCompanies.AsNoTracking()
            join node in db.OrganisationNodes.AsNoTracking() on company.OrganisationNodeId equals node.Id
            where companyIds.Contains(company.Id) && company.IsActive select new { company.Id, node.Path }).ToArrayAsync(ct);
        return companies.Length == companyIds.Distinct().Count()
            && companies.All(x => x.Path == root.Path || x.Path.StartsWith(root.Path + ".", StringComparison.Ordinal));
    }


    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string action, string eventType, string entityType, Guid entityId, Guid organisationNodeId,
        object payload, DateTimeOffset now, UseCaseContext context)
    {
        string json = JsonSerializer.Serialize(payload);
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, entityType, entityId, null, organisationNodeId, json,
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now));
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

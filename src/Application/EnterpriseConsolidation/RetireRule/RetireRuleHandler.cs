using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.EnterpriseConsolidation.RetireRule;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using IntercompanyEliminationRule = global::EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule.IntercompanyEliminationRule;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class RetireRuleHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<RetireRuleCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RetireRuleCommand message, CancellationToken cancellationToken) => RetireRuleAsync(message.id, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> RetireRuleAsync(Guid id, IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor,
        ITenantContext tenant, TimeProvider clock, UseCaseContext context, CancellationToken ct)
    {
        IntercompanyEliminationRule? rule = await db.IntercompanyEliminationRules.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (rule is null || !(await scope.ResolveAsync(ct)).AllowsNode(rule.ConsolidationOrganisationNodeId)) return UseCaseResults.NotFound();
        try
        {
            DateTimeOffset now = clock.GetUtcNow(); rule.Retire(now, actor.Subject);
            AddRecords(db, tenant, actor, "EliminationRuleRetired", "Enterprise.EliminationRuleRetired",
                nameof(IntercompanyEliminationRule), rule.Id, rule.ConsolidationOrganisationNodeId, rule, now, context);
            await db.SaveChangesAsync(ct); return UseCaseResults.Ok(rule);
        }
        catch (InvalidOperationException ex) { return Conflict("elimination_rule_transition_invalid", ex.Message); }
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

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

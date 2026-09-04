using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;
using EdgIQ.MurimiOS.Domain.FieldRiskRule;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.ActivateRiskRule;
using ScoutingObservation = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingObservation;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using FieldRiskRule = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.FieldRiskRule;
using RiskAggregation = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskAggregation;
using RiskComparison = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskComparison;
using RiskMetric = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskMetric;
using RiskRuleStatus = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskRuleStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;
using ScoutingCategory = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingCategory;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class ActivateRiskRuleHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<ActivateRiskRuleCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ActivateRiskRuleCommand message, CancellationToken cancellationToken) => ActivateRiskRuleAsync(message.ruleId, db, tenant, actor, clock, context, cancellationToken);

    private static Task<UseCaseResult> ActivateRiskRuleAsync(Guid ruleId, IPlatformDbContext db, ITenantContext tenant,
        IRequestActorContext actor, TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken) =>        ChangeRiskRuleStatusAsync(ruleId, true, db, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> ChangeRiskRuleStatusAsync(Guid ruleId, bool activate,
        IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock,
        UseCaseContext context, CancellationToken cancellationToken)
    {
        FieldRiskRule? rule = await db.FieldRiskRules.SingleOrDefaultAsync(x => x.Id == ruleId, cancellationToken);
        if (rule is null) return UseCaseResults.NotFound();
        DateTimeOffset now = clock.GetUtcNow();
        try
        {
            if (activate)
            {
                if (await db.FieldRiskRules.AnyAsync(x => x.RuleKey == rule.RuleKey && x.Status == RiskRuleStatus.Active && x.Id != rule.Id, cancellationToken))
                    return Conflict("active_risk_rule_exists", "Retire the currently active version before activating this version.");
                rule.Activate(now);
            }
            else rule.Retire(now);
        }
        catch (InvalidOperationException ex) { return Conflict("risk_rule_transition_invalid", ex.Message); }
        string action = activate ? "RiskRuleActivated" : "RiskRuleRetired";
        AddEvent(db, tenant, actor, nameof(FieldRiskRule), rule.Id, null, action,
            $"FieldIntelligence.{action}", new { rule.Id, rule.RuleKey, rule.RuleVersion, rule.Status }, now, context);
        await db.SaveChangesAsync(cancellationToken); return UseCaseResults.Ok(ToRiskRule(rule));
    }

    private static FieldRiskRuleResponse ToRiskRule(FieldRiskRule x) => new(x.Id, x.RuleKey, x.RuleVersion, x.Name,
        x.Crop, x.Metric, x.Aggregation, x.Comparison, x.ScoutingCategory, x.IssueCode, x.Threshold, x.LookbackHours, x.MinimumSamples,
        x.Severity, x.Message, x.EffectiveFrom, x.EffectiveTo, x.CreatedBy, x.Status, x.ActivatedAt, x.RetiredAt, x.Version);


    private static void AddEvent(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string entityType, Guid entityId, Guid? farmId, string action, string eventType, object payload,
        DateTimeOffset now, UseCaseContext context)
    {
        string json = JsonSerializer.Serialize(payload);
        string syncOperation = action switch
        {
            "InsightAlertAcknowledged" => "AlertAcknowledged",
            "InsightAlertDismissed" => "AlertDismissed",
            "AlertWorkOrderCreated" => "AlertWorkCreated",
            _ => action
        };
        if (farmId is { } farm) db.SyncChanges.Add(new SyncChange(tenant.TenantId, farm, entityType, entityId, syncOperation, json, now));
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, entityType, entityId, farmId, actor.ScopeOrganisationNodeId, json,
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now));
    }

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

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
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.CreateRiskRule;
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

public sealed class CreateRiskRuleHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<CreateRiskRuleCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateRiskRuleCommand message, CancellationToken cancellationToken) => CreateRiskRuleAsync(message.request, db, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> CreateRiskRuleAsync(CreateFieldRiskRuleRequest request,
        IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock,
        UseCaseContext context, CancellationToken cancellationToken)
    {
        try
        {
            Guid id = NonEmptyOrNew(request.Id); DateTimeOffset now = clock.GetUtcNow();
            int version = request.RuleVersion ?? ((await db.FieldRiskRules.Where(x => x.RuleKey == request.RuleKey.Trim().ToLower())
                .MaxAsync(x => (int?)x.RuleVersion, cancellationToken) ?? 0) + 1);
            var rule = new FieldRiskRule(id, request.RuleKey, version, request.Name, request.Crop,
                request.Metric, request.Aggregation, request.Comparison, request.ScoutingCategory,
                request.IssueCode, request.Threshold,
                request.LookbackHours, request.MinimumSamples, request.Severity, request.Message,
                request.EffectiveFrom ?? now, request.EffectiveTo, actor.Subject);
            db.FieldRiskRules.Add(rule);
            AddEvent(db, tenant, actor, nameof(FieldRiskRule), id, null, "RiskRuleCreated",
                "FieldIntelligence.RiskRuleCreated", new { rule.Id, rule.RuleKey, rule.RuleVersion, rule.Metric, rule.Aggregation, rule.Comparison, rule.Threshold }, now, context);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/field-intelligence/risk-rules/{id}", ToRiskRule(rule));
        }
        catch (ArgumentException ex) { return Bad("risk_rule_invalid", ex.Message); }
        catch (DbUpdateException ex) { return Conflict("risk_rule_conflict", DbMessage(ex)); }
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

    private static Guid NonEmptyOrNew(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static string DbMessage(DbUpdateException ex) => ex.InnerException?.Message ?? ex.Message;

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

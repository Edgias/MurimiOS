using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;
using EdgIQ.MurimiOS.Domain.FieldRiskRule;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.ListRiskRules;
using ScoutingObservation = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingObservation;

using FieldRiskRule = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.FieldRiskRule;
using RiskAggregation = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskAggregation;
using RiskComparison = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskComparison;
using RiskMetric = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskMetric;
using RiskRuleStatus = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskRuleStatus;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;
using ScoutingCategory = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingCategory;

public sealed class ListRiskRulesHandler(IPlatformDbContext db) : IRequestHandler<ListRiskRulesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListRiskRulesQuery message, CancellationToken cancellationToken) => ListRiskRulesAsync(message.status, db, cancellationToken);

    private static async Task<UseCaseResult> ListRiskRulesAsync(RiskRuleStatus? status, IPlatformDbContext db,
        CancellationToken cancellationToken)
    {
        IQueryable<FieldRiskRule> query = db.FieldRiskRules.AsNoTracking();
        if (status is { } selected) query = query.Where(x => x.Status == selected);
        FieldRiskRule[] rows = await query.OrderBy(x => x.RuleKey).ThenByDescending(x => x.RuleVersion).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows.Select(ToRiskRule).ToArray());
    }

    private static FieldRiskRuleResponse ToRiskRule(FieldRiskRule x) => new(x.Id, x.RuleKey, x.RuleVersion, x.Name,
        x.Crop, x.Metric, x.Aggregation, x.Comparison, x.ScoutingCategory, x.IssueCode, x.Threshold, x.LookbackHours, x.MinimumSamples,
        x.Severity, x.Message, x.EffectiveFrom, x.EffectiveTo, x.CreatedBy, x.Status, x.ActivatedAt, x.RetiredAt, x.Version);

}

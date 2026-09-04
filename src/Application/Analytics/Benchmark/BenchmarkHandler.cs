using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.Benchmark;
using BenchmarkCohortDefinition = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.BenchmarkCohortDefinition;

using AnalyticsGovernanceStatus = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.AnalyticsGovernanceStatus;

public sealed class BenchmarkHandler(IPlatformDbContext db) : IRequestHandler<BenchmarkQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(BenchmarkQuery message, CancellationToken cancellationToken) => BenchmarkAsync(message.metricCode, message.cohortKey, message.from, message.to, db, cancellationToken);
    private static async Task<UseCaseResult> BenchmarkAsync(string metricCode, string cohortKey, DateOnly from, DateOnly to, IPlatformDbContext db, CancellationToken ct)
    { if (to < from) return Bad("benchmark_period_invalid", "The benchmark end date cannot precede its start."); if (!await db.BenchmarkEnrollments.AnyAsync(x => x.MetricCode == metricCode.ToLower() && x.CohortKey == cohortKey.ToLower() && x.IsActive && x.CohortDefinitionId != null && db.BenchmarkCohortDefinitions.Any(c => c.Id == x.CohortDefinitionId && c.Status == AnalyticsGovernanceStatus.Active && c.EffectiveFrom <= to && (c.EffectiveTo == null || c.EffectiveTo >= from)), ct)) return UseCaseResults.Forbid(); BenchmarkAggregateProjection? result = await db.BenchmarkAggregates.FromSqlInterpolated($"select * from analytics.benchmark_metric({metricCode.ToLower()}, {cohortKey.ToLower()}, {from}, {to})").AsNoTracking().SingleOrDefaultAsync(ct); return result is null ? UseCaseResults.Conflict(new { code = "benchmark_cohort_too_small", message = "The approved anonymized cohort has not reached its privacy threshold." }) : UseCaseResults.Ok(result); }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

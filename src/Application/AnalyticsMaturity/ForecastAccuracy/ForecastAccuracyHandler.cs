using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AnalyticsForecastEvaluation;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.AnalyticsMaturity.ForecastAccuracy;
using AnalyticsForecastEvaluation = global::EdgIQ.MurimiOS.Domain.AnalyticsForecastEvaluation.AnalyticsForecastEvaluation;

public sealed class ForecastAccuracyHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ForecastAccuracyQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ForecastAccuracyQuery message, CancellationToken cancellationToken) => ForecastAccuracyAsync(message.farmId, message.metricCode, message.from, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ForecastAccuracyAsync(Guid farmId, string? metricCode, DateTimeOffset? from,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        IQueryable<AnalyticsForecastEvaluation> query = db.AnalyticsForecastEvaluations.AsNoTracking().Where(x => x.FarmId == farmId);
        if (!string.IsNullOrWhiteSpace(metricCode)) query = query.Where(x => x.MetricCode == metricCode.Trim().ToLowerInvariant());
        if (from is { } start) query = query.Where(x => x.EvaluatedAt >= start);
        AnalyticsForecastEvaluation[] rows = await query.ToArrayAsync(ct);
        return UseCaseResults.Ok(new { farmId, evaluations = rows.Length,
            metrics = rows.GroupBy(x => new { x.MetricCode, x.Unit, x.ModelVersion }).Select(group => new
            { group.Key.MetricCode, group.Key.Unit, group.Key.ModelVersion, observations = group.Count(),
                meanAbsoluteError = decimal.Round(group.Average(x => x.AbsoluteError), 8),
                meanAbsolutePercentageError = group.Any(x => x.AbsolutePercentageError != null)
                    ? decimal.Round(group.Where(x => x.AbsolutePercentageError != null).Average(x => x.AbsolutePercentageError!.Value), 8) : (decimal?)null,
                meanBias = decimal.Round(group.Average(x => x.Bias), 8) }).ToArray() });
    }


}

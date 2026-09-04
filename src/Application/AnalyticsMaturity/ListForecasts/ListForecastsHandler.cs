using EdgIQ.MurimiOS.Application.Common.Analytics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.AnalyticsMaturity.ListForecasts;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;

using AnalyticsFact = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsFact;
using AnalyticsValueKind = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsValueKind;

public sealed class ListForecastsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListForecastsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListForecastsQuery message, CancellationToken cancellationToken) => ListForecastsAsync(message.farmId, message.metricCode, message.through, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListForecastsAsync(Guid farmId, string? metricCode, DateOnly? through,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        IQueryable<AnalyticsFact> query = AnalyticsFacts.CurrentFacts(db).Where(x => x.FarmId == farmId && x.ValueKind == AnalyticsValueKind.Forecast);
        if (!string.IsNullOrWhiteSpace(metricCode)) query = query.Where(x => x.MetricCode == metricCode.Trim().ToLowerInvariant());
        if (through is { } date) query = query.Where(x => x.PeriodEnd <= date);
        return UseCaseResults.Ok(await query.OrderByDescending(x => x.PeriodEnd).Take(5_000).ToArrayAsync(ct));
    }


}

using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition;
using EdgIQ.MurimiOS.Domain.PredictiveModelDefinition;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.AnalyticsMaturity.ListModels;
using BenchmarkCohortDefinition = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.BenchmarkCohortDefinition;

using AnalyticsGovernanceStatus = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.AnalyticsGovernanceStatus;
using PredictiveModelDefinition = global::EdgIQ.MurimiOS.Domain.PredictiveModelDefinition.PredictiveModelDefinition;

public sealed class ListModelsHandler(IPlatformDbContext db) : IRequestHandler<ListModelsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListModelsQuery message, CancellationToken cancellationToken) => ListModelsAsync(message.status, db, cancellationToken);

    private static async Task<UseCaseResult> ListModelsAsync(AnalyticsGovernanceStatus? status,
        IPlatformDbContext db, CancellationToken ct)
    {
        IQueryable<PredictiveModelDefinition> query = db.PredictiveModelDefinitions.AsNoTracking();
        if (status is { } value) query = query.Where(x => x.Status == value);
        return UseCaseResults.Ok(await query.OrderBy(x => x.TargetMetricCode).ThenByDescending(x => x.ModelVersion).ToArrayAsync(ct));
    }


}

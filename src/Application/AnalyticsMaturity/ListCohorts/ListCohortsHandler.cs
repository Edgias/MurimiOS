using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.AnalyticsMaturity.ListCohorts;
using AnalyticsGovernanceStatus = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.AnalyticsGovernanceStatus;
using BenchmarkCohortDefinition = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.BenchmarkCohortDefinition;

public sealed class ListCohortsHandler(IPlatformDbContext db) : IRequestHandler<ListCohortsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListCohortsQuery message, CancellationToken cancellationToken) => ListCohortsAsync(message.status, db, cancellationToken);

    private static async Task<UseCaseResult> ListCohortsAsync(AnalyticsGovernanceStatus? status,
        IPlatformDbContext db, CancellationToken ct)
    {
        IQueryable<BenchmarkCohortDefinition> query = db.BenchmarkCohortDefinitions.AsNoTracking();
        if (status is { } value) query = query.Where(x => x.Status == value);
        return UseCaseResults.Ok(await query.OrderBy(x => x.CohortKey).ThenByDescending(x => x.DefinitionVersion).ToArrayAsync(ct));
    }


}

using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.ListEnrollments;


public sealed class ListEnrollmentsHandler(IPlatformDbContext db) : IRequestHandler<ListEnrollmentsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListEnrollmentsQuery message, CancellationToken cancellationToken) => ListEnrollmentsAsync(db, cancellationToken);

    private static async Task<UseCaseResult> ListEnrollmentsAsync(IPlatformDbContext db, CancellationToken ct) =>UseCaseResults.Ok(await db.BenchmarkEnrollments.AsNoTracking().OrderBy(x => x.MetricCode).ToArrayAsync(ct));

}

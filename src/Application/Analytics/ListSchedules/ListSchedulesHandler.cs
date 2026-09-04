using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.ListSchedules;


public sealed class ListSchedulesHandler(IPlatformDbContext db) : IRequestHandler<ListSchedulesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListSchedulesQuery message, CancellationToken cancellationToken) => ListSchedulesAsync(db, cancellationToken);

    private static async Task<UseCaseResult> ListSchedulesAsync(IPlatformDbContext db, CancellationToken ct)
=>UseCaseResults.Ok(await db.AnalyticsProjectionSchedules.AsNoTracking().OrderBy(x => x.NextRunAt).ToArrayAsync(ct));

}

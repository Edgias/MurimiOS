using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.CancelProjection;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;

public sealed class CancelProjectionHandler(IPlatformDbContext db) : IRequestHandler<CancelProjectionCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CancelProjectionCommand message, CancellationToken cancellationToken) => CancelProjectionAsync(message.runId, db, cancellationToken);

    private static async Task<UseCaseResult> CancelProjectionAsync(Guid runId, IPlatformDbContext db, CancellationToken ct)
    {
        AnalyticsProjectionRun? run = await db.AnalyticsProjectionRuns.SingleOrDefaultAsync(x => x.Id == runId, ct);
        if (run is null) return UseCaseResults.NotFound();
        try { run.Cancel(DateTimeOffset.UtcNow); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(run); }
        catch (InvalidOperationException e) { return Conflict("projection_transition_invalid", e.Message); }
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

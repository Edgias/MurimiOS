using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.CreateProjection;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;

public sealed class CreateProjectionHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor) : IRequestHandler<CreateProjectionCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateProjectionCommand message, CancellationToken cancellationToken) => CreateProjectionAsync(message.farmId, message.request, db, scope, actor, cancellationToken);

    private static async Task<UseCaseResult> CreateProjectionAsync(Guid farmId, CreateAnalyticsProjectionRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct) || !await db.Farms.AnyAsync(x => x.Id == farmId, ct)) return UseCaseResults.NotFound();
        try { int revision = (await db.AnalyticsProjectionRuns.Where(x => x.FarmId == farmId && x.PeriodStart == request.PeriodStart && x.PeriodEnd == request.PeriodEnd && x.ProjectionVersion == request.ProjectionVersion).MaxAsync(x => (int?)x.Revision, ct) ?? 0) + 1; var run = new AnalyticsProjectionRun(Id(request.Id), farmId, request.PeriodStart, request.PeriodEnd, request.Currency, request.ProjectionVersion, request.ScheduledFor ?? DateTimeOffset.UtcNow, actor.Subject, revision); db.AnalyticsProjectionRuns.Add(run); await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/analytics/projections/{run.Id}", run); }
        catch (ArgumentException e) { return Bad("projection_invalid", e.Message); }
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

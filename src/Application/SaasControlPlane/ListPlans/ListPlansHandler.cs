using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SaasPlanVersion;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.ListPlans;
using PlanEntitlement = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.PlanEntitlement;
using SaasPlanStatus = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanStatus;
using SaasPlanVersion = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanVersion;

public sealed class ListPlansHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<ListPlansQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListPlansQuery message, CancellationToken cancellationToken) => ListPlansAsync(message.status, db, routing, cancellationToken);

    private static async Task<UseCaseResult> ListPlansAsync(SaasPlanStatus? status, IPlatformDbContext db,
        TenantDatabaseRoutingScope routing, CancellationToken ct)
    {
        using IDisposable control = routing.EnterControlPlane(); IQueryable<SaasPlanVersion> query = db.SaasPlanVersions.AsNoTracking();
        if (status.HasValue) query = query.Where(x => x.Status == status);
        var plans = await query.OrderBy(x => x.Code).ThenByDescending(x => x.PlanVersion).ToArrayAsync(ct);
        Guid[] ids = plans.Select(x => x.Id).ToArray(); PlanEntitlement[] entitlements = await db.PlanEntitlements.AsNoTracking()
            .Where(x => ids.Contains(x.PlanVersionId)).OrderBy(x => x.Key).ToArrayAsync(ct);
        return UseCaseResults.Ok(plans.Select(plan => new { plan, entitlements = entitlements.Where(x => x.PlanVersionId == plan.Id) }));
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

}

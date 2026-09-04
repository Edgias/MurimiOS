using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SaasPlanVersion;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.ChangePlan;
using SaasPlanVersion = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanVersion;

public sealed class ChangePlanHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<ChangePlanCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangePlanCommand message, CancellationToken cancellationToken) => ChangePlanAsync(message.planId, message.command, db, routing, cancellationToken);

    private static async Task<UseCaseResult> ChangePlanAsync(Guid planId, string command, IPlatformDbContext db,
        TenantDatabaseRoutingScope routing, CancellationToken ct)
    {
        using IDisposable control = routing.EnterControlPlane(); SaasPlanVersion? plan = await db.SaasPlanVersions.SingleOrDefaultAsync(x => x.Id == planId, ct);
        if (plan is null) return UseCaseResults.NotFound();
        try
        {
            if (command == "publish")
            {
                if (!await db.PlanEntitlements.AnyAsync(x => x.PlanVersionId == plan.Id, ct)) return Conflict("plan_entitlements_required", "A plan needs at least one entitlement before publication.");
                plan.Publish(DateTimeOffset.UtcNow);
            }
            else plan.Retire(DateTimeOffset.UtcNow);
            await db.SaveChangesAsync(ct); return UseCaseResults.Ok(plan);
        }
        catch (InvalidOperationException e) { return Conflict("saas_plan_transition_invalid", e.Message); }
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

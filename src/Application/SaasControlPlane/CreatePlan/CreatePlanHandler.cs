using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SaasPlanVersion;
using EdgIQ.MurimiOS.Domain.TenantSubscription;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.CreatePlan;
using TenantSubscription = global::EdgIQ.MurimiOS.Domain.TenantSubscription.TenantSubscription;

using SaasPlanVersion = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanVersion;
using BillingInterval = global::EdgIQ.MurimiOS.Domain.TenantSubscription.BillingInterval;

public sealed class CreatePlanHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing, IRequestActorContext actor) : IRequestHandler<CreatePlanCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreatePlanCommand message, CancellationToken cancellationToken) => CreatePlanAsync(message.request, db, routing, actor, cancellationToken);

    private static async Task<UseCaseResult> CreatePlanAsync(CreateSaasPlanRequest request, IPlatformDbContext db,
        TenantDatabaseRoutingScope routing, IRequestActorContext actor, CancellationToken ct)
    {
        using IDisposable control = routing.EnterControlPlane();
        try
        {
            var plan = new SaasPlanVersion(Id(request.Id), request.SeriesId is { } series && series != Guid.Empty ? series : Guid.NewGuid(),
                request.Code, request.Name, request.PlanVersion, request.BillingInterval, request.BasePrice, request.Currency,
                request.EffectiveFrom, request.EffectiveTo, DateTimeOffset.UtcNow, actor.Subject);
            db.SaasPlanVersions.Add(plan); await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/saas/plans/{plan.Id}", plan);
        }
        catch (ArgumentException e) { return Bad("saas_plan_invalid", e.Message); }
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SaasPlanVersion;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.AddEntitlement;
using EntitlementEnforcement = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.EntitlementEnforcement;
using EntitlementKind = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.EntitlementKind;
using PlanEntitlement = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.PlanEntitlement;
using SaasPlanStatus = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanStatus;
using SaasPlanVersion = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanVersion;

public sealed class AddEntitlementHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<AddEntitlementCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(AddEntitlementCommand message, CancellationToken cancellationToken) => AddEntitlementAsync(message.planId, message.request, db, routing, cancellationToken);

    private static async Task<UseCaseResult> AddEntitlementAsync(Guid planId, CreatePlanEntitlementRequest request,
        IPlatformDbContext db, TenantDatabaseRoutingScope routing, CancellationToken ct)
    {
        using IDisposable control = routing.EnterControlPlane(); SaasPlanVersion? plan = await db.SaasPlanVersions.SingleOrDefaultAsync(x => x.Id == planId, ct);
        if (plan is null) return UseCaseResults.NotFound();
        if (plan.Status != SaasPlanStatus.Draft) return Conflict("published_plan_immutable", "Entitlements can only be added to a draft plan version.");
        try
        {
            var entitlement = new PlanEntitlement(Id(request.Id), plan.Id, request.Key, request.Kind, request.IncludedAmount, request.Unit, request.Enforcement);
            db.PlanEntitlements.Add(entitlement); await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/saas/plans/{plan.Id}/entitlements/{entitlement.Id}", entitlement);
        }
        catch (ArgumentException e) { return Bad("plan_entitlement_invalid", e.Message); }
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

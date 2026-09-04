using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TenantPlacement;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.ClosePlacement;
using TenantPlacement = global::EdgIQ.MurimiOS.Domain.TenantPlacement.TenantPlacement;

public sealed class ClosePlacementHandler(IPlatformDbContext db, ITenantDatabaseRouteResolver routes, TenantDatabaseRoutingScope routing, ITenantContext tenant) : IRequestHandler<ClosePlacementCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ClosePlacementCommand message, CancellationToken cancellationToken) => ClosePlacementAsync(message.placementId, db, routes, routing, tenant, cancellationToken);
    private static async Task<UseCaseResult> ClosePlacementAsync(Guid placementId, IPlatformDbContext db, ITenantDatabaseRouteResolver routes,
        TenantDatabaseRoutingScope routing, ITenantContext tenant, CancellationToken ct)
    { using IDisposable control = routing.EnterControlPlane(); TenantPlacement? placement = await db.TenantPlacements.SingleOrDefaultAsync(x => x.Id == placementId, ct); if (placement is null) return UseCaseResults.NotFound(); try { placement.Close(DateTimeOffset.UtcNow); await db.SaveChangesAsync(ct); routes.Invalidate(tenant.TenantId); return UseCaseResults.Ok(placement); } catch (InvalidOperationException e) { return Conflict("tenant_placement_transition_invalid", e.Message); } }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

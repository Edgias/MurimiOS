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
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.CreatePlacement;
using TenantDatabaseMode = global::EdgIQ.MurimiOS.Domain.TenantPlacement.TenantDatabaseMode;
using TenantPlacement = global::EdgIQ.MurimiOS.Domain.TenantPlacement.TenantPlacement;

public sealed class CreatePlacementHandler(IPlatformDbContext db, ITenantDatabaseRegistry registry, ITenantDatabaseRouteResolver routes, TenantDatabaseRoutingScope routing, ITenantContext tenant) : IRequestHandler<CreatePlacementCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreatePlacementCommand message, CancellationToken cancellationToken) => CreatePlacementAsync(message.request, db, registry, routes, routing, tenant, cancellationToken);
    private static async Task<UseCaseResult> CreatePlacementAsync(CreateTenantPlacementRequest request, IPlatformDbContext db,
        ITenantDatabaseRegistry registry, ITenantDatabaseRouteResolver routes, TenantDatabaseRoutingScope routing, ITenantContext tenant, CancellationToken ct)
    { using IDisposable control = routing.EnterControlPlane(); if (!registry.Contains(request.DatabaseKey)) return Bad("database_key_unconfigured", "The database key is not configured on this host."); if (request.IsPrimary && !request.DatabaseKey.Equals(registry.ControlDatabaseKey, StringComparison.OrdinalIgnoreCase)) return Conflict("placement_cutover_managed", "A non-control primary placement can only be activated by an executed migration."); if (request.IsPrimary && await db.TenantPlacements.AnyAsync(x => x.IsPrimary && x.EffectiveTo == null, ct)) return Conflict("primary_placement_exists", "Close the current primary placement first."); try { var placement = new TenantPlacement(Id(request.Id), request.Region, request.DatabaseKey, request.Mode, request.ResidencyCountry, request.IsPrimary, request.EffectiveFrom ?? DateTimeOffset.UtcNow); db.TenantPlacements.Add(placement); await db.SaveChangesAsync(ct); routes.Invalidate(tenant.TenantId); return UseCaseResults.Created($"/api/v1/enterprise/placements/{placement.Id}", placement); } catch (ArgumentException e) { return Bad("tenant_placement_invalid", e.Message); } }

    private static void Add(Dictionary<string, object?> target, string[] attributes, string key, object? value) { if (attributes.Contains(key, StringComparer.Ordinal)) target[key] = value; }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message }); 
private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

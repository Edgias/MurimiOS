using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.ListPlacements;


public sealed class ListPlacementsHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<ListPlacementsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListPlacementsQuery message, CancellationToken cancellationToken) => ListPlacementsAsync(db, routing, cancellationToken);

    private static async Task<UseCaseResult> ListPlacementsAsync(IPlatformDbContext db, TenantDatabaseRoutingScope routing, CancellationToken ct)
    { using IDisposable control = routing.EnterControlPlane(); return UseCaseResults.Ok(await db.TenantPlacements.AsNoTracking().OrderByDescending(x => x.EffectiveFrom).ToArrayAsync(ct)); }


}

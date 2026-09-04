using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.ListSubscriptions;


public sealed class ListSubscriptionsHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<ListSubscriptionsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListSubscriptionsQuery message, CancellationToken cancellationToken) => ListSubscriptionsAsync(db, routing, cancellationToken);

    private static async Task<UseCaseResult> ListSubscriptionsAsync(IPlatformDbContext db, TenantDatabaseRoutingScope routing, CancellationToken ct)
    { using IDisposable control = routing.EnterControlPlane(); return UseCaseResults.Ok(await db.TenantSubscriptions.AsNoTracking().OrderByDescending(x => x.CreatedAt).ToArrayAsync(ct)); }


}

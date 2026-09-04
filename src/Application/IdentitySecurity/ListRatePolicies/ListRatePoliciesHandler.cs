using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.ListRatePolicies;


public sealed class ListRatePoliciesHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<ListRatePoliciesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListRatePoliciesQuery message, CancellationToken cancellationToken) => ListRatePoliciesAsync(db, routing, cancellationToken);

    private static async Task<UseCaseResult> ListRatePoliciesAsync(IPlatformDbContext db, TenantDatabaseRoutingScope routing, CancellationToken ct)
    { using IDisposable control = routing.EnterControlPlane(); return UseCaseResults.Ok(await db.ApiRateLimitPolicies.AsNoTracking().OrderByDescending(x => x.IsActive).ThenByDescending(x => x.CreatedAt).ToArrayAsync(ct)); }


}

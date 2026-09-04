using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.ListRotationPolicies;


public sealed class ListRotationPoliciesHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<ListRotationPoliciesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListRotationPoliciesQuery message, CancellationToken cancellationToken) => ListRotationPoliciesAsync(db, routing, cancellationToken);

    private static async Task<UseCaseResult> ListRotationPoliciesAsync(IPlatformDbContext db, TenantDatabaseRoutingScope routing, CancellationToken ct)
    { using IDisposable control = routing.EnterControlPlane(); return UseCaseResults.Ok(await db.ApiCredentialRotationPolicies.AsNoTracking().OrderByDescending(x => x.IsActive).ThenByDescending(x => x.CreatedAt).ToArrayAsync(ct)); }


}

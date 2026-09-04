using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.ListBillingAccounts;


public sealed class ListBillingAccountsHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<ListBillingAccountsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListBillingAccountsQuery message, CancellationToken cancellationToken) => ListBillingAccountsAsync(db, routing, cancellationToken);

    private static async Task<UseCaseResult> ListBillingAccountsAsync(IPlatformDbContext db, TenantDatabaseRoutingScope routing, CancellationToken ct)
    { using IDisposable control = routing.EnterControlPlane(); return UseCaseResults.Ok(await db.BillingAccounts.AsNoTracking().OrderByDescending(x => x.CreatedAt).ToArrayAsync(ct)); }


}

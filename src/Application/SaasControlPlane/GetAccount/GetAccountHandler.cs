using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TenantAccount;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.GetAccount;
using TenantAccount = global::EdgIQ.MurimiOS.Domain.TenantAccount.TenantAccount;

public sealed class GetAccountHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<GetAccountQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetAccountQuery message, CancellationToken cancellationToken) => GetAccountAsync(db, routing, cancellationToken);

    private static async Task<UseCaseResult> GetAccountAsync(IPlatformDbContext db, TenantDatabaseRoutingScope routing, CancellationToken ct)
    { using IDisposable control = routing.EnterControlPlane(); TenantAccount? account = await db.TenantAccounts.AsNoTracking().SingleOrDefaultAsync(ct); return account is null ? UseCaseResults.NotFound() : UseCaseResults.Ok(account); }


}

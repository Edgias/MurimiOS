using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TenantAccount;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.ProvisionAccount;
using TenantAccount = global::EdgIQ.MurimiOS.Domain.TenantAccount.TenantAccount;

public sealed class ProvisionAccountHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing, IRequestActorContext actor) : IRequestHandler<ProvisionAccountCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ProvisionAccountCommand message, CancellationToken cancellationToken) => ProvisionAccountAsync(message.request, db, routing, actor, cancellationToken);

    private static async Task<UseCaseResult> ProvisionAccountAsync(ProvisionTenantAccountRequest request, IPlatformDbContext db,
        TenantDatabaseRoutingScope routing, IRequestActorContext actor, CancellationToken ct)
    {
        using IDisposable control = routing.EnterControlPlane();
        if (await db.TenantAccounts.AnyAsync(ct)) return Conflict("tenant_account_exists", "This tenant already has a SaaS account.");
        try
        {
            var account = new TenantAccount(Id(request.Id), request.LegalName, request.DisplayName, request.Slug,
                request.CountryCode, request.BillingCurrency, request.TimeZone, DateTimeOffset.UtcNow, actor.Subject);
            db.TenantAccounts.Add(account); await db.SaveChangesAsync(ct);
            return UseCaseResults.Created("/api/v1/saas/account", account);
        }
        catch (ArgumentException e) { return Bad("tenant_account_invalid", e.Message); }
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

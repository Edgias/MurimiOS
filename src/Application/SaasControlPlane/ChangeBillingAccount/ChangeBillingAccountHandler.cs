using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.BillingAccount;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.ChangeBillingAccount;
using BillingAccount = global::EdgIQ.MurimiOS.Domain.BillingAccount.BillingAccount;

public sealed class ChangeBillingAccountHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<ChangeBillingAccountCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeBillingAccountCommand message, CancellationToken cancellationToken) => ChangeBillingAccountAsync(message.accountId, message.command, db, routing, cancellationToken);

    private static async Task<UseCaseResult> ChangeBillingAccountAsync(Guid accountId, string command, IPlatformDbContext db,
        TenantDatabaseRoutingScope routing, CancellationToken ct)
    {
        using IDisposable control = routing.EnterControlPlane(); BillingAccount? account = await db.BillingAccounts.SingleOrDefaultAsync(x => x.Id == accountId, ct); if (account is null) return UseCaseResults.NotFound();
        try { if (command == "activate") account.Activate(); else if (command == "suspend") account.Suspend(); else account.Close(); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(account); }
        catch (InvalidOperationException e) { return Conflict("billing_account_transition_invalid", e.Message); }
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

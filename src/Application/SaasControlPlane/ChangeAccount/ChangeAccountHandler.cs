using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TenantAccount;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.ChangeAccount;
using TenantAccount = global::EdgIQ.MurimiOS.Domain.TenantAccount.TenantAccount;

public sealed class ChangeAccountHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<ChangeAccountCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeAccountCommand message, CancellationToken cancellationToken) => ChangeAccountAsync(message.command, message.request, db, routing, cancellationToken);

    private static async Task<UseCaseResult> ChangeAccountAsync(string command, LifecycleReasonRequest request,
        IPlatformDbContext db, TenantDatabaseRoutingScope routing, CancellationToken ct)
    {
        using IDisposable control = routing.EnterControlPlane(); TenantAccount? account = await db.TenantAccounts.SingleOrDefaultAsync(ct);
        if (account is null) return UseCaseResults.NotFound();
        try
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            if (command is "activate" or "resume") account.Activate(now);
            else if (command == "suspend") account.Suspend(now, request.Reason);
            else account.Close(now, request.Reason);
            await db.SaveChangesAsync(ct); return UseCaseResults.Ok(account);
        }
        catch (Exception e) when (e is InvalidOperationException or ArgumentException) { return Conflict("tenant_account_transition_invalid", e.Message); }
    }

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

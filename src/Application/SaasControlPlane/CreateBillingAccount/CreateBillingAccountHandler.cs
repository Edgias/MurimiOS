using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.BillingAccount;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.CreateBillingAccount;
using BillingAccount = global::EdgIQ.MurimiOS.Domain.BillingAccount.BillingAccount;
using BillingAccountStatus = global::EdgIQ.MurimiOS.Domain.BillingAccount.BillingAccountStatus;

public sealed class CreateBillingAccountHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<CreateBillingAccountCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateBillingAccountCommand message, CancellationToken cancellationToken) => CreateBillingAccountAsync(message.request, db, routing, cancellationToken);

    private static async Task<UseCaseResult> CreateBillingAccountAsync(CreateBillingAccountRequest request, IPlatformDbContext db,
        TenantDatabaseRoutingScope routing, CancellationToken ct)
    {
        using IDisposable control = routing.EnterControlPlane();
        if (await db.BillingAccounts.AnyAsync(x => x.Status != BillingAccountStatus.Closed, ct)) return Conflict("billing_account_exists", "Close the current billing account first.");
        try { var account = new BillingAccount(Id(request.Id), request.Provider, request.ProviderCustomerReference, request.BillingEmail, request.TaxIdentifier, DateTimeOffset.UtcNow); db.BillingAccounts.Add(account); await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/saas/billing/accounts/{account.Id}", account); }
        catch (ArgumentException e) { return Bad("billing_account_invalid", e.Message); }
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

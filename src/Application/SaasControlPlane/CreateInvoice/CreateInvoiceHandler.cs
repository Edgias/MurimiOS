using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.BillingAccount;
using EdgIQ.MurimiOS.Domain.BillingInvoice;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.CreateInvoice;
using BillingAccount = global::EdgIQ.MurimiOS.Domain.BillingAccount.BillingAccount;

using BillingAccountStatus = global::EdgIQ.MurimiOS.Domain.BillingAccount.BillingAccountStatus;
using BillingInvoice = global::EdgIQ.MurimiOS.Domain.BillingInvoice.BillingInvoice;

public sealed class CreateInvoiceHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<CreateInvoiceCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateInvoiceCommand message, CancellationToken cancellationToken) => CreateInvoiceAsync(message.request, db, routing, cancellationToken);

    private static async Task<UseCaseResult> CreateInvoiceAsync(CreateBillingInvoiceRequest request, IPlatformDbContext db,
        TenantDatabaseRoutingScope routing, CancellationToken ct)
    {
        using IDisposable control = routing.EnterControlPlane();
        if (!await db.BillingAccounts.AnyAsync(x => x.Id == request.BillingAccountId && x.Status != BillingAccountStatus.Closed, ct)) return Bad("billing_account_invalid", "An active or suspended billing account is required.");
        if (request.SubscriptionId.HasValue && !await db.TenantSubscriptions.AnyAsync(x => x.Id == request.SubscriptionId, ct)) return Bad("subscription_invalid", "Invoice subscription does not exist.");
        try { var invoice = new BillingInvoice(Id(request.Id), request.BillingAccountId, request.SubscriptionId, request.InvoiceNumber, request.ProviderInvoiceReference, request.Subtotal, request.Tax, request.Total, request.Currency, request.IssuedAt, request.DueAt); db.BillingInvoices.Add(invoice); await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/saas/billing/invoices/{invoice.Id}", invoice); }
        catch (ArgumentException e) { return Bad("billing_invoice_invalid", e.Message); }
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

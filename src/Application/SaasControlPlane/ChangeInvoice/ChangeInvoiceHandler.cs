using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.BillingInvoice;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.ChangeInvoice;
using BillingInvoice = global::EdgIQ.MurimiOS.Domain.BillingInvoice.BillingInvoice;

public sealed class ChangeInvoiceHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<ChangeInvoiceCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeInvoiceCommand message, CancellationToken cancellationToken) => ChangeInvoiceAsync(message.invoiceId, message.command, message.request, db, routing, cancellationToken);

    private static async Task<UseCaseResult> ChangeInvoiceAsync(Guid invoiceId, string command, LifecycleReasonRequest request,
        IPlatformDbContext db, TenantDatabaseRoutingScope routing, CancellationToken ct)
    {
        using IDisposable control = routing.EnterControlPlane(); BillingInvoice? invoice = await db.BillingInvoices.SingleOrDefaultAsync(x => x.Id == invoiceId, ct); if (invoice is null) return UseCaseResults.NotFound();
        try { DateTimeOffset now = DateTimeOffset.UtcNow; if (command == "paid") invoice.MarkPaid(now); else if (command == "void") invoice.Void(now, request.Reason); else invoice.MarkUncollectible(now, request.Reason); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(invoice); }
        catch (Exception e) when (e is InvalidOperationException or ArgumentException) { return Conflict("billing_invoice_transition_invalid", e.Message); }
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

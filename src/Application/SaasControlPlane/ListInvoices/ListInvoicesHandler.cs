using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.BillingInvoice;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.ListInvoices;
using BillingInvoice = global::EdgIQ.MurimiOS.Domain.BillingInvoice.BillingInvoice;
using BillingInvoiceStatus = global::EdgIQ.MurimiOS.Domain.BillingInvoice.BillingInvoiceStatus;

public sealed class ListInvoicesHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<ListInvoicesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListInvoicesQuery message, CancellationToken cancellationToken) => ListInvoicesAsync(message.status, db, routing, cancellationToken);

    private static async Task<UseCaseResult> ListInvoicesAsync(BillingInvoiceStatus? status, IPlatformDbContext db,
        TenantDatabaseRoutingScope routing, CancellationToken ct)
    { using IDisposable control = routing.EnterControlPlane(); IQueryable<BillingInvoice> query = db.BillingInvoices.AsNoTracking(); if (status.HasValue) query = query.Where(x => x.Status == status); return UseCaseResults.Ok(await query.OrderByDescending(x => x.IssuedAt).Take(5_000).ToArrayAsync(ct)); }


}

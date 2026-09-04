using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.BillingInvoice;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.SaasControlPlaneEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ListInvoices;
using BillingInvoiceStatus = global::EdgIQ.MurimiOS.Domain.BillingInvoice.BillingInvoiceStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListInvoicesEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapGet("/billing/invoices", async Task<IResult> (BillingInvoiceStatus? status, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.ListInvoices.ListInvoicesQuery(status), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SaasRead);

    }
}

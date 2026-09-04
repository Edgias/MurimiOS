using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.CreateInvoice;
using static EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.SaasControlPlaneEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.CreateInvoice;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateInvoiceEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/billing/invoices", async Task<IResult> (CreateBillingInvoiceRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.CreateInvoice.CreateInvoiceCommand(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.CreateInvoice.CreateBillingInvoiceRequest(request.Id, request.BillingAccountId, request.SubscriptionId, request.InvoiceNumber, request.ProviderInvoiceReference, request.Subtotal, request.Tax, request.Total, request.Currency, request.IssuedAt, request.DueAt)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SaasBillingManage, PlatformOperatorPolicy);

    }
}

using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.SaasControlPlaneEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ChangeInvoice;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using LifecycleReasonRequest = global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.Shared.LifecycleReasonRequest;
internal static class ChangeInvoiceEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/billing/invoices/{invoiceId:guid}/{command:regex(^paid|void|uncollectible$)}", async Task<IResult> (Guid invoiceId, string command, LifecycleReasonRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.ChangeInvoice.ChangeInvoiceCommand(invoiceId, command, new global::EdgIQ.MurimiOS.Application.SaasControlPlane.ChangeInvoice.LifecycleReasonRequest(request.Reason)), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.SaasBillingManage, PlatformOperatorPolicy);

    }
}

using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.CreateBillingAccount;
using static EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.SaasControlPlaneEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.CreateBillingAccount;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateBillingAccountEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/billing/accounts", async Task<IResult> (CreateBillingAccountRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.CreateBillingAccount.CreateBillingAccountCommand(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.CreateBillingAccount.CreateBillingAccountRequest(request.Id, request.Provider, request.ProviderCustomerReference, request.BillingEmail, request.TaxIdentifier)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SaasBillingManage, PlatformOperatorPolicy);

    }
}

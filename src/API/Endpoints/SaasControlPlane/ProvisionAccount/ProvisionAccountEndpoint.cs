using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ProvisionAccount;
using static EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.SaasControlPlaneEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ProvisionAccount;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ProvisionAccountEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/account", async Task<IResult> (ProvisionTenantAccountRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.ProvisionAccount.ProvisionAccountCommand(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.ProvisionAccount.ProvisionTenantAccountRequest(request.Id, request.LegalName, request.DisplayName, request.Slug, request.CountryCode, request.BillingCurrency, request.TimeZone)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SaasManage, PlatformOperatorPolicy);

    }
}

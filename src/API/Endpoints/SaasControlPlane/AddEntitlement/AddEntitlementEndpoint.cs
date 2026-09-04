using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.AddEntitlement;
using static EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.SaasControlPlaneEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.AddEntitlement;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class AddEntitlementEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/plans/{planId:guid}/entitlements", async Task<IResult> (Guid planId, CreatePlanEntitlementRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.AddEntitlement.AddEntitlementCommand(planId, new global::EdgIQ.MurimiOS.Application.SaasControlPlane.AddEntitlement.CreatePlanEntitlementRequest(request.Id, request.Key, request.Kind, request.IncludedAmount, request.Unit, request.Enforcement)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SaasManage, PlatformOperatorPolicy);

    }
}

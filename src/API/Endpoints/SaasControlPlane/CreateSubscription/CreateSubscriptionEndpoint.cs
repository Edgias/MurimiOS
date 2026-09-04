using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.CreateSubscription;
using static EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.SaasControlPlaneEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.CreateSubscription;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateSubscriptionEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/subscriptions", async Task<IResult> (CreateTenantSubscriptionRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.CreateSubscription.CreateSubscriptionCommand(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.CreateSubscription.CreateTenantSubscriptionRequest(request.Id, request.PlanVersionId, request.StartsAt, request.CurrentPeriodEndsAt, request.TrialEndsAt, request.ProviderSubscriptionReference)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SaasManage, PlatformOperatorPolicy);

    }
}

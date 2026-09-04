using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ChangeSubscription;
using static EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.SaasControlPlaneEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ChangeSubscription;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ChangeSubscriptionEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/subscriptions/{subscriptionId:guid}/{command:regex(^activate|past-due|suspend|renew|schedule-cancellation|cancel$)}",
            async Task<IResult> (Guid subscriptionId, string command, SubscriptionCommandRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.ChangeSubscription.ChangeSubscriptionCommand(subscriptionId, command, new global::EdgIQ.MurimiOS.Application.SaasControlPlane.ChangeSubscription.SubscriptionCommandRequest(request.Reason, request.PeriodStartsAt, request.PeriodEndsAt, request.CancelAt)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SaasManage, PlatformOperatorPolicy);

    }
}

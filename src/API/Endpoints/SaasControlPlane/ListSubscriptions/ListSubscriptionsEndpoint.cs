using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.SaasControlPlaneEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ListSubscriptions;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListSubscriptionsEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/subscriptions", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.ListSubscriptions.ListSubscriptionsQuery(), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SaasRead);

    }
}

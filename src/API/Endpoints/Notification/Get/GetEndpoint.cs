using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Notification.Get;
using static EdgIQ.MurimiOS.API.Endpoints.Notification.NotificationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Notification.Get;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapGet("/notifications", async Task<IResult> (bool? unreadOnly, int? limit, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Notification.Get.GetQuery(unreadOnly, limit), cancellationToken)).ToHttpResult())
            .WithTags("Notifications")
            .RequireAuthorization(PlatformPermissions.NotificationsRead);

    }
}

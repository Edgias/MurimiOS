using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Notification.NotificationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Notification.GetPreferences;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetPreferencesEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapGet("/notification-preferences", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Notification.GetPreferences.GetPreferencesQuery(), cancellationToken)).ToHttpResult())
            .WithTags("Notifications")
            .RequireAuthorization(PlatformPermissions.NotificationsRead);

    }
}

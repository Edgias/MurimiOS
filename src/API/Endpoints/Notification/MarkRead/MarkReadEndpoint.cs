using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Notification.NotificationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Notification.MarkRead;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class MarkReadEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/notifications/{notificationId:guid}/read", async Task<IResult> (Guid notificationId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Notification.MarkRead.MarkReadCommand(notificationId), cancellationToken)).ToHttpResult())
            .WithTags("Notifications")
            .RequireAuthorization(PlatformPermissions.NotificationsRead);

    }
}

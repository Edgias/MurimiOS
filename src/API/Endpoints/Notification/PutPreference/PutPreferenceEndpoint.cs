using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Notification.PutPreference;
using static EdgIQ.MurimiOS.API.Endpoints.Notification.NotificationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Notification.PutPreference;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class PutPreferenceEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPut("/notification-preferences/{category}", async Task<IResult> (string category, UpdateNotificationPreferenceRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Notification.PutPreference.PutPreferenceCommand(category, new global::EdgIQ.MurimiOS.Application.Notification.PutPreference.UpdateNotificationPreferenceRequest(request.InAppEnabled, request.EmailEnabled, request.SmsEnabled)), cancellationToken)).ToHttpResult())
            .WithTags("Notifications")
            .RequireAuthorization(PlatformPermissions.NotificationsRead);

    }
}

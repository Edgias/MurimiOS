using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.API.Endpoints.Access.Create;
using EdgIQ.MurimiOS.API.Endpoints.Organisation.Create;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Create;
using EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack.Create;
using EdgIQ.MurimiOS.API.Endpoints.Notification.Create;
using static EdgIQ.MurimiOS.API.Endpoints.Notification.NotificationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Notification.Create;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/notifications", async Task<IResult> (CreateNotificationRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Notification.Create.CreateCommand(new global::EdgIQ.MurimiOS.Application.Notification.Create.CreateNotificationRequest(request.RecipientMembershipId, request.Category, request.Title, request.Body, request.Severity, request.ResourceType, request.ResourceId, request.ActionUrl)), cancellationToken)).ToHttpResult())
            .WithTags("Notifications")
            .RequireAuthorization(PlatformPermissions.NotificationsManage);

    }
}

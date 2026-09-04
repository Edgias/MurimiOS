using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PostGroupWebhookDeliveriesDeliveryIdGuidReplay;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class PostGroupWebhookDeliveriesDeliveryIdGuidReplayEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/webhook-deliveries/{deliveryId:guid}/replay", async Task<IResult> (Guid deliveryId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.PostGroupWebhookDeliveriesDeliveryIdGuidReplay.PostGroupWebhookDeliveriesDeliveryIdGuidReplayCommand(deliveryId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.PublicApiManage);

    }
}

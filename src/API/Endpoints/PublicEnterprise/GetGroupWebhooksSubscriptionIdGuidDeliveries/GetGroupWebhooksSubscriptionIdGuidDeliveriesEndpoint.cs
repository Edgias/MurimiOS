using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.WebhookSubscription;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.GetGroupWebhooksSubscriptionIdGuidDeliveries;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using WebhookDeliveryStatus = global::EdgIQ.MurimiOS.Domain.WebhookSubscription.WebhookDeliveryStatus;
internal static class GetGroupWebhooksSubscriptionIdGuidDeliveriesEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapGet("/webhooks/{subscriptionId:guid}/deliveries", async Task<IResult> (Guid subscriptionId, WebhookDeliveryStatus? status, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.GetGroupWebhooksSubscriptionIdGuidDeliveries.GetGroupWebhooksSubscriptionIdGuidDeliveriesQuery(subscriptionId, status), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.PublicApiRead);

    }
}

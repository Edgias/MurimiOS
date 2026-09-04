using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateWebhook;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateWebhook;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateWebhookEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/webhooks", async Task<IResult> (CreateWebhookRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.CreateWebhook.CreateWebhookCommand(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.CreateWebhook.CreateWebhookRequest(request.Id, request.Name, request.Endpoint, request.EventTypes, request.MaximumAttempts)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.PublicApiManage);

    }
}

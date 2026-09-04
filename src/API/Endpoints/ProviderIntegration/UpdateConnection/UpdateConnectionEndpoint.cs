using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.UpdateConnection;
using static EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ProviderIntegrationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.UpdateConnection;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class UpdateConnectionEndpoint
{
    internal static void Map(IEndpointRouteBuilder integrations)
    {
        integrations.MapPut("/connections/{connectionId:guid}", async Task<IResult> (Guid connectionId, UpdateIntegrationConnectionRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ProviderIntegration.UpdateConnection.UpdateConnectionCommand(connectionId, new global::EdgIQ.MurimiOS.Application.ProviderIntegration.UpdateConnection.UpdateIntegrationConnectionRequest(request.Name, request.Configuration)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.IntegrationsManage);

    }
}

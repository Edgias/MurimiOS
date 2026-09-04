using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.GetConnection;
using static EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ProviderIntegrationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.GetConnection;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetConnectionEndpoint
{
    internal static void Map(IEndpointRouteBuilder integrations)
    {
        integrations.MapGet("/connections/{connectionId:guid}", async Task<IResult> (Guid connectionId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ProviderIntegration.GetConnection.GetConnectionQuery(connectionId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.IntegrationsRead);

    }
}

using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ProviderIntegrationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ListConnections;
using IntegrationCategory = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCategory;
using IntegrationConnectionStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnectionStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListConnectionsEndpoint
{
    internal static void Map(IEndpointRouteBuilder integrations)
    {
        integrations.MapGet("/connections", async Task<IResult> (IntegrationCategory? category, IntegrationConnectionStatus? status, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ProviderIntegration.ListConnections.ListConnectionsQuery(category, status), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.IntegrationsRead);

    }
}

using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ProviderIntegrationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.RevokeCredential;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class RevokeCredentialEndpoint
{
    internal static void Map(IEndpointRouteBuilder integrations)
    {
        integrations.MapPost("/connections/{connectionId:guid}/credentials/{credentialId:guid}/revoke", async Task<IResult> (Guid connectionId, Guid credentialId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ProviderIntegration.RevokeCredential.RevokeCredentialCommand(connectionId, credentialId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.IntegrationsManage);

    }
}

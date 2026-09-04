using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PostGroupCredentialsCredentialIdGuidRevoke;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class PostGroupCredentialsCredentialIdGuidRevokeEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/credentials/{credentialId:guid}/revoke", async Task<IResult> (Guid credentialId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.PostGroupCredentialsCredentialIdGuidRevoke.PostGroupCredentialsCredentialIdGuidRevokeCommand(credentialId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.PublicApiManage);

    }
}

using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PostRetailTokensTokenIdGuidRevoke;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class PostRetailTokensTokenIdGuidRevokeEndpoint
{
    internal static void Map(IEndpointRouteBuilder retail)
    {
        retail.MapPost("/tokens/{tokenId:guid}/revoke", async Task<IResult> (Guid tokenId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.PostRetailTokensTokenIdGuidRevoke.PostRetailTokensTokenIdGuidRevokeCommand(tokenId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.RetailTransparencyManage);

    }
}

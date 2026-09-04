using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.GetRetailAccessLog;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetRetailAccessLogEndpoint
{
    internal static void Map(IEndpointRouteBuilder retail)
    {
        retail.MapGet("/access-log", async Task<IResult> (Guid? tokenId, int? limit, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.GetRetailAccessLog.GetRetailAccessLogQuery(tokenId, limit), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.RetailTransparencyRead);

    }
}

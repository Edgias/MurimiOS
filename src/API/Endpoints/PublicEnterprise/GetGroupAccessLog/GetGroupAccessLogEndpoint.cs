using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.GetGroupAccessLog;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetGroupAccessLogEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapGet("/access-log", async Task<IResult> (Guid? clientId, int? limit, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.GetGroupAccessLog.GetGroupAccessLogQuery(clientId, limit), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.PublicApiRead);

    }
}

using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Compliance.ComplianceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.InstallStandardPack;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class InstallStandardPackEndpoint
{
    internal static void Map(IEndpointRouteBuilder standards)
    {
        standards.MapPost("/packs/{packKey}/install", async Task<IResult> (string packKey, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Compliance.InstallStandardPack.InstallStandardPackCommand(packKey), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ComplianceManage);

    }
}

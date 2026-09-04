using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Compliance.ComplianceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.ListStandardPacks;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListStandardPacksEndpoint
{
    internal static void Map(IEndpointRouteBuilder standards)
    {
        standards.MapGet("/packs", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Compliance.ListStandardPacks.ListStandardPacksQuery(), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ComplianceRead);

    }
}

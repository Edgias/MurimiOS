using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static EdgIQ.MurimiOS.API.Endpoints.Evidence.EvidenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Evidence.Download;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class DownloadEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {

        api.MapGet("/evidence/{evidenceId:guid}/content", async Task<IResult> (Guid evidenceId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Evidence.Download.DownloadQuery(evidenceId), cancellationToken)).ToHttpResult()).WithTags("Evidence").RequireAuthorization(PlatformPermissions.EvidenceRead);

    }
}

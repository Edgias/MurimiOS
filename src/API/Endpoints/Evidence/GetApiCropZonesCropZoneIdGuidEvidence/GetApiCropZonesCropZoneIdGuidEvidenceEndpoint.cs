using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static EdgIQ.MurimiOS.API.Endpoints.Evidence.EvidenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Evidence.GetApiCropZonesCropZoneIdGuidEvidence;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetApiCropZonesCropZoneIdGuidEvidenceEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {

        api.MapGet("/crop-zones/{cropZoneId:guid}/evidence", async Task<IResult> (Guid cropZoneId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Evidence.GetApiCropZonesCropZoneIdGuidEvidence.GetApiCropZonesCropZoneIdGuidEvidenceQuery(cropZoneId), cancellationToken)).ToHttpResult()).WithTags("Evidence").RequireAuthorization(PlatformPermissions.EvidenceRead);

    }
}

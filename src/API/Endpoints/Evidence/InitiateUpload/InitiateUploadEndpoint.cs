using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static EdgIQ.MurimiOS.API.Endpoints.Evidence.EvidenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Evidence.InitiateUpload;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class InitiateUploadEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/media/uploads", async Task<IResult> (InitiateEvidenceUploadRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Evidence.InitiateUpload.InitiateUploadCommand(new global::EdgIQ.MurimiOS.Application.Evidence.InitiateUpload.InitiateEvidenceUploadRequest(request.EvidenceId, request.CropZoneId, request.FileName, request.ContentType, request.ByteSize, request.Sha256, request.CapturedAt, request.Latitude, request.Longitude, request.AccuracyMetres)), cancellationToken)).ToHttpResult()).WithTags("Evidence").RequireAuthorization(PlatformPermissions.EvidenceWrite);

    }
}

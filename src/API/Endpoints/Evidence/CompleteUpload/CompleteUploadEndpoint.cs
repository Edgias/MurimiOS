using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static EdgIQ.MurimiOS.API.Endpoints.Evidence.EvidenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Evidence.CompleteUpload;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CompleteUploadEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/media/uploads/{uploadId:guid}/complete", async Task<IResult> (Guid uploadId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Evidence.CompleteUpload.CompleteUploadCommand(uploadId), cancellationToken)).ToHttpResult()).WithTags("Evidence").RequireAuthorization(PlatformPermissions.EvidenceWrite);

    }
}

using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static EdgIQ.MurimiOS.API.Endpoints.Evidence.EvidenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Evidence.AppendChunk;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using IncomingContentAdapter = global::EdgIQ.MurimiOS.API.Infrastructure.IncomingContentAdapter;
internal static class AppendChunkEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPut("/media/uploads/{uploadId:guid}", async Task<IResult> (Guid uploadId, HttpRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Evidence.AppendChunk.AppendChunkCommand(uploadId, EdgIQ.MurimiOS.API.Infrastructure.IncomingContentAdapter.Bind(request)), cancellationToken)).ToHttpResult()).WithTags("Evidence").RequireAuthorization(PlatformPermissions.EvidenceWrite);

    }
}

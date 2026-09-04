using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.LivestockStatutoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.GetDocumentArtifact;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetDocumentArtifactEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapGet("/livestock/statutory-documents/{documentId:guid}/artifact", async Task<IResult> (Guid documentId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockStatutory.GetDocumentArtifact.GetDocumentArtifactQuery(documentId), cancellationToken)).ToHttpResult())
            .WithTags("Livestock statutory reporting").RequireAuthorization(PlatformPermissions.LivestockRead);

    }
}

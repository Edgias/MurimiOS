using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.SubmitDocument;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.LivestockStatutoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.SubmitDocument;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class SubmitDocumentEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/livestock/statutory-documents/{documentId:guid}/submit", async Task<IResult> (Guid documentId, SubmitStatutoryMovementDocumentRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockStatutory.SubmitDocument.SubmitDocumentCommand(documentId, new global::EdgIQ.MurimiOS.Application.LivestockStatutory.SubmitDocument.SubmitStatutoryMovementDocumentRequest(request.ConnectionId)), cancellationToken)).ToHttpResult())
            .WithTags("Livestock statutory reporting").RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}

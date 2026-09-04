using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.CreateDocument;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.LivestockStatutoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.CreateDocument;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateDocumentEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/livestock/movements/{movementId:guid}/statutory-documents", async Task<IResult> (Guid movementId, CreateStatutoryMovementDocumentRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockStatutory.CreateDocument.CreateDocumentCommand(movementId, new global::EdgIQ.MurimiOS.Application.LivestockStatutory.CreateDocument.CreateStatutoryMovementDocumentRequest(request.Id, request.DocumentNumber, request.SchemaVersion, request.ProfileId)), cancellationToken)).ToHttpResult())
            .WithTags("Livestock statutory reporting").RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}

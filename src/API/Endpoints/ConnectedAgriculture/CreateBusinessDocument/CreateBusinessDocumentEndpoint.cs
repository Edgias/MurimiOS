using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateBusinessDocument;
using static EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ConnectedAgricultureEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateBusinessDocument;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateBusinessDocumentEndpoint
{
    internal static void Map(IEndpointRouteBuilder connected)
    {
        connected.MapPost("/farms/{farmId:guid}/business-documents", async Task<IResult> (Guid farmId, CreateBusinessDocumentRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ConnectedAgriculture.CreateBusinessDocument.CreateBusinessDocumentCommand(farmId, new global::EdgIQ.MurimiOS.Application.ConnectedAgriculture.CreateBusinessDocument.CreateBusinessDocumentRequest(request.Id, request.ConnectionId, request.DocumentType, request.Direction, request.ExternalDocumentId, request.SchemaVersion, request.MappingVersion, request.Payload, request.PayloadSha256, request.OccurredAt)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ConnectedAgricultureManage);

    }
}

using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.RegisterDerivedMedia;
using static EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ConnectedAgricultureEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.RegisterDerivedMedia;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class RegisterDerivedMediaEndpoint
{
    internal static void Map(IEndpointRouteBuilder connected)
    {
        connected.MapPost("/farms/{farmId:guid}/derived-media", async Task<IResult> (Guid farmId, RegisterDerivedMediaRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ConnectedAgriculture.RegisterDerivedMedia.RegisterDerivedMediaCommand(farmId, new global::EdgIQ.MurimiOS.Application.ConnectedAgriculture.RegisterDerivedMedia.RegisterDerivedMediaRequest(request.Id, request.FieldId, request.Kind, request.SourceType, request.SourceId, request.ObjectKey, request.ContentType, request.ByteSize, request.Sha256, request.GeneratedAt)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ConnectedAgricultureManage);

    }
}

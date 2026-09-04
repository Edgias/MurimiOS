using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ConnectedAgricultureEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ChangeTaskFile;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using ExchangeCommandRequest = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.Shared.ExchangeCommandRequest;
internal static class ChangeTaskFileEndpoint
{
    internal static void Map(IEndpointRouteBuilder connected)
    {
        connected.MapPost("/task-files/{taskFileId:guid}/{command:regex(^validate|complete|reject$)}", async Task<IResult> (Guid taskFileId, string command, ExchangeCommandRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ConnectedAgriculture.ChangeTaskFile.ChangeTaskFileCommand(taskFileId, command, new global::EdgIQ.MurimiOS.Application.ConnectedAgriculture.ChangeTaskFile.ExchangeCommandRequest(request.Notes, request.AppliedEntityId)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ConnectedAgricultureManage);

    }
}

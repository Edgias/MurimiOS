using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateTaskFile;
using static EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ConnectedAgricultureEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateTaskFile;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateTaskFileEndpoint
{
    internal static void Map(IEndpointRouteBuilder connected)
    {
        connected.MapPost("/farms/{farmId:guid}/task-files", async Task<IResult> (Guid farmId, CreateMachineryTaskFileRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ConnectedAgriculture.CreateTaskFile.CreateTaskFileCommand(farmId, new global::EdgIQ.MurimiOS.Application.ConnectedAgriculture.CreateTaskFile.CreateMachineryTaskFileRequest(request.Id, request.EquipmentId, request.WorkOrderId, request.FieldId, request.Direction, request.Format, request.FileName, request.ObjectKey, request.ByteSize, request.Sha256, request.SchemaVersion, request.MappingVersion, request.ExternalTaskId)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ConnectedAgricultureManage);

    }
}

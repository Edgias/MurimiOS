using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateDevice;
using static EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ConnectedAgricultureEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateDevice;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateDeviceEndpoint
{
    internal static void Map(IEndpointRouteBuilder connected)
    {
        connected.MapPost("/farms/{farmId:guid}/devices", async Task<IResult> (Guid farmId, CreateConnectedDeviceRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ConnectedAgriculture.CreateDevice.CreateDeviceCommand(farmId, new global::EdgIQ.MurimiOS.Application.ConnectedAgriculture.CreateDevice.CreateConnectedDeviceRequest(request.Id, request.DeviceKey, request.Name, request.Kind, request.Protocol, request.ConnectionId, request.EquipmentId, request.FieldId, request.HoldingId, request.FirmwareVersion)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ConnectedAgricultureManage);

    }
}

using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.ConnectedDevice;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ConnectedAgricultureEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ListDevices;
using ConnectedDeviceStatus = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDeviceStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListDevicesEndpoint
{
    internal static void Map(IEndpointRouteBuilder connected)
    {
        connected.MapGet("/farms/{farmId:guid}/devices", async Task<IResult> (Guid farmId, ConnectedDeviceStatus? status, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ConnectedAgriculture.ListDevices.ListDevicesQuery(farmId, status), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ConnectedAgricultureRead);

    }
}

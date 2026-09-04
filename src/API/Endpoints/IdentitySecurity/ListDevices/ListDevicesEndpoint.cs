using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.IdentityDevice;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.IdentitySecurityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.ListDevices;
using IdentityDeviceStatus = global::EdgIQ.MurimiOS.Domain.IdentityDevice.IdentityDeviceStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListDevicesEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapGet("/devices", async Task<IResult> (string? subject, IdentityDeviceStatus? status, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.IdentitySecurity.ListDevices.ListDevicesQuery(subject, status), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SecurityRead);

    }
}

using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.IdentitySecurityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.RevokeDevice;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using SecurityReasonRequest = global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.Shared.SecurityReasonRequest;
internal static class RevokeDeviceEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/devices/{deviceId:guid}/revoke", async Task<IResult> (Guid deviceId, SecurityReasonRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.IdentitySecurity.RevokeDevice.RevokeDeviceCommand(deviceId, new global::EdgIQ.MurimiOS.Application.IdentitySecurity.RevokeDevice.SecurityReasonRequest(request.Reason)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SecurityManage);

    }
}

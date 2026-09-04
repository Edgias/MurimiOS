using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.IdentitySecurityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.CancelRecovery;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CancelRecoveryEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/recovery-requests/{requestId:guid}/cancel", async Task<IResult> (Guid requestId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.IdentitySecurity.CancelRecovery.CancelRecoveryCommand(requestId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AccountRecoveryManage);

    }
}

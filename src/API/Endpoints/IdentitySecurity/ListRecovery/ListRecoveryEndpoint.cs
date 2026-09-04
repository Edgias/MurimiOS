using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.AccountRecoveryRequest;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.IdentitySecurityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.ListRecovery;
using AccountRecoveryStatus = global::EdgIQ.MurimiOS.Domain.AccountRecoveryRequest.AccountRecoveryStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListRecoveryEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapGet("/recovery-requests", async Task<IResult> (AccountRecoveryStatus? status, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.IdentitySecurity.ListRecovery.ListRecoveryQuery(status), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AccountRecoveryManage);

    }
}

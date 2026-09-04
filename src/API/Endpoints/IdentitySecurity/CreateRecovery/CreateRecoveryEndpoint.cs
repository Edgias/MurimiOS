using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.CreateRecovery;
using static EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.IdentitySecurityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.CreateRecovery;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateRecoveryEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/recovery-requests", async Task<IResult> (CreateRecoveryRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.IdentitySecurity.CreateRecovery.CreateRecoveryCommand(new global::EdgIQ.MurimiOS.Application.IdentitySecurity.CreateRecovery.CreateRecoveryRequest(request.Id, request.MembershipId, request.ExpiresAt, request.MaximumAttempts)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AccountRecoveryManage);

    }
}

using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.IdentitySecurityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.RevokeSession;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using SecurityReasonRequest = global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.Shared.SecurityReasonRequest;
internal static class RevokeSessionEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/sessions/{sessionId:guid}/revoke", async Task<IResult> (Guid sessionId, SecurityReasonRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.IdentitySecurity.RevokeSession.RevokeSessionCommand(sessionId, new global::EdgIQ.MurimiOS.Application.IdentitySecurity.RevokeSession.SecurityReasonRequest(request.Reason)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SecurityManage);

    }
}

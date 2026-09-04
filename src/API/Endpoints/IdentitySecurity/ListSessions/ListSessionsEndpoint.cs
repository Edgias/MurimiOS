using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.IdentitySession;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.IdentitySecurityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.ListSessions;
using IdentitySessionStatus = global::EdgIQ.MurimiOS.Domain.IdentitySession.IdentitySessionStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListSessionsEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapGet("/sessions", async Task<IResult> (string? subject, IdentitySessionStatus? status, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.IdentitySecurity.ListSessions.ListSessionsQuery(subject, status), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SecurityRead);

    }
}

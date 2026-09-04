using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.IdentitySecurityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.RetireRotationPolicy;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class RetireRotationPolicyEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/credential-rotation-policies/{policyId:guid}/retire", async Task<IResult> (Guid policyId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.IdentitySecurity.RetireRotationPolicy.RetireRotationPolicyCommand(policyId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SecurityManage);

    }
}

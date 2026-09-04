using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.CreateRotationPolicy;
using static EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.IdentitySecurityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.CreateRotationPolicy;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateRotationPolicyEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/credential-rotation-policies", async Task<IResult> (CreateCredentialRotationPolicyRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.IdentitySecurity.CreateRotationPolicy.CreateRotationPolicyCommand(new global::EdgIQ.MurimiOS.Application.IdentitySecurity.CreateRotationPolicy.CreateCredentialRotationPolicyRequest(request.Id, request.PublicApiClientId, request.MaximumAgeDays, request.WarningDays, request.RequireExpiry)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SecurityManage);

    }
}

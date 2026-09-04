using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.CreateRatePolicy;
using static EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.IdentitySecurityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.CreateRatePolicy;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateRatePolicyEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/rate-limit-policies", async Task<IResult> (CreateApiRateLimitPolicyRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.IdentitySecurity.CreateRatePolicy.CreateRatePolicyCommand(new global::EdgIQ.MurimiOS.Application.IdentitySecurity.CreateRatePolicy.CreateApiRateLimitPolicyRequest(request.Id, request.PublicApiClientId, request.RequestLimit, request.WindowSeconds, request.DailyQuota)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SecurityManage);

    }
}

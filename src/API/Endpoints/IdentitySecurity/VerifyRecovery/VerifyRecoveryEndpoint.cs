using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.VerifyRecovery;
using static EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.IdentitySecurityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.VerifyRecovery;
internal static class VerifyRecoveryEndpoint
{
    internal static void Map(IEndpointRouteBuilder endpoints)
    {

        endpoints.MapPost("/api/public/v1/security/recovery/verify", async Task<IResult> (VerifyRecoveryRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.IdentitySecurity.VerifyRecovery.VerifyRecoveryCommand(new global::EdgIQ.MurimiOS.Application.IdentitySecurity.VerifyRecovery.VerifyRecoveryRequest(request.TenantId, request.RequestId, request.Token)), cancellationToken)).ToHttpResult())
            .WithTags("Identity security").AllowAnonymous();

    }
}

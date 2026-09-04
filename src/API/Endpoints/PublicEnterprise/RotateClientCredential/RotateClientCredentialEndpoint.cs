using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.RotateClientCredential;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.RotateClientCredential;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class RotateClientCredentialEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/clients/{clientId:guid}/credentials", async Task<IResult> (Guid clientId, RotateCredentialRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.RotateClientCredential.RotateClientCredentialCommand(clientId, new global::EdgIQ.MurimiOS.Application.PublicEnterprise.RotateClientCredential.RotateCredentialRequest(request.ExpiresAt)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.PublicApiManage);

    }
}

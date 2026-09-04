using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateClient;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateClient;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateClientEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/clients", async Task<IResult> (CreatePublicApiClientRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.CreateClient.CreateClientCommand(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.CreateClient.CreatePublicApiClientRequest(request.Id, request.Name, request.Scopes, request.FarmIds, request.ExpiresAt)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.PublicApiManage);

    }
}

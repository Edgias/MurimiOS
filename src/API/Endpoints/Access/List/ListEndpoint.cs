using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Audit.List;
using static EdgIQ.MurimiOS.API.Endpoints.Access.AccessEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Access.List;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListEndpoint
{
    internal static void Map(IEndpointRouteBuilder access)
    {

        access.MapGet("/memberships", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Access.List.ListQuery(), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.AccessManage);

    }
}

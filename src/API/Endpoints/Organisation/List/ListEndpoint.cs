using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Audit.List;
using static EdgIQ.MurimiOS.API.Endpoints.Organisation.OrganisationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Organisation.List;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListEndpoint
{
    internal static void Map(IEndpointRouteBuilder organisations)
    {
        organisations.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Organisation.List.ListQuery(), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FarmRead);

    }
}

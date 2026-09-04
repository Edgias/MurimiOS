using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Access.Create;
using EdgIQ.MurimiOS.API.Endpoints.Organisation.Create;
using static EdgIQ.MurimiOS.API.Endpoints.Organisation.OrganisationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Organisation.Create;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateEndpoint
{
    internal static void Map(IEndpointRouteBuilder organisations)
    {
        organisations.MapPost("/", async Task<IResult> (CreateOrganisationNodeRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Organisation.Create.CreateCommand(new global::EdgIQ.MurimiOS.Application.Organisation.Create.CreateOrganisationNodeRequest(request.ParentId, request.Name, request.NodeType)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AccessManage);

    }
}

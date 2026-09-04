using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Access.Create;
using EdgIQ.MurimiOS.API.Endpoints.Organisation.Create;
using static EdgIQ.MurimiOS.API.Endpoints.Access.AccessEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Access.Create;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateEndpoint
{
    internal static void Map(IEndpointRouteBuilder access)
    {
        access.MapPost("/memberships", async Task<IResult> (CreateMembershipRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Access.Create.CreateCommand(new global::EdgIQ.MurimiOS.Application.Access.Create.CreateMembershipRequest(request.ExternalSubject, request.DisplayName, request.Email, request.Role, request.Status, request.ScopeOrganisationNodeId, request.PermissionGrants)), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.AccessManage);

    }
}

using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Access.Update;
using static EdgIQ.MurimiOS.API.Endpoints.Access.AccessEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Access.Update;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class UpdateEndpoint
{
    internal static void Map(IEndpointRouteBuilder access)
    {
        access.MapPut("/memberships/{membershipId:guid}", async Task<IResult> (Guid membershipId, UpdateMembershipAccessRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Access.Update.UpdateCommand(membershipId, new global::EdgIQ.MurimiOS.Application.Access.Update.UpdateMembershipAccessRequest(request.Role, request.Status, request.ScopeOrganisationNodeId, request.PermissionGrants)), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.AccessManage);

    }
}

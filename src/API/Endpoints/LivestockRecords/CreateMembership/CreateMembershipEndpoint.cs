using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateMembership;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.LivestockRecordsEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateMembership;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateMembershipEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/livestock/groups/{groupId:guid}/memberships", async Task<IResult> (Guid groupId, CreateLivestockGroupMembershipRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockRecords.CreateMembership.CreateMembershipCommand(groupId, new global::EdgIQ.MurimiOS.Application.LivestockRecords.CreateMembership.CreateLivestockGroupMembershipRequest(request.Id, request.MemberSubjectId, request.JoinedAt)), cancellationToken)).ToHttpResult()).WithTags("Livestock").RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}

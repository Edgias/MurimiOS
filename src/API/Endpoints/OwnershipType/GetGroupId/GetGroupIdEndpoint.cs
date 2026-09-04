using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.OwnershipType.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.OwnershipType.OwnershipTypeEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.OwnershipType.GetGroupId;
using OwnershipTypeResponse = global::EdgIQ.MurimiOS.API.Endpoints.OwnershipType.Shared.OwnershipTypeResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.OwnershipType.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<OwnershipTypeResponse>(200).Produces(404);

    }
}

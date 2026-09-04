using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.OwnershipType.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.OwnershipType.OwnershipTypeEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.OwnershipType.GetGroup;
using OwnershipTypeResponse = global::EdgIQ.MurimiOS.API.Endpoints.OwnershipType.Shared.OwnershipTypeResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.OwnershipType.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<OwnershipTypeResponse>>(200);

    }
}

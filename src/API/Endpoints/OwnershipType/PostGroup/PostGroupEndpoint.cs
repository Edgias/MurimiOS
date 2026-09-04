using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.OwnershipType.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.OwnershipType.OwnershipTypeEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.OwnershipType.PostGroup;
using OwnershipTypeRequest = global::EdgIQ.MurimiOS.API.Endpoints.OwnershipType.Shared.OwnershipTypeRequest;
using OwnershipTypeResponse = global::EdgIQ.MurimiOS.API.Endpoints.OwnershipType.Shared.OwnershipTypeResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, OwnershipTypeRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.OwnershipType.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.OwnershipType.PostGroup.OwnershipTypeRequest { Name = request.Name }), cancellationToken)).ToHttpResult()).Produces<OwnershipTypeResponse>(201);

    }
}

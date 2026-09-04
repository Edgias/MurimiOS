using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.OwnershipType.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.OwnershipType.OwnershipTypeEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.OwnershipType.PutGroupId;
using OwnershipTypeRequest = global::EdgIQ.MurimiOS.API.Endpoints.OwnershipType.Shared.OwnershipTypeRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, OwnershipTypeRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.OwnershipType.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.OwnershipType.PutGroupId.OwnershipTypeRequest { Name = request.Name }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}

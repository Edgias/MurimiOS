using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Field.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Field.FieldEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Field.GetGroupId;
using FieldResponse = global::EdgIQ.MurimiOS.API.Endpoints.Field.Shared.FieldResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Field.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<FieldResponse>(200).Produces(404);

    }
}

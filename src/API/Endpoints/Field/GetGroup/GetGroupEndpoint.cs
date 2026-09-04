using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Field.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Field.FieldEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Field.GetGroup;
using FieldResponse = global::EdgIQ.MurimiOS.API.Endpoints.Field.Shared.FieldResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Field.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<FieldResponse>>(200);

    }
}

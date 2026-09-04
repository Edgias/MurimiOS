using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Access.AccessEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Access.GetAccessMe;
internal static class GetAccessMeEndpoint
{
    internal static void Map(IEndpointRouteBuilder access)
    {
        access.MapGet("/me", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Access.GetAccessMe.GetAccessMeQuery(), cancellationToken)).ToHttpResult())
            .Produces<EdgIQ.MurimiOS.API.Endpoints.Access.GetAccessMe.GetAccessMeResponse>();

    }
}

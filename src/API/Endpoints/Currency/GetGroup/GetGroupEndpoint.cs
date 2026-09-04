using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Currency.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Currency.CurrencyEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Currency.GetGroup;
using CurrencyResponse = global::EdgIQ.MurimiOS.API.Endpoints.Currency.Shared.CurrencyResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Currency.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<CurrencyResponse>>(200);

    }
}

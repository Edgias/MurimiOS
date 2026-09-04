using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Currency.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Currency.CurrencyEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Currency.PostGroup;
using CurrencyRequest = global::EdgIQ.MurimiOS.API.Endpoints.Currency.Shared.CurrencyRequest;
using CurrencyResponse = global::EdgIQ.MurimiOS.API.Endpoints.Currency.Shared.CurrencyResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, CurrencyRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Currency.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.Currency.PostGroup.CurrencyRequest { Name = request.Name, CurrencyCode = request.CurrencyCode, Symbol = request.Symbol }), cancellationToken)).ToHttpResult()).Produces<CurrencyResponse>(201);

    }
}

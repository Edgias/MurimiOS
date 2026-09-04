using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Currency.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Currency.CurrencyEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Currency.GetGroupId;
using CurrencyResponse = global::EdgIQ.MurimiOS.API.Endpoints.Currency.Shared.CurrencyResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Currency.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<CurrencyResponse>(200).Produces(404);

    }
}

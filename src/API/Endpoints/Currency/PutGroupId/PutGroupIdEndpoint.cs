using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Currency.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Currency.CurrencyEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Currency.PutGroupId;
using CurrencyRequest = global::EdgIQ.MurimiOS.API.Endpoints.Currency.Shared.CurrencyRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, CurrencyRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Currency.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.Currency.PutGroupId.CurrencyRequest { Name = request.Name, CurrencyCode = request.CurrencyCode, Symbol = request.Symbol }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}

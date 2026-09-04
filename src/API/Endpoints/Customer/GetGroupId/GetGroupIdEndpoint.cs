using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Customer.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Customer.CustomerEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Customer.GetGroupId;
using CustomerResponse = global::EdgIQ.MurimiOS.API.Endpoints.Customer.Shared.CustomerResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Customer.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<CustomerResponse>(200).Produces(404);

    }
}

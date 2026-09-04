using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Customer.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Customer.CustomerEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Customer.GetGroup;
using CustomerResponse = global::EdgIQ.MurimiOS.API.Endpoints.Customer.Shared.CustomerResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Customer.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<CustomerResponse>>(200);

    }
}

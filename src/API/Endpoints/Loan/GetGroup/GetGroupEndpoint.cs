using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Loan.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Loan.LoanEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Loan.GetGroup;
using LoanResponse = global::EdgIQ.MurimiOS.API.Endpoints.Loan.Shared.LoanResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Loan.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<LoanResponse>>(200);

    }
}

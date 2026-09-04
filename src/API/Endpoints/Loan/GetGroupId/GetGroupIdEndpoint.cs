using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Loan.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Loan.LoanEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Loan.GetGroupId;
using LoanResponse = global::EdgIQ.MurimiOS.API.Endpoints.Loan.Shared.LoanResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Loan.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<LoanResponse>(200).Produces(404);

    }
}

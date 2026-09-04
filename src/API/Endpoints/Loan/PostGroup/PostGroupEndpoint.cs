using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Loan.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Loan.LoanEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Loan.PostGroup;
using LoanRequest = global::EdgIQ.MurimiOS.API.Endpoints.Loan.Shared.LoanRequest;
using LoanResponse = global::EdgIQ.MurimiOS.API.Endpoints.Loan.Shared.LoanResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, LoanRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Loan.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.Loan.PostGroup.LoanRequest { Name = request.Name, Description = request.Description, Duration = request.Duration, PrincipalAmount = request.PrincipalAmount, InterestRate = request.InterestRate, EffectiveRate = request.EffectiveRate, CurrencyId = request.CurrencyId }), cancellationToken)).ToHttpResult()).Produces<LoanResponse>(201);

    }
}

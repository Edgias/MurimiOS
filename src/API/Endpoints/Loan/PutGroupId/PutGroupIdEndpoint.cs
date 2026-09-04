using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Loan.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Loan.LoanEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Loan.PutGroupId;
using LoanRequest = global::EdgIQ.MurimiOS.API.Endpoints.Loan.Shared.LoanRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, LoanRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Loan.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.Loan.PutGroupId.LoanRequest { Name = request.Name, Description = request.Description, Duration = request.Duration, PrincipalAmount = request.PrincipalAmount, InterestRate = request.InterestRate, EffectiveRate = request.EffectiveRate, CurrencyId = request.CurrencyId }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}

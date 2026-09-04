using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Finance.ReverseTransaction;
using static EdgIQ.MurimiOS.API.Endpoints.Finance.FinanceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Finance.ReverseTransaction;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ReverseTransactionEndpoint
{
    internal static void Map(IEndpointRouteBuilder finance)
    {
        finance.MapPost("/transactions/{transactionId:guid}/reverse", async Task<IResult> (Guid transactionId, ReverseFinanceTransactionRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Finance.ReverseTransaction.ReverseTransactionCommand(transactionId, new global::EdgIQ.MurimiOS.Application.Finance.ReverseTransaction.ReverseFinanceTransactionRequest(request.Id, request.OccurredAt, request.Reference, request.Reason)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FinanceWrite);

    }
}

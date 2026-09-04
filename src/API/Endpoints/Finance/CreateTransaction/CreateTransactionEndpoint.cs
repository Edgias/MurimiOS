using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Finance.CreateTransaction;
using static EdgIQ.MurimiOS.API.Endpoints.Finance.FinanceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Finance.CreateTransaction;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateTransactionEndpoint
{
    internal static void Map(IEndpointRouteBuilder finance)
    {
        finance.MapPost("/farms/{farmId:guid}/transactions", async Task<IResult> (Guid farmId, CreateFinanceTransactionRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Finance.CreateTransaction.CreateTransactionCommand(farmId, new global::EdgIQ.MurimiOS.Application.Finance.CreateTransaction.CreateFinanceTransactionRequest(request.Id, request.EntryType, request.Category, request.Amount, request.Currency, request.OccurredAt, request.SourceType, request.SourceId, request.Reference, request.Counterparty, request.Description, request.Allocations.Select(item => new global::EdgIQ.MurimiOS.Application.Finance.CreateTransaction.FinanceAllocationRequest(item.Id, item.CostCentreId, item.Amount, item.Method, item.Percentage, item.Notes)).ToArray(), request.CounterpartyLegalCompanyId, request.IntercompanyReference)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FinanceWrite);

    }
}

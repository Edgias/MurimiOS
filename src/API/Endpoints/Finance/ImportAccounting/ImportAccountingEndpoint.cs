using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Finance.ImportAccounting;
using static EdgIQ.MurimiOS.API.Endpoints.Finance.FinanceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Finance.ImportAccounting;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ImportAccountingEndpoint
{
    internal static void Map(IEndpointRouteBuilder finance)
    {
        finance.MapPost("/farms/{farmId:guid}/accounting-imports", async Task<IResult> (Guid farmId, ImportAccountingRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Finance.ImportAccounting.ImportAccountingCommand(farmId, new global::EdgIQ.MurimiOS.Application.Finance.ImportAccounting.ImportAccountingRequest(request.Id, request.Provider, request.ExternalBatchId, request.StatementStart, request.StatementEnd, request.Currency, request.Lines.Select(item => new global::EdgIQ.MurimiOS.Application.Finance.ImportAccounting.ImportAccountingLineRequest(item.Id, item.ExternalLineId, item.OccurredAt, item.EntryType, item.Amount, item.Currency, item.AccountCode, item.Reference, item.Description)).ToArray())), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FinanceReconcile);

    }
}

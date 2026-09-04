using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Finance.FinanceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Finance.ListTransactions;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListTransactionsEndpoint
{
    internal static void Map(IEndpointRouteBuilder finance)
    {
        finance.MapGet("/farms/{farmId:guid}/transactions", async Task<IResult> (Guid farmId, DateTimeOffset? from, DateTimeOffset? to, string? currency, Guid? costCentreId, int? limit, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Finance.ListTransactions.ListTransactionsQuery(farmId, from, to, currency, costCentreId, limit), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FinanceRead);

    }
}

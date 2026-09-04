using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Finance.FinanceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Finance.GetFarmProfitability;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetFarmProfitabilityEndpoint
{
    internal static void Map(IEndpointRouteBuilder finance)
    {
        finance.MapGet("/farms/{farmId:guid}/profitability", async Task<IResult> (Guid farmId, DateTimeOffset from, DateTimeOffset to, string currency, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Finance.GetFarmProfitability.GetFarmProfitabilityQuery(farmId, from, to, currency), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FinanceRead);

    }
}

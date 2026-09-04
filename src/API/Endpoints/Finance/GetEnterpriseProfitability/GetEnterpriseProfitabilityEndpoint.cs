using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Finance.GetEnterpriseProfitability;
using static EdgIQ.MurimiOS.API.Endpoints.Finance.FinanceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Finance.GetEnterpriseProfitability;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetEnterpriseProfitabilityEndpoint
{
    internal static void Map(IEndpointRouteBuilder finance)
    {
        finance.MapGet("/organisation/{organisationNodeId:guid}/profitability", async Task<IResult> (Guid organisationNodeId, DateTimeOffset from, DateTimeOffset to, string currency, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Finance.GetEnterpriseProfitability.GetEnterpriseProfitabilityQuery(organisationNodeId, from, to, currency), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FinanceRead);

    }
}

using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.LivestockRecordsEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.ListHoldings;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListHoldingsEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapGet("/farms/{farmId:guid}/livestock/holdings", async Task<IResult> (Guid farmId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockRecords.ListHoldings.ListHoldingsQuery(farmId), cancellationToken)).ToHttpResult()).WithTags("Livestock").RequireAuthorization(PlatformPermissions.LivestockRead);

    }
}

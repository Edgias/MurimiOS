using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ConnectedAgricultureEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.QueryReadings;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class QueryReadingsEndpoint
{
    internal static void Map(IEndpointRouteBuilder connected)
    {
        connected.MapGet("/farms/{farmId:guid}/sensor-readings", async Task<IResult> (Guid farmId, Guid? deviceId, string? metricCode, DateTimeOffset? from, DateTimeOffset? to, int? limit, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ConnectedAgriculture.QueryReadings.QueryReadingsQuery(farmId, deviceId, metricCode, from, to, limit), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ConnectedAgricultureRead);

    }
}

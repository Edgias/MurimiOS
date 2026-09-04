using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.SaasControlPlaneEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ListUsage;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListUsageEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/usage", async Task<IResult> (string? metricCode, DateTimeOffset? from, DateTimeOffset? to, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.ListUsage.ListUsageQuery(metricCode, from, to), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SaasRead);

    }
}

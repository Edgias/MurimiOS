using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Overview;
using static EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.FieldIntelligenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Overview;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class OverviewEndpoint
{
    internal static void Map(IEndpointRouteBuilder insights)
    {
        insights.MapGet("/farms/{farmId:guid}/overview", async Task<IResult> (Guid farmId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FieldIntelligence.Overview.OverviewQuery(farmId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FieldIntelligenceRead);

    }
}

using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.IngestSatellite;
using static EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.FieldIntelligenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.IngestSatellite;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class IngestSatelliteEndpoint
{
    internal static void Map(IEndpointRouteBuilder insights)
    {
        insights.MapPost("/ingestion/records/{recordId:guid}/satellite-statistics", async Task<IResult> (Guid recordId, IngestSatelliteStatisticsRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FieldIntelligence.IngestSatellite.IngestSatelliteCommand(recordId, new global::EdgIQ.MurimiOS.Application.FieldIntelligence.IngestSatellite.IngestSatelliteStatisticsRequest(request.FieldId, request.BoundaryVersionId, request.Statistics.Select(item => new global::EdgIQ.MurimiOS.Application.FieldIntelligence.IngestSatellite.SatelliteStatisticItem(item.Id, item.ProviderAcquisitionId, item.Index, item.CustomIndex, item.AcquiredAt, item.Mean, item.Minimum, item.Maximum, item.StandardDeviation, item.ValidPixelPercent, item.CloudPercent, item.Quality)).ToArray())), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.IntegrationsOperate, PlatformPermissions.FieldIntelligenceWrite);

    }
}

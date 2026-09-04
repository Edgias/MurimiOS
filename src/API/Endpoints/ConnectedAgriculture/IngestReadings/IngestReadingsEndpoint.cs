using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.IngestReadings;
using static EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ConnectedAgricultureEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.IngestReadings;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class IngestReadingsEndpoint
{
    internal static void Map(IEndpointRouteBuilder connected)
    {
        connected.MapPost("/ingestion/records/{recordId:guid}/sensor-readings", async Task<IResult> (Guid recordId, IngestSensorReadingsRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ConnectedAgriculture.IngestReadings.IngestReadingsCommand(recordId, new global::EdgIQ.MurimiOS.Application.ConnectedAgriculture.IngestReadings.IngestSensorReadingsRequest(request.DeviceId, request.FirmwareVersion, request.Readings is null ? null : request.Readings.Select(item => new global::EdgIQ.MurimiOS.Application.ConnectedAgriculture.IngestReadings.IngestSensorReadingItem(item.Id, item.ChannelId, item.Value, item.Unit, item.ObservedAt, item.ExternalReadingId, item.Latitude, item.Longitude)).ToArray())), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ConnectedAgricultureIngest);

    }
}

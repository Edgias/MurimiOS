using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.IngestTelemetry;
using static EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ConnectedAgricultureEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.IngestTelemetry;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class IngestTelemetryEndpoint
{
    internal static void Map(IEndpointRouteBuilder connected)
    {
        connected.MapPost("/ingestion/records/{recordId:guid}/machinery-telemetry", async Task<IResult> (Guid recordId, IngestMachineryTelemetryRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ConnectedAgriculture.IngestTelemetry.IngestTelemetryCommand(recordId, new global::EdgIQ.MurimiOS.Application.ConnectedAgriculture.IngestTelemetry.IngestMachineryTelemetryRequest(request.Id, request.EquipmentId, request.DeviceId, request.EngineHours, request.SpeedKph, request.FuelLevelPercent, request.State, request.Latitude, request.Longitude)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ConnectedAgricultureIngest);

    }
}

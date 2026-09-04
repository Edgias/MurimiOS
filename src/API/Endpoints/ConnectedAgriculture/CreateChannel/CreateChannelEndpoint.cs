using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateChannel;
using static EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ConnectedAgricultureEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.CreateChannel;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateChannelEndpoint
{
    internal static void Map(IEndpointRouteBuilder connected)
    {
        connected.MapPost("/devices/{deviceId:guid}/channels", async Task<IResult> (Guid deviceId, CreateSensorChannelRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ConnectedAgriculture.CreateChannel.CreateChannelCommand(deviceId, new global::EdgIQ.MurimiOS.Application.ConnectedAgriculture.CreateChannel.CreateSensorChannelRequest(request.Id, request.ChannelKey, request.MetricCode, request.Unit, request.MinimumValidValue, request.MaximumValidValue, request.SamplingIntervalSeconds)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ConnectedAgricultureManage);

    }
}

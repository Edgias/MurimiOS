using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ConnectedDevice;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.CreateChannel;
using ConnectedDevice = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDevice;

using ConnectedDeviceStatus = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDeviceStatus;
using SensorChannel = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.SensorChannel;

public sealed class CreateChannelHandler(IPlatformDbContext db) : IRequestHandler<CreateChannelCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateChannelCommand message, CancellationToken cancellationToken) => CreateChannelAsync(message.deviceId, message.request, db, cancellationToken);

    private static async Task<UseCaseResult> CreateChannelAsync(Guid deviceId, CreateSensorChannelRequest request, IPlatformDbContext db, CancellationToken ct)
    {
        if (!await db.ConnectedDevices.AnyAsync(x => x.Id == deviceId && x.Status != ConnectedDeviceStatus.Retired, ct)) return UseCaseResults.NotFound();
        try
        {
            var channel = new SensorChannel(Id(request.Id), deviceId, request.ChannelKey, request.MetricCode, request.Unit,
                request.MinimumValidValue, request.MaximumValidValue, request.SamplingIntervalSeconds);
            db.SensorChannels.Add(channel); await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/connected/devices/{deviceId}/channels/{channel.Id}", channel);
        }
        catch (ArgumentException e) { return Bad("channel_invalid", e.Message); }
    }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

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
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.Freshness;
using ConnectedDevice = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDevice;
using ConnectedDeviceStatus = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDeviceStatus;
using SensorChannel = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.SensorChannel;

public sealed class FreshnessHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<FreshnessQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(FreshnessQuery message, CancellationToken cancellationToken) => FreshnessAsync(message.farmId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> FreshnessAsync(Guid farmId, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        ConnectedDevice[] devices = await db.ConnectedDevices.AsNoTracking().Where(x => x.FarmId == farmId && x.Status != ConnectedDeviceStatus.Retired).ToArrayAsync(ct);
        Guid[] ids = devices.Select(x => x.Id).ToArray(); SensorChannel[] channels = await db.SensorChannels.AsNoTracking().Where(x => ids.Contains(x.DeviceId) && x.IsActive).ToArrayAsync(ct);
        var latest = await db.SensorReadings.AsNoTracking().Where(x => ids.Contains(x.DeviceId)).GroupBy(x => new { x.DeviceId, x.ChannelId }).Select(x => new { x.Key.DeviceId, x.Key.ChannelId, observedAt = x.Max(r => r.ObservedAt) }).ToArrayAsync(ct);
        DateTimeOffset now = DateTimeOffset.UtcNow;
        return UseCaseResults.Ok(devices.Select(device => new { device.Id, device.DeviceKey, device.Status, device.LastSeenAt,
            channels = channels.Where(c => c.DeviceId == device.Id).Select(channel => { DateTimeOffset? observed = latest.SingleOrDefault(x => x.ChannelId == channel.Id)?.observedAt; return new { channel.Id, channel.MetricCode, channel.Unit, lastObservedAt = observed, stale = !observed.HasValue || observed < now.AddSeconds(-Math.Max(channel.SamplingIntervalSeconds * 3, 300)) }; }) }));
    }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

}

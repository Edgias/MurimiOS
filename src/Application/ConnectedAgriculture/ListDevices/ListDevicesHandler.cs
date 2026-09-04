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
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.ListDevices;
using ConnectedDevice = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDevice;
using ConnectedDeviceStatus = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDeviceStatus;
using SensorChannel = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.SensorChannel;

public sealed class ListDevicesHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListDevicesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListDevicesQuery message, CancellationToken cancellationToken) => ListDevicesAsync(message.farmId, message.status, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListDevicesAsync(Guid farmId, ConnectedDeviceStatus? status, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        IQueryable<ConnectedDevice> query = db.ConnectedDevices.AsNoTracking().Where(x => x.FarmId == farmId);
        if (status.HasValue) query = query.Where(x => x.Status == status);
        ConnectedDevice[] devices = await query.OrderBy(x => x.DeviceKey).ToArrayAsync(ct);
        Guid[] ids = devices.Select(x => x.Id).ToArray();
        SensorChannel[] channels = await db.SensorChannels.AsNoTracking().Where(x => ids.Contains(x.DeviceId)).OrderBy(x => x.ChannelKey).ToArrayAsync(ct);
        return UseCaseResults.Ok(devices.Select(x => new { device = x, channels = channels.Where(c => c.DeviceId == x.Id) }));
    }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

}

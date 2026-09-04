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
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.ChangeDevice;
using ConnectedDevice = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDevice;

public sealed class ChangeDeviceHandler(IPlatformDbContext db) : IRequestHandler<ChangeDeviceCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeDeviceCommand message, CancellationToken cancellationToken) => ChangeDeviceAsync(message.deviceId, message.command, db, cancellationToken);

    private static async Task<UseCaseResult> ChangeDeviceAsync(Guid deviceId, string command, IPlatformDbContext db, CancellationToken ct)
    {
        ConnectedDevice? device = await db.ConnectedDevices.SingleOrDefaultAsync(x => x.Id == deviceId, ct);
        if (device is null) return UseCaseResults.NotFound();
        try { if (command == "activate") device.Activate(DateTimeOffset.UtcNow); else if (command == "suspend") device.Suspend(); else device.Retire(DateTimeOffset.UtcNow); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(device); }
        catch (InvalidOperationException e) { return Conflict("device_transition_invalid", e.Message); }
    }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

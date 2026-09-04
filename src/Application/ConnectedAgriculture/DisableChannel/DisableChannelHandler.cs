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
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.DisableChannel;
using ConnectedDevice = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDevice;

using SensorChannel = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.SensorChannel;

public sealed class DisableChannelHandler(IPlatformDbContext db) : IRequestHandler<DisableChannelCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(DisableChannelCommand message, CancellationToken cancellationToken) => DisableChannelAsync(message.channelId, db, cancellationToken);

    private static async Task<UseCaseResult> DisableChannelAsync(Guid channelId, IPlatformDbContext db, CancellationToken ct)
    {
        SensorChannel? channel = await db.SensorChannels.SingleOrDefaultAsync(x => x.Id == channelId, ct);
        if (channel is null) return UseCaseResults.NotFound();
        try { channel.Disable(); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(channel); }
        catch (InvalidOperationException e) { return Conflict("channel_transition_invalid", e.Message); }
    }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

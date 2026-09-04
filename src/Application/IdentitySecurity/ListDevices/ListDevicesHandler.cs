using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.IdentityDevice;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.ListDevices;
using IdentityDevice = global::EdgIQ.MurimiOS.Domain.IdentityDevice.IdentityDevice;
using IdentityDeviceStatus = global::EdgIQ.MurimiOS.Domain.IdentityDevice.IdentityDeviceStatus;

public sealed class ListDevicesHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<ListDevicesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListDevicesQuery message, CancellationToken cancellationToken) => ListDevicesAsync(message.subject, message.status, db, routing, cancellationToken);

    private static async Task<UseCaseResult> ListDevicesAsync(string? subject, IdentityDeviceStatus? status, IPlatformDbContext db,
        TenantDatabaseRoutingScope routing, CancellationToken ct)
    { using IDisposable control = routing.EnterControlPlane(); IQueryable<IdentityDevice> q = db.IdentityDevices.AsNoTracking(); if (!string.IsNullOrWhiteSpace(subject)) q = q.Where(x => x.Subject == subject); if (status.HasValue) q = q.Where(x => x.Status == status); return UseCaseResults.Ok(await q.OrderByDescending(x => x.LastSeenAt).Take(5_000).ToArrayAsync(ct)); }


}

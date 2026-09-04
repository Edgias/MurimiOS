using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ConnectedDevice;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.CreateDevice;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;

using ConnectedDevice = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDevice;
using ConnectedDeviceKind = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDeviceKind;
using ConnectedDeviceProtocol = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDeviceProtocol;
using IntegrationConnectionStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnectionStatus;

public sealed class CreateDeviceHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<CreateDeviceCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateDeviceCommand message, CancellationToken cancellationToken) => CreateDeviceAsync(message.farmId, message.request, db, scope, cancellationToken);

    private static async Task<UseCaseResult> CreateDeviceAsync(Guid farmId, CreateConnectedDeviceRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await FarmAllowed(farmId, db, scope, ct)) return UseCaseResults.NotFound();
        if (request.ConnectionId.HasValue && !await db.IntegrationConnections.AnyAsync(x => x.Id == request.ConnectionId && x.Status != IntegrationConnectionStatus.Disabled, ct)) return Bad("connection_invalid", "The integration connection is unavailable.");
        if (request.EquipmentId.HasValue && !await db.EquipmentAssets.AnyAsync(x => x.Id == request.EquipmentId && x.FarmId == farmId, ct)) return Bad("equipment_scope_invalid", "The equipment does not belong to this farm.");
        if (request.FieldId.HasValue && !await db.FarmFields.AnyAsync(x => x.Id == request.FieldId && x.FarmId == farmId, ct)) return Bad("field_scope_invalid", "The field does not belong to this farm.");
        if (request.HoldingId.HasValue && !await db.LivestockHoldings.AnyAsync(x => x.Id == request.HoldingId && x.FarmId == farmId, ct)) return Bad("holding_scope_invalid", "The holding does not belong to this farm.");
        try
        {
            var device = new ConnectedDevice(Id(request.Id), farmId, request.DeviceKey, request.Name, request.Kind,
                request.Protocol, request.ConnectionId, request.EquipmentId, request.FieldId, request.HoldingId,
                request.FirmwareVersion, DateTimeOffset.UtcNow);
            db.ConnectedDevices.Add(device); await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/connected/devices/{device.Id}", device);
        }
        catch (ArgumentException e) { return Bad("device_invalid", e.Message); }
    }


    private static async Task<bool> FarmAllowed(Guid farmId, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct) => await scope.CanAccessFarmAsync(farmId, ct) && await db.Farms.AnyAsync(x => x.Id == farmId, ct);

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

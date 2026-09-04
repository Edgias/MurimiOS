using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.EquipmentAsset;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.CreateEquipment;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;
using EquipmentType = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentType;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class CreateEquipmentHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context) : IRequestHandler<CreateEquipmentCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateEquipmentCommand message, CancellationToken cancellationToken) => CreateEquipmentAsync(message.farmId, message.request, db, scope, tenant, actor, context, cancellationToken);

    private static async Task<UseCaseResult> CreateEquipmentAsync(Guid farmId, CreateEquipmentRequest request, IPlatformDbContext db,
        IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context, CancellationToken ct)
    {
        if (!await FarmAllowed(farmId, db, scope, ct)) return UseCaseResults.NotFound();
        try
        {
            var equipment = new EquipmentAsset(Id(request.Id), farmId, request.Code, request.Name, request.Type,
                request.Manufacturer, request.Model, request.SerialNumber, request.ModelYear);
            db.EquipmentAssets.Add(equipment); AddRecords(db, tenant, actor, farmId, "Created", equipment, "EquipmentCreated", context);
            await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/connected/equipment/{equipment.Id}", equipment);
        }
        catch (ArgumentException e) { return Bad("equipment_invalid", e.Message); }
    }


    private static async Task<bool> FarmAllowed(Guid farmId, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct) => await scope.CanAccessFarmAsync(farmId, ct) && await db.Farms.AnyAsync(x => x.Id == farmId, ct);

    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, Guid farmId, string action, object payload, string eventType, UseCaseContext context)
    { DateTimeOffset now = DateTimeOffset.UtcNow; string json = JsonSerializer.Serialize(payload); Guid entityId = payload.GetType().GetProperty("Id")?.GetValue(payload) is Guid id ? id : Guid.NewGuid(); db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId, action, payload.GetType().Name, entityId, farmId, actor.ScopeOrganisationNodeId, json, Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId)); db.SyncChanges.Add(new SyncChange(tenant.TenantId, farmId, payload.GetType().Name, entityId, action, json, now)); db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now)); }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

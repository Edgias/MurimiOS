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
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.ChangeEquipment;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class ChangeEquipmentHandler(IPlatformDbContext db, IRequestActorContext actor, ITenantContext tenant, UseCaseContext context) : IRequestHandler<ChangeEquipmentCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeEquipmentCommand message, CancellationToken cancellationToken) => ChangeEquipmentAsync(message.equipmentId, message.command, db, actor, tenant, context, cancellationToken);

    private static async Task<UseCaseResult> ChangeEquipmentAsync(Guid equipmentId, string command, IPlatformDbContext db,
        IRequestActorContext actor, ITenantContext tenant, UseCaseContext context, CancellationToken ct)
    {
        EquipmentAsset? equipment = await db.EquipmentAssets.SingleOrDefaultAsync(x => x.Id == equipmentId, ct);
        if (equipment is null) return UseCaseResults.NotFound();
        try
        {
            if (command == "maintenance") equipment.SendToMaintenance(); else if (command == "activate") equipment.ReturnToService(); else equipment.Retire(DateTimeOffset.UtcNow);
            AddRecords(db, tenant, actor, equipment.FarmId, command, equipment, $"Equipment{command}", context);
            await db.SaveChangesAsync(ct); return UseCaseResults.Ok(equipment);
        }
        catch (InvalidOperationException e) { return Conflict("equipment_transition_invalid", e.Message); }
    }

    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, Guid farmId, string action, object payload, string eventType, UseCaseContext context)
    { DateTimeOffset now = DateTimeOffset.UtcNow; string json = JsonSerializer.Serialize(payload); Guid entityId = payload.GetType().GetProperty("Id")?.GetValue(payload) is Guid id ? id : Guid.NewGuid(); db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId, action, payload.GetType().Name, entityId, farmId, actor.ScopeOrganisationNodeId, json, Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId)); db.SyncChanges.Add(new SyncChange(tenant.TenantId, farmId, payload.GetType().Name, entityId, action, json, now)); db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now)); }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

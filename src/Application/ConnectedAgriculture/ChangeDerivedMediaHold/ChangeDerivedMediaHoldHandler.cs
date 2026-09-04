using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.DerivedMediaAsset;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.ChangeDerivedMediaHold;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using DerivedMediaAsset = global::EdgIQ.MurimiOS.Domain.DerivedMediaAsset.DerivedMediaAsset;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class ChangeDerivedMediaHoldHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context) : IRequestHandler<ChangeDerivedMediaHoldCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeDerivedMediaHoldCommand message, CancellationToken cancellationToken) => ChangeDerivedMediaHoldAsync(message.assetId, message.command, message.request, db, tenant, actor, context, cancellationToken);

    private static async Task<UseCaseResult> ChangeDerivedMediaHoldAsync(Guid assetId, string command,
        DerivedMediaHoldRequest request, IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        UseCaseContext context, CancellationToken ct)
    {
        DerivedMediaAsset? asset = await db.DerivedMediaAssets.SingleOrDefaultAsync(x => x.Id == assetId, ct);
        if (asset is null) return UseCaseResults.NotFound();
        try
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            if (command == "hold") asset.PlaceLegalHold(request.Reason ?? "Legal hold", actor.Subject, now);
            else asset.ReleaseLegalHold(actor.Subject, now);
            AddRecords(db, tenant, actor, asset.FarmId, command, asset, $"DerivedMedia{command}", context);
            await db.SaveChangesAsync(ct);
            return UseCaseResults.Ok(asset);
        }
        catch (InvalidOperationException e) { return Conflict("derived_media_hold_transition_invalid", e.Message); }
        catch (ArgumentException e) { return Bad("derived_media_hold_invalid", e.Message); }
    }

    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, Guid farmId, string action, object payload, string eventType, UseCaseContext context)
    { DateTimeOffset now = DateTimeOffset.UtcNow; string json = JsonSerializer.Serialize(payload); Guid entityId = payload.GetType().GetProperty("Id")?.GetValue(payload) is Guid id ? id : Guid.NewGuid(); db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId, action, payload.GetType().Name, entityId, farmId, actor.ScopeOrganisationNodeId, json, Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId)); db.SyncChanges.Add(new SyncChange(tenant.TenantId, farmId, payload.GetType().Name, entityId, action, json, now)); db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now)); }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

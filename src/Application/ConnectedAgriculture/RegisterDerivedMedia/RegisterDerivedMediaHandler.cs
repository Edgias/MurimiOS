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
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.RegisterDerivedMedia;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using DerivedMediaAsset = global::EdgIQ.MurimiOS.Domain.DerivedMediaAsset.DerivedMediaAsset;
using DerivedMediaKind = global::EdgIQ.MurimiOS.Domain.DerivedMediaAsset.DerivedMediaKind;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class RegisterDerivedMediaHandler(IPlatformDbContext db, IMediaStorage storage, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context) : IRequestHandler<RegisterDerivedMediaCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RegisterDerivedMediaCommand message, CancellationToken cancellationToken) => RegisterDerivedMediaAsync(message.farmId, message.request, db, storage, scope, tenant, actor, context, cancellationToken);

    private static async Task<UseCaseResult> RegisterDerivedMediaAsync(Guid farmId, RegisterDerivedMediaRequest request,
        IPlatformDbContext db, IMediaStorage storage, IOrganisationScopeAccess scope, ITenantContext tenant,
        IRequestActorContext actor, UseCaseContext context, CancellationToken ct)
    {
        if (!await FarmAllowed(farmId, db, scope, ct)) return UseCaseResults.NotFound();
        if (request.FieldId.HasValue && !await db.FarmFields.AnyAsync(x => x.Id == request.FieldId && x.FarmId == farmId, ct))
            return Bad("derived_media_field_invalid", "The field does not belong to this farm.");
        if (string.IsNullOrWhiteSpace(request.ObjectKey))
            return Bad("derived_media_object_key_invalid", "A derived-media object key is required.");
        string localPrefix = $"{tenant.TenantId:N}/derived/";
        string s3Prefix = $"tenants/{tenant.TenantId:N}/derived/";
        if (!request.ObjectKey.StartsWith(localPrefix, StringComparison.Ordinal)
            && !request.ObjectKey.StartsWith(s3Prefix, StringComparison.Ordinal))
            return Bad("derived_media_object_key_invalid", "The object key is not in this tenant's derived-media namespace.");
        try
        {
            await using Stream source = await storage.OpenReadAsync(request.ObjectKey, ct);
            long actualSize = source.Length;
            string actualDigest = Convert.ToHexStringLower(await SHA256.HashDataAsync(source, ct));
            if (actualSize != request.ByteSize || !actualDigest.Equals(request.Sha256, StringComparison.OrdinalIgnoreCase))
                return Bad("derived_media_integrity_mismatch", "The registered size or SHA-256 does not match the stored object.");
            var asset = new DerivedMediaAsset(Id(request.Id), farmId, request.FieldId, request.Kind,
                request.SourceType, request.SourceId, request.ObjectKey, request.ContentType, request.ByteSize,
                request.Sha256, request.GeneratedAt);
            db.DerivedMediaAssets.Add(asset);
            AddRecords(db, tenant, actor, farmId, "Registered", asset, "DerivedMediaRegistered", context);
            await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/connected/derived-media/{asset.Id}", asset);
        }
        catch (ArgumentException e) { return Bad("derived_media_invalid", e.Message); }
        catch (IOException) { return Bad("derived_media_object_unavailable", "The derived-media object could not be read."); }
    }


    private static async Task<bool> FarmAllowed(Guid farmId, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct) => await scope.CanAccessFarmAsync(farmId, ct) && await db.Farms.AnyAsync(x => x.Id == farmId, ct);

    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, Guid farmId, string action, object payload, string eventType, UseCaseContext context)
    { DateTimeOffset now = DateTimeOffset.UtcNow; string json = JsonSerializer.Serialize(payload); Guid entityId = payload.GetType().GetProperty("Id")?.GetValue(payload) is Guid id ? id : Guid.NewGuid(); db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId, action, payload.GetType().Name, entityId, farmId, actor.ScopeOrganisationNodeId, json, Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId)); db.SyncChanges.Add(new SyncChange(tenant.TenantId, farmId, payload.GetType().Name, entityId, action, json, now)); db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now)); }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

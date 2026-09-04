using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Application.Common.Compliance;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.InstallStandardPack;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class InstallStandardPackHandler(ComplianceStandardPackCatalog catalog, ComplianceStandardPackInstaller installer, IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context) : IRequestHandler<InstallStandardPackCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(InstallStandardPackCommand message, CancellationToken cancellationToken) => InstallStandardPackAsync(message.packKey, catalog, installer, db, tenant, actor, context, cancellationToken);

    private static async Task<UseCaseResult> InstallStandardPackAsync(string packKey, ComplianceStandardPackCatalog catalog,
        ComplianceStandardPackInstaller installer, IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        UseCaseContext context, CancellationToken ct)
    {
        ComplianceStandardPack? pack = catalog.Find(packKey);
        if (pack is null) return UseCaseResults.NotFound();
        DateTimeOffset now = DateTimeOffset.UtcNow;
        (ComplianceStandardVersion standard, bool created) = await installer.InstallAsync(pack, actor.Subject, now, ct);
        if (!created) return UseCaseResults.Ok(new { standard, created = false, pack.Notice });
        AddRecords(db, tenant, actor, "Installed", nameof(ComplianceStandardVersion), standard.Id, null,
            "ComplianceStandardPackInstalled", new { standard.Id, pack.Key, pack.VersionLabel, pack.ManifestSha256, pack.SourceUri }, context);
        await db.SaveChangesAsync(ct);
        return UseCaseResults.Created($"/api/v1/compliance/standards/{standard.Id}", new { standard, created = true, pack.Notice });
    }


    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, string action,
        string entityType, Guid entityId, Guid? farmId, string eventType, object payload, UseCaseContext context)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow; string json = JsonSerializer.Serialize(payload);
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, entityType, entityId, farmId, actor.ScopeOrganisationNodeId, json, Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now));
        if (farmId.HasValue) db.SyncChanges.Add(new SyncChange(tenant.TenantId, farmId.Value, entityType, entityId, action, json, now));
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

}

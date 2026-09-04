using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.ChangeStandard;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class ChangeStandardHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context) : IRequestHandler<ChangeStandardCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeStandardCommand message, CancellationToken cancellationToken) => ChangeStandardAsync(message.standardId, message.command, db, tenant, actor, context, cancellationToken);

    private static async Task<UseCaseResult> ChangeStandardAsync(Guid standardId, string command, IPlatformDbContext db,
        ITenantContext tenant, IRequestActorContext actor, UseCaseContext context, CancellationToken ct)
    {
        ComplianceStandardVersion? standard = await db.ComplianceStandardVersions.SingleOrDefaultAsync(x => x.Id == standardId, ct);
        if (standard is null) return UseCaseResults.NotFound();
        try
        {
            if (command == "publish")
            {
                int controls = await db.ComplianceControlPoints.CountAsync(x => x.StandardVersionId == standardId, ct);
                if (controls == 0) return Bad("controls_required", "A standard needs at least one control point before publication.");
                standard.Publish(DateTimeOffset.UtcNow, actor.Subject);
            }
            else standard.Retire(DateTimeOffset.UtcNow);
            AddRecords(db, tenant, actor, command == "publish" ? "Published" : "Retired", nameof(ComplianceStandardVersion), standard.Id,
                null, command == "publish" ? "ComplianceStandardPublished" : "ComplianceStandardRetired", standard, context);
            await db.SaveChangesAsync(ct); return UseCaseResults.Ok(standard);
        }
        catch (InvalidOperationException e) { return Conflict("standard_transition_invalid", e.Message); }
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

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

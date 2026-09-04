using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.CreateAssessment;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceStandardStatus = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class CreateAssessmentHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context) : IRequestHandler<CreateAssessmentCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateAssessmentCommand message, CancellationToken cancellationToken) => CreateAssessmentAsync(message.farmId, message.request, db, scope, tenant, actor, context, cancellationToken);

    private static async Task<UseCaseResult> CreateAssessmentAsync(Guid farmId, CreateComplianceAssessmentRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        UseCaseContext context, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        if (!await db.ComplianceStandardVersions.AnyAsync(x => x.Id == request.StandardVersionId && x.Status == ComplianceStandardStatus.Published, ct))
            return Bad("standard_not_published", "Assessments require a published standard version.");
        try
        {
            var assessment = new ComplianceAssessment(Id(request.Id), farmId, request.StandardVersionId,
                request.AssessmentCode, request.ScheduledOn, request.Auditor, Json(request.Scope));
            db.ComplianceAssessments.Add(assessment);
            AddRecords(db, tenant, actor, "Created", nameof(ComplianceAssessment), assessment.Id, farmId, "ComplianceAssessmentCreated", assessment, context);
            await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/farms/{farmId}/compliance/assessments/{assessment.Id}", assessment);
        }
        catch (ArgumentException e) { return Bad("assessment_invalid", e.Message); }
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

    private static string? Json(JsonElement? value) => value.HasValue ? value.Value.GetRawText() : null;

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

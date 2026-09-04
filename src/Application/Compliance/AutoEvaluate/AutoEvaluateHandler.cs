using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Application.Common.Compliance;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.AutoEvaluate;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceAssessmentStatus = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessmentStatus;
using ComplianceControlEvaluation = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceControlEvaluation;
using ComplianceEvidenceLink = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceEvidenceLink;
using ComplianceControlPoint = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlPoint;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class AutoEvaluateHandler(IPlatformDbContext db, ComplianceEvaluationEngine engine, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, UseCaseContext context) : IRequestHandler<AutoEvaluateCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(AutoEvaluateCommand message, CancellationToken cancellationToken) => AutoEvaluateAsync(message.farmId, message.assessmentId, db, engine, scope, actor, tenant, context, cancellationToken);

    private static async Task<UseCaseResult> AutoEvaluateAsync(Guid farmId, Guid assessmentId,
        IPlatformDbContext db, ComplianceEvaluationEngine engine, IOrganisationScopeAccess scope,
        IRequestActorContext actor, ITenantContext tenant, UseCaseContext context, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        ComplianceAssessment? assessment = await db.ComplianceAssessments.SingleOrDefaultAsync(x => x.Id == assessmentId && x.FarmId == farmId, ct);
        if (assessment is null) return UseCaseResults.NotFound();
        if (assessment.Status != ComplianceAssessmentStatus.InProgress)
            return Conflict("assessment_not_editable", "Only an in-progress assessment can be evaluated.");
        Guid[] evaluatedIds = await db.ComplianceControlEvaluations.Where(x => x.AssessmentId == assessmentId).Select(x => x.ControlPointId).ToArrayAsync(ct);
        ComplianceControlPoint[] controls = await db.ComplianceControlPoints.AsNoTracking()
            .Where(x => x.StandardVersionId == assessment.StandardVersionId && !evaluatedIds.Contains(x.Id)).OrderBy(x => x.SortOrder).ToArrayAsync(ct);
        DateTimeOffset now = DateTimeOffset.UtcNow;
        var created = new List<ComplianceControlEvaluation>();
        var unsupported = new List<object>();
        foreach (ComplianceControlPoint control in controls)
        {
            try
            {
                AutomatedComplianceResult result = await engine.EvaluateAsync(assessment, control, now, ct);
                var evaluation = new ComplianceControlEvaluation(Guid.NewGuid(), assessment.Id, control.Id, result.Result,
                    result.ScorePercent, control.EvaluatorVersion, $"system:{actor.Subject}", now, result.Notes, result.EvidenceSnapshotJson);
                db.ComplianceControlEvaluations.Add(evaluation);
                db.ComplianceEvidenceLinks.AddRange(result.EvidenceAttachmentIds.Distinct().Select(evidenceId =>
                    new ComplianceEvidenceLink(Guid.NewGuid(), evaluation.Id, evidenceId, actor.Subject, now)));
                created.Add(evaluation);
            }
            catch (InvalidOperationException exception) when (exception.Message.StartsWith("Unsupported compliance evaluator", StringComparison.Ordinal))
            {
                unsupported.Add(new { control.Id, control.Code, control.EvaluatorKey });
            }
        }
        AddRecords(db, tenant, actor, "AutoEvaluated", nameof(ComplianceAssessment), assessment.Id, farmId,
            "ComplianceAssessmentAutoEvaluated", new { assessment.Id, evaluated = created.Count, unsupported = unsupported.Count }, context);
        await db.SaveChangesAsync(ct);
        return UseCaseResults.Ok(new { evaluations = created, unsupported });
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

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

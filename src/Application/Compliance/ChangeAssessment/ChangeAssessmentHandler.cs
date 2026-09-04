using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using EdgIQ.MurimiOS.Domain.ComplianceFinding;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.ChangeAssessment;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceControlEvaluation = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceControlEvaluation;
using ComplianceEvaluationResult = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceEvaluationResult;
using ComplianceFinding = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFinding;
using ComplianceFindingSeverity = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFindingSeverity;
using ComplianceControlApplicability = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlApplicability;
using ComplianceControlPoint = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlPoint;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;

public sealed class ChangeAssessmentHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context) : IRequestHandler<ChangeAssessmentCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeAssessmentCommand message, CancellationToken cancellationToken) => ChangeAssessmentAsync(message.farmId, message.assessmentId, message.command, db, scope, tenant, actor, context, cancellationToken);

    private static async Task<UseCaseResult> ChangeAssessmentAsync(Guid farmId, Guid assessmentId, string command,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        UseCaseContext context, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        ComplianceAssessment? assessment = await db.ComplianceAssessments.SingleOrDefaultAsync(x => x.Id == assessmentId && x.FarmId == farmId, ct);
        if (assessment is null) return UseCaseResults.NotFound();
        try
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            if (command == "start") assessment.Start(now);
            else if (command == "submit")
            {
                int mandatory = await db.ComplianceControlPoints.CountAsync(x => x.StandardVersionId == assessment.StandardVersionId && x.Applicability == ComplianceControlApplicability.Mandatory, ct);
                int evaluated = await (from control in db.ComplianceControlPoints
                    join evaluation in db.ComplianceControlEvaluations on control.Id equals evaluation.ControlPointId
                    where control.StandardVersionId == assessment.StandardVersionId
                        && control.Applicability == ComplianceControlApplicability.Mandatory
                        && evaluation.AssessmentId == assessmentId
                        && evaluation.Result != ComplianceEvaluationResult.NotApplicable
                    select control.Id).Distinct().CountAsync(ct);
                if (evaluated < mandatory) return Bad("mandatory_controls_incomplete", $"{mandatory - evaluated} mandatory control(s) still require evaluation.");
                assessment.Submit(now);
            }
            else if (command == "complete")
            {
                if (!actor.HasPermission(PlatformPermissions.ComplianceVerify)) return UseCaseResults.Forbid();
                ComplianceControlPoint[] controls = await db.ComplianceControlPoints.AsNoTracking().Where(x => x.StandardVersionId == assessment.StandardVersionId).ToArrayAsync(ct);
                ComplianceStandardVersion standard = await db.ComplianceStandardVersions.AsNoTracking().SingleAsync(x => x.Id == assessment.StandardVersionId, ct);
                ComplianceControlEvaluation[] evaluations = await db.ComplianceControlEvaluations.AsNoTracking().Where(x => x.AssessmentId == assessmentId).ToArrayAsync(ct);
                var scored = from control in controls join evaluation in evaluations on control.Id equals evaluation.ControlPointId where evaluation.Result != ComplianceEvaluationResult.NotApplicable select new { control, evaluation };
                decimal totalWeight = scored.Sum(x => x.control.Weight);
                decimal score = totalWeight == 0 ? 0 : decimal.Round(scored.Sum(x => x.control.Weight * x.evaluation.ScorePercent) / totalWeight, 2);
                using JsonDocument policyDocument = JsonDocument.Parse(standard.ConformancePolicyJson);
                decimal minimumScore = policyDocument.RootElement.TryGetProperty("minimumScorePercent", out JsonElement minimumElement)
                    && minimumElement.TryGetDecimal(out decimal configuredMinimum) ? configuredMinimum : 80m;
                bool criticalControlsMustConform = !policyDocument.RootElement.TryGetProperty("criticalControlsMustConform", out JsonElement criticalElement)
                    || criticalElement.ValueKind != JsonValueKind.False;
                bool criticalFailure = criticalControlsMustConform && scored.Any(x => x.control.Critical && x.evaluation.Result == ComplianceEvaluationResult.NonConformant);
                assessment.Complete(score, score >= minimumScore && !criticalFailure, now);
                foreach (var item in scored.Where(x => x.evaluation.Result == ComplianceEvaluationResult.NonConformant))
                {
                    if (await db.ComplianceFindings.AnyAsync(x => x.AssessmentId == assessmentId && x.ControlPointId == item.control.Id, ct)) continue;
                    ComplianceFindingSeverity severity = item.control.Critical ? ComplianceFindingSeverity.Critical : ComplianceFindingSeverity.Major;
                    db.ComplianceFindings.Add(new ComplianceFinding(Guid.NewGuid(), assessmentId, item.control.Id, severity,
                        $"Non-conformance: {item.control.Code}", item.evaluation.Notes ?? item.control.Title,
                        DateOnly.FromDateTime(now.UtcDateTime.AddDays(item.control.Critical ? 7 : 30)), now));
                }
            }
            else assessment.Cancel(now);
            AddRecords(db, tenant, actor, command, nameof(ComplianceAssessment), assessment.Id, farmId, $"ComplianceAssessment{command}", assessment, context);
            await db.SaveChangesAsync(ct); return UseCaseResults.Ok(assessment);
        }
        catch (InvalidOperationException e) { return Conflict("assessment_transition_invalid", e.Message); }
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

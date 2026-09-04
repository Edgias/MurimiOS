using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.EvaluateControl;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceAssessmentStatus = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessmentStatus;
using ComplianceControlEvaluation = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceControlEvaluation;
using ComplianceEvaluationResult = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceEvaluationResult;
using ComplianceEvidenceLink = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceEvidenceLink;
using ComplianceControlPoint = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlPoint;
using ComplianceEvidenceRequirement = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceEvidenceRequirement;

public sealed class EvaluateControlHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor) : IRequestHandler<EvaluateControlCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(EvaluateControlCommand message, CancellationToken cancellationToken) => EvaluateControlAsync(message.farmId, message.assessmentId, message.controlId, message.request, db, scope, actor, cancellationToken);

    private static async Task<UseCaseResult> EvaluateControlAsync(Guid farmId, Guid assessmentId, Guid controlId,
        EvaluateComplianceControlRequest request, IPlatformDbContext db, IOrganisationScopeAccess scope,
        IRequestActorContext actor, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        ComplianceAssessment? assessment = await db.ComplianceAssessments.SingleOrDefaultAsync(x => x.Id == assessmentId && x.FarmId == farmId, ct);
        if (assessment is null) return UseCaseResults.NotFound();
        if (assessment.Status != ComplianceAssessmentStatus.InProgress) return Conflict("assessment_not_editable", "Only an in-progress assessment can be evaluated.");
        ComplianceControlPoint? control = await db.ComplianceControlPoints.AsNoTracking().SingleOrDefaultAsync(x => x.Id == controlId && x.StandardVersionId == assessment.StandardVersionId, ct);
        if (control is null) return UseCaseResults.NotFound();
        if (await db.ComplianceControlEvaluations.AnyAsync(x => x.AssessmentId == assessmentId && x.ControlPointId == controlId, ct))
            return Conflict("evaluation_exists", "Evaluations are immutable. Use a new assessment to correct a completed control evaluation.");
        Guid[] evidenceIds = (request.EvidenceAttachmentIds ?? []).Where(x => x != Guid.Empty).Distinct().ToArray();
        var evidence = evidenceIds.Length == 0 ? [] : await (from attachment in db.EvidenceAttachments
            join zone in db.CropZones on attachment.CropZoneId equals zone.Id
            join season in db.GrowingSeasons on zone.SeasonId equals season.Id
            where evidenceIds.Contains(attachment.Id) && season.FarmId == farmId
            select attachment).ToArrayAsync(ct);
        if (evidence.Length != evidenceIds.Length) return Bad("evidence_scope_invalid", "All evidence must belong to the assessed farm.");
        ComplianceEvidenceRequirement[] requirements = await db.ComplianceEvidenceRequirements.AsNoTracking().Where(x => x.ControlPointId == controlId && x.Required).ToArrayAsync(ct);
        int requiredCount = requirements.Sum(x => x.MinimumCount);
        if (request.Result != ComplianceEvaluationResult.NotApplicable && evidence.Length < requiredCount)
            return Bad("evidence_incomplete", $"This control requires at least {requiredCount} evidence item(s).");
        DateTimeOffset now = DateTimeOffset.UtcNow;
        foreach (ComplianceEvidenceRequirement requirement in requirements.Where(x => x.MaximumAgeDays.HasValue))
            if (evidence.Count(x => x.CapturedAt >= now.AddDays(-requirement.MaximumAgeDays!.Value)) < requirement.MinimumCount)
                return Bad("evidence_stale", $"Requirement {requirement.Code} needs {requirement.MinimumCount} current evidence item(s).");
        try
        {
            var evaluation = new ComplianceControlEvaluation(Guid.NewGuid(), assessmentId, controlId, request.Result,
                request.ScorePercent, control.EvaluatorVersion, actor.Subject, now, request.Notes,
                JsonSerializer.Serialize(evidence.Select(x => new { x.Id, x.Sha256, x.CapturedAt })));
            db.ComplianceControlEvaluations.Add(evaluation);
            db.ComplianceEvidenceLinks.AddRange(evidence.Select(x => new ComplianceEvidenceLink(Guid.NewGuid(), evaluation.Id, x.Id, actor.Subject, now)));
            await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/farms/{farmId}/compliance/assessments/{assessmentId}/controls/{controlId}", evaluation);
        }
        catch (ArgumentException e) { return Bad("evaluation_invalid", e.Message); }
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

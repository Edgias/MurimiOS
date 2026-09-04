using EdgIQ.MurimiOS.Application.Common.Abstractions;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using EdgIQ.MurimiOS.Domain.ComplianceFinding;
using EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;
using EdgIQ.MurimiOS.Domain.TraceEvent;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Application.Common.Compliance;
using ComplianceFinding = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFinding;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;

using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceEvaluationResult = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceEvaluationResult;
using CorrectiveActionStatus = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.CorrectiveActionStatus;
using ComplianceControlPoint = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlPoint;
using ComplianceEvidenceRequirement = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceEvidenceRequirement;
using EudrCaseStatus = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrCaseStatus;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;
using EudrPlotEvidence = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrPlotEvidence;
using EudrRiskConclusion = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrRiskConclusion;
using EudrRiskDecision = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrRiskDecision;
using EudrScreeningResult = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrScreeningResult;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

public sealed class ComplianceEvaluationEngine(IPlatformDbContext db)
{
    public async Task<AutomatedComplianceResult> EvaluateAsync(ComplianceAssessment assessment,
        ComplianceControlPoint control, DateTimeOffset now, CancellationToken cancellationToken)
    {
        return control.EvaluatorKey switch
        {
            "farm.fields.boundaries" => await FieldsAsync(assessment.FarmId, cancellationToken),
            "crop.operations.records" => await OperationsAsync(assessment.FarmId, cancellationToken),
            "inputs.traceability" => await InputsAsync(assessment.FarmId, cancellationToken),
            "harvest.traceability" => await HarvestAsync(assessment.FarmId, cancellationToken),
            "evidence.minimum" => await EvidenceAsync(assessment.FarmId, control.Id, now, cancellationToken),
            "quality.objectives" => await QualityObjectivesAsync(assessment.FarmId, cancellationToken),
            "corrective.actions.controlled" => await CorrectiveActionsAsync(assessment.FarmId, now, cancellationToken),
            "eudr.plot.evidence" => await EudrPlotsAsync(assessment, cancellationToken),
            "eudr.screening.complete" => await EudrScreeningAsync(assessment, cancellationToken),
            "eudr.risk.negligible" => await EudrRiskAsync(assessment, cancellationToken),
            "eudr.statement.assembled" => await EudrStatementAsync(assessment, cancellationToken),
            "eudr.submission.recorded" => await EudrSubmissionAsync(assessment, cancellationToken),
            _ => throw new InvalidOperationException($"Unsupported compliance evaluator '{control.EvaluatorKey}'.")
        };
    }

    private async Task<AutomatedComplianceResult> FieldsAsync(Guid farmId, CancellationToken ct)
    {
        Guid[] fields = await db.FarmFields.Where(x => x.FarmId == farmId).Select(x => x.Id).ToArrayAsync(ct);
        int bounded = await db.FieldBoundaryVersions.CountAsync(x => fields.Contains(x.FieldId) && x.ValidTo == null, ct);
        return Binary(fields.Length > 0 && bounded == fields.Length, "current field boundaries", new { fields = fields.Length, currentBoundaries = bounded });
    }

    private async Task<AutomatedComplianceResult> OperationsAsync(Guid farmId, CancellationToken ct)
    {
        Guid[] seasons = await db.GrowingSeasons.Where(x => x.FarmId == farmId).Select(x => x.Id).ToArrayAsync(ct);
        Guid[] zones = await db.CropZones.Where(x => seasons.Contains(x.SeasonId)).Select(x => x.Id).ToArrayAsync(ct);
        int operations = await db.FieldOperations.CountAsync(x => x.CropZoneId != null && zones.Contains(x.CropZoneId.Value) && x.SupersededById == null, ct);
        return Binary(zones.Length > 0 && operations >= zones.Length, "crop operation records", new { cropZones = zones.Length, operations });
    }

    private async Task<AutomatedComplianceResult> InputsAsync(Guid farmId, CancellationToken ct)
    {
        Guid[] inputs = await db.AgriculturalInputs.Where(x => x.FarmId == farmId).Select(x => x.Id).ToArrayAsync(ct);
        int lots = await db.InputStockLots.CountAsync(x => inputs.Contains(x.InputId), ct);
        int movements = await db.InventoryMovements.CountAsync(x => db.InputStockLots.Where(l => inputs.Contains(l.InputId)).Select(l => l.Id).Contains(x.StockLotId), ct);
        return Binary(inputs.Length > 0 && lots > 0 && movements > 0, "input traceability records", new { inputs = inputs.Length, lots, movements });
    }

    private async Task<AutomatedComplianceResult> HarvestAsync(Guid farmId, CancellationToken ct)
    {
        Guid[] seasons = await db.GrowingSeasons.Where(x => x.FarmId == farmId).Select(x => x.Id).ToArrayAsync(ct);
        Guid[] zones = await db.CropZones.Where(x => seasons.Contains(x.SeasonId)).Select(x => x.Id).ToArrayAsync(ct);
        Guid[] lots = await db.HarvestLots.Where(x => zones.Contains(x.CropZoneId)).Select(x => x.Id).ToArrayAsync(ct);
        int traceObjects = await db.TraceEventObjects.CountAsync(x => x.ObjectType == TraceObjectType.HarvestLot && lots.Contains(x.ObjectId), ct);
        int deliveries = await db.Deliveries.CountAsync(x => x.FarmId == farmId, ct);
        return Binary(lots.Length > 0 && traceObjects >= lots.Length && deliveries > 0, "harvest and dispatch traceability", new { harvestLots = lots.Length, traceObjects, deliveries });
    }

    private async Task<AutomatedComplianceResult> EvidenceAsync(Guid farmId, Guid controlId, DateTimeOffset now, CancellationToken ct)
    {
        ComplianceEvidenceRequirement[] requirements = await db.ComplianceEvidenceRequirements.AsNoTracking()
            .Where(x => x.ControlPointId == controlId && x.Required).ToArrayAsync(ct);
        var evidence = await (from attachment in db.EvidenceAttachments.AsNoTracking()
            join zone in db.CropZones on attachment.CropZoneId equals zone.Id
            join season in db.GrowingSeasons on zone.SeasonId equals season.Id
            where season.FarmId == farmId select attachment).ToArrayAsync(ct);
        bool pass = requirements.All(requirement => evidence.Count(item => requirement.MaximumAgeDays == null
            || item.CapturedAt >= now.AddDays(-requirement.MaximumAgeDays.Value)) >= requirement.MinimumCount);
        return new AutomatedComplianceResult(pass ? ComplianceEvaluationResult.Conformant : ComplianceEvaluationResult.NonConformant,
            pass ? 100 : 0, pass ? "Required evidence counts and freshness are satisfied." : "Required evidence is missing or stale.",
            JsonSerializer.Serialize(new { requirements = requirements.Select(x => new { x.Code, x.MinimumCount, x.MaximumAgeDays }), evidence = evidence.Select(x => new { x.Id, x.Sha256, x.CapturedAt }) }),
            evidence.Select(x => x.Id).ToArray());
    }

    private async Task<AutomatedComplianceResult> QualityObjectivesAsync(Guid farmId, CancellationToken ct)
    {
        int facts = await db.AnalyticsFacts.CountAsync(x => x.FarmId == farmId && x.SourceRecordCount > 0, ct);
        return Binary(facts > 0, "measured quality objectives", new { analyticsFacts = facts });
    }

    private async Task<AutomatedComplianceResult> CorrectiveActionsAsync(Guid farmId, DateTimeOffset now, CancellationToken ct)
    {
        Guid[] assessments = await db.ComplianceAssessments.Where(x => x.FarmId == farmId).Select(x => x.Id).ToArrayAsync(ct);
        Guid[] findings = await db.ComplianceFindings.Where(x => assessments.Contains(x.AssessmentId)).Select(x => x.Id).ToArrayAsync(ct);
        int overdue = await db.ComplianceCorrectiveActions.CountAsync(x => findings.Contains(x.FindingId)
            && x.DueOn < DateOnly.FromDateTime(now.UtcDateTime)
            && x.Status != CorrectiveActionStatus.Verified && x.Status != CorrectiveActionStatus.Cancelled, ct);
        return Binary(overdue == 0, "corrective-action control", new { findings = findings.Length, overdueActions = overdue });
    }

    private async Task<AutomatedComplianceResult> EudrPlotsAsync(ComplianceAssessment assessment, CancellationToken ct)
    {
        EudrDueDiligenceCase? @case = await CaseAsync(assessment, ct);
        int plots = @case is null ? 0 : await db.EudrPlotEvidence.CountAsync(x => x.CaseId == @case.Id, ct);
        return Binary(@case is not null && plots > 0, "EUDR plot evidence", new { caseId = @case?.Id, plots });
    }

    private async Task<AutomatedComplianceResult> EudrScreeningAsync(ComplianceAssessment assessment, CancellationToken ct)
    {
        EudrDueDiligenceCase? @case = await CaseAsync(assessment, ct);
        Guid[] plots = @case is null ? [] : await db.EudrPlotEvidence.Where(x => x.CaseId == @case.Id).Select(x => x.Id).ToArrayAsync(ct);
        int screened = await db.EudrScreenings.Where(x => plots.Contains(x.PlotEvidenceId)).Select(x => x.PlotEvidenceId).Distinct().CountAsync(ct);
        bool clear = plots.Length > 0 && screened == plots.Length && !await db.EudrScreenings.AnyAsync(x => plots.Contains(x.PlotEvidenceId) && x.Result != EudrScreeningResult.Clear, ct);
        return Binary(clear, "EUDR deforestation screening", new { caseId = @case?.Id, plots = plots.Length, screened, allClear = clear });
    }

    private async Task<AutomatedComplianceResult> EudrRiskAsync(ComplianceAssessment assessment, CancellationToken ct)
    {
        EudrDueDiligenceCase? @case = await CaseAsync(assessment, ct);
        EudrRiskDecision? decision = @case is null ? null : await db.EudrRiskDecisions.Where(x => x.CaseId == @case.Id).OrderByDescending(x => x.Revision).FirstOrDefaultAsync(ct);
        bool pass = decision is { LegalProductionConfirmed: true } && decision.Conclusion != EudrRiskConclusion.NonNegligible;
        return Binary(pass, "EUDR risk decision", new { caseId = @case?.Id, decisionId = decision?.Id, decision?.Revision, decision?.Conclusion, decision?.LegalProductionConfirmed });
    }

    private async Task<AutomatedComplianceResult> EudrStatementAsync(ComplianceAssessment assessment, CancellationToken ct)
    {
        EudrDueDiligenceCase? @case = await CaseAsync(assessment, ct);
        int statements = @case is null ? 0 : await db.EudrStatements.CountAsync(x => x.CaseId == @case.Id, ct);
        return Binary(statements > 0, "assembled EUDR statement", new { caseId = @case?.Id, statements });
    }

    private async Task<AutomatedComplianceResult> EudrSubmissionAsync(ComplianceAssessment assessment, CancellationToken ct)
    {
        EudrDueDiligenceCase? @case = await CaseAsync(assessment, ct);
        if (@case is null) return NotApplicable("No EUDR case is in assessment scope.", new { caseId = (Guid?)null });
        return Binary(@case.Status == EudrCaseStatus.Submitted && @case.SubmissionReference != null,
            "recorded EUDR submission", new { caseId = @case.Id, @case.Status, @case.SubmissionReference, @case.SubmittedAt });
    }

    private async Task<EudrDueDiligenceCase?> CaseAsync(ComplianceAssessment assessment, CancellationToken ct)
    {
        Guid? scopedId = null;
        if (!string.IsNullOrWhiteSpace(assessment.ScopeJson))
        {
            using JsonDocument scope = JsonDocument.Parse(assessment.ScopeJson);
            if (scope.RootElement.TryGetProperty("eudrCaseId", out JsonElement id) && Guid.TryParse(id.GetString(), out Guid parsed)) scopedId = parsed;
        }
        return scopedId is { } value
            ? await db.EudrCases.SingleOrDefaultAsync(x => x.Id == value && x.FarmId == assessment.FarmId, ct)
            : await db.EudrCases.Where(x => x.FarmId == assessment.FarmId && x.Status != EudrCaseStatus.Cancelled).OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync(ct);
    }

    private static AutomatedComplianceResult Binary(bool pass, string subject, object snapshot) => new(
        pass ? ComplianceEvaluationResult.Conformant : ComplianceEvaluationResult.NonConformant, pass ? 100 : 0,
        pass ? $"Automated checks found conforming {subject}." : $"Automated checks did not find conforming {subject}.",
        JsonSerializer.Serialize(snapshot), []);
    private static AutomatedComplianceResult NotApplicable(string notes, object snapshot) =>
        new(ComplianceEvaluationResult.NotApplicable, 0, notes, JsonSerializer.Serialize(snapshot), []);
}

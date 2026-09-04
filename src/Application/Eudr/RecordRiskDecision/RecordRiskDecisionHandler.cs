using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Eudr.RecordRiskDecision;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using EudrCaseStatus = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrCaseStatus;
using EudrDeforestationScreening = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDeforestationScreening;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;
using EudrPlotEvidence = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrPlotEvidence;
using EudrRiskConclusion = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrRiskConclusion;
using EudrRiskDecision = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrRiskDecision;
using EudrScreeningResult = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrScreeningResult;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class RecordRiskDecisionHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<RecordRiskDecisionCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RecordRiskDecisionCommand message, CancellationToken cancellationToken) => RecordRiskDecisionAsync(message.caseId, message.request, db, scope, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> RecordRiskDecisionAsync(Guid caseId, RecordEudrRiskDecisionRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        TimeProvider clock, UseCaseContext context, CancellationToken ct)
    {
        EudrDueDiligenceCase? dueDiligenceCase = await db.EudrCases.SingleOrDefaultAsync(x => x.Id == caseId, ct);
        if (dueDiligenceCase is null || !await scope.CanAccessFarmAsync(dueDiligenceCase.FarmId, ct)) return UseCaseResults.NotFound();
        if (dueDiligenceCase.Status != EudrCaseStatus.Screening)
            return Conflict("eudr_case_not_screened", "The case must be in screening before risk can be decided.");
        if (request.LegalityEvidence.ValueKind == JsonValueKind.Undefined)
            return Bad("eudr_legality_evidence_required", "Structured legality evidence is required.");
        if (request.Mitigation is { ValueKind: JsonValueKind.Undefined })
            return Bad("eudr_mitigation_invalid", "Mitigation must be valid JSON when supplied.");
        EudrPlotEvidence[] plots = await db.EudrPlotEvidence.AsNoTracking().Where(x => x.CaseId == caseId).OrderBy(x => x.Id).ToArrayAsync(ct);
        if (plots.Length == 0) return Conflict("eudr_plots_required", "At least one plot is required.");
        Guid[] plotIds = plots.Select(x => x.Id).ToArray();
        EudrDeforestationScreening[] allScreenings = await db.EudrScreenings.AsNoTracking()
            .Where(x => plotIds.Contains(x.PlotEvidenceId)).OrderByDescending(x => x.ScreenedAt).ToArrayAsync(ct);
        EudrDeforestationScreening[] screenings = allScreenings.GroupBy(x => x.PlotEvidenceId).Select(x => x.First()).OrderBy(x => x.PlotEvidenceId).ToArray();
        Guid[] missing = plotIds.Except(screenings.Select(x => x.PlotEvidenceId)).ToArray();
        if (missing.Length > 0) return Conflict("eudr_screenings_incomplete", $"{missing.Length} plot(s) do not have screening evidence.");
        bool allClear = screenings.All(x => x.Result == EudrScreeningResult.Clear);
        if (request.Conclusion == EudrRiskConclusion.Negligible && !allClear)
            return Bad("eudr_risk_conclusion_invalid", "A negligible conclusion requires every latest plot screening to be clear.");
        if (request.Conclusion == EudrRiskConclusion.MitigatedToNegligible && allClear)
            return Bad("eudr_mitigation_not_required", "Use a negligible conclusion when every plot is clear.");

        var evidenceSnapshot = new
        {
            caseId,
            regulationCutoff = EudrDeforestationScreening.RegulationCutoff,
            plots = plots.Select(plot =>
            {
                EudrDeforestationScreening screening = screenings.Single(x => x.PlotEvidenceId == plot.Id);
                return new { plot.Id, plot.FieldId, plot.BoundaryVersionId, plot.BoundarySha256,
                    screeningId = screening.Id, screening.Result, screening.EvidenceSha256, screening.DatasetVersion };
            }).ToArray()
        };
        string snapshotJson = JsonSerializer.Serialize(evidenceSnapshot, JsonOptions);
        DateTimeOffset now = clock.GetUtcNow();
        try
        {
            int revision = await db.EudrRiskDecisions.CountAsync(x => x.CaseId == caseId, ct) + 1;
            var decision = new EudrRiskDecision(Id(request.Id), caseId, revision, request.Conclusion,
                request.LegalProductionConfirmed, request.Rationale, request.Mitigation?.GetRawText(),
                request.LegalityEvidence.GetRawText(), snapshotJson, Hash(snapshotJson), now, actor.Subject);
            dueDiligenceCase.RecordAssessment(now);
            db.EudrRiskDecisions.Add(decision);
            AddRecords(db, tenant, actor, "Created", decision.Id, dueDiligenceCase.FarmId,
                "Eudr.RiskDecided", new { decision.Id, decision.CaseId, decision.Revision, decision.Conclusion,
                    decision.LegalProductionConfirmed, decision.EvidenceSha256 }, now, context);
            await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/eudr/cases/{caseId}/risk-decisions/{decision.Id}", decision);
        }
        catch (ArgumentException e) { return Bad("eudr_risk_decision_invalid", e.Message); }
        catch (InvalidOperationException e) { return Conflict("eudr_risk_transition_invalid", e.Message); }
        catch (DbUpdateException e) { return Conflict("eudr_risk_conflict", DbMessage(e)); }
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static string Hash(string value) => Hash(Encoding.UTF8.GetBytes(value));

    private static string Hash(byte[] value) => Convert.ToHexString(SHA256.HashData(value)).ToLowerInvariant();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

    private static string DbMessage(DbUpdateException e) => e.InnerException?.Message ?? e.Message;


    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string action, Guid entityId, Guid farmId, string eventType, object payload, DateTimeOffset now, UseCaseContext context)
    {
        string json = JsonSerializer.Serialize(payload, JsonOptions);
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, "EUDR", entityId, farmId, actor.ScopeOrganisationNodeId, json,
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now));
    }

}

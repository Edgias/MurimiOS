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
namespace EdgIQ.MurimiOS.Application.Eudr.RecordScreening;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using EudrCaseStatus = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrCaseStatus;
using EudrDeforestationScreening = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDeforestationScreening;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;
using EudrPlotEvidence = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrPlotEvidence;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class RecordScreeningHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<RecordScreeningCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RecordScreeningCommand message, CancellationToken cancellationToken) => RecordScreeningAsync(message.plotId, message.request, db, scope, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> RecordScreeningAsync(Guid plotId, RecordEudrScreeningRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        TimeProvider clock, UseCaseContext context, CancellationToken ct)
    {
        EudrPlotEvidence? plot = await db.EudrPlotEvidence.SingleOrDefaultAsync(x => x.Id == plotId, ct);
        if (plot is null) return UseCaseResults.NotFound();
        EudrDueDiligenceCase? dueDiligenceCase = await db.EudrCases.SingleOrDefaultAsync(x => x.Id == plot.CaseId, ct);
        if (dueDiligenceCase is null || !await scope.CanAccessFarmAsync(dueDiligenceCase.FarmId, ct)) return UseCaseResults.NotFound();
        if (dueDiligenceCase.Status is not (EudrCaseStatus.Draft or EudrCaseStatus.Screening))
            return Conflict("eudr_case_locked", "Screening evidence cannot change after risk assessment.");
        if (request.ProviderResult.ValueKind == JsonValueKind.Undefined)
            return Bad("eudr_provider_result_required", "The provider result payload is required.");
        if (request.PostCutoffTreeCoverLossHa > plot.AreaHa || request.DeforestationOverlapHa > plot.AreaHa)
            return Bad("eudr_screening_area_invalid", "Detected loss and overlap cannot exceed the snapshotted plot area.");
        if (request.EvidenceAttachmentId is { } attachmentId)
        {
            bool validAttachment = await (from evidence in db.EvidenceAttachments.AsNoTracking()
                join zone in db.CropZones.AsNoTracking() on evidence.CropZoneId equals zone.Id
                where evidence.Id == attachmentId && zone.FieldId == plot.FieldId
                select evidence.Id).AnyAsync(ct);
            if (!validAttachment)
                return Bad("eudr_screening_attachment_invalid", "The evidence attachment must belong to a crop zone on the screened field.");
        }

        DateTimeOffset now = clock.GetUtcNow();
        var resultSnapshot = new
        {
            cutoffDate = EudrDeforestationScreening.RegulationCutoff,
            plot = new { plot.Id, plot.BoundaryVersionId, plot.BoundarySha256, plot.AreaHa },
            dataset = new { request.ProviderCode, request.DatasetName, request.DatasetVersion, request.SourceReference },
            observation = new { request.ObservationFrom, request.ObservationThrough, request.CoveragePercent,
                treeCoverLossHa = request.PostCutoffTreeCoverLossHa,
                overlapHa = request.DeforestationOverlapHa,
                request.ConfidencePercent },
            providerResult = request.ProviderResult
        };
        string resultJson = JsonSerializer.Serialize(resultSnapshot, JsonOptions);
        try
        {
            var screening = new EudrDeforestationScreening(Id(request.Id), plotId, request.ProviderCode,
                request.DatasetName, request.DatasetVersion, request.ObservationFrom, request.ObservationThrough,
                request.CoveragePercent, request.PostCutoffTreeCoverLossHa, request.DeforestationOverlapHa, request.ConfidencePercent,
                request.SourceReference, request.EvidenceAttachmentId, resultJson, Hash(resultJson), now, actor.Subject);
            dueDiligenceCase.StartScreening();
            db.EudrScreenings.Add(screening);
            AddRecords(db, tenant, actor, "Created", screening.Id, dueDiligenceCase.FarmId,
                "Eudr.PlotScreened", new { screening.Id, screening.PlotEvidenceId, screening.Result, screening.EvidenceSha256 }, now, context);
            await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/eudr/plots/{plotId}/screenings/{screening.Id}", screening);
        }
        catch (ArgumentException e) { return Bad("eudr_screening_invalid", e.Message); }
        catch (InvalidOperationException e) { return Conflict("eudr_screening_transition_invalid", e.Message); }
        catch (DbUpdateException e) { return Conflict("eudr_screening_conflict", DbMessage(e)); }
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

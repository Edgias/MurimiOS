using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.HarvestLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Eudr.AssembleStatement;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using EudrCaseStatus = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrCaseStatus;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;
using EudrDueDiligenceStatement = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceStatement;
using EudrPlotEvidence = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrPlotEvidence;
using EudrRiskConclusion = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrRiskConclusion;
using EudrRiskDecision = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrRiskDecision;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using HarvestLot = global::EdgIQ.MurimiOS.Domain.HarvestLot.HarvestLot;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;
using TraceEventObject = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventObject;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

public sealed class AssembleStatementHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<AssembleStatementCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(AssembleStatementCommand message, CancellationToken cancellationToken) => AssembleStatementAsync(message.caseId, message.request, db, scope, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> AssembleStatementAsync(Guid caseId, AssembleEudrStatementRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        TimeProvider clock, UseCaseContext context, CancellationToken ct)
    {
        EudrDueDiligenceCase? dueDiligenceCase = await db.EudrCases.SingleOrDefaultAsync(x => x.Id == caseId, ct);
        if (dueDiligenceCase is null || !await scope.CanAccessFarmAsync(dueDiligenceCase.FarmId, ct)) return UseCaseResults.NotFound();
        if (dueDiligenceCase.Status != EudrCaseStatus.Assessed)
            return Conflict("eudr_case_not_assessed", "An assessed case is required to assemble a DDS.");
        if (string.IsNullOrWhiteSpace(request.DeclarationText) || request.DeclarationText.Trim().Length > 8_000)
            return Bad("eudr_declaration_invalid", "A declaration of no more than 8,000 characters is required.");
        EudrRiskDecision? decision = await db.EudrRiskDecisions.AsNoTracking().Where(x => x.CaseId == caseId)
            .OrderByDescending(x => x.Revision).FirstOrDefaultAsync(ct);
        if (decision is null || decision.Conclusion == EudrRiskConclusion.NonNegligible || !decision.LegalProductionConfirmed)
            return Conflict("eudr_case_not_eligible", "A DDS requires a negligible or mitigated-to-negligible risk conclusion and confirmed legal production.");

        Farm farm = await db.Farms.AsNoTracking().SingleAsync(x => x.Id == dueDiligenceCase.FarmId, ct);
        EudrPlotEvidence[] plots = await db.EudrPlotEvidence.AsNoTracking().Where(x => x.CaseId == caseId)
            .OrderBy(x => x.PlotReference).ToArrayAsync(ct);
        Guid[] fieldIds = plots.Select(x => x.FieldId).Distinct().ToArray();
        Dictionary<Guid, string> fieldNames = await db.FarmFields.AsNoTracking().Where(x => fieldIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.Name, ct);
        Guid[] lotIds = plots.Where(x => x.HarvestLotId.HasValue).Select(x => x.HarvestLotId!.Value).Distinct().ToArray();
        HarvestLot[] lots = await db.HarvestLots.AsNoTracking().Where(x => lotIds.Contains(x.Id)).OrderBy(x => x.LotCode).ToArrayAsync(ct);
        TraceEventObject[] traceObjects = await db.TraceEventObjects.AsNoTracking()
            .Where(x => x.ObjectType == TraceObjectType.HarvestLot && lotIds.Contains(x.ObjectId)).ToArrayAsync(ct);
        Guid[] eventIds = traceObjects.Select(x => x.EventId).Distinct().ToArray();
        TraceEvent[] events = await db.TraceEvents.AsNoTracking().Where(x => eventIds.Contains(x.Id)).OrderBy(x => x.OccurredAt).ToArrayAsync(ct);

        DateTimeOffset now = clock.GetUtcNow();
        var payload = new
        {
            schemaVersion = DdsSchemaVersion,
            regulation = "Regulation (EU) 2023/1115",
            assembledAt = now,
            internalReference = dueDiligenceCase.CaseNumber,
            operatorData = new
            {
                name = dueDiligenceCase.OperatorName,
                registrationNumber = dueDiligenceCase.OperatorRegistrationNumber,
                address = dueDiligenceCase.OperatorAddress,
                market = dueDiligenceCase.Market
            },
            product = new
            {
                commodity = dueDiligenceCase.Commodity.ToString(),
                dueDiligenceCase.ProductDescription,
                dueDiligenceCase.HsCode,
                dueDiligenceCase.Quantity,
                dueDiligenceCase.Unit,
                dueDiligenceCase.ProductionCountry,
                dueDiligenceCase.ProductionFrom,
                dueDiligenceCase.ProductionTo
            },
            production = new { farm.Id, farm.Name, farm.Code, farm.CountryCode },
            geolocation = plots.Select(plot => new
            {
                plot.Id,
                plot.PlotReference,
                plot.FieldId,
                fieldName = fieldNames[plot.FieldId],
                plot.BoundaryVersionId,
                plot.AreaHa,
                plot.ProductionFrom,
                plot.ProductionTo,
                geometry = ParseElement(plot.BoundaryGeoJson),
                plot.BoundarySha256
            }).ToArray(),
            sourceLots = lots.Select(lot => new { lot.Id, lot.LotCode, lot.Quantity, lot.Unit, lot.HarvestedAt }).ToArray(),
            chainOfCustody = events.Select(trace => new { trace.Id, trace.EventType, trace.BusinessStep, trace.Disposition,
                trace.OccurredAt, trace.TransactionReference }).ToArray(),
            riskAssessment = new
            {
                decision.Id,
                decision.Revision,
                conclusion = decision.Conclusion.ToString(),
                decision.LegalProductionConfirmed,
                decision.Rationale,
                mitigation = decision.MitigationJson is null ? (JsonElement?)null : ParseElement(decision.MitigationJson),
                legalityEvidence = ParseElement(decision.LegalityEvidenceJson),
                evidenceSnapshot = ParseElement(decision.EvidenceSnapshotJson),
                decision.EvidenceSha256,
                decision.DecidedAt,
                decision.DecidedBy
            },
            declaration = new
            {
                deforestationFree = true,
                legallyProduced = true,
                declarationText = request.DeclarationText.Trim(),
                signatory = actor.Subject
            }
        };
        string payloadJson;
        try { payloadJson = JsonSerializer.Serialize(payload, JsonOptions); }
        catch (JsonException e) { return Bad("eudr_statement_payload_invalid", e.Message); }

        try
        {
            int version = await db.EudrStatements.CountAsync(x => x.CaseId == caseId, ct) + 1;
            var statement = new EudrDueDiligenceStatement(Id(request.Id), caseId, version, DdsSchemaVersion,
                payloadJson, Hash(payloadJson), now, actor.Subject);
            dueDiligenceCase.MarkReady(now);
            db.EudrStatements.Add(statement);
            AddRecords(db, tenant, actor, "Created", statement.Id, dueDiligenceCase.FarmId,
                "Eudr.StatementAssembled", new { statement.Id, statement.CaseId, statement.StatementVersion,
                    statement.SchemaVersion, statement.PayloadSha256 }, now, context);
            await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/eudr/statements/{statement.Id}", new
            {
                statement.Id, statement.CaseId, statement.StatementVersion, statement.SchemaVersion,
                statement.PayloadSha256, statement.Status, statement.AssembledAt
            });
        }
        catch (ArgumentException e) { return Bad("eudr_statement_invalid", e.Message); }
        catch (InvalidOperationException e) { return Conflict("eudr_statement_transition_invalid", e.Message); }
        catch (DbUpdateException e) { return Conflict("eudr_statement_conflict", DbMessage(e)); }
    }

    private const string DdsSchemaVersion = "murimios.eudr.dds.v1";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };


    private static string BoundaryGeoJson(MultiPolygon boundary)
    {
        var polygons = new List<double[][][]>();
        for (int i = 0; i < boundary.NumGeometries; i++)
        {
            var polygon = (Polygon)boundary.GetGeometryN(i);
            var rings = new List<double[][]> { Coordinates(polygon.ExteriorRing.Coordinates) };
            for (int ring = 0; ring < polygon.NumInteriorRings; ring++) rings.Add(Coordinates(polygon.GetInteriorRingN(ring).Coordinates));
            polygons.Add(rings.ToArray());
        }
        return JsonSerializer.Serialize(new { type = "MultiPolygon", coordinates = polygons }, JsonOptions);
    }


    private static double[][] Coordinates(Coordinate[] coordinates) =>
        coordinates.Select(point => new[] { point.X, point.Y }).ToArray();


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static string Hash(string value) => Hash(Encoding.UTF8.GetBytes(value));

    private static string Hash(byte[] value) => Convert.ToHexString(SHA256.HashData(value)).ToLowerInvariant();

    private static JsonElement ParseElement(string json)
    {
        using JsonDocument document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }

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

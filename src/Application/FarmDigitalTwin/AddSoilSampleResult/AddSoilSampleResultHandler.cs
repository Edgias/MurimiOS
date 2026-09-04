using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset;
using EdgIQ.MurimiOS.Domain.SoilZone;
using EdgIQ.MurimiOS.Domain.SoilSample;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FarmDigitalTwin.AddSoilSampleResult;
using SoilZone = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZone;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SiteInfrastructureAsset = global::EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset.SiteInfrastructureAsset;
using SoilSample = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSample;
using SoilSampleResult = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSampleResult;
using SoilSampleStatus = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSampleStatus;
using SoilZoneBoundaryVersion = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZoneBoundaryVersion;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class AddSoilSampleResultHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context) : IRequestHandler<AddSoilSampleResultCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(AddSoilSampleResultCommand message, CancellationToken cancellationToken) => AddSoilSampleResultAsync(message.farmId, message.sampleId, message.request, db, scope, tenant, actor, context, cancellationToken);

    private static async Task<UseCaseResult> AddSoilSampleResultAsync(Guid farmId, Guid sampleId,
        CreateSoilSampleResultRequest request, IPlatformDbContext db, IOrganisationScopeAccess scope,
        ITenantContext tenant, IRequestActorContext actor, UseCaseContext context, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        SoilSample? sample = await db.SoilSamples.SingleOrDefaultAsync(x => x.Id == sampleId && x.FarmId == farmId, ct);
        if (sample is null) return UseCaseResults.NotFound();
        if (sample.Status != SoilSampleStatus.SubmittedToLaboratory)
            return Conflict("soil_sample_not_submitted", "Laboratory results require a submitted sample.");
        try
        {
            var result = new SoilSampleResult(Id(request.Id), sampleId, request.AnalyteCode, request.AnalyteName,
                request.Value, request.Unit, request.Method, request.DetectionLimit, request.ResultFlag,
                request.RecordedAt ?? DateTimeOffset.UtcNow, actor.Subject);
            db.SoilSampleResults.Add(result); AddRecords(db, tenant, actor, "ResultRecorded", result, result.Id,
                farmId, "FarmDigitalTwin.SoilSampleResultRecorded", context); await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/farms/{farmId}/digital-twin/soil-samples/{sampleId}/results/{result.Id}", result);
        }
        catch (ArgumentException e) { return Bad("soil_sample_result_invalid", e.Message); }
        catch (DbUpdateException) { return Conflict("soil_sample_result_conflict", "This analyte result already exists or violates laboratory-result constraints."); }
    }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string action, object entity, Guid entityId, Guid farmId, string eventType, UseCaseContext context)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        object payload = entity switch
        {
            SiteInfrastructureAsset asset => Asset(asset),
            SoilZoneBoundaryVersion boundary => Boundary(boundary),
            SoilSample sample => Sample(sample, []),
            _ => entity
        };
        string json = JsonSerializer.Serialize(payload);
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, entity.GetType().Name, entityId, farmId, actor.ScopeOrganisationNodeId, json,
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now));
        db.SyncChanges.Add(new SyncChange(tenant.TenantId, farmId, entity.GetType().Name, entityId, action, json, now));
    }

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });


    private static object Asset(SiteInfrastructureAsset x) => new
    {
        x.Id, x.FarmId, x.FieldId, x.Code, x.Name, x.Type, x.Status,
        location = x.Location is null ? null : new { longitude = x.Location.X, latitude = x.Location.Y },
        x.InstalledOn, x.Capacity, x.CapacityUnit, details = ParseJson(x.DetailsJson),
        x.DecommissionedOn, x.DecommissionedAt, x.DecommissionedBy, x.DecommissionReason
    };

    private static object Boundary(SoilZoneBoundaryVersion boundary) => new
    {
        boundary.Id, boundary.SoilZoneId, boundary.FieldBoundaryVersionId, boundary.AreaHa,
        boundary.ValidFrom, boundary.ValidTo, boundary.Source,
        geometry = new { type = "MultiPolygon", coordinates = Coordinates(boundary.Boundary) }
    };

    private static object Sample(SoilSample sample, IEnumerable<SoilSampleResult> results) => new
    {
        sample.Id, sample.FarmId, sample.FieldId, sample.FieldBoundaryVersionId, sample.SoilZoneBoundaryVersionId,
        sample.SampleCode, location = new { longitude = sample.Location.X, latitude = sample.Location.Y },
        sample.SampledAt, sample.SampledBy, sample.DepthFromCm, sample.DepthToCm, sample.LaboratoryName,
        sample.LaboratoryReference, sample.Notes, sample.Status, sample.SubmittedAt, sample.AnalysedAt,
        sample.RejectedAt, sample.RejectionReason, results
    };

    private static JsonElement ParseJson(string json) { using JsonDocument document = JsonDocument.Parse(json); return document.RootElement.Clone(); }

    private static object Coordinates(MultiPolygon geometry) => geometry.Geometries.Cast<Polygon>().Select(polygon =>
        new[] { polygon.ExteriorRing.Coordinates.Select(x => new[] { x.X, x.Y }).ToArray() }).ToArray();

}

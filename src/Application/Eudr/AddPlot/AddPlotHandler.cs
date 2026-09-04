using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;
using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Eudr.AddPlot;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using EudrCaseStatus = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrCaseStatus;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;
using EudrPlotEvidence = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrPlotEvidence;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class AddPlotHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<AddPlotCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(AddPlotCommand message, CancellationToken cancellationToken) => AddPlotAsync(message.caseId, message.request, db, scope, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> AddPlotAsync(Guid caseId, AddEudrPlotRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        TimeProvider clock, UseCaseContext context, CancellationToken ct)
    {
        EudrDueDiligenceCase? dueDiligenceCase = await db.EudrCases.SingleOrDefaultAsync(x => x.Id == caseId, ct);
        if (dueDiligenceCase is null || !await scope.CanAccessFarmAsync(dueDiligenceCase.FarmId, ct)) return UseCaseResults.NotFound();
        if (dueDiligenceCase.Status != EudrCaseStatus.Draft)
            return Conflict("eudr_case_locked", "Plots can only be added while the case is in draft.");
        if (request.ProductionFrom < dueDiligenceCase.ProductionFrom || request.ProductionTo > dueDiligenceCase.ProductionTo)
            return Bad("eudr_plot_period_invalid", "The plot production period must fall within the case production period.");

        FarmField? field = await db.FarmFields.AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.FieldId && x.FarmId == dueDiligenceCase.FarmId, ct);
        FieldBoundaryVersion? boundary = await db.FieldBoundaryVersions.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.BoundaryVersionId && x.FieldId == request.FieldId, ct);
        if (field is null || boundary is null) return Bad("eudr_plot_scope_invalid", "The field and boundary version must belong to the case farm.");
        if (boundary.ValidFrom > request.ProductionTo || boundary.ValidTo < request.ProductionFrom)
            return Bad("eudr_boundary_period_invalid", "The selected boundary version does not cover the plot production period.");

        if (request.HarvestLotId is { } lotId)
        {
            bool validLot = await (from lot in db.HarvestLots.AsNoTracking()
                join zone in db.CropZones.AsNoTracking() on lot.CropZoneId equals zone.Id
                join season in db.GrowingSeasons.AsNoTracking() on zone.SeasonId equals season.Id
                where lot.Id == lotId && zone.FieldId == request.FieldId && season.FarmId == dueDiligenceCase.FarmId
                    && DateOnly.FromDateTime(lot.HarvestedAt.UtcDateTime) >= request.ProductionFrom
                    && DateOnly.FromDateTime(lot.HarvestedAt.UtcDateTime) <= request.ProductionTo
                select lot.Id).AnyAsync(ct);
            if (!validLot) return Bad("eudr_harvest_lot_invalid", "The harvest lot must originate from this plot during the declared production period.");
        }

        string geoJson = BoundaryGeoJson(boundary.Boundary);
        string digest = Hash(geoJson);
        try
        {
            DateTimeOffset now = clock.GetUtcNow();
            var plot = new EudrPlotEvidence(Id(request.Id), caseId, request.FieldId, request.BoundaryVersionId,
                request.HarvestLotId, request.PlotReference, boundary.AreaHa, request.ProductionFrom,
                request.ProductionTo, geoJson, digest, now, actor.Subject);
            db.EudrPlotEvidence.Add(plot);
            AddRecords(db, tenant, actor, "Created", plot.Id, dueDiligenceCase.FarmId,
                "Eudr.PlotSnapshotted", new { plot.Id, plot.CaseId, plot.FieldId, plot.BoundaryVersionId, plot.BoundarySha256 }, now, context);
            await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/eudr/plots/{plot.Id}", plot);
        }
        catch (ArgumentException e) { return Bad("eudr_plot_invalid", e.Message); }
        catch (DbUpdateException e) { return Conflict("eudr_plot_conflict", DbMessage(e)); }
    }


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

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };

}

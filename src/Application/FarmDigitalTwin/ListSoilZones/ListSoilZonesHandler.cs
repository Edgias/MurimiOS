using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SoilZone;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FarmDigitalTwin.ListSoilZones;
using SoilZone = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZone;
using SoilZoneBoundaryVersion = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZoneBoundaryVersion;

public sealed class ListSoilZonesHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListSoilZonesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListSoilZonesQuery message, CancellationToken cancellationToken) => ListSoilZonesAsync(message.farmId, message.fieldId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListSoilZonesAsync(Guid farmId, Guid? fieldId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        IQueryable<SoilZone> query = db.SoilZones.AsNoTracking().Where(x => x.FarmId == farmId);
        if (fieldId.HasValue) query = query.Where(x => x.FieldId == fieldId);
        SoilZone[] zones = await query.OrderBy(x => x.Code).ToArrayAsync(ct); Guid[] ids = zones.Select(x => x.Id).ToArray();
        SoilZoneBoundaryVersion[] boundaries = await db.SoilZoneBoundaryVersions.AsNoTracking()
            .Where(x => ids.Contains(x.SoilZoneId)).OrderByDescending(x => x.ValidFrom).ToArrayAsync(ct);
        return UseCaseResults.Ok(zones.Select(zone => Zone(zone, boundaries.Where(x => x.SoilZoneId == zone.Id))));
    }

    private static object Zone(SoilZone zone, IEnumerable<SoilZoneBoundaryVersion> boundaries) => new
    {
        zone, boundaries = boundaries.Select(Boundary)
    };

    private static object Boundary(SoilZoneBoundaryVersion boundary) => new
    {
        boundary.Id, boundary.SoilZoneId, boundary.FieldBoundaryVersionId, boundary.AreaHa,
        boundary.ValidFrom, boundary.ValidTo, boundary.Source,
        geometry = new { type = "MultiPolygon", coordinates = Coordinates(boundary.Boundary) }
    };

    private static object Coordinates(MultiPolygon geometry) => geometry.Geometries.Cast<Polygon>().Select(polygon =>
        new[] { polygon.ExteriorRing.Coordinates.Select(x => new[] { x.X, x.Y }).ToArray() }).ToArray();

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

}

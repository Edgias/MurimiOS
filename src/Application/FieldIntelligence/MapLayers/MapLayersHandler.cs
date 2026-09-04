using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.FieldMapLayer;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.MapLayers;
using WeatherLocation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherLocation;

using FieldMapLayer = global::EdgIQ.MurimiOS.Domain.FieldMapLayer.FieldMapLayer;
using MapLayerKind = global::EdgIQ.MurimiOS.Domain.FieldMapLayer.MapLayerKind;
using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;

public sealed class MapLayersHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<MapLayersQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(MapLayersQuery message, CancellationToken cancellationToken) => MapLayersAsync(message.fieldId, message.kind, message.at, db, scope, cancellationToken);

    private static async Task<UseCaseResult> MapLayersAsync(Guid fieldId, MapLayerKind? kind, DateTimeOffset? at,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (!await FieldAllowedAsync(fieldId, db, scope, cancellationToken)) return UseCaseResults.NotFound();
        DateTimeOffset instant = at?.ToUniversalTime() ?? DateTimeOffset.UtcNow;
        IQueryable<FieldMapLayer> query = db.FieldMapLayers.AsNoTracking().Where(x => x.FieldId == fieldId && x.ValidFrom <= instant && (x.ValidTo == null || x.ValidTo > instant));
        if (kind is { } selected) query = query.Where(x => x.Kind == selected);
        FieldMapLayer[] rows = await query.OrderByDescending(x => x.SourceObservedAt).Take(500).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows.Select(ToMapLayer).ToArray());
    }


    private static async Task<bool> FieldAllowedAsync(Guid fieldId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        Guid? farmId = await db.FarmFields.AsNoTracking().Where(x => x.Id == fieldId).Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken);
        return farmId is { } farm && await scope.CanAccessFarmAsync(farm, cancellationToken);
    }

    private static FieldMapLayerResponse ToMapLayer(FieldMapLayer x) => new(x.Id, x.FarmId, x.FieldId,
        x.BoundaryVersionId, x.Kind, x.Name, x.SourceType, x.SourceId, x.SourceObservedAt, x.GeneratedAt,
        x.ValidFrom, x.ValidTo, JsonSerializer.Deserialize<JsonElement>(x.GeoJson),
        JsonSerializer.Deserialize<JsonElement>(x.StyleJson), x.Quality);

}

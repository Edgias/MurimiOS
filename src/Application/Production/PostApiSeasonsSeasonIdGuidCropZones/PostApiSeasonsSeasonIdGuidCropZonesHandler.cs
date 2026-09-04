using EdgIQ.MurimiOS.Domain.GrowingSeason;
using EdgIQ.MurimiOS.Domain.CropZone;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.FarmField;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Production.PostApiSeasonsSeasonIdGuidCropZones;
using CropZone = global::EdgIQ.MurimiOS.Domain.CropZone.CropZone;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using GrowingSeason = global::EdgIQ.MurimiOS.Domain.GrowingSeason.GrowingSeason;

public sealed partial class PostApiSeasonsSeasonIdGuidCropZonesHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<PostApiSeasonsSeasonIdGuidCropZonesCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PostApiSeasonsSeasonIdGuidCropZonesCommand message, CancellationToken cancellationToken) => ExecuteAsync(message.seasonId, message.request, db, scopeAccess, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(
            Guid seasonId,
            CreateCropZoneRequest request,
            IPlatformDbContext db,
            IOrganisationScopeAccess scopeAccess,
            CancellationToken cancellationToken)         {
            GrowingSeason? season = await db.GrowingSeasons.SingleOrDefaultAsync(x => x.Id == seasonId, cancellationToken);
            FarmField? field = await db.FarmFields.SingleOrDefaultAsync(x => x.Id == request.FieldId, cancellationToken);
            FieldBoundaryVersion? fieldBoundary = await db.FieldBoundaryVersions.SingleOrDefaultAsync(
                x => x.Id == request.BoundaryVersionId && x.FieldId == request.FieldId,
                cancellationToken);

            if (season is null || field is null || fieldBoundary is null || season.FarmId != field.FarmId
                || !await scopeAccess.CanAccessFarmAsync(season.FarmId, cancellationToken))
            {
                return UseCaseResults.BadRequest(new
                {
                    code = "production_context_invalid",
                    message = "The season, field, and boundary version must belong to the same tenant and farm."
                });
            }

            MultiPolygon? boundary = request.Exterior is { Count: > 0 }
                ? BuildBoundary(request.Exterior)
                : (MultiPolygon)fieldBoundary.Boundary.Copy();
            if (boundary is null)
            {
                return UseCaseResults.BadRequest(new { code = "invalid_boundary", message = "The crop-zone boundary is invalid." });
            }

            if (!fieldBoundary.Boundary.Covers(boundary))
            {
                return UseCaseResults.BadRequest(new
                {
                    code = "zone_outside_field",
                    message = "A crop zone must remain inside its versioned field boundary."
                });
            }

            Guid id = request.Id is { } requestedId && requestedId != Guid.Empty ? requestedId : Guid.NewGuid();
            var cropZone = new CropZone(
                id,
                seasonId,
                request.FieldId,
                request.BoundaryVersionId,
                request.Name,
                request.Crop,
                request.Variety,
                boundary);
            db.CropZones.Add(cropZone);
            await db.SaveChangesAsync(cancellationToken);

            return UseCaseResults.Created(
                $"/api/v1/seasons/{seasonId}/crop-zones/{id}",
                new CropZoneResponse(
                    id,
                    seasonId,
                    cropZone.FieldId,
                    cropZone.BoundaryVersionId,
                    cropZone.Name,
                    cropZone.Crop,
                    cropZone.Variety,
                    cropZone.AreaHa));
        }

    private static MultiPolygon? BuildBoundary(IReadOnlyList<CoordinateRequest> exterior)
    {
        if (exterior.Count < 3)
        {
            return null;
        }

        if (exterior.Any(x => x.Longitude is < -180 or > 180 || x.Latitude is < -90 or > 90))
        {
            return null;
        }

        var coordinates = exterior
            .Select(x => new Coordinate(x.Longitude, x.Latitude))
            .ToList();
        if (!coordinates[0].Equals2D(coordinates[^1]))
        {
            coordinates.Add(coordinates[0].Copy());
        }

        if (coordinates.Select(x => (x.X, x.Y)).Distinct().Count() < 3)
        {
            return null;
        }

        GeometryFactory factory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var polygon = factory.CreatePolygon(coordinates.ToArray());
        if (!polygon.IsValid || polygon.Area == 0)
        {
            return null;
        }

        return factory.CreateMultiPolygon([polygon]);
    }

}

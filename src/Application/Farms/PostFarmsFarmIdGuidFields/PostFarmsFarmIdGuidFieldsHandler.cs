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
namespace EdgIQ.MurimiOS.Application.Farms.PostFarmsFarmIdGuidFields;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;

public sealed partial class PostFarmsFarmIdGuidFieldsHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<PostFarmsFarmIdGuidFieldsCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PostFarmsFarmIdGuidFieldsCommand message, CancellationToken cancellationToken) => ExecuteAsync(message.farmId, message.request, db, scopeAccess, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(
            Guid farmId,
            CreateFieldRequest request,
            IPlatformDbContext db,
            IOrganisationScopeAccess scopeAccess,
            CancellationToken cancellationToken)         {
            if (!await scopeAccess.CanAccessFarmAsync(farmId, cancellationToken)
                || !await db.Farms.AnyAsync(x => x.Id == farmId, cancellationToken))
            {
                return UseCaseResults.NotFound();
            }

            MultiPolygon? geometry = BuildBoundary(request.Exterior);
            if (geometry is null)
            {
                return UseCaseResults.BadRequest(new
                {
                    code = "invalid_boundary",
                    message = "Provide at least three distinct longitude/latitude points forming a valid polygon."
                });
            }

            Guid fieldId = request.Id is { } requestedId && requestedId != Guid.Empty ? requestedId : Guid.NewGuid();
            Guid boundaryId = request.BoundaryId is { } requestedBoundaryId && requestedBoundaryId != Guid.Empty
                ? requestedBoundaryId
                : Guid.NewGuid();
            var field = new FarmField(fieldId, farmId, request.Name);
            var boundary = new FieldBoundaryVersion(
                boundaryId,
                fieldId,
                geometry,
                request.ValidFrom,
                request.Source ?? "drawn");

            db.FarmFields.Add(field);
            db.FieldBoundaryVersions.Add(boundary);
            await db.SaveChangesAsync(cancellationToken);

            return UseCaseResults.Created(
                $"/api/v1/farms/{farmId}/fields/{fieldId}",
                new FieldResponse(fieldId, farmId, field.Name, boundaryId, boundary.AreaHa, boundary.ValidFrom));
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

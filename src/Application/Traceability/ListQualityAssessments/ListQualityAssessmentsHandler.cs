using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.QualityAssessment;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;
using EdgIQ.MurimiOS.Domain.Delivery;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Traceability.ListQualityAssessments;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;

using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using QualityAssessment = global::EdgIQ.MurimiOS.Domain.QualityAssessment.QualityAssessment;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

public sealed class ListQualityAssessmentsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListQualityAssessmentsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListQualityAssessmentsQuery message, CancellationToken cancellationToken) => ListQualityAssessmentsAsync(message.objectType, message.objectId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListQualityAssessmentsAsync(string objectType, Guid objectId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (!TryObjectType(objectType, out TraceObjectType parsed)) return Bad("unsupported_object_type", "Unknown trace object type.");
        Guid? farmId = await ResolveObjectFarmAsync(parsed, objectId, db, cancellationToken);
        if (farmId is null || !await scope.CanAccessFarmAsync(farmId.Value, cancellationToken)) return UseCaseResults.NotFound();
        QualityAssessment[] rows = await db.QualityAssessments.AsNoTracking()
            .Where(x => x.ObjectType == parsed && x.ObjectId == objectId).OrderByDescending(x => x.AssessedAt)
            .ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows.Select(ToResponse));
    }


    private static QualityAssessmentResponse ToResponse(QualityAssessment x) => new(x.Id, x.ObjectType, x.ObjectId,
        x.Metric, x.NumericResult, x.TextResult, x.Unit, x.Passed, x.AssessedAt, x.Notes);


    private static async Task<Guid?> ResolveObjectFarmAsync(TraceObjectType type, Guid id, IPlatformDbContext db,
        CancellationToken cancellationToken) => type switch
    {
        TraceObjectType.HarvestLot => await (from lot in db.HarvestLots join zone in db.CropZones on lot.CropZoneId equals zone.Id
            join season in db.GrowingSeasons on zone.SeasonId equals season.Id where lot.Id == id select (Guid?)season.FarmId)
            .SingleOrDefaultAsync(cancellationToken),
        TraceObjectType.StorageLot => await db.StorageLots.Where(x => x.Id == id).Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken),
        TraceObjectType.Delivery => await db.Deliveries.Where(x => x.Id == id).Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken),
        TraceObjectType.LivestockFeedBatch => await db.LivestockFeedBatches.Where(x => x.Id == id)
            .Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken),
        TraceObjectType.InputStockLot => await (from lot in db.InputStockLots join input in db.AgriculturalInputs on lot.InputId equals input.Id
            where lot.Id == id select (Guid?)input.FarmId).SingleOrDefaultAsync(cancellationToken),
        _ => null
    };


    private static bool TryObjectType(string value, out TraceObjectType type) =>
        Enum.TryParse(value.Replace("-", string.Empty), true, out type) && Enum.IsDefined(type);

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

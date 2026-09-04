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
namespace EdgIQ.MurimiOS.Application.Traceability.CreateQualityAssessment;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;

using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using QualityAssessment = global::EdgIQ.MurimiOS.Domain.QualityAssessment.QualityAssessment;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

public sealed class CreateQualityAssessmentHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<CreateQualityAssessmentCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateQualityAssessmentCommand message, CancellationToken cancellationToken) => CreateQualityAssessmentAsync(message.request, db, scope, cancellationToken);

    private static async Task<UseCaseResult> CreateQualityAssessmentAsync(CreateQualityAssessmentRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        Guid? farmId = await ResolveObjectFarmAsync(request.ObjectType, request.ObjectId, db, cancellationToken);
        if (farmId is null || !await scope.CanAccessFarmAsync(farmId.Value, cancellationToken)) return UseCaseResults.NotFound();
        Guid id = request.Id is { } requestedId && requestedId != Guid.Empty ? requestedId : Guid.NewGuid();
        var assessment = new QualityAssessment(id, request.ObjectType, request.ObjectId, request.Metric,
            request.NumericResult, request.TextResult, request.Unit, request.Passed, request.AssessedAt, request.Notes);
        db.QualityAssessments.Add(assessment);
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Created($"/api/v1/traceability/quality-assessments/{id}", ToResponse(assessment));
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

}

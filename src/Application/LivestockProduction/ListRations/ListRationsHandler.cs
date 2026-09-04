using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockRationPlan;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockProduction.ListRations;
using LivestockRationLine = global::EdgIQ.MurimiOS.Domain.LivestockRationPlan.LivestockRationLine;
using LivestockRationPlan = global::EdgIQ.MurimiOS.Domain.LivestockRationPlan.LivestockRationPlan;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

public sealed class ListRationsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListRationsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListRationsQuery message, CancellationToken cancellationToken) => ListRationsAsync(message.subjectId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListRationsAsync(Guid subjectId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (await AccessibleSubject(subjectId, db, scope, cancellationToken) is null) return UseCaseResults.NotFound();
        LivestockRationPlan[] plans = await db.LivestockRationPlans.AsNoTracking().Where(x => x.SubjectId == subjectId)
            .OrderByDescending(x => x.EffectiveFrom).ToArrayAsync(cancellationToken);
        Guid[] ids = plans.Select(x => x.Id).ToArray();
        LivestockRationLine[] lines = await db.LivestockRationLines.AsNoTracking().Where(x => ids.Contains(x.PlanId)).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(plans.Select(x => RationResponse(x, lines.Where(line => line.PlanId == x.Id))).ToArray());
    }


    private static async Task<LivestockSubject?> AccessibleSubject(Guid id, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        LivestockSubject? subject = await db.LivestockSubjects.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return subject is not null && await scope.CanAccessFarmAsync(subject.FarmId, cancellationToken) ? subject : null;
    }

    private static object RationResponse(LivestockRationPlan x, IEnumerable<LivestockRationLine> lines) => new
    {
        x.Id, x.FarmId, x.SubjectId, x.Name, x.EffectiveFrom, x.EffectiveTo, x.TargetHeadCount, x.Status,
        x.ActivatedAt, x.RetiredAt, x.Notes,
        TotalKgPerHeadPerDay = lines.Sum(line => line.QuantityKgPerHeadPerDay),
        DryMatterKgPerHeadPerDay = lines.Sum(line => line.QuantityKgPerHeadPerDay * line.DryMatterPercent / 100m),
        Lines = lines.Select(line => new { line.Id, line.FeedBatchId, line.FeedName, line.QuantityKgPerHeadPerDay,
            line.EnteredQuantity, line.EnteredUnit, line.DryMatterPercent }).ToArray()
    };

}

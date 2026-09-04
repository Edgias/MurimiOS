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
namespace EdgIQ.MurimiOS.Application.LivestockProduction.CreateRation;
using LivestockRationLine = global::EdgIQ.MurimiOS.Domain.LivestockRationPlan.LivestockRationLine;
using LivestockRationPlan = global::EdgIQ.MurimiOS.Domain.LivestockRationPlan.LivestockRationPlan;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

public sealed class CreateRationHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor) : IRequestHandler<CreateRationCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateRationCommand message, CancellationToken cancellationToken) => CreateRationAsync(message.subjectId, message.request, db, scope, actor, cancellationToken);

    private static async Task<UseCaseResult> CreateRationAsync(Guid subjectId, CreateRationPlanRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, CancellationToken cancellationToken)
    {
        LivestockSubject? subject = await AccessibleSubject(subjectId, db, scope, cancellationToken);
        if (subject is null) return UseCaseResults.NotFound();
        IReadOnlyList<CreateRationLineRequest> requestedLines = request.Lines ?? [];
        Guid[] batchIds = requestedLines.Where(x => x.FeedBatchId.HasValue).Select(x => x.FeedBatchId!.Value).Distinct().ToArray();
        if (batchIds.Length > 0 && await db.LivestockFeedBatches.CountAsync(x => batchIds.Contains(x.Id) && x.FarmId == subject.FarmId, cancellationToken) != batchIds.Length)
            return Bad("ration_feed_scope_invalid", "Every feed batch must belong to the subject farm.");
        if (requestedLines.Count == 0) return Bad("ration_lines_required", "A ration requires at least one feed line.");
        try
        {
            var plan = new LivestockRationPlan(Guid.NewGuid(), subject.FarmId, subjectId, request.Name,
                request.EffectiveFrom, request.EffectiveTo, request.TargetHeadCount, request.Notes, actor.Subject);
            db.LivestockRationPlans.Add(plan);
            foreach (CreateRationLineRequest line in requestedLines)
                db.LivestockRationLines.Add(new LivestockRationLine(Guid.NewGuid(), plan.Id, line.FeedBatchId,
                    line.FeedName, line.QuantityPerHeadPerDay, line.Unit, line.DryMatterPercent));
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/livestock/subjects/{subjectId}/ration-plans/{plan.Id}",
                RationResponse(plan, db.LivestockRationLines.Local.Where(x => x.PlanId == plan.Id)));
        }
        catch (ArgumentException exception) { return Bad("ration_plan_invalid", exception.Message); }
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

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

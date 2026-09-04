using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockGrazingPlan;
using EdgIQ.MurimiOS.Domain.LivestockHolding;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockProduction.CreateGrazing;
using LivestockHolding = global::EdgIQ.MurimiOS.Domain.LivestockHolding.LivestockHolding;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

using GrazingPlanStatus = global::EdgIQ.MurimiOS.Domain.LivestockGrazingPlan.GrazingPlanStatus;
using LivestockGrazingPlan = global::EdgIQ.MurimiOS.Domain.LivestockGrazingPlan.LivestockGrazingPlan;
using LivestockHoldingType = global::EdgIQ.MurimiOS.Domain.LivestockHolding.LivestockHoldingType;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;

public sealed class CreateGrazingHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor) : IRequestHandler<CreateGrazingCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateGrazingCommand message, CancellationToken cancellationToken) => CreateGrazingAsync(message.farmId, message.request, db, scope, actor, cancellationToken);

    private static async Task<UseCaseResult> CreateGrazingAsync(Guid farmId, CreateGrazingPlanRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessFarmAsync(farmId, cancellationToken)) return UseCaseResults.NotFound();
        bool groupValid = await db.LivestockSubjects.AnyAsync(x => x.Id == request.GroupSubjectId && x.FarmId == farmId
            && x.Kind == LivestockSubjectKind.Group, cancellationToken);
        bool holdingValid = await db.LivestockHoldings.AnyAsync(x => x.Id == request.HoldingId && x.FarmId == farmId
            && x.Type == LivestockHoldingType.Paddock, cancellationToken);
        if (!groupValid || !holdingValid) return Bad("grazing_scope_invalid", "Grazing requires a group and paddock on the same farm.");
        if (await db.LivestockGrazingPlans.AnyAsync(x => x.HoldingId == request.HoldingId
            && x.Status != GrazingPlanStatus.Cancelled && x.Status != GrazingPlanStatus.Completed
            && x.StartsAt < request.EndsAt && request.StartsAt < x.EndsAt, cancellationToken))
            return UseCaseResults.Conflict(new { code = "paddock_schedule_conflict", message = "The paddock already has an overlapping open grazing plan." });
        try
        {
            var plan = new LivestockGrazingPlan(Guid.NewGuid(), farmId, request.GroupSubjectId, request.HoldingId,
                request.StartsAt, request.EndsAt, request.TargetHeadCount, request.RestUntil,
                request.AvailableDryMatterKg, request.Notes, actor.Subject);
            db.LivestockGrazingPlans.Add(plan); await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/farms/{farmId}/livestock/grazing-plans/{plan.Id}", GrazingResponse(plan));
        }
        catch (ArgumentException exception) { return Bad("grazing_plan_invalid", exception.Message); }
    }

    private static object GrazingResponse(LivestockGrazingPlan x) => new { x.Id, x.FarmId, x.GroupSubjectId, x.HoldingId,
        x.StartsAt, x.EndsAt, x.TargetHeadCount, x.RestUntil, x.AvailableDryMatterKg,
        DryMatterKgPerHead = x.AvailableDryMatterKg / x.TargetHeadCount,
        PlannedDays = decimal.Round((decimal)(x.EndsAt - x.StartsAt).TotalDays, 2), x.Status, x.ClosedAt, x.Notes };

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

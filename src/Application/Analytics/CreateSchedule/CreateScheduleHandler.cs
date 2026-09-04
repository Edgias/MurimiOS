using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.CreateSchedule;
using AnalyticsProjectionSchedule = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule.AnalyticsProjectionSchedule;
using AnalyticsScheduleFrequency = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule.AnalyticsScheduleFrequency;

public sealed class CreateScheduleHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor) : IRequestHandler<CreateScheduleCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateScheduleCommand message, CancellationToken cancellationToken) => CreateScheduleAsync(message.farmId, message.request, db, scope, actor, cancellationToken);
    private static async Task<UseCaseResult> CreateScheduleAsync(Guid farmId, CreateAnalyticsScheduleRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, CancellationToken ct)
    { if (!await scope.CanAccessFarmAsync(farmId, ct) || !await db.Farms.AnyAsync(x => x.Id == farmId, ct)) return UseCaseResults.NotFound(); try { var value = new AnalyticsProjectionSchedule(Id(request.Id), farmId, request.Name, request.Frequency, request.LookbackDays, request.Currency, request.ProjectionVersion, request.NextRunAt, actor.Subject); db.AnalyticsProjectionSchedules.Add(value); await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/analytics/schedules/{value.Id}", value); } catch (ArgumentException e) { return Bad("analytics_schedule_invalid", e.Message); } }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

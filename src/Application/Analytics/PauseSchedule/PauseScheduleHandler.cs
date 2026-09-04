using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.PauseSchedule;
using AnalyticsProjectionSchedule = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule.AnalyticsProjectionSchedule;

public sealed class PauseScheduleHandler(IPlatformDbContext db) : IRequestHandler<PauseScheduleCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PauseScheduleCommand message, CancellationToken cancellationToken) => PauseScheduleAsync(message.scheduleId, db, cancellationToken);
    private static async Task<UseCaseResult> PauseScheduleAsync(Guid scheduleId, IPlatformDbContext db, CancellationToken ct)
    { AnalyticsProjectionSchedule? value = await db.AnalyticsProjectionSchedules.SingleOrDefaultAsync(x => x.Id == scheduleId, ct); if (value is null) return UseCaseResults.NotFound(); try { value.Pause(); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(value); } catch (InvalidOperationException e) { return Conflict("analytics_schedule_transition_invalid", e.Message); } }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

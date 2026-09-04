using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.BenchmarkEnrollment;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.RevokeEnrollment;
using BenchmarkEnrollment = global::EdgIQ.MurimiOS.Domain.BenchmarkEnrollment.BenchmarkEnrollment;

public sealed class RevokeEnrollmentHandler(IPlatformDbContext db, IRequestActorContext actor) : IRequestHandler<RevokeEnrollmentCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RevokeEnrollmentCommand message, CancellationToken cancellationToken) => RevokeEnrollmentAsync(message.enrollmentId, db, actor, cancellationToken);
    private static async Task<UseCaseResult> RevokeEnrollmentAsync(Guid enrollmentId, IPlatformDbContext db, IRequestActorContext actor, CancellationToken ct)
    { BenchmarkEnrollment? enrollment = await db.BenchmarkEnrollments.SingleOrDefaultAsync(x => x.Id == enrollmentId, ct); if (enrollment is null) return UseCaseResults.NotFound(); try { enrollment.Revoke(DateTimeOffset.UtcNow, actor.Subject); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(enrollment); } catch (InvalidOperationException e) { return Conflict("benchmark_transition_invalid", e.Message); } }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

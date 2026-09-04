using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition;
using EdgIQ.MurimiOS.Domain.BenchmarkEnrollment;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.EnrollBenchmark;
using AnalyticsGovernanceStatus = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.AnalyticsGovernanceStatus;
using BenchmarkCohortDefinition = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.BenchmarkCohortDefinition;
using BenchmarkEnrollment = global::EdgIQ.MurimiOS.Domain.BenchmarkEnrollment.BenchmarkEnrollment;

public sealed class EnrollBenchmarkHandler(IPlatformDbContext db, IRequestActorContext actor) : IRequestHandler<EnrollBenchmarkCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(EnrollBenchmarkCommand message, CancellationToken cancellationToken) => EnrollBenchmarkAsync(message.request, db, actor, cancellationToken);
    private static async Task<UseCaseResult> EnrollBenchmarkAsync(CreateBenchmarkEnrollmentRequest request, IPlatformDbContext db, IRequestActorContext actor, CancellationToken ct)
    { BenchmarkCohortDefinition? cohort = request.CohortDefinitionId is { } cohortId ? await db.BenchmarkCohortDefinitions.AsNoTracking().SingleOrDefaultAsync(x => x.Id == cohortId && x.Status == AnalyticsGovernanceStatus.Active, ct) : null; if (cohort is null) return Bad("benchmark_cohort_approval_required", "Select an approved benchmark cohort definition."); try { var enrollment = new BenchmarkEnrollment(Id(request.Id), request.MetricCode, cohort.CohortKey, cohort.MinimumCohortSize, DateTimeOffset.UtcNow, actor.Subject, cohort.Id); db.BenchmarkEnrollments.Add(enrollment); await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/analytics/benchmarks/enrollments/{enrollment.Id}", enrollment); } catch (ArgumentException e) { return Bad("benchmark_enrollment_invalid", e.Message); } }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

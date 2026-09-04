using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.AnalyticsMaturity.CreateCohort;
using BenchmarkCohortDefinition = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.BenchmarkCohortDefinition;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class CreateCohortHandler(IPlatformDbContext db, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock) : IRequestHandler<CreateCohortCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateCohortCommand message, CancellationToken cancellationToken) => CreateCohortAsync(message.request, db, actor, tenant, clock, cancellationToken);

    private static async Task<UseCaseResult> CreateCohortAsync(CreateBenchmarkCohortRequest request,
        IPlatformDbContext db, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, CancellationToken ct)
    {
        try
        {
            var cohort = new BenchmarkCohortDefinition(Id(request.Id), request.CohortKey, request.DefinitionVersion,
                request.Name, request.Description, request.InclusionCriteriaJson, request.MinimumCohortSize,
                request.EffectiveFrom, request.EffectiveTo, request.Owner, actor.Subject);
            if (await db.BenchmarkCohortDefinitions.AnyAsync(x => x.CohortKey == cohort.CohortKey
                && x.DefinitionVersion == cohort.DefinitionVersion, ct))
                return Conflict("benchmark_cohort_version_exists", "This benchmark cohort version already exists.");
            db.BenchmarkCohortDefinitions.Add(cohort); AddEvent(db, tenant, "Analytics.BenchmarkCohortCreated", cohort.Id, clock.GetUtcNow());
            await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/analytics/benchmark-cohorts/{cohort.Id}", cohort);
        }
        catch (Exception ex) when (ex is ArgumentException or JsonException) { return Bad("benchmark_cohort_invalid", ex.Message); }
    }

    private static void AddEvent(IPlatformDbContext db, ITenantContext tenant, string type, Guid id, DateTimeOffset at) =>
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, type, JsonSerializer.Serialize(new { id }), at));

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

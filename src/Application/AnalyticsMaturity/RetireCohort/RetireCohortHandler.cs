using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.AnalyticsMaturity.RetireCohort;
using BenchmarkCohortDefinition = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.BenchmarkCohortDefinition;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class RetireCohortHandler(IPlatformDbContext db, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock) : IRequestHandler<RetireCohortCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RetireCohortCommand message, CancellationToken cancellationToken) => RetireCohortAsync(message.id, db, actor, tenant, clock, cancellationToken);

    private static async Task<UseCaseResult> RetireCohortAsync(Guid id, IPlatformDbContext db, IRequestActorContext actor,
        ITenantContext tenant, TimeProvider clock, CancellationToken ct)
    {
        BenchmarkCohortDefinition? cohort = await db.BenchmarkCohortDefinitions.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (cohort is null) return UseCaseResults.NotFound();
        try { DateTimeOffset now = clock.GetUtcNow(); cohort.Retire(now, actor.Subject);
            AddEvent(db, tenant, "Analytics.BenchmarkCohortRetired", cohort.Id, now); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(cohort); }
        catch (InvalidOperationException ex) { return Conflict("benchmark_cohort_transition_invalid", ex.Message); }
    }

    private static void AddEvent(IPlatformDbContext db, ITenantContext tenant, string type, Guid id, DateTimeOffset at) =>
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, type, JsonSerializer.Serialize(new { id }), at));

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

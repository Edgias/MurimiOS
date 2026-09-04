using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition;
using EdgIQ.MurimiOS.Domain.PredictiveModelDefinition;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.AnalyticsMaturity.ApproveModel;
using BenchmarkCohortDefinition = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.BenchmarkCohortDefinition;

using AnalyticsGovernanceStatus = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.AnalyticsGovernanceStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using PredictiveModelDefinition = global::EdgIQ.MurimiOS.Domain.PredictiveModelDefinition.PredictiveModelDefinition;

public sealed class ApproveModelHandler(IPlatformDbContext db, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock) : IRequestHandler<ApproveModelCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ApproveModelCommand message, CancellationToken cancellationToken) => ApproveModelAsync(message.id, message.request, db, actor, tenant, clock, cancellationToken);

    private static async Task<UseCaseResult> ApproveModelAsync(Guid id, AnalyticsApprovalRequest request,
        IPlatformDbContext db, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, CancellationToken ct)
    {
        PredictiveModelDefinition? model = await db.PredictiveModelDefinitions.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (model is null) return UseCaseResults.NotFound();
        if (await db.PredictiveModelDefinitions.AnyAsync(x => x.Id != id && x.TargetMetricCode == model.TargetMetricCode
            && x.Status == AnalyticsGovernanceStatus.Active, ct))
            return Conflict("predictive_model_active_version_exists", "Retire the active target model before approving another.");
        try { DateTimeOffset now = clock.GetUtcNow(); model.Approve(now, actor.Subject, request.ApprovalReference);
            AddEvent(db, tenant, "Analytics.PredictiveModelApproved", model.Id, now); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(model); }
        catch (InvalidOperationException ex) { return Conflict("predictive_model_transition_invalid", ex.Message); }
    }

    private static void AddEvent(IPlatformDbContext db, ITenantContext tenant, string type, Guid id, DateTimeOffset at) =>
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, type, JsonSerializer.Serialize(new { id }), at));

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

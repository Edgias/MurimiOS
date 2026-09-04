using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.PredictiveModelDefinition;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.AnalyticsMaturity.RetireModel;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using PredictiveModelDefinition = global::EdgIQ.MurimiOS.Domain.PredictiveModelDefinition.PredictiveModelDefinition;

public sealed class RetireModelHandler(IPlatformDbContext db, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock) : IRequestHandler<RetireModelCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RetireModelCommand message, CancellationToken cancellationToken) => RetireModelAsync(message.id, db, actor, tenant, clock, cancellationToken);

    private static async Task<UseCaseResult> RetireModelAsync(Guid id, IPlatformDbContext db, IRequestActorContext actor,
        ITenantContext tenant, TimeProvider clock, CancellationToken ct)
    {
        PredictiveModelDefinition? model = await db.PredictiveModelDefinitions.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (model is null) return UseCaseResults.NotFound();
        try { DateTimeOffset now = clock.GetUtcNow(); model.Retire(now, actor.Subject);
            AddEvent(db, tenant, "Analytics.PredictiveModelRetired", model.Id, now); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(model); }
        catch (InvalidOperationException ex) { return Conflict("predictive_model_transition_invalid", ex.Message); }
    }

    private static void AddEvent(IPlatformDbContext db, ITenantContext tenant, string type, Guid id, DateTimeOffset at) =>
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, type, JsonSerializer.Serialize(new { id }), at));

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

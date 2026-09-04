using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.PredictiveModelDefinition;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.AnalyticsMaturity.CreateModel;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using PredictiveModelDefinition = global::EdgIQ.MurimiOS.Domain.PredictiveModelDefinition.PredictiveModelDefinition;

public sealed class CreateModelHandler(IPlatformDbContext db, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock) : IRequestHandler<CreateModelCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateModelCommand message, CancellationToken cancellationToken) => CreateModelAsync(message.request, db, actor, tenant, clock, cancellationToken);

    private static async Task<UseCaseResult> CreateModelAsync(CreatePredictiveModelRequest request, IPlatformDbContext db,
        IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, CancellationToken ct)
    {
        try
        {
            var model = new PredictiveModelDefinition(Id(request.Id), request.ModelKey, request.ModelVersion,
                request.Name, request.TargetMetricCode, request.Algorithm, request.FeatureSchemaJson,
                request.TrainingWindowDays, request.MinimumObservations, request.ValidationMae,
                request.ValidationMapePercent, request.ValidationDatasetReference, actor.Subject);
            if (await db.PredictiveModelDefinitions.AnyAsync(x => x.ModelKey == model.ModelKey
                && x.ModelVersion == model.ModelVersion, ct))
                return Conflict("predictive_model_version_exists", "This predictive model version already exists.");
            db.PredictiveModelDefinitions.Add(model); AddEvent(db, tenant, "Analytics.PredictiveModelCreated", model.Id, clock.GetUtcNow());
            await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/analytics/predictive-models/{model.Id}", model);
        }
        catch (Exception ex) when (ex is ArgumentException or JsonException) { return Bad("predictive_model_invalid", ex.Message); }
    }

    private static void AddEvent(IPlatformDbContext db, ITenantContext tenant, string type, Guid id, DateTimeOffset at) =>
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, type, JsonSerializer.Serialize(new { id }), at));

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

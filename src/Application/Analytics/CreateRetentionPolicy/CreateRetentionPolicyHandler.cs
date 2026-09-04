using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AnalyticsRetentionPolicy;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.CreateRetentionPolicy;
using AnalyticsRetentionPolicy = global::EdgIQ.MurimiOS.Domain.AnalyticsRetentionPolicy.AnalyticsRetentionPolicy;

public sealed class CreateRetentionPolicyHandler(IPlatformDbContext db, IRequestActorContext actor) : IRequestHandler<CreateRetentionPolicyCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateRetentionPolicyCommand message, CancellationToken cancellationToken) => CreateRetentionPolicyAsync(message.request, db, actor, cancellationToken);
    private static async Task<UseCaseResult> CreateRetentionPolicyAsync(CreateAnalyticsRetentionPolicyRequest request, IPlatformDbContext db, IRequestActorContext actor, CancellationToken ct)
    { if (await db.AnalyticsRetentionPolicies.AnyAsync(x => x.IsActive, ct)) return Conflict("active_retention_policy_exists", "Disable the active retention policy before creating another."); try { var value = new AnalyticsRetentionPolicy(Id(request.Id), request.RawSensorDays, request.RawTelemetryDays, request.FailedProjectionDays, request.NextRunAt, actor.Subject, request.DerivedMediaDays, request.ExpiredMapLayerDays); db.AnalyticsRetentionPolicies.Add(value); await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/analytics/retention-policies/{value.Id}", value); } catch (ArgumentException e) { return Bad("analytics_retention_policy_invalid", e.Message); } }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}

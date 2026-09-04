using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.FieldRiskRule;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.RiskAssessments;
using FieldRiskRule = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.FieldRiskRule;
using ScoutingObservation = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingObservation;

using FieldRiskAssessment = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.FieldRiskAssessment;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;

public sealed class RiskAssessmentsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<RiskAssessmentsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RiskAssessmentsQuery message, CancellationToken cancellationToken) => RiskAssessmentsAsync(message.fieldId, message.ruleId, message.from, message.limit, db, scope, cancellationToken);

    private static async Task<UseCaseResult> RiskAssessmentsAsync(Guid fieldId, Guid? ruleId, DateTimeOffset? from,
        int? limit, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (!await FieldAllowedAsync(fieldId, db, scope, cancellationToken)) return UseCaseResults.NotFound();
        IQueryable<FieldRiskAssessment> query = db.FieldRiskAssessments.AsNoTracking().Where(x => x.FieldId == fieldId);
        if (ruleId is { } selected) query = query.Where(x => x.RuleId == selected);
        if (from is { } start) query = query.Where(x => x.AssessedAt >= start.ToUniversalTime());
        FieldRiskAssessment[] rows = await query.OrderByDescending(x => x.AssessedAt).Take(Math.Clamp(limit ?? 250, 1, 2_000)).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows.Select(ToAssessment).ToArray());
    }


    private static async Task<bool> FieldAllowedAsync(Guid fieldId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        Guid? farmId = await db.FarmFields.AsNoTracking().Where(x => x.Id == fieldId).Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken);
        return farmId is { } farm && await scope.CanAccessFarmAsync(farm, cancellationToken);
    }

    private static FieldRiskAssessmentResponse ToAssessment(FieldRiskAssessment x) => new(x.Id, x.FarmId, x.FieldId,
        x.BoundaryVersionId, x.RuleId, x.RuleVersion, x.AssessedAt, x.WindowStart, x.WindowEnd, x.ObservedValue,
        x.SampleCount, x.Triggered, x.Severity, x.EvaluatorVersion, JsonSerializer.Deserialize<JsonElement>(x.InputSnapshotJson));

    private const string EvaluatorVersion = "field-risk-v1";

}

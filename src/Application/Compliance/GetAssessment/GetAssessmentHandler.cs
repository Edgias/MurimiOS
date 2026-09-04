using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using EdgIQ.MurimiOS.Domain.ComplianceFinding;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.GetAssessment;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceControlEvaluation = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceControlEvaluation;
using ComplianceCorrectiveAction = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceCorrectiveAction;
using ComplianceFinding = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFinding;
using ComplianceControlPoint = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlPoint;

public sealed class GetAssessmentHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<GetAssessmentQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetAssessmentQuery message, CancellationToken cancellationToken) => GetAssessmentAsync(message.farmId, message.assessmentId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> GetAssessmentAsync(Guid farmId, Guid assessmentId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        ComplianceAssessment? assessment = await db.ComplianceAssessments.AsNoTracking().SingleOrDefaultAsync(x => x.Id == assessmentId && x.FarmId == farmId, ct);
        if (assessment is null) return UseCaseResults.NotFound();
        ComplianceControlPoint[] controls = await db.ComplianceControlPoints.AsNoTracking().Where(x => x.StandardVersionId == assessment.StandardVersionId).OrderBy(x => x.SortOrder).ToArrayAsync(ct);
        ComplianceControlEvaluation[] evaluations = await db.ComplianceControlEvaluations.AsNoTracking().Where(x => x.AssessmentId == assessmentId).ToArrayAsync(ct);
        ComplianceFinding[] findings = await db.ComplianceFindings.AsNoTracking().Where(x => x.AssessmentId == assessmentId).ToArrayAsync(ct);
        Guid[] findingIds = findings.Select(x => x.Id).ToArray();
        ComplianceCorrectiveAction[] actions = await db.ComplianceCorrectiveActions.AsNoTracking().Where(x => findingIds.Contains(x.FindingId)).ToArrayAsync(ct);
        return UseCaseResults.Ok(new { assessment, controls, evaluations, findings = findings.Select(x => new { finding = x, actions = actions.Where(a => a.FindingId == x.Id) }) });
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

}

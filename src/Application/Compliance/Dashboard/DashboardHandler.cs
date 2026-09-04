using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using EdgIQ.MurimiOS.Domain.ComplianceFinding;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.Dashboard;
using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceAssessmentStatus = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessmentStatus;
using ComplianceCorrectiveAction = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceCorrectiveAction;
using ComplianceFinding = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFinding;
using ComplianceFindingSeverity = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFindingSeverity;
using ComplianceFindingStatus = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFindingStatus;
using CorrectiveActionStatus = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.CorrectiveActionStatus;

public sealed class DashboardHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<DashboardQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(DashboardQuery message, CancellationToken cancellationToken) => DashboardAsync(message.farmId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> DashboardAsync(Guid farmId, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        ComplianceAssessment[] assessments = await db.ComplianceAssessments.AsNoTracking().Where(x => x.FarmId == farmId).ToArrayAsync(ct);
        Guid[] ids = assessments.Select(x => x.Id).ToArray();
        ComplianceFinding[] findings = await db.ComplianceFindings.AsNoTracking().Where(x => ids.Contains(x.AssessmentId)).ToArrayAsync(ct);
        Guid[] findingIds = findings.Select(x => x.Id).ToArray();
        ComplianceCorrectiveAction[] actions = await db.ComplianceCorrectiveActions.AsNoTracking().Where(x => findingIds.Contains(x.FindingId)).ToArrayAsync(ct);
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        return UseCaseResults.Ok(new
        {
            assessments = Enum.GetValues<ComplianceAssessmentStatus>().ToDictionary(x => x.ToString(), x => assessments.Count(a => a.Status == x)),
            latestScores = assessments.Where(x => x.Status == ComplianceAssessmentStatus.Completed).OrderByDescending(x => x.CompletedAt).Take(10).Select(x => new { x.Id, x.StandardVersionId, x.ScorePercent, x.Conforms, x.CompletedAt }),
            openFindings = findings.Count(x => x.Status is not (ComplianceFindingStatus.Closed or ComplianceFindingStatus.Waived)),
            findingsBySeverity = Enum.GetValues<ComplianceFindingSeverity>().ToDictionary(x => x.ToString(), x => findings.Count(f => f.Severity == x && f.Status is not (ComplianceFindingStatus.Closed or ComplianceFindingStatus.Waived))),
            overdueCorrectiveActions = actions.Count(x => x.DueOn < today && x.Status is not (CorrectiveActionStatus.Verified or CorrectiveActionStatus.Cancelled))
        });
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

}

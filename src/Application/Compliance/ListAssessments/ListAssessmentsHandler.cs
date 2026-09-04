using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.ListAssessments;
using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceAssessmentStatus = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessmentStatus;

public sealed class ListAssessmentsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListAssessmentsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListAssessmentsQuery message, CancellationToken cancellationToken) => ListAssessmentsAsync(message.farmId, message.status, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListAssessmentsAsync(Guid farmId, ComplianceAssessmentStatus? status,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        IQueryable<ComplianceAssessment> query = db.ComplianceAssessments.AsNoTracking().Where(x => x.FarmId == farmId);
        if (status.HasValue) query = query.Where(x => x.Status == status);
        return UseCaseResults.Ok(await query.OrderByDescending(x => x.ScheduledOn).ToArrayAsync(ct));
    }


}

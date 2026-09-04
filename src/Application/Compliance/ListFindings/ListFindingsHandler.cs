using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ComplianceFinding;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.ListFindings;
using ComplianceFinding = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFinding;
using ComplianceFindingStatus = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFindingStatus;

public sealed class ListFindingsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListFindingsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListFindingsQuery message, CancellationToken cancellationToken) => ListFindingsAsync(message.farmId, message.status, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListFindingsAsync(Guid farmId, ComplianceFindingStatus? status,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        Guid[] assessmentIds = await db.ComplianceAssessments.Where(x => x.FarmId == farmId).Select(x => x.Id).ToArrayAsync(ct);
        IQueryable<ComplianceFinding> query = db.ComplianceFindings.AsNoTracking().Where(x => assessmentIds.Contains(x.AssessmentId));
        if (status.HasValue) query = query.Where(x => x.Status == status);
        return UseCaseResults.Ok(await query.OrderByDescending(x => x.Severity).ThenBy(x => x.DueOn).ToArrayAsync(ct));
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

}

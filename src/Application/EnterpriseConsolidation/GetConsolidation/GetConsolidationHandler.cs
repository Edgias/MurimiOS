using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.EnterpriseConsolidation.GetConsolidation;
using EnterpriseConsolidationRun = global::EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun.EnterpriseConsolidationRun;
using EnterpriseEliminationEntry = global::EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun.EnterpriseEliminationEntry;

public sealed class GetConsolidationHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<GetConsolidationQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetConsolidationQuery message, CancellationToken cancellationToken) => GetConsolidationAsync(message.id, db, scope, cancellationToken);

    private static async Task<UseCaseResult> GetConsolidationAsync(Guid id, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken ct)
    {
        EnterpriseConsolidationRun? run = await db.EnterpriseConsolidationRuns.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, ct);
        if (run is null || !(await scope.ResolveAsync(ct)).AllowsNode(run.OrganisationNodeId)) return UseCaseResults.NotFound();
        EnterpriseEliminationEntry[] entries = await db.EnterpriseEliminationEntries.AsNoTracking()
            .Where(x => x.ConsolidationRunId == id).OrderBy(x => x.IntercompanyReference).ToArrayAsync(ct);
        return UseCaseResults.Ok(new { run, entries });
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

}

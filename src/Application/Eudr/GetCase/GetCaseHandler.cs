using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Eudr.GetCase;
using EudrDeforestationScreening = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDeforestationScreening;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;
using EudrDueDiligenceStatement = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceStatement;
using EudrPlotEvidence = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrPlotEvidence;
using EudrRiskDecision = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrRiskDecision;

public sealed class GetCaseHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<GetCaseQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetCaseQuery message, CancellationToken cancellationToken) => GetCaseAsync(message.caseId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> GetCaseAsync(Guid caseId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken ct)
    {
        EudrDueDiligenceCase? dueDiligenceCase = await db.EudrCases.AsNoTracking().SingleOrDefaultAsync(x => x.Id == caseId, ct);
        if (dueDiligenceCase is null || !await scope.CanAccessFarmAsync(dueDiligenceCase.FarmId, ct)) return UseCaseResults.NotFound();
        EudrPlotEvidence[] plots = await db.EudrPlotEvidence.AsNoTracking().Where(x => x.CaseId == caseId)
            .OrderBy(x => x.PlotReference).ToArrayAsync(ct);
        Guid[] plotIds = plots.Select(x => x.Id).ToArray();
        EudrDeforestationScreening[] screenings = await db.EudrScreenings.AsNoTracking()
            .Where(x => plotIds.Contains(x.PlotEvidenceId)).OrderBy(x => x.PlotEvidenceId).ThenByDescending(x => x.ScreenedAt).ToArrayAsync(ct);
        EudrRiskDecision[] decisions = await db.EudrRiskDecisions.AsNoTracking().Where(x => x.CaseId == caseId)
            .OrderByDescending(x => x.Revision).ToArrayAsync(ct);
        EudrDueDiligenceStatement[] statements = await db.EudrStatements.AsNoTracking().Where(x => x.CaseId == caseId)
            .OrderByDescending(x => x.StatementVersion).ToArrayAsync(ct);
        return UseCaseResults.Ok(new { dueDiligenceCase, plots, screenings, decisions, statements });
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

}

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
namespace EdgIQ.MurimiOS.Application.Eudr.GetStatement;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;
using EudrDueDiligenceStatement = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceStatement;
using EudrStatementExport = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrStatementExport;

public sealed class GetStatementHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<GetStatementQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetStatementQuery message, CancellationToken cancellationToken) => GetStatementAsync(message.statementId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> GetStatementAsync(Guid statementId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken ct)
    {
        EudrDueDiligenceStatement? statement = await db.EudrStatements.AsNoTracking().SingleOrDefaultAsync(x => x.Id == statementId, ct);
        if (statement is null) return UseCaseResults.NotFound();
        EudrDueDiligenceCase? dueDiligenceCase = await db.EudrCases.AsNoTracking().SingleOrDefaultAsync(x => x.Id == statement.CaseId, ct);
        if (dueDiligenceCase is null || !await scope.CanAccessFarmAsync(dueDiligenceCase.FarmId, ct)) return UseCaseResults.NotFound();
        EudrStatementExport[] exports = await db.EudrStatementExports.AsNoTracking().Where(x => x.StatementId == statementId)
            .OrderByDescending(x => x.ExportedAt).ToArrayAsync(ct);
        return UseCaseResults.Ok(new { statement, payload = ParseElement(statement.PayloadJson), exports });
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static JsonElement ParseElement(string json)
    {
        using JsonDocument document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }

}

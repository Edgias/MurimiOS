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
namespace EdgIQ.MurimiOS.Application.Eudr.ListCases;
using EudrCaseStatus = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrCaseStatus;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;

public sealed class ListCasesHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListCasesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListCasesQuery message, CancellationToken cancellationToken) => ListCasesAsync(message.farmId, message.status, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListCasesAsync(Guid? farmId, EudrCaseStatus? status,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        IQueryable<EudrDueDiligenceCase> query = db.EudrCases.AsNoTracking();
        if (farmId is { } requestedFarm)
        {
            if (!await scope.CanAccessFarmAsync(requestedFarm, ct)) return UseCaseResults.NotFound();
            query = query.Where(x => x.FarmId == requestedFarm);
        }
        else
        {
            ResolvedOrganisationScope resolved = await scope.ResolveAsync(ct);
            if (!resolved.IsUnrestricted) query = query.Where(x => resolved.FarmIds!.Contains(x.FarmId));
        }
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        EudrDueDiligenceCase[] cases = await query.OrderByDescending(x => x.CreatedAt).Take(1_000).ToArrayAsync(ct);
        return UseCaseResults.Ok(cases);
    }


}

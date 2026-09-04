using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.ListIdentifiers;


public sealed class ListIdentifiersHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListIdentifiersQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListIdentifiersQuery message, CancellationToken cancellationToken) => ListIdentifiersAsync(message.subjectId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListIdentifiersAsync(Guid subjectId, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken)
    {
        if (!await CanAccessSubjectAsync(subjectId, db, scope, cancellationToken)) return UseCaseResults.NotFound();
        LivestockIdentifierResponse[] rows = await db.LivestockIdentifiers.Where(x => x.SubjectId == subjectId)
            .OrderByDescending(x => x.IsPrimary).ThenByDescending(x => x.ValidFrom)
            .Select(x => new LivestockIdentifierResponse(x.Id, x.SubjectId, x.IdentifierNamespace, x.Value,
                x.ValidFrom, x.ValidTo, x.IsPrimary)).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows);
    }


    private static async Task<bool> CanAccessSubjectAsync(Guid subjectId, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken)
    {
        Guid? farmId = await SubjectFarmAsync(subjectId, db, cancellationToken);
        return farmId is not null && await scope.CanAccessFarmAsync(farmId.Value, cancellationToken);
    }

    private static Task<Guid?> SubjectFarmAsync(Guid subjectId, IPlatformDbContext db, CancellationToken cancellationToken) =>
        db.LivestockSubjects.Where(x => x.Id == subjectId).Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken);

}

using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.ListMovements;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

using LivestockMovementType = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockMovementType;

public sealed class ListMovementsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListMovementsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListMovementsQuery message, CancellationToken cancellationToken) => ListMovementsAsync(message.subjectId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListMovementsAsync(Guid subjectId, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken)
    {
        if (!await CanAccessSubjectAsync(subjectId, db, scope, cancellationToken)) return UseCaseResults.NotFound();
        LivestockMovementResponse[] rows = await db.LivestockMovements.Where(x => x.SubjectId == subjectId)
            .OrderByDescending(x => x.OccurredAt).Select(x => new LivestockMovementResponse(x.Id, x.SubjectId, x.Type,
                x.FromHoldingId, x.ToHoldingId, x.OccurredAt, x.Reference, x.Notes)).ToArrayAsync(cancellationToken);
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

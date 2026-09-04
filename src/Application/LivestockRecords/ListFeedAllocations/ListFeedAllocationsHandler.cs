using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.ListFeedAllocations;


public sealed class ListFeedAllocationsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListFeedAllocationsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListFeedAllocationsQuery message, CancellationToken cancellationToken) => ListFeedAllocationsAsync(message.subjectId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListFeedAllocationsAsync(Guid subjectId, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken)
    {
        if (!await CanAccessSubjectAsync(subjectId, db, scope, cancellationToken)) return UseCaseResults.NotFound();
        LivestockFeedAllocationResponse[] rows = await db.LivestockFeedAllocations.Where(x => x.SubjectId == subjectId)
            .OrderByDescending(x => x.OccurredAt).Select(x => new LivestockFeedAllocationResponse(x.Id, x.SubjectId,
                x.FeedBatchId, x.Quantity, x.Unit, x.EnteredQuantity, x.EnteredUnit, x.OccurredAt, x.Notes)).ToArrayAsync(cancellationToken);
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

using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.ListMemberships;


public sealed class ListMembershipsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListMembershipsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListMembershipsQuery message, CancellationToken cancellationToken) => ListMembershipsAsync(message.groupId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListMembershipsAsync(Guid groupId, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken)
    {
        if (!await CanAccessSubjectAsync(groupId, db, scope, cancellationToken)) return UseCaseResults.NotFound();
        LivestockGroupMembershipResponse[] rows = await db.LivestockGroupMemberships.Where(x => x.GroupId == groupId)
            .OrderByDescending(x => x.JoinedAt).Select(x => new LivestockGroupMembershipResponse(x.Id, x.GroupId,
                x.MemberSubjectId, x.JoinedAt, x.LeftAt)).ToArrayAsync(cancellationToken);
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

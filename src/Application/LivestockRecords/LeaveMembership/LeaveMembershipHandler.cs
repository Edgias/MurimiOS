using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.LeaveMembership;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

using LivestockGroupMembership = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockGroupMembership;

public sealed class LeaveMembershipHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<LeaveMembershipCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(LeaveMembershipCommand message, CancellationToken cancellationToken) => LeaveMembershipAsync(message.membershipId, message.request, db, scope, cancellationToken);

    private static async Task<UseCaseResult> LeaveMembershipAsync(Guid membershipId, LeaveLivestockGroupMembershipRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        LivestockGroupMembership? membership = await db.LivestockGroupMemberships.SingleOrDefaultAsync(x => x.Id == membershipId, cancellationToken);
        if (membership is null || !await CanAccessSubjectAsync(membership.GroupId, db, scope, cancellationToken)) return UseCaseResults.NotFound();
        try { membership.Leave(request.LeftAt); }
        catch (InvalidOperationException ex) { return UseCaseResults.Conflict(new { code = "membership_already_ended", message = ex.Message }); }
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Ok(new LivestockGroupMembershipResponse(membership.Id, membership.GroupId, membership.MemberSubjectId, membership.JoinedAt, membership.LeftAt));
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

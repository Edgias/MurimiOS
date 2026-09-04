using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.CreateMembership;
using LivestockGroupMembership = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockGroupMembership;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

public sealed class CreateMembershipHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<CreateMembershipCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateMembershipCommand message, CancellationToken cancellationToken) => CreateMembershipAsync(message.groupId, message.request, db, scope, cancellationToken);

    private static async Task<UseCaseResult> CreateMembershipAsync(Guid groupId, CreateLivestockGroupMembershipRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        var subjects = await db.LivestockSubjects.Where(x => x.Id == groupId || x.Id == request.MemberSubjectId).ToArrayAsync(cancellationToken);
        LivestockSubject? group = subjects.SingleOrDefault(x => x.Id == groupId);
        LivestockSubject? member = subjects.SingleOrDefault(x => x.Id == request.MemberSubjectId);
        if (group is null || member is null || group.FarmId != member.FarmId || !await scope.CanAccessFarmAsync(group.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        if (group.Kind != LivestockSubjectKind.Group || member.Kind != LivestockSubjectKind.Individual)
            return Bad("invalid_group_membership", "Membership requires a group and an individual animal from the same farm.");
        if (await db.LivestockGroupMemberships.AnyAsync(x => x.GroupId == groupId && x.MemberSubjectId == member.Id && x.LeftAt == null, cancellationToken))
            return UseCaseResults.Conflict(new { code = "active_membership_exists" });
        Guid id = ValidId(request.Id); var membership = new LivestockGroupMembership(id, groupId, member.Id, request.JoinedAt);
        db.LivestockGroupMemberships.Add(membership); await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Created($"/api/v1/livestock/groups/{groupId}/memberships/{id}", new LivestockGroupMembershipResponse(id, groupId, member.Id, membership.JoinedAt, null));
    }

    private static Guid ValidId(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}

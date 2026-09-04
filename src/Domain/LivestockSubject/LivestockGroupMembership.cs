using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.LivestockSubject;

public sealed class LivestockGroupMembership : TenantEntity
{
    private LivestockGroupMembership() { }
    public LivestockGroupMembership(Guid id, Guid groupId, Guid memberSubjectId, DateTimeOffset joinedAt) : base(id)
    {
        if (groupId == Guid.Empty || memberSubjectId == Guid.Empty || groupId == memberSubjectId)
            throw new ArgumentException("Distinct group and member identifiers are required.");
        GroupId = groupId; MemberSubjectId = memberSubjectId; JoinedAt = joinedAt;
    }
    public Guid GroupId { get; private set; }
    public Guid MemberSubjectId { get; private set; }
    public DateTimeOffset JoinedAt { get; private set; }
    public DateTimeOffset? LeftAt { get; private set; }
    public void Leave(DateTimeOffset at) { if (LeftAt is not null) throw new InvalidOperationException("Membership has already ended."); if (at < JoinedAt) throw new ArgumentOutOfRangeException(nameof(at)); LeftAt = at; }
}

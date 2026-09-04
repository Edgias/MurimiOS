namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public interface IAuditActorContext
{
    bool IsResolved { get; }
    string Subject { get; }
    Guid? MembershipId { get; }
    Guid? ScopeOrganisationNodeId { get; }
}

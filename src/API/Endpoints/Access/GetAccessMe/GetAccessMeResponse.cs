namespace EdgIQ.MurimiOS.API.Endpoints.Access.GetAccessMe;

// Explicit wire metadata for the existing anonymous response. Payload production
// stays in the Application handler; this contract preserves its published schema.
public sealed record GetAccessMeResponse(Guid TenantId, string? Subject, Guid? MembershipId,
    string? Role, Guid? ScopeOrganisationNodeId, IEnumerable<string>? Permissions);

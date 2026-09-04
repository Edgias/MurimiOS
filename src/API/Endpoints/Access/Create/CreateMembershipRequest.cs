using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Access.Create;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;
using MembershipStatus = global::EdgIQ.MurimiOS.Domain.TenantMembership.MembershipStatus;

public sealed record CreateMembershipRequest(
    string ExternalSubject,
    string DisplayName,
    string? Email,
    TenantRole Role,
    MembershipStatus Status,
    Guid? ScopeOrganisationNodeId,
    IReadOnlyList<string>? PermissionGrants);

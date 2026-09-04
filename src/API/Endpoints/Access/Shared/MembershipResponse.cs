using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Access.Shared;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;
using MembershipStatus = global::EdgIQ.MurimiOS.Domain.TenantMembership.MembershipStatus;

public sealed record MembershipResponse(
    Guid Id,
    string ExternalSubject,
    string DisplayName,
    string? Email,
    TenantRole Role,
    MembershipStatus Status,
    Guid? ScopeOrganisationNodeId,
    IReadOnlyList<string> PermissionGrants,
    IReadOnlyList<string> EffectivePermissions);

using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
namespace EdgIQ.MurimiOS.Application.Access.Update;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;

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

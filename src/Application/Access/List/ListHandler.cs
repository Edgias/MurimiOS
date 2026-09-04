using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
namespace EdgIQ.MurimiOS.Application.Access.List;
using MembershipPermissionGrant = global::EdgIQ.MurimiOS.Domain.TenantMembership.MembershipPermissionGrant;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;


public sealed class ListHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<ListQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListQuery message, CancellationToken cancellationToken) => ListAsync(db, scopeAccess, cancellationToken);

    private static async Task<UseCaseResult>ListAsync(
        IPlatformDbContext db,
        IOrganisationScopeAccess scopeAccess,
        CancellationToken cancellationToken)
    {
        ResolvedOrganisationScope scope = await scopeAccess.ResolveAsync(cancellationToken);
        IQueryable<TenantMembership> membershipQuery = db.TenantMemberships.AsNoTracking();
        if (!scope.IsUnrestricted)
        {
            membershipQuery = membershipQuery.Where(membership =>
                membership.ScopeOrganisationNodeId != null
                && scope.OrganisationNodeIds!.Contains(membership.ScopeOrganisationNodeId.Value));
        }
        TenantMembership[] memberships = await membershipQuery
            .AsNoTracking()
            .OrderBy(x => x.DisplayName)
            .ToArrayAsync(cancellationToken);
        Guid[] ids = memberships.Select(x => x.Id).ToArray();
        MembershipPermissionGrant[] grants = await db.MembershipPermissionGrants
            .AsNoTracking()
            .Where(x => ids.Contains(x.MembershipId))
            .ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(memberships.Select(membership => ToResponse(
            membership,
            grants.Where(x => x.MembershipId == membership.Id).Select(x => x.Permission))));
    }


    private static MembershipResponse ToResponse(TenantMembership membership, IEnumerable<string> grants)
    {
        string[] explicitGrants = grants.Order(StringComparer.Ordinal).ToArray();
        string[] effective = PlatformPermissions.ForRole(membership.Role)
            .Concat(explicitGrants)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        return new MembershipResponse(
            membership.Id, membership.ExternalSubject, membership.DisplayName, membership.Email,
            membership.Role, membership.Status, membership.ScopeOrganisationNodeId, explicitGrants, effective);
    }

}

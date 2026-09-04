using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Infrastructure.Identity;
using IOrganisationScopeAccess = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IOrganisationScopeAccess;
using IRequestActorContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IRequestActorContext;
using ResolvedOrganisationScope = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ResolvedOrganisationScope;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;

public sealed class OrganisationScopeAccess(
    PlatformDbContext db,
    IRequestActorContext actor) : EdgIQ.MurimiOS.Application.Common.Abstractions.IOrganisationScopeAccess
{
    private ResolvedOrganisationScope? resolved;

    public async Task<ResolvedOrganisationScope> ResolveAsync(CancellationToken cancellationToken)
    {
        if (resolved is not null)
        {
            return resolved;
        }

        if (actor.ScopeOrganisationNodeId is not { } rootNodeId)
        {
            return resolved = new ResolvedOrganisationScope(null, null, null, null);
        }

        string? rootPath = await db.OrganisationNodes
            .AsNoTracking()
            .Where(node => node.Id == rootNodeId)
            .Select(node => node.Path)
            .SingleOrDefaultAsync(cancellationToken);
        if (rootPath is null)
        {
            // A stale or deleted scope must fail closed.
            return resolved = new ResolvedOrganisationScope(
                rootNodeId,
                null,
                new HashSet<Guid>(),
                new HashSet<Guid>());
        }

        HashSet<Guid> organisationNodeIds;
        if (db.Database.ProviderName?.Contains("Npgsql", StringComparison.Ordinal) == true)
        {
            Guid[] descendantIds = await db.OrganisationNodes
                .FromSqlInterpolated($$"""
                    SELECT node.*
                    FROM platform.org_nodes AS node
                    JOIN platform.org_nodes AS root
                      ON root.tenant_id = node.tenant_id
                     AND root.id = {{rootNodeId}}
                    WHERE node.path <@ root.path
                    """)
                .AsNoTracking()
                .Select(node => node.Id)
                .ToArrayAsync(cancellationToken);
            organisationNodeIds = descendantIds.ToHashSet();
        }
        else
        {
            var nodes = await db.OrganisationNodes
                .AsNoTracking()
                .Select(node => new { node.Id, node.Path })
                .ToArrayAsync(cancellationToken);
            string descendantPrefix = $"{rootPath}.";
            organisationNodeIds = nodes
                .Where(node => node.Path.Equals(rootPath, StringComparison.Ordinal)
                    || node.Path.StartsWith(descendantPrefix, StringComparison.Ordinal))
                .Select(node => node.Id)
                .ToHashSet();
        }
        HashSet<Guid> farmIds = (await db.Farms
            .AsNoTracking()
            .Where(farm => organisationNodeIds.Contains(farm.OrganisationNodeId))
            .Select(farm => farm.Id)
            .ToArrayAsync(cancellationToken))
            .ToHashSet();

        return resolved = new ResolvedOrganisationScope(rootNodeId, rootPath, farmIds, organisationNodeIds);
    }

    public async Task<bool> CanAccessFarmAsync(Guid farmId, CancellationToken cancellationToken) =>
        (await ResolveAsync(cancellationToken)).AllowsFarm(farmId);
}

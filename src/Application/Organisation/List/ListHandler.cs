using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.OrganisationNode;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
namespace EdgIQ.MurimiOS.Application.Organisation.List;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;


public sealed class ListHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<ListQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListQuery message, CancellationToken cancellationToken) => ListAsync(db, scopeAccess, cancellationToken);

    private static async Task<UseCaseResult>ListAsync(
        IPlatformDbContext db,
        IOrganisationScopeAccess scopeAccess,
        CancellationToken cancellationToken)
    {
        ResolvedOrganisationScope scope = await scopeAccess.ResolveAsync(cancellationToken);
        IQueryable<OrganisationNode> query = db.OrganisationNodes.AsNoTracking();
        if (!scope.IsUnrestricted)
        {
            query = query.Where(node => scope.OrganisationNodeIds!.Contains(node.Id));
        }

        OrganisationNodeResponse[] nodes = await query
            .OrderBy(node => node.Path)
            .Select(node => new OrganisationNodeResponse(
                node.Id, node.ParentId, node.Name, node.NodeType, node.Path))
            .ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(nodes);
    }


}

using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.OrganisationNode;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
namespace EdgIQ.MurimiOS.Application.Organisation.Create;
using OrganisationNodeType = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNodeType;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;


public sealed class CreateHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<CreateCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateCommand message, CancellationToken cancellationToken) => CreateAsync(message.request, db, scopeAccess, cancellationToken);

    private static async Task<UseCaseResult>CreateAsync(
        CreateOrganisationNodeRequest request,
        IPlatformDbContext db,
        IOrganisationScopeAccess scopeAccess,
        CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(request.NodeType) || request.NodeType == OrganisationNodeType.Farm)
        {
            return UseCaseResults.BadRequest(new
            {
                code = "invalid_node_type",
                message = "Farm nodes are created through the farm endpoint."
            });
        }
        if (request.ParentId is null && request.NodeType != OrganisationNodeType.Organisation)
        {
            return UseCaseResults.BadRequest(new
            {
                code = "invalid_root_node",
                message = "Only an organisation may be created at the root of the hierarchy."
            });
        }

        ResolvedOrganisationScope scope = await scopeAccess.ResolveAsync(cancellationToken);
        OrganisationNode? parent = null;
        if (request.ParentId is { } parentId)
        {
            if (!scope.AllowsNode(parentId))
            {
                return UseCaseResults.NotFound();
            }

            parent = await db.OrganisationNodes.SingleOrDefaultAsync(node => node.Id == parentId, cancellationToken);
            if (parent is null)
            {
                return UseCaseResults.NotFound();
            }
            if (parent.NodeType is OrganisationNodeType.Warehouse or OrganisationNodeType.CostCentre)
            {
                return UseCaseResults.BadRequest(new
                {
                    code = "invalid_parent_node",
                    message = "Warehouse and cost-centre nodes cannot contain child nodes."
                });
            }
        }
        else if (!scope.IsUnrestricted)
        {
            return UseCaseResults.Json(new
            {
                code = "scope_parent_required",
                message = "A scoped member may only create nodes below their assigned organisation scope."
            }, statusCode: 403);
        }

        Guid id = Guid.NewGuid();
        string label = $"node_{id:N}";
        string path = parent is null ? label : $"{parent.Path}.{label}";
        var node = new OrganisationNode(id, request.Name, request.NodeType, path, parent?.Id);
        db.OrganisationNodes.Add(node);
        await db.SaveChangesAsync(cancellationToken);

        return UseCaseResults.Created(
            $"/api/v1/organisation/nodes/{node.Id}",
            new OrganisationNodeResponse(node.Id, node.ParentId, node.Name, node.NodeType, node.Path));
    }


}

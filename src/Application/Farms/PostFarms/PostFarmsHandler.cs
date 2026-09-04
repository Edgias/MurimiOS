using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.OrganisationNode;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Farms.PostFarms;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using OrganisationNodeType = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNodeType;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;

public sealed partial class PostFarmsHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<PostFarmsCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PostFarmsCommand message, CancellationToken cancellationToken) => ExecuteAsync(message.request, db, scopeAccess, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(
            CreateFarmRequest request,
            IPlatformDbContext db,
            IOrganisationScopeAccess scopeAccess,
            CancellationToken cancellationToken)         {
            ResolvedOrganisationScope scope = await scopeAccess.ResolveAsync(cancellationToken);
            Guid? parentId = request.ParentOrganisationNodeId ?? scope.RootNodeId;
            OrganisationNode? parent = null;
            if (parentId is { } requestedParentId)
            {
                if (!scope.AllowsNode(requestedParentId)) return UseCaseResults.NotFound();
                parent = await db.OrganisationNodes.SingleOrDefaultAsync(x => x.Id == requestedParentId, cancellationToken);
                if (parent is null) return UseCaseResults.NotFound();
                if (parent.NodeType != OrganisationNodeType.Organisation)
                    return UseCaseResults.BadRequest(new { code = "invalid_farm_parent", message = "A farm must belong to an organisation node." });
            }
            Guid farmId = request.Id is { } requestedId && requestedId != Guid.Empty ? requestedId : Guid.NewGuid();
            Guid nodeId = Guid.NewGuid();
            string nodeLabel = $"farm_{farmId:N}";
            string path = parent is null ? nodeLabel : $"{parent.Path}.{nodeLabel}";

            var node = new OrganisationNode(nodeId, request.Name, OrganisationNodeType.Farm, path, parent?.Id);
            var farm = new Farm(farmId, nodeId, request.Name, request.Code, request.CountryCode, request.TimeZone);
            db.OrganisationNodes.Add(node);
            db.Farms.Add(farm);
            await db.SaveChangesAsync(cancellationToken);

            var response = new FarmResponse(farm.Id, farm.OrganisationNodeId, farm.Name, farm.Code, farm.CountryCode, farm.TimeZone);
            return UseCaseResults.Created($"/api/v1/farms/{farm.Id}", response);
        }

}

using EdgIQ.MurimiOS.Domain.OrganisationNode;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
namespace EdgIQ.MurimiOS.Application.Organisation.Create;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;

using OrganisationNodeType = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNodeType;


public sealed record CreateOrganisationNodeRequest(
    Guid? ParentId,
    string Name,
    OrganisationNodeType NodeType);

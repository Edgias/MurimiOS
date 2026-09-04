using EdgIQ.MurimiOS.Domain.OrganisationNode;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Organisation.Create;
using OrganisationNodeType = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNodeType;

public sealed record CreateOrganisationNodeRequest(
    Guid? ParentId,
    string Name,
    OrganisationNodeType NodeType);

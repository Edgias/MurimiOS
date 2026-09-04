using EdgIQ.MurimiOS.Domain.OrganisationNode;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Organisation.Shared;
using OrganisationNodeType = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNodeType;

public sealed record OrganisationNodeResponse(
    Guid Id,
    Guid? ParentId,
    string Name,
    OrganisationNodeType NodeType,
    string Path);

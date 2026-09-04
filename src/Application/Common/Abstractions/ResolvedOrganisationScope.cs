using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed record ResolvedOrganisationScope(
    Guid? RootNodeId,
    string? RootPath,
    IReadOnlySet<Guid>? FarmIds,
    IReadOnlySet<Guid>? OrganisationNodeIds)
{
    public bool IsUnrestricted => RootNodeId is null;

    public bool AllowsFarm(Guid farmId) => IsUnrestricted || FarmIds!.Contains(farmId);

    public bool AllowsNode(Guid nodeId) => IsUnrestricted || OrganisationNodeIds!.Contains(nodeId);
}

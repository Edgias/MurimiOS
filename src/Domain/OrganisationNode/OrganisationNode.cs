using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.OrganisationNode;

public enum OrganisationNodeType
{
    Organisation = 1,
    Farm = 2,
    Site = 3,
    Warehouse = 4,
    CostCentre = 5
}

public sealed class OrganisationNode : TenantEntity
{
    private OrganisationNode()
    {
    }

    public OrganisationNode(
        Guid id,
        string name,
        OrganisationNodeType nodeType,
        string path,
        Guid? parentId = null) : base(id)
    {
        Name = RequireText(name, nameof(name), 200);
        NodeType = nodeType;
        Path = RequireText(path, nameof(path), 800).ToLowerInvariant();
        ParentId = parentId;
    }

    public string Name { get; private set; } = string.Empty;

    public OrganisationNodeType NodeType { get; private set; }

    public string Path { get; private set; } = string.Empty;

    public Guid? ParentId { get; private set; }

    private static string RequireText(string value, string parameter, int maximumLength)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximumLength)
        {
            throw new ArgumentException($"{parameter} must contain between 1 and {maximumLength} characters.", parameter);
        }

        return result;
    }
}

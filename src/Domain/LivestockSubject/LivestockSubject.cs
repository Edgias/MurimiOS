using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.LivestockSubject;

public enum LivestockSubjectKind
{
    Individual = 1,
    Group = 2
}

public sealed class LivestockSubject : TenantEntity
{
    private LivestockSubject()
    {
    }

    public LivestockSubject(
        Guid id,
        Guid farmId,
        LivestockSubjectKind kind,
        string name,
        string species,
        string? officialIdentifier) : base(id)
    {
        if (farmId == Guid.Empty)
        {
            throw new ArgumentException("A farm is required.", nameof(farmId));
        }

        FarmId = farmId;
        Kind = kind;
        Name = RequireText(name, nameof(name), 200);
        Species = RequireText(species, nameof(species), 80);
        OfficialIdentifier = officialIdentifier?.Trim();
    }

    public Guid FarmId { get; private set; }

    public LivestockSubjectKind Kind { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Species { get; private set; } = string.Empty;

    public string? OfficialIdentifier { get; private set; }

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

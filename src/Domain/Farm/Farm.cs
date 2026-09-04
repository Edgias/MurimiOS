using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.Farm;

public sealed class Farm : TenantEntity
{
    private Farm()
    {
    }

    public Farm(Guid id, Guid organisationNodeId, string name, string code, string countryCode, string timeZone) : base(id)
    {
        if (organisationNodeId == Guid.Empty)
        {
            throw new ArgumentException("An organisation node is required.", nameof(organisationNodeId));
        }

        OrganisationNodeId = organisationNodeId;
        Name = RequireText(name, nameof(name), 200);
        Code = RequireText(code, nameof(code), 40).ToUpperInvariant();
        CountryCode = RequireText(countryCode, nameof(countryCode), 2).ToUpperInvariant();
        TimeZone = RequireText(timeZone, nameof(timeZone), 100);
    }

    public Guid OrganisationNodeId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Code { get; private set; } = string.Empty;

    public string CountryCode { get; private set; } = string.Empty;

    public string TimeZone { get; private set; } = string.Empty;

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

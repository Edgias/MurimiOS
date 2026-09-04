using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Domain.GeospatialImportJob;

public sealed class GeospatialImportFeature : TenantEntity
{
    private GeospatialImportFeature() { }

    public GeospatialImportFeature(Guid id, Guid jobId, int ordinal, string sourceName, MultiPolygon boundary,
        string propertiesJson, string issuesJson, bool isEligible) : base(id)
    {
        if (jobId == Guid.Empty) throw new ArgumentException("An import job is required.", nameof(jobId));
        if (ordinal < 1) throw new ArgumentOutOfRangeException(nameof(ordinal));
        if (boundary is null || boundary.IsEmpty) throw new ArgumentException("A boundary is required.", nameof(boundary));
        JobId = jobId;
        Ordinal = ordinal;
        SourceName = string.IsNullOrWhiteSpace(sourceName) ? $"Feature {ordinal}" : sourceName.Trim()[..Math.Min(sourceName.Trim().Length, 200)];
        Boundary = boundary;
        Boundary.SRID = 4326;
        PropertiesJson = RequireJson(propertiesJson, nameof(propertiesJson));
        IssuesJson = RequireJson(issuesJson, nameof(issuesJson));
        IsEligible = isEligible;
    }

    public Guid JobId { get; private set; }
    public int Ordinal { get; private set; }
    public string SourceName { get; private set; } = string.Empty;
    public MultiPolygon Boundary { get; private set; } = default!;
    public decimal AreaHa { get; private set; }
    public string PropertiesJson { get; private set; } = "{}";
    public string IssuesJson { get; private set; } = "[]";
    public bool IsEligible { get; private set; }
    public Guid? AppliedFieldId { get; private set; }
    public Guid? AppliedBoundaryVersionId { get; private set; }
    public DateTimeOffset? AppliedAt { get; private set; }

    public void MarkApplied(Guid fieldId, Guid boundaryVersionId, DateTimeOffset appliedAt)
    {
        if (!IsEligible) throw new InvalidOperationException("An ineligible feature cannot be applied.");
        if (AppliedAt.HasValue)
        {
            if (AppliedFieldId == fieldId && AppliedBoundaryVersionId == boundaryVersionId) return;
            throw new InvalidOperationException("An import feature cannot be applied more than once.");
        }
        if (fieldId == Guid.Empty || boundaryVersionId == Guid.Empty) throw new ArgumentException("Applied field and boundary identifiers are required.");
        AppliedFieldId = fieldId;
        AppliedBoundaryVersionId = boundaryVersionId;
        AppliedAt = appliedAt;
    }

    private static string RequireJson(string? value, string parameter)
    {
        string normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length is 0 or > 100_000) throw new ArgumentException("JSON payload is empty or too large.", parameter);
        return normalized;
    }
}

using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;

public sealed class ComplianceEvidenceRequirement : TenantEntity
{
    private ComplianceEvidenceRequirement() { }
    public ComplianceEvidenceRequirement(Guid id, Guid controlPointId, string code, string name,
        ComplianceEvidenceType evidenceType, bool required, int minimumCount, int? maximumAgeDays, string? instructions) : base(id)
    {
        if (controlPointId == Guid.Empty) throw new ArgumentException("A control point is required.");
        if (!Enum.IsDefined(evidenceType) || minimumCount < 0 || maximumAgeDays <= 0 || (required && minimumCount == 0)) throw new ArgumentOutOfRangeException(nameof(minimumCount));
        ControlPointId = controlPointId; Code = ComplianceStandardVersion.Text(code, 80); Name = ComplianceStandardVersion.Text(name, 200);
        EvidenceType = evidenceType; Required = required; MinimumCount = minimumCount; MaximumAgeDays = maximumAgeDays;
        Instructions = ComplianceStandardVersion.Optional(instructions, 2_000);
    }
    public Guid ControlPointId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public ComplianceEvidenceType EvidenceType { get; private set; }
    public bool Required { get; private set; }
    public int MinimumCount { get; private set; }
    public int? MaximumAgeDays { get; private set; }
    public string? Instructions { get; private set; }
}

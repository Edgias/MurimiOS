using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;

public sealed class ComplianceControlPoint : TenantEntity
{
    private ComplianceControlPoint() { }
    public ComplianceControlPoint(Guid id, Guid standardVersionId, string code, string title, string description,
        ComplianceControlApplicability applicability, decimal weight, bool critical, string evaluatorKey,
        string evaluatorVersion, int sortOrder) : base(id)
    {
        if (standardVersionId == Guid.Empty) throw new ArgumentException("A standard version is required.");
        if (!Enum.IsDefined(applicability) || weight <= 0 || weight > 100 || sortOrder < 0) throw new ArgumentOutOfRangeException(nameof(weight));
        StandardVersionId = standardVersionId; Code = ComplianceStandardVersion.Text(code, 80);
        Title = ComplianceStandardVersion.Text(title, 240); Description = ComplianceStandardVersion.Text(description, 4_000);
        Applicability = applicability; Weight = weight; Critical = critical;
        EvaluatorKey = ComplianceStandardVersion.Text(evaluatorKey, 120);
        EvaluatorVersion = ComplianceStandardVersion.Text(evaluatorVersion, 60); SortOrder = sortOrder;
    }
    public Guid StandardVersionId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public ComplianceControlApplicability Applicability { get; private set; }
    public decimal Weight { get; private set; }
    public bool Critical { get; private set; }
    public string EvaluatorKey { get; private set; } = string.Empty;
    public string EvaluatorVersion { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
}

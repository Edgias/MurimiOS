using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;

public sealed class EudrDueDiligenceStatement : TenantEntity
{
    private EudrDueDiligenceStatement() { }

    public EudrDueDiligenceStatement(Guid id, Guid caseId, int statementVersion, string schemaVersion,
        string payloadJson, string payloadSha256, DateTimeOffset assembledAt, string assembledBy) : base(id)
    {
        if (caseId == Guid.Empty || statementVersion <= 0) throw new ArgumentException("Case and statement version are required.");
        CaseId = caseId;
        StatementVersion = statementVersion;
        SchemaVersion = EudrDueDiligenceCase.Text(schemaVersion, 80);
        PayloadJson = EudrDueDiligenceCase.Text(payloadJson, 2_000_000);
        PayloadSha256 = EudrDueDiligenceCase.Sha256(payloadSha256);
        AssembledAt = assembledAt.ToUniversalTime();
        AssembledBy = EudrDueDiligenceCase.Text(assembledBy, 240);
        Status = EudrStatementStatus.Assembled;
    }

    public Guid CaseId { get; private set; }
    public int StatementVersion { get; private set; }
    public string SchemaVersion { get; private set; } = string.Empty;
    public string PayloadJson { get; private set; } = "{}";
    public string PayloadSha256 { get; private set; } = string.Empty;
    public EudrStatementStatus Status { get; private set; }
    public DateTimeOffset AssembledAt { get; private set; }
    public string AssembledBy { get; private set; } = string.Empty;
    public DateTimeOffset? ExportedAt { get; private set; }
    public DateTimeOffset? SubmittedAt { get; private set; }
    public string? SubmissionReference { get; private set; }

    public void MarkExported(DateTimeOffset at)
    {
        if (Status is not (EudrStatementStatus.Assembled or EudrStatementStatus.Exported))
            throw new InvalidOperationException("Only an assembled statement can be exported.");
        Status = EudrStatementStatus.Exported;
        ExportedAt ??= at.ToUniversalTime();
    }

    public void MarkSubmitted(DateTimeOffset at, string reference)
    {
        if (Status != EudrStatementStatus.Exported) throw new InvalidOperationException("Export the statement before submission.");
        Status = EudrStatementStatus.Submitted;
        SubmittedAt = at.ToUniversalTime();
        SubmissionReference = EudrDueDiligenceCase.Text(reference, 200);
    }
}

using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;

public sealed class EudrStatementExport : TenantEntity
{
    private EudrStatementExport() { }

    public EudrStatementExport(Guid id, Guid statementId, string format, string sha256,
        DateTimeOffset exportedAt, string exportedBy) : base(id)
    {
        if (statementId == Guid.Empty) throw new ArgumentException("A statement is required.", nameof(statementId));
        StatementId = statementId;
        Format = EudrDueDiligenceCase.Text(format, 20).ToLowerInvariant();
        Sha256 = EudrDueDiligenceCase.Sha256(sha256);
        ExportedAt = exportedAt.ToUniversalTime();
        ExportedBy = EudrDueDiligenceCase.Text(exportedBy, 240);
    }

    public Guid StatementId { get; private set; }
    public string Format { get; private set; } = string.Empty;
    public string Sha256 { get; private set; } = string.Empty;
    public DateTimeOffset ExportedAt { get; private set; }
    public string ExportedBy { get; private set; } = string.Empty;
}

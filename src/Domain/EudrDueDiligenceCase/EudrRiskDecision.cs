using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;

public sealed class EudrRiskDecision : TenantEntity
{
    private EudrRiskDecision() { }

    public EudrRiskDecision(Guid id, Guid caseId, int revision, EudrRiskConclusion conclusion,
        bool legalProductionConfirmed, string rationale, string? mitigationJson, string legalityEvidenceJson,
        string evidenceSnapshotJson, string evidenceSha256, DateTimeOffset decidedAt, string decidedBy) : base(id)
    {
        if (caseId == Guid.Empty) throw new ArgumentException("A due-diligence case is required.", nameof(caseId));
        if (revision <= 0 || !Enum.IsDefined(conclusion)) throw new ArgumentOutOfRangeException(nameof(revision));
        if (!legalProductionConfirmed && conclusion != EudrRiskConclusion.NonNegligible)
            throw new ArgumentException("A negligible-risk conclusion requires confirmed legal production.");
        if (conclusion == EudrRiskConclusion.MitigatedToNegligible && string.IsNullOrWhiteSpace(mitigationJson))
            throw new ArgumentException("Mitigation evidence is required for a mitigated conclusion.", nameof(mitigationJson));
        CaseId = caseId;
        Revision = revision;
        Conclusion = conclusion;
        LegalProductionConfirmed = legalProductionConfirmed;
        Rationale = EudrDueDiligenceCase.Text(rationale, 8_000);
        MitigationJson = EudrDueDiligenceCase.Optional(mitigationJson, 100_000);
        LegalityEvidenceJson = EudrDueDiligenceCase.Text(legalityEvidenceJson, 100_000);
        EvidenceSnapshotJson = EudrDueDiligenceCase.Text(evidenceSnapshotJson, 1_000_000);
        EvidenceSha256 = EudrDueDiligenceCase.Sha256(evidenceSha256);
        DecidedAt = decidedAt.ToUniversalTime();
        DecidedBy = EudrDueDiligenceCase.Text(decidedBy, 240);
    }

    public Guid CaseId { get; private set; }
    public int Revision { get; private set; }
    public EudrRiskConclusion Conclusion { get; private set; }
    public bool LegalProductionConfirmed { get; private set; }
    public string Rationale { get; private set; } = string.Empty;
    public string? MitigationJson { get; private set; }
    public string LegalityEvidenceJson { get; private set; } = "{}";
    public string EvidenceSnapshotJson { get; private set; } = "{}";
    public string EvidenceSha256 { get; private set; } = string.Empty;
    public DateTimeOffset DecidedAt { get; private set; }
    public string DecidedBy { get; private set; } = string.Empty;
}

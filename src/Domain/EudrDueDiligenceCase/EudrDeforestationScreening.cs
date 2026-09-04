using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;

public sealed class EudrDeforestationScreening : TenantEntity
{
    public static readonly DateOnly RegulationCutoff = new(2020, 12, 31);

    private EudrDeforestationScreening() { }

    public EudrDeforestationScreening(Guid id, Guid plotEvidenceId, string providerCode, string datasetName,
        string datasetVersion, DateOnly observationFrom, DateOnly observationThrough, decimal coveragePercent,
        decimal treeCoverLossHa, decimal overlapHa, decimal confidencePercent, string sourceReference,
        Guid? evidenceAttachmentId, string resultJson, string evidenceSha256, DateTimeOffset screenedAt,
        string screenedBy) : base(id)
    {
        if (plotEvidenceId == Guid.Empty || evidenceAttachmentId == Guid.Empty)
            throw new ArgumentException("Plot evidence and a valid optional attachment are required.");
        if (observationFrom > RegulationCutoff || observationThrough < RegulationCutoff || observationThrough < observationFrom)
            throw new ArgumentException("The screening observation range must include 31 December 2020.");
        if (coveragePercent is < 0 or > 100 || confidencePercent is < 0 or > 100 || treeCoverLossHa < 0 || overlapHa < 0)
            throw new ArgumentOutOfRangeException(nameof(coveragePercent));
        PlotEvidenceId = plotEvidenceId;
        ProviderCode = EudrDueDiligenceCase.Text(providerCode, 80).ToLowerInvariant();
        DatasetName = EudrDueDiligenceCase.Text(datasetName, 200);
        DatasetVersion = EudrDueDiligenceCase.Text(datasetVersion, 80);
        ObservationFrom = observationFrom;
        ObservationThrough = observationThrough;
        CoveragePercent = coveragePercent;
        TreeCoverLossHa = treeCoverLossHa;
        OverlapHa = overlapHa;
        ConfidencePercent = confidencePercent;
        Result = DeriveResult(coveragePercent, treeCoverLossHa, overlapHa, confidencePercent);
        SourceReference = EudrDueDiligenceCase.Text(sourceReference, 1_000);
        EvidenceAttachmentId = evidenceAttachmentId;
        ResultJson = EudrDueDiligenceCase.Text(resultJson, 100_000);
        EvidenceSha256 = EudrDueDiligenceCase.Sha256(evidenceSha256);
        ScreenedAt = screenedAt.ToUniversalTime();
        ScreenedBy = EudrDueDiligenceCase.Text(screenedBy, 240);
    }

    public Guid PlotEvidenceId { get; private set; }
    public string ProviderCode { get; private set; } = string.Empty;
    public string DatasetName { get; private set; } = string.Empty;
    public string DatasetVersion { get; private set; } = string.Empty;
    public DateOnly ObservationFrom { get; private set; }
    public DateOnly ObservationThrough { get; private set; }
    public decimal CoveragePercent { get; private set; }
    public decimal TreeCoverLossHa { get; private set; }
    public decimal OverlapHa { get; private set; }
    public decimal ConfidencePercent { get; private set; }
    public EudrScreeningResult Result { get; private set; }
    public string SourceReference { get; private set; } = string.Empty;
    public Guid? EvidenceAttachmentId { get; private set; }
    public string ResultJson { get; private set; } = "{}";
    public string EvidenceSha256 { get; private set; } = string.Empty;
    public DateTimeOffset ScreenedAt { get; private set; }
    public string ScreenedBy { get; private set; } = string.Empty;

    public static EudrScreeningResult DeriveResult(decimal coveragePercent, decimal lossHa, decimal overlapHa,
        decimal confidencePercent) =>
        lossHa > 0 || overlapHa > 0
            ? EudrScreeningResult.PotentialDeforestation
            : coveragePercent < 95 || confidencePercent < 80
                ? EudrScreeningResult.ReviewRequired
                : EudrScreeningResult.Clear;
}

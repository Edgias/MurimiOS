using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;

public sealed class EudrPlotEvidence : TenantEntity
{
    private EudrPlotEvidence() { }

    public EudrPlotEvidence(Guid id, Guid caseId, Guid fieldId, Guid boundaryVersionId, Guid? harvestLotId,
        string plotReference, decimal areaHa, DateOnly productionFrom, DateOnly productionTo,
        string boundaryGeoJson, string boundarySha256, DateTimeOffset capturedAt, string capturedBy) : base(id)
    {
        if (caseId == Guid.Empty || fieldId == Guid.Empty || boundaryVersionId == Guid.Empty || harvestLotId == Guid.Empty)
            throw new ArgumentException("Case, field, boundary, and valid optional harvest-lot identifiers are required.");
        if (areaHa <= 0 || productionTo < productionFrom) throw new ArgumentOutOfRangeException(nameof(areaHa));
        CaseId = caseId;
        FieldId = fieldId;
        BoundaryVersionId = boundaryVersionId;
        HarvestLotId = harvestLotId;
        PlotReference = EudrDueDiligenceCase.Text(plotReference, 120);
        AreaHa = areaHa;
        ProductionFrom = productionFrom;
        ProductionTo = productionTo;
        BoundaryGeoJson = EudrDueDiligenceCase.Text(boundaryGeoJson, 1_000_000);
        BoundarySha256 = EudrDueDiligenceCase.Sha256(boundarySha256);
        CapturedAt = capturedAt.ToUniversalTime();
        CapturedBy = EudrDueDiligenceCase.Text(capturedBy, 240);
    }

    public Guid CaseId { get; private set; }
    public Guid FieldId { get; private set; }
    public Guid BoundaryVersionId { get; private set; }
    public Guid? HarvestLotId { get; private set; }
    public string PlotReference { get; private set; } = string.Empty;
    public decimal AreaHa { get; private set; }
    public DateOnly ProductionFrom { get; private set; }
    public DateOnly ProductionTo { get; private set; }
    public string BoundaryGeoJson { get; private set; } = "{}";
    public string BoundarySha256 { get; private set; } = string.Empty;
    public DateTimeOffset CapturedAt { get; private set; }
    public string CapturedBy { get; private set; } = string.Empty;
}

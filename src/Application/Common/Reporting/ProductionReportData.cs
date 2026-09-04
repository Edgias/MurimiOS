using System.Globalization;
using System.Text;

namespace EdgIQ.MurimiOS.Application.Common.Reporting;

public sealed record ProductionReportData(
    Guid ReportId,
    DateTimeOffset GeneratedAt,
    string Farm,
    string Field,
    string Season,
    string CropZone,
    string Crop,
    string? Variety,
    decimal AreaHa,
    Guid BoundaryVersionId,
    IReadOnlyList<ProductionOperationRow> Operations,
    IReadOnlyList<InputApplicationRow> InputApplications,
    IReadOnlyList<HarvestLotRow> HarvestLots,
    IReadOnlyList<EvidenceRow> Evidence);

using System.Text;
using EdgIQ.MurimiOS.API.Endpoints.ProductionReport;
using EdgIQ.MurimiOS.Application.Common.Reporting;
using EdgIQ.MurimiOS.Domain.ReportExportRecord;

namespace EdgIQ.MurimiOS.API.FunctionalTests;
using ReportExportRecord = global::EdgIQ.MurimiOS.Domain.ReportExportRecord.ReportExportRecord;
using EvidenceRow = global::EdgIQ.MurimiOS.Application.Common.Reporting.EvidenceRow;
using HarvestLotRow = global::EdgIQ.MurimiOS.Application.Common.Reporting.HarvestLotRow;
using InputApplicationRow = global::EdgIQ.MurimiOS.Application.Common.Reporting.InputApplicationRow;
using ProductionOperationRow = global::EdgIQ.MurimiOS.Application.Common.Reporting.ProductionOperationRow;
using ProductionReportData = global::EdgIQ.MurimiOS.Application.Common.Reporting.ProductionReportData;
using ProductionReportPdfBuilder = global::EdgIQ.MurimiOS.Application.Common.Reporting.ProductionReportPdfBuilder;

public sealed class ProductionReportTests
{
    [Fact]
    public void PdfBuilderCreatesAReadableMultiSectionDocument()
    {
        DateTimeOffset generatedAt = new(2026, 8, 28, 12, 0, 0, TimeSpan.Zero);
        byte[] result = new ProductionReportPdfBuilder().Build(new ProductionReportData(
            Guid.Parse("11111111-2222-3333-4444-555555555555"),
            generatedAt,
            "Mupfumi Farm",
            "North Field",
            "Main 2026",
            "North maize",
            "Maize",
            "SC 719",
            12.75m,
            Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
            [new ProductionOperationRow(generatedAt, "Scouting", "Crop stand healthy")],
            [new InputApplicationRow(generatedAt, "Compound D", "FERT-001", 50m, "kg", "Basal application")],
            [new HarvestLotRow(generatedAt, "HARVEST-001", 850m, "kg", null)],
            [new EvidenceRow(Guid.NewGuid(), generatedAt, "field.jpg", "image/jpeg", 2048, -17.8, 31.0, 4.2, new string('a', 64))]));

        string document = Encoding.ASCII.GetString(result);
        Assert.StartsWith("%PDF-1.4", document);
        Assert.Contains("Production and evidence report", document);
        Assert.Contains("Input applications", document);
        Assert.Contains("Photo and GPS evidence", document);
        Assert.True(result.Length > 2_000);
    }

    [Fact]
    public void ExportAuditRejectsInvalidDigest()
    {
        Assert.Throws<ArgumentException>(() => new ReportExportRecord(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Crop zone production and evidence",
            "pdf",
            "not-a-sha256",
            DateTimeOffset.UtcNow,
            null));
    }
}

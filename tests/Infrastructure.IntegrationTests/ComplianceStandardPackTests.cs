using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using EdgIQ.MurimiOS.Domain.ComplianceFinding;
using EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport;
using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.LandTenureRecord;
using EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset;
using EdgIQ.MurimiOS.Domain.SoilZone;
using EdgIQ.MurimiOS.Domain.SoilSample;
using EdgIQ.MurimiOS.Domain.Common;



using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;


using EdgIQ.MurimiOS.Infrastructure.Storage.Compliance;
using EdgIQ.MurimiOS.Application.Common.Compliance;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceEvaluationResult = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceEvaluationResult;
using ComplianceControlApplicability = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlApplicability;
using ComplianceControlPoint = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlPoint;
using ComplianceStandardStatus = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardStatus;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;
using AutomatedComplianceResult = global::EdgIQ.MurimiOS.Application.Common.Compliance.AutomatedComplianceResult;
using ComplianceEvaluationEngine = global::EdgIQ.MurimiOS.Application.Common.Compliance.ComplianceEvaluationEngine;
using ComplianceStandardPack = global::EdgIQ.MurimiOS.Application.Common.Compliance.ComplianceStandardPack;
using ComplianceStandardPackCatalog = global::EdgIQ.MurimiOS.Application.Common.Compliance.ComplianceStandardPackCatalog;
using ComplianceStandardPackInstaller = global::EdgIQ.MurimiOS.Application.Common.Compliance.ComplianceStandardPackInstaller;

public sealed class ComplianceStandardPackTests
{
    [Fact]
    public void Catalog_contains_provenance_locked_initial_standard_set()
    {
        var catalog = new ComplianceStandardPackCatalog();
        Assert.Equal(4, catalog.Packs.Count);
        Assert.All(catalog.Packs, pack =>
        {
            Assert.Equal(64, pack.ManifestSha256.Length);
            Assert.NotEmpty(pack.Controls);
            Assert.StartsWith("https://", pack.SourceUri);
        });
        Assert.NotNull(catalog.Find("GLOBALGAP-IFA-V6-SMART-FV"));
    }

    [Fact]
    public async Task Installer_is_idempotent_and_publishes_exact_manifest()
    {
        Guid tenantId = Guid.NewGuid();
        await using PlatformDbContext db = Database(tenantId);
        ComplianceStandardPack pack = new ComplianceStandardPackCatalog().Find("codex-haccp-cxc1-2022")!;
        var installer = new ComplianceStandardPackInstaller(db);

        var first = await installer.InstallAsync(pack, "test", DateTimeOffset.UtcNow, default);
        await db.SaveChangesAsync();
        var second = await installer.InstallAsync(pack, "test", DateTimeOffset.UtcNow, default);

        Assert.True(first.Created);
        Assert.False(second.Created);
        Assert.Equal(ComplianceStandardStatus.Published, first.Standard.Status);
        Assert.Equal(pack.ManifestSha256, first.Standard.ManifestSha256);
        Assert.Equal(pack.Controls.Count, await db.ComplianceControlPoints.CountAsync());
    }

    [Fact]
    public async Task Field_boundary_evaluator_uses_current_farm_records()
    {
        Guid tenantId = Guid.NewGuid();
        await using PlatformDbContext db = Database(tenantId);
        Guid farmId = Guid.NewGuid();
        Guid fieldId = Guid.NewGuid();
        db.Farms.Add(new Farm(farmId, Guid.NewGuid(), "Farm", "F1", "ZW", "Africa/Harare"));
        db.FarmFields.Add(new FarmField(fieldId, farmId, "North"));
        var factory = new GeometryFactory(new PrecisionModel(), 4326);
        var ring = factory.CreateLinearRing([new(30, -18), new(31, -18), new(31, -17), new(30, -18)]);
        db.FieldBoundaryVersions.Add(new FieldBoundaryVersion(Guid.NewGuid(), fieldId,
            factory.CreateMultiPolygon([factory.CreatePolygon(ring)]), new DateOnly(2026, 1, 1), "test"));
        await db.SaveChangesAsync();
        var assessment = new ComplianceAssessment(Guid.NewGuid(), farmId, Guid.NewGuid(), "A-1", new DateOnly(2026, 1, 1), null, null);
        var control = new ComplianceControlPoint(Guid.NewGuid(), assessment.StandardVersionId, "FIELD", "Fields", "Fields",
            ComplianceControlApplicability.Mandatory, 1, true, "farm.fields.boundaries", "1.0.0", 1);

        AutomatedComplianceResult result = await new ComplianceEvaluationEngine(db).EvaluateAsync(assessment, control, DateTimeOffset.UtcNow, default);

        Assert.Equal(ComplianceEvaluationResult.Conformant, result.Result);
        Assert.Equal(100, result.ScorePercent);
    }

    private static PlatformDbContext Database(Guid tenantId) => new(
        new DbContextOptionsBuilder<PlatformDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options,
        new TestTenantContext(tenantId));
    private sealed class TestTenantContext(Guid tenantId) : ITenantContext
    {
        public Guid TenantId { get; } = tenantId;
        public bool IsResolved => true;
    }
}

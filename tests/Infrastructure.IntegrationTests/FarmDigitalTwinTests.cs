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

using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using LandTenureRecord = global::EdgIQ.MurimiOS.Domain.LandTenureRecord.LandTenureRecord;
using LandTenureStatus = global::EdgIQ.MurimiOS.Domain.LandTenureRecord.LandTenureStatus;
using LandTenureType = global::EdgIQ.MurimiOS.Domain.LandTenureRecord.LandTenureType;
using InfrastructureAssetStatus = global::EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset.InfrastructureAssetStatus;
using InfrastructureAssetType = global::EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset.InfrastructureAssetType;
using SiteInfrastructureAsset = global::EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset.SiteInfrastructureAsset;
using SoilSample = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSample;
using SoilSampleResult = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSampleResult;
using SoilSampleStatus = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSampleStatus;
using SoilZoneBoundaryVersion = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZoneBoundaryVersion;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;

public sealed class FarmDigitalTwinTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-09-01T18:00:00Z");

    [Fact]
    public void Tenure_history_closes_once_with_an_explicit_outcome_and_actor()
    {
        var tenure = new LandTenureRecord(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            LandTenureType.Leasehold, "Murimi Holdings", "Landowner", "LEASE-2026-01", 42.5m,
            new DateOnly(2026, 1, 1), null, "documents/lease.pdf", null);

        tenure.Close(new DateOnly(2030, 12, 31), LandTenureStatus.Superseded, Now, "oidc|land-manager");

        Assert.Equal(LandTenureStatus.Superseded, tenure.Status);
        Assert.Equal(new DateOnly(2030, 12, 31), tenure.ValidTo);
        Assert.Equal("oidc|land-manager", tenure.ClosedBy);
        Assert.Throws<InvalidOperationException>(() => tenure.Close(
            new DateOnly(2031, 1, 1), LandTenureStatus.Ended, Now, "oidc|second-close"));
    }

    [Fact]
    public void Soil_zone_boundary_and_sample_keep_exact_spatial_versions()
    {
        GeometryFactory factory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        Polygon polygon = factory.CreatePolygon([
            new Coordinate(31.00, -17.80), new Coordinate(31.01, -17.80),
            new Coordinate(31.01, -17.81), new Coordinate(31.00, -17.81),
            new Coordinate(31.00, -17.80)
        ]);
        Guid fieldBoundaryId = Guid.NewGuid();
        var boundary = new SoilZoneBoundaryVersion(Guid.NewGuid(), Guid.NewGuid(), fieldBoundaryId,
            factory.CreateMultiPolygon([polygon]), new DateOnly(2026, 9, 1), "Surveyed");
        var sample = new SoilSample(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), fieldBoundaryId,
            boundary.Id, "SOIL-001", new Point(31.005, -17.805), Now, "agronomist", 0, 20,
            "Soil Lab", "LAB-42", null);

        boundary.Close(new DateOnly(2027, 8, 31));
        sample.Submit(Now.AddHours(1));
        sample.MarkAnalysed(Now.AddDays(2));

        Assert.Equal(fieldBoundaryId, sample.FieldBoundaryVersionId);
        Assert.Equal(boundary.Id, sample.SoilZoneBoundaryVersionId);
        Assert.Equal(SoilSampleStatus.Analysed, sample.Status);
        Assert.Equal(new DateOnly(2027, 8, 31), boundary.ValidTo);
    }

    [Fact]
    public void Infrastructure_requires_paired_capacity_units_and_has_a_terminal_decommission_transition()
    {
        Assert.Throws<ArgumentException>(() => new SiteInfrastructureAsset(Guid.NewGuid(), Guid.NewGuid(), null,
            "DAM-1", "Irrigation dam", InfrastructureAssetType.Reservoir, InfrastructureAssetStatus.Active,
            null, new DateOnly(2025, 1, 1), 1_000, null, "{}"));
        var asset = new SiteInfrastructureAsset(Guid.NewGuid(), Guid.NewGuid(), null, "DAM-1", "Irrigation dam",
            InfrastructureAssetType.Reservoir, InfrastructureAssetStatus.Active, new Point(31, -17.8),
            new DateOnly(2025, 1, 1), 1_000, "m3", "{\"lined\":true}");

        asset.Decommission(new DateOnly(2035, 1, 1), Now, "oidc|engineer", "Replacement commissioned");

        Assert.Equal(InfrastructureAssetStatus.Decommissioned, asset.Status);
        Assert.Throws<InvalidOperationException>(() => asset.SetOperationalStatus(InfrastructureAssetStatus.Active));
    }

    [Fact]
    public async Task Laboratory_results_are_append_only_in_the_application_persistence_boundary()
    {
        Guid tenantId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new PlatformDbContext(options, new TestTenantContext(tenantId));
        var result = new SoilSampleResult(Guid.NewGuid(), Guid.NewGuid(), "PH", "Soil pH", 6.4m, "pH",
            "ISO 10390", null, "normal", Now, "lab@example.test");
        db.SoilSampleResults.Add(result);
        await db.SaveChangesAsync();

        db.Entry(result).State = EntityState.Modified;

        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
    }

    private sealed class TestTenantContext(Guid tenantId) : ITenantContext
    {
        public Guid TenantId { get; } = tenantId;
        public bool IsResolved => true;
    }
}

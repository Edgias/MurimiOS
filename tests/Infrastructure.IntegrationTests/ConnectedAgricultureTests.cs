using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.EquipmentAsset;
using EdgIQ.MurimiOS.Domain.ConnectedDevice;
using EdgIQ.MurimiOS.Domain.SensorReading;
using EdgIQ.MurimiOS.Domain.MachineryTaskFile;
using EdgIQ.MurimiOS.Domain.BusinessDocumentExchange;
using EdgIQ.MurimiOS.Domain.MachineryTelemetryRecord;
using EdgIQ.MurimiOS.Domain.DerivedMediaAsset;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using BusinessDocumentExchange = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.BusinessDocumentExchange;
using BusinessDocumentType = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.BusinessDocumentType;
using ExchangeDirection = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.ExchangeDirection;
using ConnectedDevice = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDevice;
using ConnectedDeviceKind = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDeviceKind;
using ConnectedDeviceProtocol = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDeviceProtocol;
using ConnectedDeviceStatus = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDeviceStatus;
using SensorChannel = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.SensorChannel;
using DerivedMediaAsset = global::EdgIQ.MurimiOS.Domain.DerivedMediaAsset.DerivedMediaAsset;
using DerivedMediaKind = global::EdgIQ.MurimiOS.Domain.DerivedMediaAsset.DerivedMediaKind;
using DerivedMediaLifecycleStatus = global::EdgIQ.MurimiOS.Domain.DerivedMediaAsset.DerivedMediaLifecycleStatus;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;
using EquipmentStatus = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentStatus;
using EquipmentType = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentType;
using ExchangeStatus = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.ExchangeStatus;
using MachineryTaskDirection = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.MachineryTaskDirection;
using MachineryTaskFile = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.MachineryTaskFile;
using MachineryTaskFormat = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.MachineryTaskFormat;
using SensorReading = global::EdgIQ.MurimiOS.Domain.SensorReading.SensorReading;
using SensorValueQuality = global::EdgIQ.MurimiOS.Domain.SensorReading.SensorValueQuality;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;

public sealed class ConnectedAgricultureTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-08-31T08:00:00Z");
    private static readonly string Hash = new('a', 64);

    [Fact]
    public void Equipment_has_explicit_maintenance_and_retirement_lifecycle()
    {
        var equipment = new EquipmentAsset(Guid.NewGuid(), Guid.NewGuid(), "tr-01", "Main tractor",
            EquipmentType.Tractor, "John Deere", "6120", "SN-1", 2024);
        equipment.SendToMaintenance(); equipment.ReturnToService(); equipment.Retire(Now);
        Assert.Equal(EquipmentStatus.Retired, equipment.Status);
        Assert.Throws<InvalidOperationException>(() => equipment.ReturnToService());
    }

    [Fact]
    public void Device_accepts_only_one_physical_target_and_ordered_lifecycle()
    {
        Guid farmId = Guid.NewGuid();
        Assert.Throws<ArgumentException>(() => new ConnectedDevice(Guid.NewGuid(), farmId, "dev-1", "Probe",
            ConnectedDeviceKind.SoilSensor, ConnectedDeviceProtocol.LoRaWan, null, Guid.NewGuid(), Guid.NewGuid(), null, null, Now));
        var device = new ConnectedDevice(Guid.NewGuid(), farmId, "dev-1", "Probe", ConnectedDeviceKind.SoilSensor,
            ConnectedDeviceProtocol.LoRaWan, null, null, Guid.NewGuid(), null, "1.0", Now);
        device.Activate(Now); device.RecordSeen(Now.AddMinutes(1), "1.1"); device.Suspend(); device.Activate(Now.AddMinutes(2));
        Assert.Equal("1.1", device.FirmwareVersion);
        Assert.Equal(ConnectedDeviceStatus.Active, device.Status);
    }

    [Fact]
    public void Channel_range_determines_reading_quality_without_changing_evidence()
    {
        var channel = new SensorChannel(Guid.NewGuid(), Guid.NewGuid(), "soil-vwc", "soil.moisture.percent", "%", 0, 100, 900);
        Assert.True(channel.IsWithinRange(42));
        Assert.False(channel.IsWithinRange(101));
        var reading = new SensorReading(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), channel.Id,
            channel.MetricCode, 101, channel.Unit, Now, Now.AddSeconds(2), SensorValueQuality.Suspect,
            "reading-1", Hash, null, -17.8, 31.0);
        Assert.Equal(SensorValueQuality.Suspect, reading.Quality);
    }

    [Fact]
    public void Task_and_business_document_payloads_follow_validation_lifecycle()
    {
        var file = new MachineryTaskFile(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, null,
            MachineryTaskDirection.Export, MachineryTaskFormat.IsoXml, "task.zip", "tenant/task.zip", 100,
            Hash, 1, "iso11783-v1", null, Now);
        file.Validate(); file.Complete(Now.AddMinutes(1));
        Assert.Equal(ExchangeStatus.Delivered, file.Status);

        var document = new BusinessDocumentExchange(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            BusinessDocumentType.GoodsReceipt, ExchangeDirection.Inbound, "GR-1", 1, "erp-v1", "{}", Hash, Now, Now);
        document.Validate(); document.Complete(Now.AddMinutes(1), Guid.NewGuid());
        Assert.Equal(ExchangeStatus.Applied, document.Status);
    }

    [Fact]
    public async Task Sensor_readings_are_append_only_in_application_persistence()
    {
        Guid tenantId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<PlatformDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new PlatformDbContext(options, new TestTenantContext(tenantId));
        var reading = new SensorReading(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "air.temperature", 25,
            "C", Now, Now, SensorValueQuality.Valid, "r-1", Hash, null, null, null);
        db.SensorReadings.Add(reading); await db.SaveChangesAsync(); db.SensorReadings.Remove(reading);
        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public void Connected_permissions_separate_ingestion_from_read_only_roles()
    {
        Assert.Contains(PlatformPermissions.ConnectedAgricultureIngest, PlatformPermissions.ForRole(TenantRole.FarmManager));
        Assert.Contains(PlatformPermissions.ConnectedAgricultureRead, PlatformPermissions.ForRole(TenantRole.Operator));
        Assert.DoesNotContain(PlatformPermissions.ConnectedAgricultureIngest, PlatformPermissions.ForRole(TenantRole.Viewer));
    }

    [Fact]
    public void Derived_media_has_an_explicit_legal_hold_lifecycle()
    {
        var asset = new DerivedMediaAsset(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            DerivedMediaKind.SatelliteRaster, "satellite-observation", Guid.NewGuid(),
            $"{Guid.NewGuid():N}/derived/{Guid.NewGuid():N}.tif", "image/tiff", 1_024, Hash, Now);

        asset.PlaceLegalHold("Certification dispute", "compliance-owner", Now.AddHours(1));
        Assert.True(asset.LegalHold);
        Assert.Equal(DerivedMediaLifecycleStatus.Active, asset.Status);

        asset.ReleaseLegalHold("compliance-owner", Now.AddHours(2));
        Assert.False(asset.LegalHold);
        Assert.Null(asset.LegalHoldReason);
        Assert.Throws<InvalidOperationException>(() => asset.ReleaseLegalHold("owner", Now.AddHours(3)));
    }

    private sealed class TestTenantContext(Guid tenantId) : ITenantContext { public Guid TenantId { get; } = tenantId; public bool IsResolved => true; }
}

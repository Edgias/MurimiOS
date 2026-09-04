using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using ReferenceCatalogue = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogue;
using ReferenceCatalogueItem = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogueItem;
using TenantRegionalSettings = global::EdgIQ.MurimiOS.Domain.TenantRegionalSettings.TenantRegionalSettings;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;

public sealed class RegionalFoundationTests
{
    [Fact]
    public void Measurement_conversion_preserves_entered_and_canonical_values()
    {
        MeasuredQuantity quantity = MeasurementUnits.Capture(10m, "acre", "ha");

        Assert.Equal(4.04685642240m, quantity.CanonicalValue);
        Assert.Equal("ha", quantity.CanonicalUnit);
        Assert.Equal(10m, quantity.EnteredValue);
        Assert.Equal("acre", quantity.EnteredUnit);
        Assert.Equal(4.04685642240m, quantity.DisplayValue);
        Assert.Throws<ArgumentException>(() => MeasurementUnits.Convert(1m, "kg", "L"));
    }

    [Fact]
    public void Temperature_conversion_handles_affine_units()
    {
        decimal freezing = MeasurementUnits.Convert(32m, "F", "C");
        decimal boiling = MeasurementUnits.Convert(100m, "C", "F");

        Assert.InRange(freezing, -0.00000000000000000000000001m, 0.00000000000000000000000001m);
        Assert.InRange(boiling, 211.99999999999999999999999m, 212.00000000000000000000000001m);
    }

    [Fact]
    public void Money_arithmetic_rejects_currency_mismatches()
    {
        var usd = new Money(12.50m, new CurrencyCode("usd"));
        var moreUsd = new Money(2.25m, new CurrencyCode("USD"));
        var zar = new Money(10m, new CurrencyCode("ZAR"));

        Assert.Equal(14.75m, usd.Add(moreUsd).Amount);
        Assert.Equal("USD", usd.Currency.Value);
        Assert.Throws<InvalidOperationException>(() => usd.Add(zar));
        Assert.Throws<ArgumentException>(() => new CurrencyCode("US"));
    }

    [Fact]
    public void Regional_settings_validate_locale_timezone_and_unit_dimensions()
    {
        var settings = new TenantRegionalSettings(
            Guid.NewGuid(), "zw", "usd", "en-ZW", "Africa/Harare", 8,
            "ha", "kg", "L", "C");

        Assert.Equal("ZW", settings.CountryCode);
        Assert.Equal("USD", settings.CurrencyCode);
        Assert.Equal("en-ZW", settings.Locale);
        Assert.Throws<ArgumentException>(() => settings.Update(
            "ZW", "USD", "en-ZW", "Africa/Harare", 8,
            "kg", "kg", "L", "C"));
    }

    [Fact]
    public void Business_time_retains_occurrence_and_recording_instants()
    {
        DateTimeOffset occurred = new(2026, 8, 29, 8, 0, 0, TimeSpan.FromHours(2));
        DateTimeOffset recorded = occurred.AddHours(5);
        var time = new BusinessTime(occurred, recorded);

        Assert.Equal(TimeSpan.FromHours(5), time.RecordingDelay);
        Assert.Equal(TimeSpan.Zero, time.OccurredAtUtc.Offset);
        Assert.Equal(occurred.ToUniversalTime(), RegionalTime.ToUtc(
            new DateTime(2026, 8, 29, 8, 0, 0), "Africa/Harare"));
    }

    [Fact]
    public void Reference_catalogues_are_versioned_effective_and_require_object_attributes()
    {
        Guid catalogueId = Guid.NewGuid();
        var catalogue = new ReferenceCatalogue(
            catalogueId, "crop-protection-products", "Crop protection products", "2026.1",
            "ZW", "Pesticides Registration Authority", new DateOnly(2026, 1, 1));
        var item = new ReferenceCatalogueItem(
            Guid.NewGuid(), catalogueId, "REG-001", "Example product", null,
            "{\"activeIngredient\":\"example\"}", new DateOnly(2026, 1, 1));

        Assert.True(catalogue.IsEffectiveOn(new DateOnly(2026, 8, 29)));
        Assert.True(item.IsEffectiveOn(new DateOnly(2026, 8, 29)));
        Assert.Throws<ArgumentException>(() => new ReferenceCatalogueItem(
            Guid.NewGuid(), catalogueId, "BAD", "Bad item", null, "[]", new DateOnly(2026, 1, 1)));
    }

    [Fact]
    public async Task Regional_configuration_is_tenant_isolated()
    {
        Guid tenantA = Guid.NewGuid();
        Guid tenantB = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using (var db = new PlatformDbContext(options, new TestTenantContext(tenantA)))
        {
            db.TenantRegionalSettings.Add(new TenantRegionalSettings(
                Guid.NewGuid(), "ZW", "USD", "en-ZW", "Africa/Harare", 8,
                "ha", "kg", "L", "C"));
            db.ReferenceCatalogues.Add(new ReferenceCatalogue(
                Guid.NewGuid(), "crops", "Crops", "1", "ZW", null, new DateOnly(2026, 1, 1)));
            await db.SaveChangesAsync();
        }

        await using var otherTenant = new PlatformDbContext(options, new TestTenantContext(tenantB));
        Assert.Empty(await otherTenant.TenantRegionalSettings.ToArrayAsync());
        Assert.Empty(await otherTenant.ReferenceCatalogues.ToArrayAsync());
    }

    private sealed class TestTenantContext(Guid tenantId) : ITenantContext
    {
        public Guid TenantId { get; } = tenantId;
        public bool IsResolved => true;
    }
}

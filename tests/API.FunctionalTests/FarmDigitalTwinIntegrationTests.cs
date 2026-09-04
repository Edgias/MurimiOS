using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Npgsql;

namespace EdgIQ.MurimiOS.API.FunctionalTests;

public sealed class FarmDigitalTwinIntegrationTests
{
    [IntegrationFact]
    public async Task Digital_twin_preserves_tenure_assets_soil_zones_samples_and_lab_results()
    {
        await using var factory = new MurimiApiFactory();
        Guid marker = Guid.NewGuid();
        using HttpClient client = factory.CreateTenantClient(Guid.NewGuid());

        HttpResponseMessage farmCreated = await client.PostAsJsonAsync("/api/v1/farms", new
        {
            name = "Digital Twin Integration Farm",
            code = $"DT{marker:N}"[..12],
            countryCode = "ZW",
            timeZone = "Africa/Harare"
        });
        Assert.Equal(HttpStatusCode.Created, farmCreated.StatusCode);
        Guid farmId = (await JsonAsync(farmCreated)).GetProperty("id").GetGuid();

        HttpResponseMessage fieldCreated = await client.PostAsJsonAsync($"/api/v1/farms/{farmId}/fields", new
        {
            name = "North Block",
            validFrom = new DateOnly(2026, 1, 1),
            source = "surveyed",
            exterior = Ring(31.000, -17.810, 31.020, -17.790)
        });
        Assert.True(fieldCreated.StatusCode == HttpStatusCode.Created,
            $"Field creation failed with {(int)fieldCreated.StatusCode}: {await fieldCreated.Content.ReadAsStringAsync()}");
        JsonElement field = await JsonAsync(fieldCreated);
        Guid fieldId = field.GetProperty("id").GetGuid();
        Guid fieldBoundaryId = field.GetProperty("boundaryId").GetGuid();

        HttpResponseMessage tenureCreated = await client.PostAsJsonAsync(
            $"/api/v1/farms/{farmId}/digital-twin/land-tenures", new
            {
                fieldId,
                type = 2,
                holderName = "Murimi Holdings",
                counterpartyName = "Landowner Trust",
                reference = $"LEASE-{marker:N}",
                registeredAreaHa = 42.5m,
                validFrom = new DateOnly(2026, 1, 1),
                documentReference = "documents/lease-2026.pdf"
            });
        Assert.Equal(HttpStatusCode.Created, tenureCreated.StatusCode);
        HttpResponseMessage overlap = await client.PostAsJsonAsync(
            $"/api/v1/farms/{farmId}/digital-twin/land-tenures", new
            {
                fieldId,
                type = 2,
                holderName = "Conflicting Holder",
                reference = $"LEASE-CONFLICT-{marker:N}",
                validFrom = new DateOnly(2027, 1, 1)
            });
        Assert.Equal(HttpStatusCode.Conflict, overlap.StatusCode);

        HttpResponseMessage assetCreated = await client.PostAsJsonAsync(
            $"/api/v1/farms/{farmId}/digital-twin/infrastructure", new
            {
                fieldId,
                code = $"BORE-{marker:N}"[..16],
                name = "North borehole",
                type = 2,
                status = 2,
                longitude = 31.004,
                latitude = -17.804,
                installedOn = new DateOnly(2025, 6, 1),
                capacity = 15m,
                capacityUnit = "m3/h",
                details = new { pump = "solar" }
            });
        Assert.Equal(HttpStatusCode.Created, assetCreated.StatusCode);

        HttpResponseMessage zoneCreated = await client.PostAsJsonAsync(
            $"/api/v1/farms/{farmId}/digital-twin/soil-zones", new
            {
                fieldId,
                fieldBoundaryVersionId = fieldBoundaryId,
                code = "SZ-NORTH-1",
                name = "North sandy loam",
                classification = "Sandy loam",
                managementGuidance = "Increase organic matter and split nitrogen applications.",
                exterior = Ring(31.002, -17.808, 31.010, -17.800),
                validFrom = new DateOnly(2026, 9, 1),
                source = "soil survey"
            });
        Assert.Equal(HttpStatusCode.Created, zoneCreated.StatusCode);
        JsonElement zone = await JsonAsync(zoneCreated);
        Guid zoneBoundaryId = zone.GetProperty("boundaries")[0].GetProperty("id").GetGuid();

        HttpResponseMessage sampleCreated = await client.PostAsJsonAsync(
            $"/api/v1/farms/{farmId}/digital-twin/soil-samples", new
            {
                fieldId,
                fieldBoundaryVersionId = fieldBoundaryId,
                soilZoneBoundaryVersionId = zoneBoundaryId,
                sampleCode = $"SOIL-{marker:N}"[..18],
                longitude = 31.005,
                latitude = -17.805,
                sampledAt = DateTimeOffset.Parse("2026-09-01T10:00:00Z"),
                sampledBy = "integration-agronomist",
                depthFromCm = 0,
                depthToCm = 20,
                laboratoryName = "Integration Soil Laboratory",
                laboratoryReference = $"LAB-{marker:N}"[..18]
            });
        Assert.Equal(HttpStatusCode.Created, sampleCreated.StatusCode);
        Guid sampleId = (await JsonAsync(sampleCreated)).GetProperty("id").GetGuid();

        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync(
            $"/api/v1/farms/{farmId}/digital-twin/soil-samples/{sampleId}/submit", new { })).StatusCode);
        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync(
            $"/api/v1/farms/{farmId}/digital-twin/soil-samples/{sampleId}/results", new
            {
                analyteCode = "PH",
                analyteName = "Soil pH",
                value = 6.4m,
                unit = "pH",
                method = "ISO 10390",
                resultFlag = "normal"
            })).StatusCode);
        HttpResponseMessage analysed = await client.PostAsJsonAsync(
            $"/api/v1/farms/{farmId}/digital-twin/soil-samples/{sampleId}/analyse", new { });
        Assert.Equal(HttpStatusCode.OK, analysed.StatusCode);
        Assert.Equal(3, (await JsonAsync(analysed)).GetProperty("status").GetInt32());

        HttpResponseMessage outsideSample = await client.PostAsJsonAsync(
            $"/api/v1/farms/{farmId}/digital-twin/soil-samples", new
            {
                fieldId,
                fieldBoundaryVersionId = fieldBoundaryId,
                soilZoneBoundaryVersionId = zoneBoundaryId,
                sampleCode = $"OUT-{marker:N}"[..18],
                longitude = 31.018,
                latitude = -17.792,
                sampledAt = DateTimeOffset.Parse("2026-09-01T11:00:00Z"),
                sampledBy = "integration-agronomist",
                depthFromCm = 0,
                depthToCm = 20
            });
        Assert.Equal(HttpStatusCode.Conflict, outsideSample.StatusCode);

        HttpResponseMessage overviewResponse = await client.GetAsync($"/api/v1/farms/{farmId}/digital-twin");
        Assert.Equal(HttpStatusCode.OK, overviewResponse.StatusCode);
        JsonElement overview = await JsonAsync(overviewResponse);
        Assert.Equal(1, overview.GetProperty("summary").GetProperty("activeTenures").GetInt32());
        Assert.Equal(1, overview.GetProperty("summary").GetProperty("activeSoilZones").GetInt32());
        Assert.Equal(1, overview.GetProperty("summary").GetProperty("analysedSamples").GetInt32());
        Assert.Equal("FeatureCollection", overview.GetProperty("map").GetProperty("type").GetString());
    }

    [IntegrationFact]
    public async Task Digital_twin_tables_are_forced_rls_and_history_has_least_privilege_grants()
    {
        await using var connection = new NpgsqlConnection(MurimiApiFactory.ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand("""
            select
              (select bool_and(c.relrowsecurity and c.relforcerowsecurity)
               from pg_class c join pg_namespace n on n.oid=c.relnamespace
               where n.nspname='farm' and c.relname in ('land_tenure_records','infrastructure_assets',
                 'soil_zones','soil_zone_boundary_versions','soil_samples','soil_sample_results')),
              not has_table_privilege(current_user,'farm.soil_sample_results','UPDATE'),
              not has_table_privilege(current_user,'farm.soil_sample_results','DELETE'),
              not has_table_privilege(current_user,'farm.land_tenure_records','DELETE')
            """, connection);
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.True(reader.GetBoolean(0));
        Assert.True(reader.GetBoolean(1));
        Assert.True(reader.GetBoolean(2));
        Assert.True(reader.GetBoolean(3));
    }

    private static object[] Ring(double west, double south, double east, double north) =>
    [
        new { longitude = west, latitude = south },
        new { longitude = east, latitude = south },
        new { longitude = east, latitude = north },
        new { longitude = west, latitude = north },
        new { longitude = west, latitude = south }
    ];

    private static async Task<JsonElement> JsonAsync(HttpResponseMessage response)
    {
        using JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.Clone();
    }
}

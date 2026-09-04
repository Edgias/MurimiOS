using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Npgsql;

namespace EdgIQ.MurimiOS.API.FunctionalTests;

public sealed class EpcisInteropIntegrationTests
{
    [IntegrationFact]
    public async Task Capture_is_idempotent_and_simple_event_query_returns_epcis_query_document()
    {
        await using var factory = new MurimiApiFactory();
        Guid tenantId = Guid.NewGuid();
        Guid farmId = Guid.NewGuid();
        using HttpClient client = factory.CreateTenantClient(tenantId);
        HttpResponseMessage farm = await client.PostAsJsonAsync("/api/v1/farms", new
        {
            id = farmId,
            name = "EPCIS Interchange Farm",
            code = $"EP{farmId:N}"[..12].ToUpperInvariant(),
            countryCode = "ZW",
            timeZone = "Africa/Harare"
        });
        Assert.Equal(HttpStatusCode.Created, farm.StatusCode);

        Guid documentId = Guid.NewGuid();
        Guid eventId = Guid.NewGuid();
        const string epc = "urn:epc:id:sgtin:0614141.107346.2017";
        string payload = $$"""
            {
              "@context":"https://ref.gs1.org/standards/epcis/epcis-context.jsonld",
              "id":"urn:uuid:{{documentId:D}}",
              "type":"EPCISDocument",
              "schemaVersion":"2.0",
              "creationDate":"2026-09-01T06:00:00Z",
              "epcisBody":{"eventList":[{
                "eventID":"urn:uuid:{{eventId:D}}",
                "type":"ObjectEvent",
                "eventTime":"2026-09-01T08:00:00+02:00",
                "eventTimeZoneOffset":"+02:00",
                "action":"OBSERVE",
                "bizStep":"shipping",
                "disposition":"in_transit",
                "epcList":["{{epc}}"],
                "https://example.test/coldChainTemperature":4.0
              }]}
            }
            """;

        HttpResponseMessage captured = await client.PostAsync($"/api/v1/epcis/capture?farmId={farmId}",
            new StringContent(payload, Encoding.UTF8, "application/json"));
        string capturedBody = await captured.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.Accepted, captured.StatusCode);
        Assert.Equal("2.0.1", captured.Headers.GetValues("GS1-EPCIS-Version").Single());
        Assert.NotNull(captured.Headers.Location);
        using JsonDocument capturedDocument = JsonDocument.Parse(capturedBody);
        Guid captureId = capturedDocument.RootElement.GetProperty("captureID").GetGuid();

        HttpResponseMessage repeated = await client.PostAsync($"/api/v1/epcis/capture?farmId={farmId}",
            new StringContent(payload, Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Accepted, repeated.StatusCode);
        using JsonDocument repeatedDocument = JsonDocument.Parse(await repeated.Content.ReadAsStringAsync());
        Assert.Equal(captureId, repeatedDocument.RootElement.GetProperty("captureID").GetGuid());

        HttpResponseMessage status = await client.GetAsync($"/api/v1/epcis/capture/{captureId}");
        Assert.Equal(HttpStatusCode.OK, status.StatusCode);
        HttpResponseMessage result = await client.GetAsync(
            $"/api/v1/epcis/events?farmId={farmId}&eventType=ObjectEvent&EQ_action=OBSERVE" +
            $"&EQ_bizStep=shipping&EQ_disposition=in_transit&MATCH_anyEPC={Uri.EscapeDataString(epc)}");
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        using JsonDocument queryDocument = JsonDocument.Parse(await result.Content.ReadAsStringAsync());
        JsonElement root = queryDocument.RootElement;
        Assert.Equal("EPCISQueryDocument", root.GetProperty("type").GetString());
        JsonElement returned = Assert.Single(root.GetProperty("epcisBody").GetProperty("queryResults")
            .GetProperty("resultsBody").GetProperty("eventList").EnumerateArray());
        Assert.Equal($"urn:uuid:{eventId:D}", returned.GetProperty("eventID").GetString());
        Assert.Equal(4.0m, returned.GetProperty("https://example.test/coldChainTemperature").GetDecimal());

        HttpResponseMessage mappings = await client.GetAsync("/api/v1/epcis/cbv/mappings");
        Assert.Equal(HttpStatusCode.OK, mappings.StatusCode);
    }

    [IntegrationFact]
    public async Task Epcis_repository_tables_are_forced_rls_and_insert_only_for_the_app_role()
    {
        await using var connection = new NpgsqlConnection(MurimiApiFactory.ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand("""
            select
              (select bool_and(c.relrowsecurity and c.relforcerowsecurity)
               from pg_class c join pg_namespace n on n.oid=c.relnamespace
               where n.nspname='traceability' and c.relname in ('epcis_exchanges','epcis_captured_events')),
              has_table_privilege(current_user,'traceability.epcis_exchanges','SELECT'),
              has_table_privilege(current_user,'traceability.epcis_exchanges','INSERT'),
              not has_table_privilege(current_user,'traceability.epcis_exchanges','UPDATE'),
              not has_table_privilege(current_user,'traceability.epcis_exchanges','DELETE'),
              not has_table_privilege(current_user,'traceability.epcis_captured_events','UPDATE'),
              not has_table_privilege(current_user,'traceability.epcis_captured_events','DELETE')
            """, connection);
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        for (int index = 0; index < 7; index++) Assert.True(reader.GetBoolean(index));
    }
}

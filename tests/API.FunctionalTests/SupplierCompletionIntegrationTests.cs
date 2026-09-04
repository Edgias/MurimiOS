using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Npgsql;

namespace EdgIQ.MurimiOS.API.FunctionalTests;

public sealed class SupplierCompletionIntegrationTests
{
    [IntegrationFact]
    public async Task Supplier_risk_pricing_reconciliation_and_public_claim_detach_work_end_to_end()
    {
        await using var factory = new MurimiApiFactory();
        using HttpClient buyer = factory.CreateTenantClient(Guid.NewGuid());
        Guid claimedTenantId = Guid.NewGuid();
        Guid marker = Guid.NewGuid();

        HttpResponseMessage supplierCreated = await buyer.PostAsJsonAsync("/api/v1/supplier-portal/suppliers", new
        {
            supplierCode = $"SUP-{marker:N}", name = "Completion Growers", contactName = "Tariro",
            email = $"tariro-{marker:N}@example.test", phone = "+263700000000"
        });
        Assert.Equal(HttpStatusCode.Created, supplierCreated.StatusCode);
        Guid supplierId = (await JsonAsync(supplierCreated)).GetProperty("id").GetGuid();

        string claimSecret = await IssueActionAsync(buyer, supplierId, "Claim", claimedTenantId);
        using HttpClient publicClient = factory.CreateClient();
        HttpResponseMessage preview = await publicClient.GetAsync($"/api/public/v1/supplier-accounts/actions/{claimSecret}");
        Assert.Equal(HttpStatusCode.OK, preview.StatusCode);
        HttpResponseMessage claimed = await publicClient.PostAsync($"/api/public/v1/supplier-accounts/actions/{claimSecret}/consume", null);
        Assert.Equal(HttpStatusCode.OK, claimed.StatusCode);
        Assert.Equal(3, (await JsonAsync(claimed)).GetProperty("status").GetInt32());
        Assert.Equal(HttpStatusCode.Gone,
            (await publicClient.PostAsync($"/api/public/v1/supplier-accounts/actions/{claimSecret}/consume", null)).StatusCode);

        string detachSecret = await IssueActionAsync(buyer, supplierId, "Detach", claimedTenantId);
        HttpResponseMessage detached = await publicClient.PostAsync($"/api/public/v1/supplier-accounts/actions/{detachSecret}/consume", null);
        Assert.Equal(HttpStatusCode.OK, detached.StatusCode);
        Assert.Equal(1, (await JsonAsync(detached)).GetProperty("status").GetInt32());
        JsonElement history = await JsonAsync(await buyer.GetAsync($"/api/v1/supplier-portal/suppliers/{supplierId}/link-history"));
        Assert.Equal(2, history.GetArrayLength());

        HttpResponseMessage riskCreated = await buyer.PostAsJsonAsync("/api/v1/supplier-portal/risk-assessments", new
        {
            hostedSupplierId = supplierId
        });
        Assert.Equal(HttpStatusCode.Created, riskCreated.StatusCode);
        JsonElement risk = await JsonAsync(riskCreated);
        Assert.Equal("supplier-risk-v1", risk.GetProperty("modelVersion").GetString());
        Assert.InRange(risk.GetProperty("score").GetInt32(), 0, 100);

        HttpResponseMessage contractCreated = await buyer.PostAsJsonAsync("/api/v1/supplier-portal/contracts", new
        {
            contractNumber = $"CTR-{marker:N}", hostedSupplierId = supplierId, commodity = "Maize",
            validFrom = "2026-08-01", validTo = "2027-01-31", contractedQuantity = 100m,
            quantityUnit = "t", unitPrice = 200m, currency = "USD", terms = new { incoterm = "DAP" }
        });
        Assert.Equal(HttpStatusCode.Created, contractCreated.StatusCode);
        Guid contractId = (await JsonAsync(contractCreated)).GetProperty("id").GetGuid();
        HttpResponseMessage ruleCreated = await buyer.PostAsJsonAsync(
            $"/api/v1/supplier-portal/contracts/{contractId}/price-rules", new
            {
                priority = 1, method = 3, minimumQuantity = 0m, qualityGrade = "A",
                unitPrice = 220m, currency = "USD", effectiveFrom = "2026-08-01", effectiveTo = "2027-01-31"
            });
        Assert.Equal(HttpStatusCode.Created, ruleCreated.StatusCode);
        Assert.Equal(HttpStatusCode.OK,
            (await buyer.PostAsJsonAsync($"/api/v1/supplier-portal/contracts/{contractId}/activate", new { })).StatusCode);
        HttpResponseMessage allocationCreated = await buyer.PostAsJsonAsync(
            $"/api/v1/supplier-portal/contracts/{contractId}/allocations", new
            {
                reference = $"ALLOC-{marker:N}", deliveryFrom = "2026-08-01", deliveryTo = "2026-09-01",
                allocatedQuantity = 100m, quantityUnit = "t"
            });
        Assert.Equal(HttpStatusCode.Created, allocationCreated.StatusCode);
        Guid allocationId = (await JsonAsync(allocationCreated)).GetProperty("id").GetGuid();
        Assert.Equal(HttpStatusCode.OK, (await buyer.PostAsJsonAsync(
            $"/api/v1/supplier-portal/contracts/{contractId}/allocations/{allocationId}/fulfil", new { quantity = 100m })).StatusCode);

        JsonElement quote = await JsonAsync(await buyer.GetAsync(
            $"/api/v1/supplier-portal/contracts/{contractId}/price-quote?quantity=100&qualityGrade=A&effectiveOn=2026-09-01"));
        Assert.Equal(220m, quote.GetProperty("unitPrice").GetDecimal());
        HttpResponseMessage reconciled = await buyer.PostAsJsonAsync(
            $"/api/v1/supplier-portal/contracts/{contractId}/reconciliations", new
            {
                reference = $"REC-{marker:N}", acceptedQuantity = 90m, rejectedQuantity = 10m,
                qualityGrade = "A", qualityAdjustmentAmount = -100m, otherAdjustmentAmount = 0m,
                effectiveOn = "2026-09-01", notes = "Ten tonnes rejected on grading"
            });
        Assert.Equal(HttpStatusCode.Created, reconciled.StatusCode);
        JsonElement reconciliation = await JsonAsync(reconciled);
        Assert.Equal(19_700m, reconciliation.GetProperty("payableAmount").GetDecimal());
        Assert.Equal(2, reconciliation.GetProperty("status").GetInt32());

        JsonElement status = await JsonAsync(await buyer.GetAsync($"/api/v1/supplier-portal/suppliers/{supplierId}/status"));
        Assert.Equal("supplier-risk-v1", status.GetProperty("risk").GetProperty("modelVersion").GetString());
    }

    [IntegrationFact]
    public async Task Supplier_completion_tables_are_forced_rls_append_only_and_public_resolver_is_not_public()
    {
        await using var connection = new NpgsqlConnection(MurimiApiFactory.ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand("""
            select
              (select bool_and(c.relrowsecurity and c.relforcerowsecurity)
               from pg_class c join pg_namespace n on n.oid=c.relnamespace
               where n.nspname='supply' and c.relname in (
                 'supplier_risk_assessments','supplier_self_service_tokens','hosted_supplier_link_events',
                 'contract_price_rules','contract_reconciliations')),
              not has_table_privilege(current_user,'supply.supplier_risk_assessments','UPDATE'),
              not has_table_privilege(current_user,'supply.hosted_supplier_link_events','DELETE'),
              not has_table_privilege(current_user,'supply.contract_price_rules','UPDATE'),
              not has_table_privilege(current_user,'supply.contract_reconciliations','DELETE'),
              not has_table_privilege(current_user,'supply.supplier_self_service_tokens','DELETE'),
              not has_function_privilege('public','supply.resolve_supplier_self_service_token(text)','EXECUTE'),
              has_function_privilege(current_user,'supply.resolve_supplier_self_service_token(text)','EXECUTE')
            """, connection);
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        for (int index = 0; index < 8; index++) Assert.True(reader.GetBoolean(index));
    }

    private static async Task<string> IssueActionAsync(HttpClient buyer, Guid supplierId, string action, Guid claimedTenantId)
    {
        HttpResponseMessage response = await buyer.PostAsJsonAsync(
            $"/api/v1/supplier-portal/suppliers/{supplierId}/self-service-actions", new
            {
                action = action == "Claim" ? 1 : 2, claimedTenantId, expiresAt = DateTimeOffset.UtcNow.AddHours(1)
            });
        string body = await response.Content.ReadAsStringAsync();
        Assert.True(response.StatusCode == HttpStatusCode.Created, $"HTTP {(int)response.StatusCode}: {body}");
        using JsonDocument document = JsonDocument.Parse(body);
        return document.RootElement.GetProperty("secret").GetString()!;
    }

    private static async Task<JsonElement> JsonAsync(HttpResponseMessage response)
    {
        string body = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"HTTP {(int)response.StatusCode}: {body}");
        using JsonDocument document = JsonDocument.Parse(body);
        return document.RootElement.Clone();
    }
}

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.FinanceOpeningBalance;
using EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule;
using EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun;
using EdgIQ.MurimiOS.Domain.LegalCompany;
using EdgIQ.MurimiOS.Domain.CostCentre;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using EdgIQ.MurimiOS.Domain.AccountingImportBatch;
using EdgIQ.MurimiOS.Domain.AccountingReconciliation;
using EdgIQ.MurimiOS.Domain.LabourCostEntry;
using EdgIQ.MurimiOS.Domain.MachineryCostEntry;
using EdgIQ.MurimiOS.Domain.OrganisationNode;

namespace EdgIQ.MurimiOS.API.FunctionalTests;
using CostCentreType = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentreType;
using ConsolidationEntryStatus = global::EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun.ConsolidationEntryStatus;
using CostAllocationMethod = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocationMethod;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
using FinanceSourceType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceSourceType;
using OrganisationNodeType = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNodeType;

public sealed class EnterpriseConsolidationIntegrationTests
{
    [IntegrationFact]
    public async Task Legal_companies_intercompany_ledgers_and_elimination_snapshot_round_trip()
    {
        await using var factory = new MurimiApiFactory();
        Guid tenantId = Guid.NewGuid(); using HttpClient client = factory.CreateTenantClient(tenantId);
        Guid rootId = await CreateNodeAsync(client, null, "Integration Holding", OrganisationNodeType.Organisation);
        Guid sellerNodeId = await CreateNodeAsync(client, rootId, "Seller Legal Entity", OrganisationNodeType.Organisation);
        Guid buyerNodeId = await CreateNodeAsync(client, rootId, "Buyer Legal Entity", OrganisationNodeType.Organisation);
        Guid sellerFarmId = Guid.NewGuid(); Guid buyerFarmId = Guid.NewGuid();
        await CreateFarmAsync(client, sellerFarmId, sellerNodeId, "Seller Integration Farm", "ICS");
        await CreateFarmAsync(client, buyerFarmId, buyerNodeId, "Buyer Integration Farm", "ICB");
        Guid sellerCompanyId = await CreateCompanyAsync(client, sellerNodeId, "SELLCO", "REG-SELL");
        Guid buyerCompanyId = await CreateCompanyAsync(client, buyerNodeId, "BUYCO", "REG-BUY");
        Guid sellerCentreId = await CreateCentreAsync(client, sellerFarmId, "SELL-REV");
        Guid buyerCentreId = await CreateCentreAsync(client, buyerFarmId, "BUY-EXP");
        DateTimeOffset occurred = DateTimeOffset.Parse("2026-09-02T10:00:00Z");
        await PostTransactionAsync(client, sellerFarmId, sellerCentreId, FinanceEntryType.Revenue, 100m,
            "IC-SELL-001", buyerCompanyId, "GROUP-GRAIN-001", occurred);
        await PostTransactionAsync(client, buyerFarmId, buyerCentreId, FinanceEntryType.Expense, 98m,
            "IC-BUY-001", sellerCompanyId, "GROUP-GRAIN-001", occurred);

        HttpResponseMessage ruleResponse = await client.PostAsJsonAsync("/api/v1/enterprise/elimination-rules", new
        {
            consolidationOrganisationNodeId = rootId, sellerCompanyId, buyerCompanyId,
            ruleKey = "internal-grain", ruleVersion = 1, name = "Internal grain sales", currency = "USD",
            balancingTolerance = 0.50m, effectiveFrom = new DateOnly(2026, 1, 1), effectiveTo = (DateOnly?)null
        });
        Assert.Equal(HttpStatusCode.Created, ruleResponse.StatusCode);
        Guid ruleId = (await ruleResponse.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync($"/api/v1/enterprise/elimination-rules/{ruleId}/approve",
            new { approvalReference = "BOARD-FIN-2026-09" })).StatusCode);

        DateTimeOffset from = DateTimeOffset.Parse("2026-09-01T00:00:00Z");
        DateTimeOffset to = DateTimeOffset.Parse("2026-09-03T00:00:00Z");
        HttpResponseMessage consolidationResponse = await client.PostAsJsonAsync(
            $"/api/v1/enterprise/organisations/{rootId}/consolidations", new { periodStart = from, periodEnd = to, currency = "USD" });
        Assert.Equal(HttpStatusCode.Created, consolidationResponse.StatusCode);
        JsonElement result = await consolidationResponse.Content.ReadFromJsonAsync<JsonElement>();
        JsonElement run = result.GetProperty("run");
        Assert.Equal(98m, run.GetProperty("eliminatedRevenue").GetDecimal());
        Assert.Equal(2m, run.GetProperty("consolidatedRevenue").GetDecimal());
        Assert.Equal(0m, run.GetProperty("consolidatedExpenses").GetDecimal());
        Assert.Equal(1, run.GetProperty("exceptionCount").GetInt32());
        Assert.Equal((int)ConsolidationEntryStatus.Exception,
            result.GetProperty("entries")[0].GetProperty("status").GetInt32());
        Guid runId = run.GetProperty("id").GetGuid();
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/v1/enterprise/consolidations/{runId}")).StatusCode);

        string profitabilityUrl = $"/api/v1/finance/organisation/{rootId}/profitability?from={Uri.EscapeDataString(from.ToString("O"))}&to={Uri.EscapeDataString(to.ToString("O"))}&currency=USD";
        JsonElement profitability = await client.GetFromJsonAsync<JsonElement>(profitabilityUrl);
        Assert.Equal(runId, profitability.GetProperty("latestConsolidation").GetProperty("id").GetGuid());
    }

    private static async Task<Guid> CreateNodeAsync(HttpClient client, Guid? parentId, string name, OrganisationNodeType type)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/organisation/nodes", new { parentId, name, nodeType = (int)type });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
    }

    private static async Task CreateFarmAsync(HttpClient client, Guid id, Guid parentId, string name, string prefix)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/farms", new
        { id, parentOrganisationNodeId = parentId, name, code = $"{prefix}{id:N}"[..12], countryCode = "ZW", timeZone = "Africa/Harare" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private static async Task<Guid> CreateCompanyAsync(HttpClient client, Guid organisationNodeId, string code, string registration)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/enterprise/legal-companies", new
        {
            organisationNodeId, companyCode = code, legalName = $"{code} Limited", registrationNumber = registration,
            taxIdentifier = $"TIN-{registration}", countryCode = "ZW", functionalCurrency = "USD",
            effectiveFrom = new DateOnly(2026, 1, 1)
        });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateCentreAsync(HttpClient client, Guid farmId, string code)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync($"/api/v1/finance/farms/{farmId}/cost-centres", new
        { code, name = code, type = (int)CostCentreType.Farm, targetId = farmId, currency = "USD" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
    }

    private static async Task PostTransactionAsync(HttpClient client, Guid farmId, Guid centreId,
        FinanceEntryType type, decimal amount, string reference, Guid counterpartyCompanyId,
        string intercompanyReference, DateTimeOffset occurredAt)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync($"/api/v1/finance/farms/{farmId}/transactions", new
        {
            entryType = (int)type, category = "Grain", amount, currency = "USD", occurredAt,
            sourceType = (int)FinanceSourceType.Manual, sourceId = (Guid?)null, reference,
            counterparty = "Group company", description = "Intercompany integration test",
            allocations = new[] { new { costCentreId = centreId, amount, method = (int)CostAllocationMethod.Direct, percentage = (decimal?)100m, notes = "Intercompany" } },
            counterpartyLegalCompanyId = counterpartyCompanyId, intercompanyReference
        });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}

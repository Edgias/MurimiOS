using EdgIQ.MurimiOS.Domain.TenantMembership;
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
using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.LandTenureRecord;
using EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset;
using EdgIQ.MurimiOS.Domain.SoilZone;
using EdgIQ.MurimiOS.Domain.SoilSample;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.OrganisationNode;



using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;

using EdgIQ.MurimiOS.Application.Common.Finance;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using AccountingImportBatch = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingImportBatch;
using AccountingImportStatus = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingImportStatus;
using CostCentre = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentre;
using CostCentreType = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentreType;
using ConsolidationEntryStatus = global::EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun.ConsolidationEntryStatus;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
using FinanceSourceType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceSourceType;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;
using EliminationRuleStatus = global::EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule.EliminationRuleStatus;
using IntercompanyEliminationRule = global::EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule.IntercompanyEliminationRule;
using LabourCostEntry = global::EdgIQ.MurimiOS.Domain.LabourCostEntry.LabourCostEntry;
using LegalCompany = global::EdgIQ.MurimiOS.Domain.LegalCompany.LegalCompany;
using MachineryCostEntry = global::EdgIQ.MurimiOS.Domain.MachineryCostEntry.MachineryCostEntry;
using OrganisationNodeType = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNodeType;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;
using EnterpriseConsolidationBuild = global::EdgIQ.MurimiOS.Application.Common.Finance.EnterpriseConsolidationBuild;
using EnterpriseConsolidationEngine = global::EdgIQ.MurimiOS.Application.Common.Finance.EnterpriseConsolidationEngine;

public sealed class FinanceTests
{
    [Fact]
    public void Transactions_and_reversals_preserve_signed_ledger_effects()
    {
        Guid farmId = Guid.NewGuid(); Guid originalId = Guid.NewGuid(); DateTimeOffset now = DateTimeOffset.UtcNow;
        var original = new FinanceTransaction(originalId, farmId, FinanceEntryType.Expense, "Seed",
            125.50m, "usd", now, FinanceSourceType.Manual, null, "INV-1", "Supplier", null, now, "manager");
        var reversal = new FinanceTransaction(Guid.NewGuid(), farmId, FinanceEntryType.Expense, "Seed",
            125.50m, "USD", now.AddDays(1), FinanceSourceType.Reversal, originalId, "REV-INV-1",
            "Supplier", "Correction", now.AddDays(1), "manager", originalId);

        Assert.Equal(125.50m, original.SignedAmount);
        Assert.Equal(-125.50m, reversal.SignedAmount);
        Assert.Equal(0m, original.SignedAmount + reversal.SignedAmount);
    }

    [Fact]
    public void Labour_and_machinery_entries_calculate_canonical_duration_and_total_cost()
    {
        Guid farmId = Guid.NewGuid(); Guid centreId = Guid.NewGuid(); DateTimeOffset now = DateTimeOffset.UtcNow;
        var labour = new LabourCostEntry(Guid.NewGuid(), farmId, Guid.NewGuid(), centreId, null,
            null, null, "TEAM-A", now, 2.5m, 8m, "USD", null);
        var machinery = new MachineryCostEntry(Guid.NewGuid(), farmId, Guid.NewGuid(), centreId,
            null, null, null, "TRACTOR-1", now, 3m, 20m, 15m, "USD", null);

        Assert.Equal(150m, labour.DurationMinutes);
        Assert.Equal(20m, labour.TotalCost);
        Assert.Equal(180m, machinery.DurationMinutes);
        Assert.Equal(75m, machinery.TotalCost);
    }

    [Fact]
    public void Cost_centre_and_accounting_import_have_controlled_lifecycles()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        var centre = new CostCentre(Guid.NewGuid(), Guid.NewGuid(), "crop-01", "Maize",
            CostCentreType.CropZone, Guid.NewGuid(), "USD");
        centre.Close(now, "manager");
        Assert.False(centre.IsActive);
        Assert.Throws<InvalidOperationException>(() => centre.Close(now, "manager"));

        var batch = new AccountingImportBatch(Guid.NewGuid(), Guid.NewGuid(), "Xero", "BATCH-1",
            new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 31), "USD", now, "accountant");
        batch.CompleteReconciliation(true, now.AddMinutes(1), "accountant");
        Assert.Equal(AccountingImportStatus.Exceptions, batch.Status);
        Assert.Throws<InvalidOperationException>(() => batch.CompleteReconciliation(false, now, "accountant"));
    }

    [Fact]
    public void Legal_company_and_elimination_rule_have_controlled_lifecycles()
    {
        DateTimeOffset now = DateTimeOffset.Parse("2026-09-02T10:00:00Z");
        var company = new LegalCompany(Guid.NewGuid(), Guid.NewGuid(), "mze", "Mazowe Estates Ltd",
            "ZW-REG-100", "TIN-100", "zw", "usd", new DateOnly(2026, 1, 1), "company-secretary");
        company.Close(new DateOnly(2026, 12, 31), now, "company-secretary");
        Assert.False(company.IsActive);
        Assert.Throws<InvalidOperationException>(() => company.Close(new DateOnly(2027, 1, 1), now, "actor"));

        var rule = new IntercompanyEliminationRule(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "grain-sales", 1, "Internal grain sales", "usd", 0.50m, new DateOnly(2026, 1, 1), null, "controller");
        rule.Approve(now, "group-controller", "FIN-ELIM-001");
        rule.Retire(now.AddDays(1), "group-controller");
        Assert.Equal(EliminationRuleStatus.Retired, rule.Status);
    }

    [Fact]
    public async Task Consolidation_matches_both_ledger_sides_and_retains_imbalances()
    {
        Guid tenantId = Guid.NewGuid(); Guid rootId = Guid.NewGuid(); Guid sellerNodeId = Guid.NewGuid();
        Guid buyerNodeId = Guid.NewGuid(); Guid sellerFarmNodeId = Guid.NewGuid(); Guid buyerFarmNodeId = Guid.NewGuid();
        Guid sellerFarmId = Guid.NewGuid(); Guid buyerFarmId = Guid.NewGuid(); Guid sellerCompanyId = Guid.NewGuid();
        Guid buyerCompanyId = Guid.NewGuid(); DateTimeOffset now = DateTimeOffset.Parse("2026-09-02T10:00:00Z");
        var options = new DbContextOptionsBuilder<PlatformDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new PlatformDbContext(options, new FinanceTenantContext(tenantId));
        db.OrganisationNodes.AddRange(
            new OrganisationNode(rootId, "Holding", OrganisationNodeType.Organisation, "holding"),
            new OrganisationNode(sellerNodeId, "Seller Co", OrganisationNodeType.Organisation, "holding.seller", rootId),
            new OrganisationNode(buyerNodeId, "Buyer Co", OrganisationNodeType.Organisation, "holding.buyer", rootId),
            new OrganisationNode(sellerFarmNodeId, "Seller Farm", OrganisationNodeType.Farm, "holding.seller.farm", sellerNodeId),
            new OrganisationNode(buyerFarmNodeId, "Buyer Farm", OrganisationNodeType.Farm, "holding.buyer.farm", buyerNodeId));
        db.Farms.AddRange(new Farm(sellerFarmId, sellerFarmNodeId, "Seller Farm", "SELL", "ZW", "Africa/Harare"),
            new Farm(buyerFarmId, buyerFarmNodeId, "Buyer Farm", "BUY", "ZW", "Africa/Harare"));
        db.LegalCompanies.AddRange(
            new LegalCompany(sellerCompanyId, sellerNodeId, "SELLCO", "Seller Company", "REG-S", null, "ZW", "USD", new DateOnly(2026, 1, 1), "secretary"),
            new LegalCompany(buyerCompanyId, buyerNodeId, "BUYCO", "Buyer Company", "REG-B", null, "ZW", "USD", new DateOnly(2026, 1, 1), "secretary"));
        var rule = new IntercompanyEliminationRule(Guid.NewGuid(), rootId, sellerCompanyId, buyerCompanyId,
            "grain", 1, "Internal grain", "USD", 0.50m, new DateOnly(2026, 1, 1), null, "controller");
        rule.Approve(now, "controller", "APP-1"); db.IntercompanyEliminationRules.Add(rule);
        db.FinanceTransactions.AddRange(
            new FinanceTransaction(Guid.NewGuid(), sellerFarmId, FinanceEntryType.Revenue, "Grain", 100m, "USD", now,
                FinanceSourceType.Manual, null, "SELL-1", "Buyer Company", null, now, "accountant", null, buyerCompanyId, "IC-001"),
            new FinanceTransaction(Guid.NewGuid(), buyerFarmId, FinanceEntryType.Expense, "Grain", 98m, "USD", now,
                FinanceSourceType.Manual, null, "BUY-1", "Seller Company", null, now, "accountant", null, sellerCompanyId, "IC-001"));
        await db.SaveChangesAsync();

        EnterpriseConsolidationBuild result = await new EnterpriseConsolidationEngine().BuildAsync(Guid.NewGuid(), rootId,
            now.AddDays(-1), now.AddDays(1), "USD", "controller", now.AddMinutes(1), db, default);

        Assert.Equal(98m, result.Run.EliminatedRevenue);
        Assert.Equal(2m, result.Run.ConsolidatedRevenue);
        Assert.Equal(0m, result.Run.ConsolidatedExpenses);
        Assert.Equal(1, result.Run.ExceptionCount);
        Assert.Equal(ConsolidationEntryStatus.Exception, Assert.Single(result.Entries).Status);
    }

    [Fact]
    public async Task Posted_finance_records_are_append_only()
    {
        Guid tenantId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new PlatformDbContext(options, new FinanceTenantContext(tenantId));
        var transaction = new FinanceTransaction(Guid.NewGuid(), Guid.NewGuid(), FinanceEntryType.Revenue,
            "Sale", 500m, "USD", DateTimeOffset.UtcNow, FinanceSourceType.Manual, null,
            "SALE-1", null, null, DateTimeOffset.UtcNow, "manager");
        db.FinanceTransactions.Add(transaction);
        await db.SaveChangesAsync();
        db.FinanceTransactions.Remove(transaction);
        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public void Finance_permissions_separate_visibility_posting_and_reconciliation()
    {
        Assert.Contains(PlatformPermissions.FinanceReconcile, PlatformPermissions.ForRole(TenantRole.FarmManager));
        Assert.Contains(PlatformPermissions.FinanceRead, PlatformPermissions.ForRole(TenantRole.Agronomist));
        Assert.DoesNotContain(PlatformPermissions.FinanceWrite, PlatformPermissions.ForRole(TenantRole.Agronomist));
        Assert.Contains(PlatformPermissions.FinanceRead, PlatformPermissions.ForRole(TenantRole.Auditor));
        Assert.DoesNotContain(PlatformPermissions.FinanceRead, PlatformPermissions.ForRole(TenantRole.Operator));
    }

    private sealed class FinanceTenantContext(Guid tenantId) : ITenantContext
    {
        public Guid TenantId { get; } = tenantId;
        public bool IsResolved => true;
    }
}

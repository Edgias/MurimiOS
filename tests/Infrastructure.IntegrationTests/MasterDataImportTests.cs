using System.Text;
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
using EdgIQ.MurimiOS.Domain.GeospatialImportJob;
using EdgIQ.MurimiOS.Domain.MasterDataImportJob;
using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;

using EdgIQ.MurimiOS.Application.Common.Imports;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using OpeningBalanceDirection = global::EdgIQ.MurimiOS.Domain.FinanceOpeningBalance.OpeningBalanceDirection;
using FinanceOpeningBalance = global::EdgIQ.MurimiOS.Domain.FinanceOpeningBalance.FinanceOpeningBalance;
using MasterDataImportJob = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportJob;
using MasterDataImportRow = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportRow;
using MasterDataImportStatus = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportStatus;
using MasterDataImportType = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportType;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;
using MasterDataCsv = global::EdgIQ.MurimiOS.Application.Common.Imports.MasterDataCsv;
using MasterDataCsvParseResult = global::EdgIQ.MurimiOS.Application.Common.Imports.MasterDataCsvParseResult;

public sealed class MasterDataImportTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-09-01T20:00:00Z");

    [Fact]
    public void Csv_parser_supports_quoted_fields_and_rejects_missing_required_headers()
    {
        MasterDataCsvParseResult parsed = MasterDataCsv.Parse(MasterDataImportType.Suppliers,
            Encoding.UTF8.GetBytes("supplier_code,name,contact_name,email,phone\r\nSUP-1,\"Inputs, Limited\",Jane,jane@example.test,+2631\r\n"));
        MasterDataCsvParseResult invalid = MasterDataCsv.Parse(MasterDataImportType.Suppliers,
            Encoding.UTF8.GetBytes("supplier_code,name\r\nSUP-1,Incomplete\r\n"));

        Assert.Empty(parsed.FileIssues);
        Assert.Single(parsed.Rows);
        Assert.Equal("Inputs, Limited", parsed.Rows[0].Values["name"]);
        Assert.Contains(invalid.FileIssues, x => x.Code == "csv_header_missing");
    }

    [Fact]
    public void Import_checkpoint_only_moves_forward_and_completes_idempotently()
    {
        var job = new MasterDataImportJob(Guid.NewGuid(), MasterDataImportType.Livestock, "animals.csv", 100,
            new string('a', 64), "imports/animals.csv", MasterDataCsv.Version, "[\"farm_code\"]");
        job.CompleteValidation(3, 2, 1, Now);
        job.BeginApply("oidc|importer", Now.AddMinutes(1));
        job.RecordBatch(1, 1, 0, false, Now.AddMinutes(2));
        job.RecordBatch(3, 2, 0, true, Now.AddMinutes(3));

        Assert.Equal(MasterDataImportStatus.Applied, job.Status);
        Assert.Equal(3, job.CheckpointOrdinal);
        Assert.Equal(2, job.AppliedRowCount);
        Assert.Throws<InvalidOperationException>(() => job.RecordBatch(2, 2, 0, false, Now));
    }

    [Fact]
    public void Opening_balance_preserves_accounting_direction_and_currency()
    {
        var balance = new FinanceOpeningBalance(Guid.NewGuid(), Guid.NewGuid(), "1000", "Cash",
            OpeningBalanceDirection.Debit, 1250.555m, "usd", new DateOnly(2026, 9, 1), "OPEN-2026", Now, "accountant");

        Assert.Equal(1250.56m, balance.Amount);
        Assert.Equal("USD", balance.Currency);
        Assert.Equal(OpeningBalanceDirection.Debit, balance.Direction);
    }

    [Fact]
    public async Task Opening_balances_and_validated_import_payloads_are_immutable()
    {
        Guid tenantId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new PlatformDbContext(options, new TestTenantContext(tenantId));
        var balance = new FinanceOpeningBalance(Guid.NewGuid(), Guid.NewGuid(), "1000", "Cash",
            OpeningBalanceDirection.Debit, 100, "USD", new DateOnly(2026, 9, 1), "OPEN", Now, "accountant");
        var row = new MasterDataImportRow(Guid.NewGuid(), Guid.NewGuid(), 1, "{\"raw\":\"value\"}",
            "{\"normalized\":\"value\"}", "[]", true);
        db.FinanceOpeningBalances.Add(balance);
        db.MasterDataImportRows.Add(row);
        await db.SaveChangesAsync();

        db.Entry(balance).State = EntityState.Modified;
        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
        db.Entry(balance).State = EntityState.Unchanged;
        db.Entry(row).Property(x => x.RawJson).IsModified = true;
        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
    }

    private sealed class TestTenantContext(Guid tenantId) : ITenantContext
    {
        public Guid TenantId { get; } = tenantId;
        public bool IsResolved => true;
    }
}

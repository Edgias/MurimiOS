using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.AccountingImportBatch;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Finance.GetAccountingImport;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;

using AccountingImportBatch = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingImportBatch;
using AccountingImportStatus = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingImportStatus;
using AccountingStatementLine = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingStatementLine;
using AccountingMatchStatus = global::EdgIQ.MurimiOS.Domain.AccountingReconciliation.AccountingMatchStatus;
using AccountingReconciliation = global::EdgIQ.MurimiOS.Domain.AccountingReconciliation.AccountingReconciliation;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
public sealed record AccountingImportResponse(Guid Id, Guid FarmId, string Provider,
    string ExternalBatchId, DateOnly StatementStart, DateOnly StatementEnd, string Currency,
    AccountingImportStatus Status, DateTimeOffset ImportedAt, string ImportedBy,
    DateTimeOffset? ReconciledAt, string? ReconciledBy,
    IReadOnlyList<AccountingStatementLineResponse> Lines);

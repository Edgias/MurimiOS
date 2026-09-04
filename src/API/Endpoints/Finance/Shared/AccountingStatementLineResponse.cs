using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using EdgIQ.MurimiOS.Domain.AccountingReconciliation;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance.Shared;
using AccountingMatchStatus = global::EdgIQ.MurimiOS.Domain.AccountingReconciliation.AccountingMatchStatus;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
public sealed record AccountingStatementLineResponse(Guid Id, string ExternalLineId,
    DateTimeOffset OccurredAt, FinanceEntryType EntryType, decimal Amount, string Currency,
    string AccountCode, string? Reference, string Description, AccountingMatchStatus? MatchStatus,
    Guid? TransactionId, decimal? Difference, decimal? Tolerance, string? MatchNotes);

using System.Text.Json;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;

using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;

public sealed record AccountingLineValue(string ExternalLineId, DateTimeOffset OccurredAt,
    FinanceEntryType EntryType, decimal Amount, string AccountCode, string? Reference, string Description);

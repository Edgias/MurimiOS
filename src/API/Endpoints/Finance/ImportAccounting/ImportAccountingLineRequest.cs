using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance.ImportAccounting;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
public sealed record ImportAccountingLineRequest(Guid Id, string ExternalLineId,
    DateTimeOffset OccurredAt, FinanceEntryType EntryType, decimal Amount, string Currency,
    string AccountCode, string? Reference, string Description);

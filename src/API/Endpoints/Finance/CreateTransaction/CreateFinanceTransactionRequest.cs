using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance.CreateTransaction;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
using FinanceSourceType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceSourceType;
public sealed record CreateFinanceTransactionRequest(Guid? Id, FinanceEntryType EntryType,
    string Category, decimal Amount, string Currency, DateTimeOffset OccurredAt,
    FinanceSourceType SourceType, Guid? SourceId, string Reference, string? Counterparty,
    string? Description, IReadOnlyList<FinanceAllocationRequest> Allocations,
    Guid? CounterpartyLegalCompanyId = null, string? IntercompanyReference = null);

using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance.Shared;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
using FinanceSourceType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceSourceType;
public sealed record FinanceTransactionResponse(Guid Id, Guid FarmId, FinanceEntryType EntryType,
    string Category, decimal Amount, decimal SignedAmount, string Currency, DateTimeOffset OccurredAt,
    FinanceSourceType SourceType, Guid? SourceId, string Reference, string? Counterparty,
    string? Description, DateTimeOffset PostedAt, string PostedBy, Guid? ReversalOfId,
    Guid? CounterpartyLegalCompanyId, string? IntercompanyReference,
    IReadOnlyList<FinanceAllocationResponse> Allocations);

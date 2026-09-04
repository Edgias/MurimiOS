using System.Text.Json;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed record AccountingBatchValue(string ExternalBatchId, DateOnly StatementStart, DateOnly StatementEnd,
    string Currency, IReadOnlyList<AccountingLineValue> Lines) : ProviderNormalizedRecord;

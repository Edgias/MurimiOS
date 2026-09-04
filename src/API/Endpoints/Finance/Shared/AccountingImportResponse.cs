using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.AccountingImportBatch;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance.Shared;
using AccountingImportStatus = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingImportStatus;
public sealed record AccountingImportResponse(Guid Id, Guid FarmId, string Provider,
    string ExternalBatchId, DateOnly StatementStart, DateOnly StatementEnd, string Currency,
    AccountingImportStatus Status, DateTimeOffset ImportedAt, string ImportedBy,
    DateTimeOffset? ReconciledAt, string? ReconciledBy,
    IReadOnlyList<AccountingStatementLineResponse> Lines);

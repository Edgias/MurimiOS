using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance.ImportAccounting;

public sealed record ImportAccountingRequest(Guid? Id, string Provider, string ExternalBatchId,
    DateOnly StatementStart, DateOnly StatementEnd, string Currency,
    IReadOnlyList<ImportAccountingLineRequest> Lines);

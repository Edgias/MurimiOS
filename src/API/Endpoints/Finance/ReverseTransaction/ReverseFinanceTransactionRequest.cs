using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance.ReverseTransaction;
public sealed record ReverseFinanceTransactionRequest(Guid? Id, DateTimeOffset OccurredAt,
    string Reference, string Reason);

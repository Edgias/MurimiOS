using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance.GetEnterpriseProfitability;
public sealed record EnterpriseConsolidationSummary(Guid Id, DateTimeOffset CompletedAt,
    decimal EliminatedExpenses, decimal EliminatedRevenue, decimal ConsolidatedExpenses,
    decimal ConsolidatedRevenue, decimal ConsolidatedMargin, int ExceptionCount, string SourceSha256);

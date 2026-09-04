using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance.Shared;
public sealed record FarmProfitabilityResponse(Guid FarmId, DateTimeOffset From, DateTimeOffset To,
    string Currency, decimal Expenses, decimal Revenue, decimal GrossMargin,
    decimal AccountingReconciliationCoveragePercent,
    IReadOnlyList<CostCentreProfitabilityResponse> CostCentres);

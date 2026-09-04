using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance.GetEnterpriseProfitability;
using FarmProfitabilityResponse = global::EdgIQ.MurimiOS.API.Endpoints.Finance.Shared.FarmProfitabilityResponse;
public sealed record EnterpriseProfitabilityResponse(Guid OrganisationNodeId, DateTimeOffset From,
    DateTimeOffset To, string Currency, decimal Expenses, decimal Revenue, decimal GrossMargin,
    IReadOnlyList<FarmProfitabilityResponse> Farms, EnterpriseConsolidationSummary? LatestConsolidation);

using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.CostCentre;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance.Shared;
using CostCentreType = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentreType;

public sealed record CostCentreProfitabilityResponse(Guid CostCentreId, string Code, string Name,
    CostCentreType Type, Guid? TargetId, decimal Expenses, decimal Revenue, decimal GrossMargin,
    decimal? AreaHa, decimal YieldKg, decimal? HeadCount, decimal? CostPerHa,
    decimal? CostPerTonne, decimal? MarginPerHa, decimal? CostPerHead, decimal? MarginPerHead);

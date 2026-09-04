using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance.CreateTransaction;
using CostAllocationMethod = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocationMethod;

public sealed record FinanceAllocationRequest(Guid? Id, Guid CostCentreId, decimal Amount,
    CostAllocationMethod Method, decimal? Percentage, string? Notes);

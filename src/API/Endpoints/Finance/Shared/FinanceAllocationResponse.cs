using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance.Shared;
using CostAllocationMethod = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocationMethod;
public sealed record FinanceAllocationResponse(Guid Id, Guid CostCentreId, decimal Amount,
    decimal Percentage, CostAllocationMethod Method, string? Notes);
